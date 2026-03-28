class BankAccount:
    # Class attributes (shared by all instances)
    bank_name = "Python Bank"
    total_accounts = 0
    
    def __init__(self, owner, balance=0):
        # Instance attributes (unique to each instance)
        self.owner = owner
        self.balance = balance
        # Increment class attribute
        BankAccount.total_accounts += 1
    
    def deposit(self, amount):
        """Add amount to balance if valid"""
        if self.is_valid_amount(amount):
            self.balance += amount
            return f"Deposited ${amount}. New balance: ${self.balance}"
        return "Invalid amount"
    
    def withdraw(self, amount):
        """Subtract amount if valid and sufficient balance"""
        if not self.is_valid_amount(amount):
            return "Invalid amount"
        if amount > self.balance:
            return "Insufficient funds"
        self.balance -= amount
        return f"Withdrew ${amount}. New balance: ${self.balance}"
    
    @classmethod
    def get_total_accounts(cls):
        """Class method to get total number of accounts"""
        return f"Total accounts at {cls.bank_name}: {cls.total_accounts}"
    
    @staticmethod
    def is_valid_amount(amount):
        """Static method to validate amount"""
        return isinstance(amount, (int, float)) and amount > 0

# Create accounts and test
print("=== Creating Bank Accounts ===")
account1 = BankAccount("Alice", 1000)
account2 = BankAccount("Bob", 500)
account3 = BankAccount("Charlie")

print(f"Bank: {BankAccount.bank_name}")
print(BankAccount.get_total_accounts())

print("\n=== Testing Instance Methods ===")
print(account1.deposit(500))
print(account1.withdraw(200))
print(account2.withdraw(1000))  # Should fail - insufficient funds

print("\n=== Testing Validation ===")
print(f"Is 100 valid? {BankAccount.is_valid_amount(100)}")
print(f"Is -50 valid? {BankAccount.is_valid_amount(-50)}")
print(f"Is 'abc' valid? {BankAccount.is_valid_amount('abc')}")
