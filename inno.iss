[Setup]
AppName=UR_GE_2D_Platformer
AppVersion=1.0
DefaultDirName={pf}\UR_GE_2D_Platformer
DefaultGroupName=UR_GE_2D_Platformer
OutputDir=.
OutputBaseFilename=installer_win
Compression=lzma
SolidCompression=yes

[Files]
Source: "build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\StandaloneWindows"; Filename: "{app}\StandaloneWindows.exe"

[Run]
Filename: "{app}\StandaloneWindows.exe"; Description: "Start Game"; Flags: postinstall skipifsilent
