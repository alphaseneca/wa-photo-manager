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
AppVerName=WhatsApp Photo Manager v{#AppVer}
AppPublisher=Ukesh Aryal
AppMutex=Local\WhatsAppPhotoManagerTrayAppMutex,WhatsAppPhotoManagerTrayAppMutex
DefaultDirName={localappdata}\WhatsAppPhotoManager
DefaultGroupName=WhatsApp Photo Manager
OutputDir=..\installer
OutputBaseFilename=WhatsAppPhotoManager-Installer-{#AppVer}-{#AppArch}
SetupIconFile=..\assets\app-logo.ico
UninstallDisplayIcon={app}\whatsapp-photo-manager.exe
CloseApplications=yes
CloseApplicationsFilter=*.exe
DisableProgramGroupPage=yes
Compression=lzma2/fast
SolidCompression=yes
PrivilegesRequired=lowest

[Files]
Source: "..\out-build\whatsapp-photo-manager\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion; Excludes: "*.log,*.data.json,config.json,qr_code*.png,_IGNORE_*"

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
Type: files; Name: "{app}\*.log"
Type: files; Name: "{app}\*.png"
Type: files; Name: "{app}\config.json"
Type: filesandordirs; Name: "{app}\_IGNORE_*"
Type: filesandordirs; Name: "{app}\app"
Type: filesandordirs; Name: "{app}\chrome"

[Code]
// Clean installation & upgrade routine:
// When upgrading over an existing version, terminate running processes and
// purge old code/binaries while strictly preserving user configuration, session, and downloads.
procedure CurStepChanged(CurStep: TSetupStep);
var
  AppDir: string;
  ErrorCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    AppDir := ExpandConstant('{app}');
    
    // 1. Terminate any currently running bot/launcher instances before replacing files
    Exec('taskkill.exe', '/F /IM whatsapp-photo-manager.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
    Sleep(500);

    // 2. If upgrading an existing installation, purge old application code while strictly preserving user data
    if DirExists(AppDir) then
    begin
      // Purge old app code & node_modules so no obsolete files remain
      if DirExists(AppDir + '\app') then
      begin
        DelTree(AppDir + '\app', True, True, True);
      end;
      
      // Purge old chrome bundle so fresh version extracts cleanly
      if DirExists(AppDir + '\chrome') then
      begin
        DelTree(AppDir + '\chrome', True, True, True);
      end;

      // Clean up old log files and temporary QR images
      DeleteFile(AppDir + '\*.log');
      DeleteFile(AppDir + '\qr_code_*.png');

      // Note:
      // - config.json is NOT deleted (user configuration preserved)
      // - downloads\ is NOT deleted (user photos preserved)
      // - _IGNORE_*\ is NOT deleted (WhatsApp session preserved so user stays logged in)
    end;
  end;
end;

// Clean uninstallation: Terminate running background instances first
function InitializeUninstall(): Boolean;
var
  ErrorCode: Integer;
begin
  Result := True;
  
  // Terminate any running instances of whatsapp-photo-manager and child processes
  Exec('taskkill.exe', '/F /IM whatsapp-photo-manager.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode);
  Sleep(500);
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DownloadsPath: string;
  RemoveData: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    DownloadsPath := ExpandConstant('{app}\downloads');
    
    // Check if downloads directory exists and has files
    if DirExists(DownloadsPath) then
    begin
      RemoveData := MsgBox(
        'Do you want to completely remove all downloaded photos and media?' + #13#10 + #13#10 +
        'Click "Yes" to delete all downloads and perform a 100% clean uninstall.' + #13#10 +
        'Click "No" to preserve your downloaded photos in:' + #13#10 + DownloadsPath,
        mbConfirmation, MB_YESNO
      );
      
      if RemoveData = IDYES then
      begin
        DelTree(DownloadsPath, True, True, True);
      end;
    end;
  end
  else if CurUninstallStep = usPostUninstall then
  begin
    // Clean up temporary logs, session caches, and app files
    DelTree(ExpandConstant('{app}\_IGNORE_*'), True, True, True);
    DelTree(ExpandConstant('{app}\app'), True, True, True);
    DelTree(ExpandConstant('{app}\chrome'), True, True, True);
    DeleteFile(ExpandConstant('{app}\*.log'));
    DeleteFile(ExpandConstant('{app}\*.png'));
    DeleteFile(ExpandConstant('{app}\config.json'));
    
    // If downloads directory was deleted (or does not exist), remove the root directory completely
    if not DirExists(ExpandConstant('{app}\downloads')) then
    begin
      DelTree(ExpandConstant('{app}'), True, True, True);
      RemoveDir(ExpandConstant('{app}'));
    end;
  end;
end;
