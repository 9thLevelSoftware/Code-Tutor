---
type: "THEORY"
title: "📊 SQL Basics: Tables, Rows, and Columns"
---


### The Spreadsheet Analogy

A SQL table is like a spreadsheet:

**Books Table:**

- **Table**: Like a sheet in your spreadsheet (e.g., "Books")
- **Columns**: The headers (id, title, author, year)
- **Rows**: Each entry/record
- **Primary Key**: Unique identifier (usually `id`)

### SQL Commands You'll Use


Don't worry—you won't write SQL directly. Exposed does it for you!

---



```sql
-- Create a table
CREATE TABLE books (
    id INT PRIMARY KEY,
    title VARCHAR(255),
    author VARCHAR(255),
    year INT
);

-- Insert data
INSERT INTO books (id, title, author, year)
VALUES (1, '1984', 'George Orwell', 1949);

-- Query data
SELECT * FROM books;
SELECT * FROM books WHERE year > 1940;
SELECT * FROM books WHERE author = 'George Orwell';

-- Update data
UPDATE books SET year = 1950 WHERE id = 1;

-- Delete data
DELETE FROM books WHERE id = 1;
```
