import org.koin.core.annotation.*

// ========== Required Setup ==========
// Add to gradle/libs.versions.toml:
// [versions]
// koin = "4.1.1"
// ksp = "2.3.4"
//
// [libraries]
// koin-annotations = { module = "io.insert-koin:koin-annotations", version.ref = "koin" }
// koin-ksp-compiler = { module = "io.insert-koin:koin-ksp-compiler", version.ref = "koin" }
//
// [plugins]
// ksp = { id = "com.google.devtools.ksp", version.ref = "ksp" }
//
// Add to shared/build.gradle.kts:
// plugins {
//     alias(libs.plugins.ksp)
// }
//
// kotlin {
//     sourceSets {
//         commonMain.dependencies {
//             implementation(libs.koin.annotations)
//         }
//     }
// }
//
// dependencies {
//     add("kspCommonMainMetadata", libs.koin.ksp.compiler)
//     add("kspAndroid", libs.koin.ksp.compiler)
//     add("kspIosArm64", libs.koin.ksp.compiler)
//     add("kspIosSimulatorArm64", libs.koin.ksp.compiler)
// }

// Module declaration
@Module
@ComponentScan("com.example.app.user")
class UserModule

// Network
@Single
class HttpClient()

// API layer
interface UserApi {
    suspend fun getUser(id: String): User
}

@Single
@Bind(UserApi::class)
class UserApiImpl(
    private val client: HttpClient
) : UserApi {
    override suspend fun getUser(id: String): User {
        // In a real app, this would make an HTTP request
        return User(id, "User $id", "user$id@example.com")
    }
}

// Repository layer
interface UserRepository {
    suspend fun getUser(id: String): User
}

@Single
@Bind(UserRepository::class)
class UserRepositoryImpl(
    private val api: UserApi
) : UserRepository {
    override suspend fun getUser(id: String) = api.getUser(id)
}

// Use cases (factory - new instance each time)
@Factory
class GetUserUseCase(
    private val repository: UserRepository
) {
    suspend operator fun invoke(id: String) = repository.getUser(id)
}

@Factory
class UpdateUserUseCase(
    private val repository: UserRepository
) {
    suspend operator fun invoke(user: User) {
        // In a real app, this would update the user via the repository
        println("Updating user: ${user.id}")
    }
}

// ViewModel
@KoinViewModel
class UserProfileViewModel(
    private val getUser: GetUserUseCase,
    private val updateUser: UpdateUserUseCase
)

// Usage in startKoin:
// modules(UserModule().module)