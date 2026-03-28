---
type: "ANALOGY"
title: "Problem 4: Emulator Won't Start"
---


### Symptom:
Emulator starts but shows a black screen or crashes.

### Solution 1: Enable Hardware Acceleration

**Windows**:
1. Open **Task Manager** → **Performance**
2. Check if "Virtualization" is enabled
3. If not, enable Intel VT-x or AMD-V in BIOS

**Mac**:
Hardware acceleration is enabled by default.

**Linux**:

### Solution 2: Allocate More RAM

1. Open Android Studio
2. **Tools** → **Device Manager**
3. Click the pencil icon (Edit) on your emulator
4. Click **Show Advanced Settings**
5. Increase RAM to at least 2048 MB
6. Click **Finish**

### Solution 3: Use a Different System Image

Some system images work better than others:
- Try **API 33** (Android 13) instead of the latest
- Use **x86_64** images (faster than ARM)



```bash
# Install KVM
sudo apt-get install qemu-kvm libvirt-daemon-system libvirt-clients bridge-utils

# Add yourself to the kvm group
sudo adduser $USER kvm
```
