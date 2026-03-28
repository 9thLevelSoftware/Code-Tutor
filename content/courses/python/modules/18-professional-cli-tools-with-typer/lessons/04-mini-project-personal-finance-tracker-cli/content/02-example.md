---
type: "EXAMPLE"
title: "Complete Implementation"
---

**Usage:**
```
$ finance add 45.00 food --note "Groceries"
Added: -$45.00 (food) - Groceries

$ finance add 2000.00 salary --income
Added: +$2000.00 (salary)

$ finance list
                    Transactions
┌────┬────────────┬──────────┬───────────┬───────────┐
│ ID │ Date       │ Category │ Amount    │ Note      │
├────┼────────────┼──────────┼───────────┼───────────┤
│ 1  │ 2024-01-15 │ food     │ -$45.00   │ Groceries │
│ 2  │ 2024-01-15 │ salary   │ +$2000.00 │           │
└────┴────────────┴──────────┴───────────┴───────────┘

$ finance summary
Income:   $2000.00
Expenses: $45.00
Balance:  $1955.00
```

```python
# cli.py - Personal Finance Tracker (Typer 0.12+)
import typer
from enum import Enum
from pathlib import Path
from rich.console import Console
from rich.table import Table
import json
from datetime import datetime

# Typer 0.12+: rich_markup_mode enables Rich in help text
app = typer.Typer(
    help="Personal Finance Tracker - manage your money.",
    rich_markup_mode="rich"
)
console = Console()
DATA_FILE = Path.home() / ".finance.json"

class Category(str, Enum):
    food = "food"
    transport = "transport"
    utilities = "utilities"
    entertainment = "entertainment"
    salary = "salary"
    other = "other"

def load_data() -> list[dict]:
    if DATA_FILE.exists():
        return json.loads(DATA_FILE.read_text())
    return []

def save_data(transactions: list[dict]):
    DATA_FILE.write_text(json.dumps(transactions, indent=2))

def next_id(transactions: list[dict]) -> int:
    if not transactions:
        return 1
    return max(t["id"] for t in transactions) + 1

@app.command()
def add(
    amount: float,
    category: Category,
    note: str = "",
    income: bool = False
):
    """Add a new [bold green]expense[/bold green] or [bold blue]income[/bold blue]."""
    transactions = load_data()
    entry = {
        "id": next_id(transactions),
        "amount": amount if income else -amount,
        "category": category.value,
        "note": note,
        "date": datetime.now().strftime("%Y-%m-%d")
    }
    transactions.append(entry)
    save_data(transactions)
    
    sign = "+" if income else "-"
    color = "green" if income else "red"
    msg = f"Added: [{color}]{sign}${amount:.2f}[/{color}] ({category.value})"
    if note:
        msg += f" - {note}"
    console.print(msg)

@app.command(name="list")
def list_transactions(category: Category = None):
    """List all transactions in a [bold]beautiful table[/bold]."""
    transactions = load_data()
    if category:
        transactions = [t for t in transactions if t["category"] == category.value]
    
    if not transactions:
        console.print("[dim]No transactions yet. Add one with: finance add 50 food[/dim]")
        return
    
    table = Table(title="Transactions")
    table.add_column("ID", style="cyan")
    table.add_column("Date")
    table.add_column("Category", style="magenta")
    table.add_column("Amount")
    table.add_column("Note")
    
    for t in transactions:
        color = "green" if t["amount"] > 0 else "red"
        sign = "+" if t["amount"] > 0 else ""
        table.add_row(
            str(t["id"]),
            t["date"],
            t["category"],
            f"[{color}]{sign}${abs(t['amount']):.2f}[/{color}]",
            t.get("note", "")
        )
    console.print(table)

@app.command()
def summary():
    """Show financial [bold]summary[/bold]."""
    transactions = load_data()
    income = sum(t["amount"] for t in transactions if t["amount"] > 0)
    expenses = abs(sum(t["amount"] for t in transactions if t["amount"] < 0))
    balance = income - expenses
    
    console.print(f"[green]Income:[/green]   ${income:.2f}")
    console.print(f"[red]Expenses:[/red] ${expenses:.2f}")
    balance_color = "green" if balance >= 0 else "red"
    console.print(f"[bold]Balance:[/bold]  [{balance_color}]${balance:.2f}[/{balance_color}]")

@app.command()
def delete(transaction_id: int):
    """Delete a transaction by ID."""
    transactions = load_data()
    for i, t in enumerate(transactions):
        if t["id"] == transaction_id:
            removed = transactions.pop(i)
            save_data(transactions)
            console.print(f"[red]Deleted:[/red] ${abs(removed['amount']):.2f} ({removed['category']})")
            return
    console.print(f"[red]Transaction {transaction_id} not found[/red]")
    raise typer.Exit(code=1)

if __name__ == "__main__":
    app()

```
