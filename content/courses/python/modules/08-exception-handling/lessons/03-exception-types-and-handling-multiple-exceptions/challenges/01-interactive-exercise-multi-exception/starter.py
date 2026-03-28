def get_first_letter(user_dict, key):
    """Safely extract first letter of a dictionary value."""
    
    # TODO: Add try block
    # TODO: Get the value from dictionary using user_dict[key]
    # TODO: Convert to uppercase using .upper()
    # TODO: Get first character using [0]
    
    # TODO: Add except KeyError block
    # Print message: "Key not found"
    
    # TODO: Add except AttributeError block
    # Print message: "Value is not a string"
    
    # TODO: Add except IndexError block
    # Print message: "Value is empty"
    
    # Return None or the character
    return None

# Test cases
user1 = {"name": "Alice", "role": "admin"}
user2 = {"name": "Bob"}
user3 = {"name": "Carol", "role": 12345}
user4 = {"name": "Dave", "role": ""}

print(get_first_letter(user1, "role"))  # Should work
print(get_first_letter(user2, "role"))  # KeyError
print(get_first_letter(user3, "role"))  # AttributeError
print(get_first_letter(user4, "role"))  # IndexError