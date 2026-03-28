---
type: "THEORY"
title: "Writing Your First KSP Processor"
---

**Estimated Time**: 75 minutes
**Difficulty**: Advanced
**Prerequisites**: KSP basics, Kotlin reflection concepts, code generation patterns

---

## Building a Custom KSP Processor

While existing KSP processors handle common use cases (JSON serialization, dependency injection), many projects need custom code generation—validation logic, API wrappers, or boilerplate elimination. Writing your own KSP processor puts you in control of compile-time code generation.

### Processor Architecture

A KSP processor consists of three main components:

1. **SymbolProcessor**: The core logic that scans and processes annotations
2. **SymbolProcessorProvider**: Factory that creates processor instances
3. **Annotations**: Markers that trigger processing

### Project Structure

```
processor/
├── build.gradle.kts
└── src/main/kotlin/
    ├── ValidationProcessor.kt      # Main processor logic
    ├── ValidationProcessorProvider.kt
    ├── annotations/
    │   ├── Validate.kt              # Target annotation
    │   └── ValidationRule.kt        # Rule definition
    └── generators/
        └── ValidatorGenerator.kt    # Code generation
```

### Building the Processor

**Step 1: Setup Processor Module**

```kotlin
// processor/build.gradle.kts
plugins {
    kotlin("jvm")
    id("com.google.devtools.ksp") version "2.0.0-1.0.21"
}

dependencies {
    implementation("com.google.devtools.ksp:symbol-processing-api:2.0.0-1.0.21")
    implementation("com.squareup:kotlinpoet:1.16.0")  // For code generation
}
```

**Step 2: Define Annotations**

```kotlin
// annotations/Validate.kt
@Target(AnnotationTarget.CLASS)
@Retention(AnnotationRetention.SOURCE)
annotation class Validate {
    companion object
}

// annotations/ValidationRule.kt
@Target(AnnotationTarget.PROPERTY)
@Retention(AnnotationRetention.SOURCE)
annotation class ValidationRule(
    val minLength: Int = 0,
    val maxLength: Int = Int.MAX_VALUE,
    val required: Boolean = true,
    val pattern: String = ""
)
```

**Step 3: Implement the Processor**

```kotlin
// ValidationProcessor.kt
class ValidationProcessor(
    private val codeGenerator: CodeGenerator,
    private val logger: KSPLogger
) : SymbolProcessor {
    
    override fun process(resolver: Resolver): List<KSAnnotated> {
        val symbols = resolver.getSymbolsWithAnnotation(Validate::class.qualifiedName!!)
        val deferred = mutableListOf<KSAnnotated>()
        
        symbols.filterIsInstance<KSClassDeclaration>().forEach { classDeclaration ->
            try {
                generateValidator(classDeclaration)
            } catch (e: Exception) {
                logger.error("Failed to process ${classDeclaration.simpleName.asString()}: ${e.message}")
                deferred.add(classDeclaration)
            }
        }
        
        return deferred
    }
    
    private fun generateValidator(classDeclaration: KSClassDeclaration) {
        val packageName = classDeclaration.packageName.asString()
        val className = classDeclaration.simpleName.asString()
        val validatorName = "${className}Validator"
        
        // Extract properties with validation rules
        val properties = classDeclaration.getAllProperties()
            .mapNotNull { property ->
                val rule = property.annotations
                    .find { it.shortName.asString() == "ValidationRule" }
                    ?.let { parseValidationRule(it) }
                
                if (rule != null) property to rule else null
            }
        
        // Generate validator using KotlinPoet
        val fileSpec = FileSpec.builder(packageName, validatorName)
            .addType(
                TypeSpec.classBuilder(validatorName)
                    .addFunction(
                        FunSpec.builder("validate")
                            .addParameter("obj", ClassName(packageName, className))
                            .returns(
                                ClassName("kotlin", "Result")
                                    .parameterizedBy(ClassName(packageName, className))
                            )
                            .addCode(generateValidationLogic(properties))
                            .build()
                    )
                    .build()
            )
            .build()
        
        // Write generated file
        val file = codeGenerator.createNewFile(
            dependencies = Dependencies(false, classDeclaration.containingFile!!),
            packageName = packageName,
            fileName = validatorName
        )
        fileSpec.writeTo(file.writer())
        file.close()
    }
    
    private fun parseValidationRule(annotation: KSAnnotation): ValidationRuleData {
        val arguments = annotation.arguments
        return ValidationRuleData(
            minLength = arguments.find { it.name?.asString() == "minLength" }?.value as? Int ?: 0,
            maxLength = arguments.find { it.name?.asString() == "maxLength" }?.value as? Int ?: Int.MAX_VALUE,
            required = arguments.find { it.name?.asString() == "required" }?.value as? Boolean ?: true,
            pattern = arguments.find { it.name?.asString() == "pattern" }?.value as? String ?: ""
        )
    }
    
    private fun generateValidationLogic(
        properties: List<Pair<KSPropertyDeclaration, ValidationRuleData>>
    ): CodeBlock {
        val builder = CodeBlock.builder()
        builder.addStatement("val errors = mutableListOf<String>()")
        
        properties.forEach { (property, rule) ->
            val propName = property.simpleName.asString()
            
            if (rule.required) {
                builder.addStatement("if (obj.$propName == null) errors.add(\"$propName is required\")")
            }
            
            if (rule.minLength > 0) {
                builder.beginControlFlow("obj.$propName?.let")
                builder.addStatement("if (it.length < ${rule.minLength}) errors.add(\"$propName must be at least ${rule.minLength} characters\")")
                builder.endControlFlow()
            }
        }
        
        builder.beginControlFlow("return if (errors.isEmpty())")
        builder.addStatement("Result.success(obj)")
        builder.nextControlFlow("else")
        builder.addStatement("Result.failure(IllegalArgumentException(errors.joinToString()))")
        builder.endControlFlow()
        
        return builder.build()
    }
}

data class ValidationRuleData(
    val minLength: Int,
    val maxLength: Int,
    val required: Boolean,
    val pattern: String
)
```

