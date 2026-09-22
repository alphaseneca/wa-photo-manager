; ---------------------------------------
; WhatsApp Photo Manager — Inno Setup installer
; Requires Inno Setup Compiler (ISCC)
; ---------------------------------------

#ifndef AppVer
#define AppVer "1.0.0"
#endif

#ifndef AppArch
#define AppArch "x64"
#endif

[Setup]
AppName=WhatsApp Photo Manager
AppVersion={#AppVer}
AppPublisher=Ukesh Aryal
DefaultDirName={localappdata}\WhatsAppPhotoManager
DefaultGroupName=WhatsApp Photo Manager
OutputDir=..\installer
OutputBaseFilename=WhatsAppPhotoManager-Installer-{#AppVer}-{#AppArch}
SetupIconFile=..\assets\app-logo.ico
UninstallDisplayIcon={app}\whatsapp-photo-manager.exe
DisableProgramGroupPage=yes
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest

[Files]
Source: "..\out-build\whatsapp-photo-manager\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion

[Icons]
Name: "{group}\WhatsApp Photo Manager"; Filename: "{app}\whatsapp-photo-manager.exe"; WorkingDir: "{app}"
Name: "{userdesktop}\WhatsApp Photo Manager"; Filename: "{app}\whatsapp-photo-manager.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Tasks]
Name: desktopicon; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"; Flags: unchecked
Name: autostart; Description: "Start WhatsApp Photo Manager automatically at login"

[Run]
Filename: "{app}\whatsapp-photo-manager.exe"; Description: "Launch WhatsApp Photo Manager"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; \
ValueType: string; ValueName: "WhatsAppPhotoManager"; ValueData: "{app}\whatsapp-photo-manager.exe"; Flags: uninsdeletevalue; Tasks: autostart

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
