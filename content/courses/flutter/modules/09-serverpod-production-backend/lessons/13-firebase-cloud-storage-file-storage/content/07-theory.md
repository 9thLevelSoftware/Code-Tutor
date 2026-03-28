---
type: "THEORY"
title: "Basic Storage Operations"
---


### Create Storage Service




```dart
// lib/services/storage_service.dart
import 'package:firebase_storage/firebase_storage.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'dart:io';

class StorageService {
  final FirebaseStorage _storage = FirebaseStorage.instance;
  final FirebaseAuth _auth = FirebaseAuth.instance;

  String? get currentUserId => _auth.currentUser?.uid;

  // ========== UPLOAD ==========

  // Upload file with progress tracking
  Future<String> uploadFile({
    required File file,
    required String path,
    Function(double)? onProgress,
  }) async {
    try {
      // Create reference to the file location
      final storageRef = _storage.ref().child(path);

      // Upload the file
      final uploadTask = storageRef.putFile(file);

      // Listen to upload progress
      uploadTask.snapshotEvents.listen((TaskSnapshot snapshot) {
        final progress = snapshot.bytesTransferred / snapshot.totalBytes;
        if (onProgress != null) {
          onProgress(progress);
        }
      });

      // Wait for upload to complete
      final snapshot = await uploadTask;

      // Get download URL
      final downloadUrl = await snapshot.ref.getDownloadURL();

      return downloadUrl;
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // Upload user profile picture
  Future<String> uploadProfilePicture(File imageFile) async {
    if (currentUserId == null) throw 'User not authenticated';

    final fileName = 'profile_${DateTime.now().millisecondsSinceEpoch}.jpg';
    final path = 'users/$currentUserId/profile/$fileName';

    return uploadFile(file: imageFile, path: path);
  }

  // Upload post image
  Future<String> uploadPostImage(File imageFile) async {
    if (currentUserId == null) throw 'User not authenticated';

    final fileName = 'post_${DateTime.now().millisecondsSinceEpoch}.jpg';
    final path = 'users/$currentUserId/posts/$fileName';

    return uploadFile(file: imageFile, path: path);
  }

  // Upload document
  Future<String> uploadDocument(File file, String documentName) async {
    if (currentUserId == null) throw 'User not authenticated';

    final fileName = '${DateTime.now().millisecondsSinceEpoch}_$documentName';
    final path = 'users/$currentUserId/documents/$fileName';

    return uploadFile(file: file, path: path);
  }

  // ========== DOWNLOAD ==========

  // Get download URL for a file
  Future<String> getDownloadUrl(String path) async {
    try {
      final ref = _storage.ref().child(path);
      return await ref.getDownloadURL();
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // Download file to local storage
  Future<File> downloadFile({
    required String path,
    required String localPath,
    Function(double)? onProgress,
  }) async {
    try {
      final ref = _storage.ref().child(path);
      final file = File(localPath);

      final downloadTask = ref.writeToFile(file);

      // Listen to download progress
      downloadTask.snapshotEvents.listen((TaskSnapshot snapshot) {
        final progress = snapshot.bytesTransferred / snapshot.totalBytes;
        if (onProgress != null) {
          onProgress(progress);
        }
      });

      await downloadTask;
      return file;
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // ========== DELETE ==========

  // Delete file by path
  Future<void> deleteFile(String path) async {
    try {
      final ref = _storage.ref().child(path);
      await ref.delete();
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // Delete file by URL
  Future<void> deleteFileByUrl(String downloadUrl) async {
    try {
      final ref = _storage.refFromURL(downloadUrl);
      await ref.delete();
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // Delete user's profile picture
  Future<void> deleteProfilePicture() async {
    if (currentUserId == null) throw 'User not authenticated';

    final path = 'users/$currentUserId/profile/';
    await _deleteFolder(path);
  }

  // ========== METADATA ==========

  // Get file metadata
  Future<FullMetadata> getMetadata(String path) async {
    try {
      final ref = _storage.ref().child(path);
      return await ref.getMetadata();
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // Update file metadata
  Future<void> updateMetadata({
    required String path,
    Map<String, String>? customMetadata,
    String? contentType,
  }) async {
    try {
      final ref = _storage.ref().child(path);
      final metadata = SettableMetadata(
        customMetadata: customMetadata,
        contentType: contentType,
      );
      await ref.updateMetadata(metadata);
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // ========== LIST FILES ==========

  // List all files in a directory
  Future<List<String>> listFiles(String path) async {
    try {
      final ref = _storage.ref().child(path);
      final result = await ref.listAll();

      return result.items.map((item) => item.fullPath).toList();
    } on FirebaseException catch (e) {
      throw _handleStorageException(e);
    }
  }

  // List user's profile pictures
  Future<List<String>> listUserImages() async {
    if (currentUserId == null) throw 'User not authenticated';

    final path = 'users/$currentUserId/posts/';
    return listFiles(path);
  }

  // ========== HELPER METHODS ==========

  // Delete entire folder (recursively)
  Future<void> _deleteFolder(String path) async {
    final ref = _storage.ref().child(path);
    final result = await ref.listAll();

    // Delete all files
    for (var item in result.items) {
      await item.delete();
    }

    // Delete subfolders recursively
    for (var prefix in result.prefixes) {
      await _deleteFolder(prefix.fullPath);
    }
  }

  // Handle Storage exceptions
  String _handleStorageException(FirebaseException e) {
    switch (e.code) {
      case 'unauthorized':
        return 'You don\'t have permission to access this file.';
      case 'canceled':
        return 'Upload/download was canceled.';
      case 'unknown':
        return 'An unknown error occurred.';
      case 'object-not-found':
        return 'File not found.';
      case 'bucket-not-found':
        return 'Storage bucket not found.';
      case 'project-not-found':
        return 'Firebase project not found.';
      case 'quota-exceeded':
        return 'Storage quota exceeded.';
      case 'unauthenticated':
        return 'Please login to upload files.';
      case 'retry-limit-exceeded':
        return 'Operation timed out. Please try again.';
      default:
        return 'Storage error: ${e.message}';
    }
  }
}
```