**Step 4: Register the Processor**

```kotlin
// ValidationProcessorProvider.kt
class ValidationProcessorProvider : SymbolProcessorProvider {
    override fun create(environment: SymbolProcessorEnvironment): SymbolProcessor {
        return ValidationProcessor(
            codeGenerator = environment.codeGenerator,
            logger = environment.logger
        )
    }
}
```

**Step 5: Service Registration**

Create `META-INF/services/com.google.devtools.ksp.processing.SymbolProcessorProvider`:

```
com.example.processor.ValidationProcessorProvider
```

### Using the Processor

```kotlin
// In your application code
@Validate
class UserRegistration(
    @ValidationRule(minLength = 3, maxLength = 50)
    val username: String,
    
    @ValidationRule(required = true, pattern = "^[A-Za-z0-9+_.-]+@(.+)$")
    val email: String,
    
    @ValidationRule(minLength = 8)
    val password: String
)

// Generated code (UserRegistrationValidator.kt):
class UserRegistrationValidator {
    fun validate(obj: UserRegistration): Result<UserRegistration> {
        val errors = mutableListOf<String>()
        
        if (obj.username.length < 3) errors.add("username must be at least 3 characters")
        if (obj.email.isEmpty()) errors.add("email is required")
        if (obj.password.length < 8) errors.add("password must be at least 8 characters")
        
        return if (errors.isEmpty()) {
            Result.success(obj)
        } else {
            Result.failure(IllegalArgumentException(errors.joinToString()))
        }
    }
}

// Usage:
val validator = UserRegistrationValidator()
val result = validator.validate(UserRegistration("ab", "", "123"))
// result.isFailure with validation errors
```

### Debugging Tips

```kotlin
// Log processing information
logger.info("Processing ${classDeclaration.simpleName.asString()}")
logger.warn("Deprecated annotation used")
logger.error("Failed to generate code", exception)

// View generated sources
// Build → Generated Sources → ksp/
```

### Real-World Relevance

Custom KSP processors enable:
- **API contract validation**: Generate validators from OpenAPI specs
- **Database schema generation**: Create SQL from entity annotations
- **Boilerplate elimination**: Auto-generate builders, mappers, and adapters
- **Architecture enforcement**: Generate wiring code for DI frameworks
- **Multiplatform serialization**: Custom formats beyond JSON/Protobuf

While writing processors requires upfront investment, they pay dividends in reduced boilerplate, compile-time safety, and team productivity. Large Kotlin teams often develop internal processors as infrastructure investments that accelerate feature development across all projects.
