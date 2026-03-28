import 'package:flutter/material.dart';

class NoteScreen extends StatefulWidget {
  const NoteScreen({super.key});

  @override
  State<NoteScreen> createState() => _NoteScreenState();
}

class _NoteScreenState extends State<NoteScreen> {
  final _controller = TextEditingController();
  bool _hasUnsavedChanges = false;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _showDiscardDialog() {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Discard changes?'),
        content: const Text('You have unsaved changes. Are you sure you want to leave?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () {
              Navigator.pop(context);
              Navigator.pop(context);
            },
            child: const Text('Discard'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: !_hasUnsavedChanges,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          _showDiscardDialog();
        }
      },
      child: Scaffold(
        appBar: AppBar(title: const Text('New Note')),
        body: Padding(
          padding: const EdgeInsets.all(16),
          child: TextField(
            controller: _controller,
            maxLines: null,
            onChanged: (value) {
              if (!_hasUnsavedChanges) {
                setState(() => _hasUnsavedChanges = true);
              }
            },
            decoration: const InputDecoration(
              hintText: 'Start typing...',
            ),
          ),
        ),
      ),
    );
  }
}