[Setup]
AppName=UR_GE_Platformer
AppVersion=1.0
DefaultDirName={pf}\UR_GE_Platformer
DefaultGroupName=UR_GE_Platformer
OutputDir=.
OutputBaseFilename=installer_win
Compression=lzma
SolidCompression=yes
; Custom Installer
SetupIconFile=ur-logo.ico
DisableWelcomePage=no
AlwaysShowDirOnReadyPage=yes
ShowComponentSizes=yes


[Messages]
WelcomeLabel1=Welcome to the installer!
WelcomeLabel2=This is the setup wizard for a 2D platformer game for the Game Engineering course at Uni Regensburg. Please follow the instructions to install the game on your computer.

; Custom Color
[Code]
procedure InitializeWizard;
begin
    // Darker: $181818
    WizardForm.Color := $212121;
end;

[Files]
Source: "..\build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Possibly include a script that runs on the targets PC that grabs data we log in Unity and sends it to us in case we do a remote study

[Icons]
Name: "{group}\StandaloneWindows"; Filename: "{app}\StandaloneWindows.exe"

[Run]
Filename: "{app}\StandaloneWindows.exe"; Description: "Start Game"; Flags: postinstall skipifsilent
