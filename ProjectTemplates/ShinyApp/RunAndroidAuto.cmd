: https://cvstrydom.co.za
: This script forwards your ADB port and launches the Android Auto Desktop Head Unit if it's installed in the default location
: Be sure to start the head unit server on your Android phone first
adb forward tcp:5277 tcp:5277
start "" "%USERPROFILE%\AppData\Local\Android\Sdk\extras\google\auto\desktop-head-unit.exe"