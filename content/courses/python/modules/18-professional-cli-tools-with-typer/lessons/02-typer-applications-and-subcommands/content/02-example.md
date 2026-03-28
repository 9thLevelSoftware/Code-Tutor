---
type: "EXAMPLE"
title: "Code Example"
---

**Expected Output:**
```
$ python finance.py --help
Usage: finance.py [OPTIONS] COMMAND [ARGS]...

  Personal Finance Tracker - manage your money.

Commands:
  add      Add a new expense or income.
  balance  Show current balance.
  list     List all transactions.

$ python finance.py add 50.00 food --note "Groceries"
Added: -$50.00 (food) - Groceries

$ python finance.py add 2000.00 salary --income
Added: +$2000.00 (salary)

$ python finance.py balance
Balance: $1950.00
```

```python
import typer

# Typer 0.12+: rich_markup_mode enables Rich formatting in help
app = typer.Typer(
    help="Personal Finance Tracker - manage your money.",
    rich_markup_mode="rich"  # New in 0.12+
)

# In-memory storage (use a file or DB in real apps)
transactions: list[dict] = []

@app.command()
def add(
    amount: float,
    category: str,
    note: str = "",
    income: bool = False
):
    """Add a new [bold green]expense[/bold green] or [bold blue]income[/bold blue]."""
    entry = {
        "amount": amount if income else -amount,
        "category": category,
        "note": note
    }
    transactions.append(entry)
    sign = "+" if income else "-"
    msg = f"Added: {sign}${amount:.2f} ({category})"
    if note:
        msg += f" - {note}"
    print(msg)

@app.command(name="list")
def list_transactions():
    """List all transactions."""
    if not transactions:
        print("No transactions yet!")
        return
    for i, t in enumerate(transactions, 1):
        sign = "+" if t["amount"] > 0 else ""
        print(f"{i}. {sign}${t['amount']:.2f} ({t['category']})")

@app.command()
def balance():
    """Show current [bold]balance[/bold]."""
    total = sum(t["amount"] for t in transactions)
    print(f"Balance: ${total:.2f}")

if __name__ == "__main__":
    app()

```
