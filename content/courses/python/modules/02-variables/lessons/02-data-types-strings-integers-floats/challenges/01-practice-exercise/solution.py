# Player Profile Program - SOLUTION

# Create your variables
player_name = "DragonSlayer"    # String
player_level = 42               # Integer
health_points = 87.5            # Float
is_online = True                # Boolean

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
print(f"player_name type: {type(player_name)}")
print(f"player_level type: {type(player_level)}")
print(f"health_points type: {type(health_points)}")
print(f"is_online type: {type(is_online)}")

# Do some math
next_level = player_level + 1
print(f"\nNext level will be: {next_level}")

# String multiplication
battle_cry = "Victory" * 3
print(f"Battle cry: {battle_cry}")

# BONUS: Calculate health percentage
max_health = 100.0
health_percent = (health_points / max_health) * 100
print(f"\nHealth: {health_percent}%")

# Additional examples:
print("\n--- EXTRA EXAMPLES ---")
print(f"Level squared: {player_level ** 2}")
print(f"Half health: {health_points / 2}")
print(f"Name in uppercase: {player_name.upper()}")