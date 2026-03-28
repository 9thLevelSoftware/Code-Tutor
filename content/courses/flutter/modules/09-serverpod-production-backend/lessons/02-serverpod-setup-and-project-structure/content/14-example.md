---
type: "EXAMPLE"
title: "Testing Your API"
---


With the server running, test your endpoint using curl or your browser:



```bash
# Using curl to test the endpoint
curl http://localhost:8080/example/hello?name=Developer

# Expected response:
# {"data":"Hello, Developer!"}

# Or test from the Flutter app (my_app_flutter):
# 1. Open a new terminal
# 2. Navigate to my_app_flutter
# 3. Run the app

cd ../my_app_flutter
flutter run

# The sample app has a button that calls the example endpoint
# and displays the response.
```
