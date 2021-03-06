; ###########################################################################
; #  INNO Setup Builder config file              (c) Ertms Solutions
; #
; ###########################################################################
;
; This script creates a self-installing archive for ERTMSFormalSpecs.
;
; It Uses INNO Setup
[Setup]
AppName=ERTMSFormalSpecs
AppVerName=ERTMSFormalSpecs
AppPublisher=ERTMS Solutions
AppPublisherURL=http://www.ertmssolutions.com/
DefaultGroupName=ERTMSFormalSpecs
DefaultDirName={pf}\ERTMSSolutions\ERTMSFormalSpecs
OutputDir=release
OutputBaseFileName=ERTMSFormalSpecs_Setup
ChangesAssociations=yes
SetupIconFile=..\GUI\EFS.ico        
UninstallDisplayIcon={app}\bin\ERTMSFormalSpecs.exe
PrivilegesRequired=lowest
DisableProgramGroupPage=yes

[Dirs]
Name: "{app}\"; Permissions: everyone-modify
;needed for windows 7, so that windows 7 allows sniffer and shell to create logfile.

[Files]
Source: ..\..\bin\ERTMSFormalSpecs.exe; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\ERTMSFormalSpecs.exe.config; DestDir: {app}\bin; 
Source: ..\..\bin\DataDictionary.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\ErtmsSolutions.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Importer.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\MigraDoc.DocumentObjectModel.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\MigraDoc.Rendering.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\PdfSharp.Charting.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\PdfSharp.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Reports.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\HistoricalData.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Utils.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\XmlBooster.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\log4net.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\ICSharpCode.SharpZipLib.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\LibGit2Sharp.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\WeifenLuo.WinFormsUI.Docking.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\ObjectListView.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\GuiUtils.dll; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\NativeBinaries\*; DestDir: {app}\bin\NativeBinaries; Flags: recursesubdirs
Source: ..\etc\logconfig.xml; DestDir: {app}\bin; Flags: ignoreversion

; also provide the pdb files
Source: ..\..\bin\ERTMSFormalSpecs.pdb; DestDir: {app}\bin; 
Source: ..\..\bin\DataDictionary.pdb; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\ErtmsSolutions.pdb; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Importer.pdb; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Reports.pdb; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\HistoricalData.pdb; DestDir: {app}\bin; Flags: ignoreversion
Source: ..\..\bin\Utils.pdb; DestDir: {app}\bin; Flags: ignoreversion

Source: ..\..\doc\EFSW_User_Guide.pdf; DestDir: {app}\doc;
Source: ..\..\doc\EFS_Product_Release_Info.pdf; DestDir: {app}\doc;
Source: ..\..\doc\specs\subset-026.efs; DestDir: "{code:GetDir|0}";
Source: ..\..\doc\specs\subset-026\*; DestDir: "{code:GetDir|0}\subset-026"; Flags: recursesubdirs
Source: ..\..\doc\specs\braking curves verification.efs; DestDir: "{code:GetDir|0}";
Source: ..\..\doc\specs\DMIFunctionalTests.efs; DestDir: "{code:GetDir|0}";

[Icons]
Name: "{group}\docs\User's Manual"; Filename: "{app}\doc\EFSW_User_Guide.pdf";
Name: "{group}\docs\EFS Product release info"; Filename: "{app}\doc\EFS_Product_Release_Info.pdf";
Name: "{group}\EFS Workbench"; Filename: "{app}\bin\ERTMSFormalSpecs.exe";
Name: "{group}\Uninstall EFS"; Filename: "{app}\unins000.exe";

; checking for .Net 3.5 installation
; courtesy of http://www.idev.ch/content/view/291/1/
[CustomMessages]
dotnetmissing=This application needs Microsoft .Net 4.0 which is not yet installed. Would you like to download it now?

[Code]
function InitializeSetup(): Boolean;
var
    ErrorCode: Integer;
    netFrameWorkInstalled : Boolean;
    isInstalled: Cardinal;
begin
  result := true;
 
  // Check for the .Net 3.5 framework
  isInstalled := 0;
  netFrameworkInstalled := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client', 'Install', isInstalled);
  if ((netFrameworkInstalled)  and (isInstalled <> 1)) then netFrameworkInstalled := false;
 
  if netFrameworkInstalled = false then
  begin
    if (MsgBox(ExpandConstant('{cm:dotnetmissing}'),
        mbConfirmation, MB_YESNO) = idYes) then
    begin
      ShellExec('open',
      'http://www.microsoft.com/en-us/download/details.aspx?id=17851',
      '','',SW_SHOWNORMAL,ewNoWait,ErrorCode);
    end;
    result := false;
  end;
end;

var
  DirPage: TInputDirWizardPage;

function GetDir(Param: String): String;
begin
  Result := DirPage.Values[StrToInt(Param)];
end;

procedure InitializeWizard;
begin
  // create a directory input page
  DirPage := CreateInputDirPage(wpSelectDir, 'Select the location for the Model files', 'Where should the model files be installed ?', 'Setup will install the files related to the model in the following folder.', False, '');
  DirPage.Add('Location of the model files');
  DirPage.Values[0] := GetPreviousData('ModelFiles', ExpandConstant('{userdocs}') + '\ERTMSFormalSpecs');
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  // store chosen directories for the next run of the setup
  SetPreviousData(PreviousDataKey, 'ModelFiles', DirPage.Values[0]);
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
begin
  Result := MemoDirInfo + NewLine + NewLine +
        'Model install location:' + NewLine + Space + GetDir('0');
end;

