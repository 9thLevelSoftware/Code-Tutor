import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

class AdaptiveConfirmDialog {
  // Implement static show() method as follows
  // - Check platform using Theme.of(context).platform
  // - Show CupertinoAlertDialog on iOS
  // - Show AlertDialog on Android
  // - Return true for confirm, false for cancel
  
  static Future<bool?> show({
    required BuildContext context,
    required String title,
    required String message,
    String confirmText = 'Confirm',
    String cancelText = 'Cancel',
  }) async {
    // Your implementation here
    return null;
  }
}