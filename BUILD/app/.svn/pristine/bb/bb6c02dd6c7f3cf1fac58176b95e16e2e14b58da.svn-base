del c:\inetpub\wwwroot\CBH\app.* /S /F /Q
del c:\inetpub\wwwroot\CBH\app\app\*.* /S /F /Q
del c:\inetpub\wwwroot\CBH\app\scripts\*.* /S /F /Q
del c:\inetpub\wwwroot\CBH\app\images\*.* /S /F /Q
del c:\inetpub\wwwroot\CBH\app\ux\*.* /S /F /Q
del c:\inetpub\wwwroot\CBH\app\overrides\*.* /S /F /Q

XCOPY *.* "c:\inetpub\wwwroot\CBH\app\*.*" /D /Y /S /EXCLUDE:EXCLUDE.TXT

REM java -jar yuicompressor-2.4.8.jar app.js -o /inetpub\wwwroot\cbh\app\app.js --charset utf-8
REM java -jar yuicompressor-2.4.8.jar scripts/behavior.js -o /inetpub\wwwroot\cbh\app\scripts\behavior.js --charset utf-8
