[Setup]
AppName=installer_win
AppVersion=1.0
DefaultDirName={pf}\game
DefaultGroupName=game
OutputDir=.
OutputBaseFilename=UR_GE_2D_Platformer_Installer
Compression=lzma
SolidCompression=yes

[Files]
Source: "build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\StandaloneWindows"; Filename: "{app}\StandaloneWindows.exe"

[Run]
Filename: "{app}\StandaloneWindows.exe"; Description: "Start Game"; Flags: postinstall skipifsilent
