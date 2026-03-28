// Solution: Contact List Manager
// This solution demonstrates working with Lists and Maps together

void main() {
  // Create a list of contacts, where each contact is a Map
  List<Map<String, String>> contacts = [
    {
      'name': 'Alice',
      'phone': '555-1234',
      'email': 'alice@email.com',
    },
    {
      'name': 'Bob',
      'phone': '555-5678',
      'email': 'bob@email.com',
    },
  ];

  // Display all contacts
  print('=== All Contacts ===');
  for (var contact in contacts) {
    print('Name: ${contact['name']}');
    print('Phone: ${contact['phone']}');
    print('Email: ${contact['email']}');
    print('');
  }

  // Find a specific contact by name
  print('=== Finding Alice ===');
  var searchName = 'Alice';
  for (var contact in contacts) {
    if (contact['name'] == searchName) {
      print('Found: ${contact['name']}, ${contact['phone']}');
      break;
    }
  }

  // Remove a contact by name
  print('');
  print('=== After removing Bob ===');
  contacts.removeWhere((contact) => contact['name'] == 'Bob');
  print('Remaining contacts: ${contacts.length}');
}

// Expected Output:
// === All Contacts ===
// Name: Alice
// Phone: 555-1234
// Email: alice@email.com
//
// Name: Bob
// Phone: 555-5678
// Email: bob@email.com
//
// === Finding Alice ===
// Found: Alice, 555-1234
//
// === After removing Bob ===
// Remaining contacts: 1
