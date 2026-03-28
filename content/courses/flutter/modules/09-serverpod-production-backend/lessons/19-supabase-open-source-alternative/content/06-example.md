---
type: "EXAMPLE"
title: "File Storage"
---


### Upload a File

```dart
import 'dart:io';

Future<String> uploadImage(File file, String fileName) async {
  final bytes = await file.readAsBytes();
  
  await supabase.storage
      .from('avatars') // bucket name
      .uploadBinary(
        fileName,
        bytes,
        fileOptions: const FileOptions(
          contentType: 'image/png',
          upsert: true,
        ),
      );
  
  // Get public URL
  final url = supabase.storage
      .from('avatars')
      .getPublicUrl(fileName);
  
  return url;
}
```

### Download/Display

```dart
// Just use the public URL in Image.network
Image.network(
  supabase.storage.from('avatars').getPublicUrl('user-123.png'),
  fit: BoxFit.cover,
)
```



```dart
// Storage comparison:
// Firebase:  FirebaseStorage.instance.ref(path).putFile(file)
// Supabase:  supabase.storage.from(bucket).uploadBinary(path, bytes)
```
