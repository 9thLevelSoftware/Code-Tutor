# Player Profile Program

# Create your variables (replace the ____ )
player_name = "____"        # A string (use quotes)
player_level = ____         # An integer (no quotes, no decimal)
health_points = ____        # A float (use decimal point)
is_online = ____            # True or False (no quotes, capital T/F)

# Display the profile
print("=" * 40)
print("       PLAYER PROFILE")
print("=" * 40)
print(f"Name: {player_name}")
print(f"Level: {player_level}")
print(f"Health: {health_points} HP")
print(f"Status: {'Online' if is_online else 'Offline'}")
print("=" * 40)

# Check the types
print("\nDATA TYPES:")
print(f"player_name type: {type(____)}")     # Fill in the variable name
print(f"player_level type: {type(____)}")   # Fill in the variable name
print(f"health_points type: {type(____)}")  # Fill in the variable name
print(f"is_online type: {type(____)}")      # Fill in the variable name

# Do some math
next_level = player_level + ____  # Add 1 to level
print(f"\nNext level will be: {next_level}")

# String multiplication
battle_cry = "____" * 3  # Repeat any word 3 times
print(f"Battle cry: {battle_cry}")

# BONUS: Calculate health percentage
max_health = 100.0  # Maximum health
health_percent = (health_points / max_health) * 100
print(f"\nHealth: {health_percent}%")