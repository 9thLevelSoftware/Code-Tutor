class PhotoGallery extends StatelessWidget {
  final List<String> photos = [
    'https://picsum.photos/id/10/400/400',
    'https://picsum.photos/id/20/400/400',
    'https://picsum.photos/id/30/400/400',
    'https://picsum.photos/id/40/400/400',
    'https://picsum.photos/id/50/400/400',
    'https://picsum.photos/id/60/400/400',
  ];

  @override
  Widget build(BuildContext context) {
    // Build a GridView with Hero-wrapped images below
    // Each image should navigate to PhotoDetailScreen when tapped
    return Container();
  }
}

class PhotoDetailScreen extends StatelessWidget {
  final String photoUrl;
  final int index;

  const PhotoDetailScreen({
    super.key,
    required this.photoUrl,
    required this.index,
  });

  @override
  Widget build(BuildContext context) {
    // Display the photo with matching Hero tag below
    // Add a flightShuttleBuilder that adds shadow during flight
    return Container();
  }
}