REM del c:\DEV\CBH\app.* /S /F /Q
REM del c:\DEV\CBH\app\app\*.* /S /F /Q
REM del c:\DEV\CBH\app\scripts\*.* /S /F /Q
REM del c:\DEV\CBH\app\images\*.* /S /F /Q
REM del c:\DEV\CBH\app\ux\*.* /S /F /Q
REM del c:\DEV\CBH\app\overrides\*.* /S /F /Q
REM del c:\DEV\CBH\app\scripts\*.* /S /F /Q

XCOPY *.* "c:\DEV\CBH\app\*.*" /D /Y /S /EXCLUDE:EXCLUDE.TXT
XCOPY C:\DEV\CBH\API\CBHWA\bin\CBHWA.dll "c:\Workspaces\cbh\build\wa\bin\CBHWA.dll" /D /Y /S
XCOPY C:\DEV\CBH\API\CBHWA\Areas\Reports\ReportDesign\*.* "c:\Workspaces\cbh\build\wa\Areas\Reports\ReportDesign\*.*" /D /Y /S
XCOPY C:\DEV\CBH\API\CBHWA\Areas\Reports\Views\*.cshtml "c:\Workspaces\cbh\build\wa\Areas\Reports\Views\*.cshtml" /D /Y /S

REM java -jar yuicompressor-2.4.8.jar app.js -o /Workspaces\cbh\app\app.js --charset utf-8
REM java -jar yuicompressor-2.4.8.jar scripts/behavior.js -o /Workspaces\cbh\app\scripts\behavior.js --charset utf-8
