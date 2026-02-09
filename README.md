# 🛠️ alt:V HWID Spoofer & Cleaner

A lightweight C# console application designed to manipulate Hardware Identifiers (HWID) and clean client cache files to bypass bans or restrictions on the **alt:V** multiplayer platform.

> **⚠️ Disclaimer:** This tool is intended for **educational purposes and local testing only**. The author is not responsible for any misuse, server bans, or damage caused to your system. Use at your own risk.

## ✨ Features

### 1. 🔒 Permanent Spoofer (HWID)
Modifies critical Windows Registry keys to alter the machine's digital footprint. This creates a "fresh" identity for the operating system.
**Targeted Registry Paths:**
- `HKLM\SOFTWARE\Microsoft\SQMClient` (MachineId)
- `HKLM\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters` (Hostname)
- `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion` (Product ID, BuildGUID, InstallDate, RegisteredOwner)
- `HKLM\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles` (HwProfileGuid)
- `HKLM\SOFTWARE\Microsoft\Cryptography` (MachineGuid)
- `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate` (SusClientId)

### 2. 🎭 Temp Spoofer (Memory)
Injects directly into the game process to temporarily modify Social Club data.
- **Function:** Scans memory signatures in `altv-client.dll` to find and overwrite `socialName` and `socialID`.
- **Constraint:** This feature **only works on servers that do not use Cloud Auth**.
- **Requirement:** The game must be running before selecting this option.

### 3. 🧹 Cleaner
Automated cleanup tool specifically for alt:V directories.
- Wipes `AppData\Local\altv\cache`
- Wipes `AppData\Local\altv\cef\cache`
- Handles file locks (skips files currently in use).

## 🚀 Usage

1. **Download** the latest release or compile the source code.
2. **Run as Administrator** (Required for Registry and Memory access).
3. Select an option from the menu:
   - `[1]` **Permanent Spoof**: Randomizes HWID keys. *Restart recommended after use.*
   - `[2]` **Temp Spoof**: Changes SocialID/Name in memory (Game must be open).
   - `[3]` **Cleaner**: Deletes client cache files.
   - `[4]` **Exit**: Closes the application.

## ⚙️ Build Requirements

- **Language:** C#
- **Framework:** .NET Framework (4.7.2 or newer recommended)

## 🛡️ Best Practices for Evasion

To ensure a clean start after running the **Permanent Spoofer**, complete the following steps **before connecting to any server**:

1. **Change your IP Address** (Restart router or use a VPN).
2. **Clear Browser Cookies** or use a fresh browser profile.
3. **Change Discord Account** (If the server links Discord).
4. **Change Social Club Account** (If the server uses Cloud Auth).
## 📜 License

This project is open-source. Feel free to modify and adapt it for your needs.