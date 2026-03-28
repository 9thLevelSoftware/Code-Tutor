// Solution: Messaging System with Typing and Read Receipts
// Chat interface with real-time indicators

import 'package:flutter/material.dart';
import 'dart:async';

void main() {
  runApp(const MessagingApp());
}

enum MessageStatus { sent, delivered, read }

class Message {
  final String id;
  final String text;
  final bool isMe;
  final DateTime timestamp;
  MessageStatus status;

  Message({required this.id, required this.text, required this.isMe, required this.timestamp, this.status = MessageStatus.sent});
}

class MessagingApp extends StatelessWidget {
  const MessagingApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(home: const ChatScreen());
  }
}

class ChatScreen extends StatefulWidget {
  const ChatScreen({super.key});

  @override
  State<ChatScreen> createState() => _ChatScreenState();
}

class _ChatScreenState extends State<ChatScreen> {
  final TextEditingController _controller = TextEditingController();
  final List<Message> _messages = [];
  bool _isOtherTyping = false;
  Timer? _typingTimer;

  @override
  void dispose() {
    _controller.dispose();
    _typingTimer?.cancel();
    super.dispose();
  }

  void _sendMessage() {
    if (_controller.text.isEmpty) return;
    
    final msg = Message(
      id: DateTime.now().millisecondsSinceEpoch.toString(),
      text: _controller.text,
      isMe: true,
      timestamp: DateTime.now(),
    );
    
    setState(() {
      _messages.add(msg);
      _controller.clear();
    });

    // Simulate delivery after 500ms
    Future.delayed(const Duration(milliseconds: 500), () {
      setState(() => msg.status = MessageStatus.delivered);
    });

    // Simulate read after 1.5s
    Future.delayed(const Duration(milliseconds: 1500), () {
      setState(() => msg.status = MessageStatus.read);
    });

    // Simulate reply with typing indicator
    _simulateReply();
  }

  void _simulateReply() {
    setState(() => _isOtherTyping = true);
    
    Future.delayed(const Duration(seconds: 2), () {
      setState(() {
        _isOtherTyping = false;
        _messages.add(Message(
          id: DateTime.now().millisecondsSinceEpoch.toString(),
          text: 'Got it! Thanks for the message.',
          isMe: false,
          timestamp: DateTime.now(),
        ));
      });
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Chat'),
            if (_isOtherTyping)
              const Text('typing...', style: TextStyle(fontSize: 12, fontStyle: FontStyle.italic)),
          ],
        ),
      ),
      body: Column(
        children: [
          Expanded(
            child: ListView.builder(
              reverse: true,
              itemCount: _messages.length + (_isOtherTyping ? 1 : 0),
              itemBuilder: (_, index) {
                if (_isOtherTyping && index == 0) {
                  return _buildTypingIndicator();
                }
                final msgIndex = _isOtherTyping ? index - 1 : index;
                return _buildMessage(_messages[_messages.length - 1 - msgIndex]);
              },
            ),
          ),
          _buildInputBar(),
        ],
      ),
    );
  }

  Widget _buildTypingIndicator() {
    return Align(
      alignment: Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.all(8),
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          color: Colors.grey.shade200,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: List.generate(3, (i) {
            return Container(
              margin: const EdgeInsets.symmetric(horizontal: 2),
              width: 8,
              height: 8,
              decoration: BoxDecoration(
                color: Colors.grey.shade500,
                shape: BoxShape.circle,
              ),
            );
          }),
        ),
      ),
    );
  }

  Widget _buildMessage(Message msg) {
    return Align(
      alignment: msg.isMe ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        margin: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
        decoration: BoxDecoration(
          color: msg.isMe ? Colors.blue : Colors.grey.shade200,
          borderRadius: BorderRadius.circular(16),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(msg.text, style: TextStyle(color: msg.isMe ? Colors.white : Colors.black)),
            if (msg.isMe) const SizedBox(width: 8),
            if (msg.isMe) _buildStatusIcon(msg.status),
          ],
        ),
      ),
    );
  }

  Widget _buildStatusIcon(MessageStatus status) {
    switch (status) {
      case MessageStatus.sent:
        return const Icon(Icons.check, size: 14, color: Colors.white70);
      case MessageStatus.delivered:
        return const Icon(Icons.done_all, size: 14, color: Colors.white70);
      case MessageStatus.read:
        return const Icon(Icons.done_all, size: 14, color: Colors.lightBlueAccent);
    }
  }

  Widget _buildInputBar() {
    return Padding(
      padding: const EdgeInsets.all(8),
      child: Row(
        children: [
          Expanded(
            child: TextField(
              controller: _controller,
              decoration: const InputDecoration(
                hintText: 'Type a message...',
                border: OutlineInputBorder(),
              ),
            ),
          ),
          const SizedBox(width: 8),
          IconButton(
            icon: const Icon(Icons.send),
            onPressed: _sendMessage,
          ),
        ],
      ),
    );
  }
}

// Key concepts:
// - MessageStatus enum: sent/delivered/read
// - Typing indicator: Boolean state + animated dots
// - Read receipts: Icons (single check, double check, blue)
// - Future.delayed: Simulate async message status updates
// - Reverse ListView: Chat scrolls from bottom