---
type: "THEORY"
title: "Step 2: Complete Data Models"
---


### User Model


### Post Model




```dart
// lib/models/post_model.dart
import 'package:cloud_firestore/cloud_firestore.dart';

class Post {
  final String id;
  final String userId;
  final String userName;
  final String? userPhotoURL;
  final String caption;
  final String imageURL;
  final List<String> likes;
  final int commentCount;
  final DateTime createdAt;

  Post({
    required this.id,
    required this.userId,
    required this.userName,
    this.userPhotoURL,
    required this.caption,
    required this.imageURL,
    this.likes = const [],
    this.commentCount = 0,
    DateTime? createdAt,
  }) : createdAt = createdAt ?? DateTime.now();

  factory Post.fromFirestore(DocumentSnapshot doc) {
    final data = doc.data() as Map<String, dynamic>;
    return Post(
      id: doc.id,
      userId: data['userId'] ?? '',
      userName: data['userName'] ?? 'Unknown',
      userPhotoURL: data['userPhotoURL'],
      caption: data['caption'] ?? '',
      imageURL: data['imageURL'] ?? '',
      likes: List<String>.from(data['likes'] ?? []),
      commentCount: data['commentCount'] ?? 0,
      createdAt: (data['createdAt'] as Timestamp).toDate(),
    );
  }

  Map<String, dynamic> toMap() {
    return {
      'userId': userId,
      'userName': userName,
      'userPhotoURL': userPhotoURL,
      'caption': caption,
      'imageURL': imageURL,
      'likes': likes,
      'commentCount': commentCount,
      'createdAt': Timestamp.fromDate(createdAt),
    };
  }

  bool isLikedBy(String userId) => likes.contains(userId);
}
```
