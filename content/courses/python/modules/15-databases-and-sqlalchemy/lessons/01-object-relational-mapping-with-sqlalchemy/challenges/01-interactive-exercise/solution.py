from sqlalchemy import create_engine, Column, Integer, String, Float, Boolean, ForeignKey, DateTime
from sqlalchemy.orm import sessionmaker, declarative_base, relationship
from datetime import datetime, timezone

Base = declarative_base()

class Book(Base):
    """Book model for a bookstore."""
    __tablename__ = 'books'
    
    id = Column(Integer, primary_key=True)
    title = Column(String(200), nullable=False)
    author = Column(String(100), nullable=False)
    price = Column(Float, nullable=False)
    in_stock = Column(Boolean, default=True)
    
    # Relationship to orders
    orders = relationship('Order', back_populates='book')
    
    def __repr__(self):
        return f"<Book('{self.title}' by {self.author})>"

class Order(Base):
    """Order model for book purchases."""
    __tablename__ = 'orders'
    
    id = Column(Integer, primary_key=True)
    book_id = Column(Integer, ForeignKey('books.id'), nullable=False)
    quantity = Column(Integer, nullable=False, default=1)
    order_date = Column(DateTime, default=datetime.now(timezone.utc))
    
    # Relationship to book
    book = relationship('Book', back_populates='orders')
    
    def __repr__(self):
        return f"<Order({self.quantity}x {self.book.title})>"

# Setup database
engine = create_engine('sqlite:///:memory:')
Base.metadata.create_all(engine)
Session = sessionmaker(bind=engine)
session = Session()

# Add test books
books = [
    Book(title='Python Crash Course', author='Eric Matthes', price=29.99, in_stock=True),
    Book(title='Clean Code', author='Robert Martin', price=34.99, in_stock=True),
    Book(title='Design Patterns', author='Gang of Four', price=49.99, in_stock=False),
]
session.add_all(books)
session.commit()

# Add test orders
order1 = Order(book=books[0], quantity=2)
order2 = Order(book=books[0], quantity=1)
order3 = Order(book=books[1], quantity=3)
session.add_all([order1, order2, order3])
session.commit()

# Query 1: Find all books in stock
print('Books in stock:')
for book in session.query(Book).filter_by(in_stock=True).all():
    print(f'  {book.title} - ${book.price}')

# Query 2: Get orders for Python Crash Course
print('\nOrders for Python Crash Course:')
python_book = session.query(Book).filter_by(title='Python Crash Course').first()
for order in python_book.orders:
    print(f'  {order.quantity} copies on {order.order_date.strftime("%Y-%m-%d")}')

session.close()