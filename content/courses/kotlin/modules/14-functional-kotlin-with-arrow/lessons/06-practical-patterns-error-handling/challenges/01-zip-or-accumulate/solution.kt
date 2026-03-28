sealed class Either<out L, out R> {
    data class Left<out L>(val value: L) : Either<L, Nothing>()
    data class Right<out R>(val value: R) : Either<Nothing, R>()
}

fun <L> L.left(): Either<L, Nothing> = Either.Left(this)
fun <R> R.right(): Either<Nothing, R> = Either.Right(this)

fun <E, A, B, C, D> zipOrAccumulate(
    first: Either<E, A>,
    second: Either<E, B>,
    third: Either<E, C>,
    transform: (A, B, C) -> D
): Either<List<E>, D> {
    val errors = mutableListOf<E>()

    val a = when (first) {
        is Either.Left -> { errors.add(first.value); null }
        is Either.Right -> first.value
    }
    val b = when (second) {
        is Either.Left -> { errors.add(second.value); null }
        is Either.Right -> second.value
    }
    val c = when (third) {
        is Either.Left -> { errors.add(third.value); null }
        is Either.Right -> third.value
    }

    return if (errors.isEmpty()) {
        @Suppress("UNCHECKED_CAST")
        Either.Right(transform(a as A, b as B, c as C))
    } else {
        Either.Left(errors.toList())
    }
}

// Domain
sealed interface FormError {
    data class InvalidName(val value: String = "") : FormError
    data class InvalidEmail(val value: String) : FormError
    data class InvalidAge(val value: Int) : FormError
}

data class User(val name: String, val email: String, val age: Int)

fun validateName(name: String): Either<FormError, String> =
    if (name.isNotBlank()) name.right() else FormError.InvalidName().left()

fun validateEmail(email: String): Either<FormError, String> =
    if ("@" in email) email.right() else FormError.InvalidEmail(email).left()

fun validateAge(age: Int): Either<FormError, Int> =
    if (age in 0..150) age.right() else FormError.InvalidAge(age).left()

fun main() {
    val result1 = zipOrAccumulate(
        validateName("Alice"),
        validateEmail("alice@example.com"),
        validateAge(25)
    ) { name, email, age -> User(name, email, age) }
    println(result1)

    val result2 = zipOrAccumulate(
        validateName(""),
        validateEmail("bad"),
        validateAge(-5)
    ) { name, email, age -> User(name, email, age) }
    println(result2)

    val result3 = zipOrAccumulate(
        validateName("Alice"),
        validateEmail("nope"),
        validateAge(25)
    ) { name, email, age -> User(name, email, age) }
    println(result3)
}
