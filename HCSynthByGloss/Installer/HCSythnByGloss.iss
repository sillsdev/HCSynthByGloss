  ; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Hermit Crab Synthesis for FLExTrans"
#define MyAppVersion "1.6.4 Beta"
#define MyAppPublisher "SIL Iternational"
#define MyAppURL "https://software.sil.org/"
#define MyAppExeName "HCSynthByGloss.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{7E9D63A5-F97F-4783-8195-E86BC8CA39BA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf64}\SIL\FieldWorks 9
DefaultGroupName=Hermit Crab Synthesis for FLExTrans
OutputBaseFilename=HCSynthByGlossSetup
;SetupIconFile=..\AllomorphGeneratorDll\LT.ico
Compression=lzma
SolidCompression=yes
CloseApplications=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: 

[Files]
Source: "..\bin\x64\Release\HCSynthByGloss.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\HCSynthByGloss.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\HCSynthByGloss.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\HCSynthByGlossLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\x64\Release\HCSynthByGlossLib.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Transforms\FormatCommon.xsl"; DestDir: "{app}\Transforms"; Flags: ignoreversion
Source: "..\Transforms\FormatHCTrace.xsl"; DestDir: "{app}\Transforms"; Flags: ignoreversion
;Source: "..\bin\x64\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\x64\Release\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\x64\Release\SIL.Core.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\x64\Release\SIL.Machine.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\x64\Release\SIL.Machine.Morphology.HermitCrab.dll"; DestDir: "{app}"; Flags: ignoreversion
;Source: "..\bin\x64\Release\SIL.Scripture.dll"; DestDir: "{app}"; Flags: ignoreversion

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
;Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}";

;[Run]
;Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[code]
   function InitializeSetup(): Boolean;
   begin
     if (FileExists(ExpandConstant('C:\Program Files\SIL\FieldWorks 9\Flex.exe'))) then
     begin
       { MsgBox('Installation validated', mbInformation, MB_OK); }
       Result := True;
     end
     else
     begin
       MsgBox('FieldWorks 9 is not installed.  Please install it first.', mbCriticalError, MB_OK);
       Result := False;
     end;
   end;

   function GetLastError(): LongInt; external 'GetLastError@kernel32.dll stdcall';

procedure SetLineInFile(FileName: string; Line: string);
{: Boolean;}
var
  Lines: TArrayOfString;
  Count: Integer;
  Index: Integer;
  i: Integer;
begin
  {MsgBox('In SetLineInFile', mbCriticalError, MB_OK);}
  if not LoadStringsFromFile(FileName, Lines) then
  begin
    MsgBox(Format('Error reading file "%s". %s', [FileName, SysErrorMessage(GetLastError)]), mbCriticalError, MB_OK);
  end
  else
  begin
    Count := GetArrayLength(Lines);
    for i := 0 to Count - 1 do
    begin
       Index := i;
       if Pos(Line, Lines[i]) > 0 then
       begin
         {MsgBox(Format('Found at %d', [i]), mbCriticalError, MB_OK);}
         Break;
       end;
    end;
    if Index >= Count-1 then
    begin
    SetArrayLength(Lines, Count + 1);
      Lines[Count] := Lines[Count - 1];
      Lines[Count - 1] := Line; 
      {MsgBox('Updating', mbCriticalError, MB_OK);}
      if not SaveStringsToFile(FileName, Lines, False) then
      begin
        MsgBox(Format('Error writing file "%s". %s', [
              FileName, SysErrorMessage(GetLastError)]), mbCriticalError, MB_OK);
      end
      else
      begin
        {MsgBox(Format('File "%s" saved.', [FileName]), mbCriticalError, MB_OK);}
      end;
    end;
  end;
end;
