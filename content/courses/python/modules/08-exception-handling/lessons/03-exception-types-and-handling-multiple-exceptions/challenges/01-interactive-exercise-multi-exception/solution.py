# Multi-Exception Data Processor
# This solution demonstrates handling multiple exception types

def get_first_letter(user_dict, key):
    """Safely extract first letter of a dictionary value."""
    result = None
    
    try:
        # Step 1: Get the value (may raise KeyError)
        value = user_dict[key]
        # Step 2: Convert to uppercase (may raise AttributeError)
        upper_value = value.upper()
        # Step 3: Get first character (may raise IndexError)
        result = upper_value[0]
    except KeyError:
        # Handle missing key
        print(f"Key '{key}' not found in dictionary")
    except AttributeError:
        # Handle non-string value (can't call .upper())
        print(f"Value for '{key}' is not a string, got {type(user_dict.get(key)).__name__}")
    except IndexError:
        # Handle empty string
        print(f"Value for '{key}' is empty")
    
    return result

# Test cases
user1 = {"name": "Alice", "role": "admin"}
user2 = {"name": "Bob"}
user3 = {"name": "Carol", "role": 12345}
user4 = {"name": "Dave", "role": ""}

print(get_first_letter(user1, "role"))  # Should work: 'A'
print(get_first_letter(user2, "role"))  # KeyError
print(get_first_letter(user3, "role"))  # AttributeError
print(get_first_letter(user4, "role"))  # IndexError