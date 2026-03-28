sealed class Either<out L, out R> {
    data class Left<out L>(val value: L) : Either<L, Nothing>()
    data class Right<out R>(val value: R) : Either<Nothing, R>()
}

fun <L> L.left(): Either<L, Nothing> = Either.Left(this)
fun <R> R.right(): Either<Nothing, R> = Either.Right(this)

// TODO: Implement zipOrAccumulate for 3 inputs
// fun <E, A, B, C, D> zipOrAccumulate(
//     first: Either<E, A>,
//     second: Either<E, B>,
//     third: Either<E, C>,
//     transform: (A, B, C) -> D
// ): Either<List<E>, D>
//
// Strategy:
// 1. Evaluate all three (no short-circuiting)
// 2. Collect all Left values into errors list
// 3. If errors is empty -> apply transform to all Right values -> Right(result)
// 4. If errors is non-empty -> Left(errors)

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
    // All valid
    val result1 = zipOrAccumulate(
        validateName("Alice"),
        validateEmail("alice@example.com"),
        validateAge(25)
    ) { name, email, age -> User(name, email, age) }
    println(result1)

    // All invalid -- should collect ALL errors
    val result2 = zipOrAccumulate(
        validateName(""),
        validateEmail("bad"),
        validateAge(-5)
    ) { name, email, age -> User(name, email, age) }
    println(result2)

    // One invalid
    val result3 = zipOrAccumulate(
        validateName("Alice"),
        validateEmail("nope"),
        validateAge(25)
    ) { name, email, age -> User(name, email, age) }
    println(result3)
}
