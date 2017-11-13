REM del c:\inetpub\wwwroot\CBH\app.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\app\*.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\scripts\*.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\images\*.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\ux\*.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\overrides\*.* /S /F /Q
REM del c:\inetpub\wwwroot\CBH\app\scripts\*.* /S /F /Q

XCOPY *.* "c:\ftp\app\*.*" /D /Y /S /EXCLUDE:EXCLUDE.TXT
XCOPY C:\SVN\CBH_API\CBHWA\bin\CBHWA.dll "c:\ftp\wa\bin\CBHWA.dll" /D /Y /S
XCOPY C:\SVN\CBH_API\CBHWA\Areas\Reports\ReportDesign\*.* "c:\ftp\wa\Areas\Reports\ReportDesign\*.*" /D /Y /S
XCOPY C:\SVN\CBH_API\CBHWA\Areas\Reports\Views\*.cshtml "c:\ftp\wa\Areas\Reports\Views\*.cshtml" /D /Y /S

REM java -jar yuicompressor-2.4.8.jar app.js -o /ftp\app\app.js --charset utf-8
REM java -jar yuicompressor-2.4.8.jar scripts/behavior.js -o /ftp\app\scripts\behavior.js --charset utf-8
