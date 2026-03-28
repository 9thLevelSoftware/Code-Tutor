// Solution: BankAccount with primary constructor and init block validation

class BankAccount(
    val accountNumber: String,
    val ownerName: String,
    initialBalance: Double
) {
    var balance: Double = initialBalance
    
    init {
        require(accountNumber.isNotBlank()) { "Account number cannot be blank" }
        require(accountNumber.length >= 8) { "Account number must be at least 8 characters" }
        require(ownerName.isNotBlank()) { "Owner name cannot be blank" }
        require(initialBalance >= 0) { "Initial balance cannot be negative" }
    }
}

fun main() {
    val account = BankAccount("ACC123456", "John Doe", 1000.0)
    println("Account ${account.accountNumber} owned by ${account.ownerName} has balance ${account.balance}")
}
