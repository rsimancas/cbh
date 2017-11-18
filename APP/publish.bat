REM del c:\Workspaces\cbh\build\app.* /S /F /Q
REM del c:\Workspaces\cbh\build\app\app\*.* /S /F /Q
REM del c:\Workspaces\cbh\build\app\scripts\*.* /S /F /Q
REM del c:\Workspaces\cbh\build\app\images\*.* /S /F /Q
REM del c:\Workspaces\cbh\build\app\ux\*.* /S /F /Q
REM del c:\Workspaces\cbh\build\app\overrides\*.* /S /F /Q

XCOPY *.* "c:\Workspaces\cbh\build\app\*.*" /D /Y /S /EXCLUDE:EXCLUDE.TXT

REM java -jar yuicompressor-2.4.8.jar app.js -o /workspaces\cbh\build\app\app.js --charset utf-8
REM java -jar yuicompressor-2.4.8.jar scripts/behavior.js -o /workspaces\cbh\build\app\scripts\behavior.js --charset utf-8
