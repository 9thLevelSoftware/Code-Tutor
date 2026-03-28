import asyncio
from decimal import Decimal

class CategorizationError(Exception):
    pass

async def categorize_transaction(tx: dict) -> dict:
    """Categorize a transaction. May fail for invalid data."""
    await asyncio.sleep(0.05)  # Simulate ML model latency
    
    if tx.get("amount") is None:
        raise CategorizationError(f"Missing amount for tx {tx.get('id')}")
    if tx.get("description") == "":
        raise CategorizationError(f"Empty description for tx {tx.get('id')}")
    
    # Simulate categorization
    tx["category"] = "groceries" if "market" in tx.get("description", "").lower() else "other"
    return tx

async def categorize_all_transactions(transactions: list[dict]):
    """Categorize transactions with partial failure handling."""
    successes = []
    failures = []
    
    async def safe_categorize(tx: dict):
        try:
            result = await categorize_transaction(tx)
            return ("success", result)
        except CategorizationError as e:
            return ("failure", str(e))
    
    async with asyncio.TaskGroup() as tg:
        tasks = [tg.____(safe_categorize(tx)) for tx in transactions]
    
    for task in tasks:
        status, result = task.____()
        if status == "success":
            successes.append(result)
        else:
            failures.append(result)
    
    return successes, failures

# Test with some valid and invalid transactions
transactions = [
    {"id": 1, "amount": Decimal("50.00"), "description": "Farmers Market"},
    {"id": 2, "amount": None, "description": "Unknown"},  # Will fail
    {"id": 3, "amount": Decimal("25.00"), "description": "Gas Station"},
    {"id": 4, "amount": Decimal("100.00"), "description": ""},  # Will fail
]

successes, failures = asyncio.run(categorize_all_transactions(transactions))
print(f"Categorized: {len(successes)}, Failed: {len(failures)}")
