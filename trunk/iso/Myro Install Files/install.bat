msiexec /i python-2.4.4.msi
msiexec /i SAPI5VoiceInstaller.msi
pyserial-2.2.win32.exe
pywin32-210.win32-py2.4.exe
myro-1.0.0.win32.exe
PIL-1.1.5.win32-py2.4.exe
pyTTS-3.0.win32-py2.4.exe
xmpppy-0.4.0.win32.exe
copy /Y snack\tkSnack.py c:\Python24\Lib\
copy /Y snack\snacklib\ c:\Python24\tcl\
copy /Y "Start Python.pyw"  "c:\Documents and Settings\%USERNAME%\Desktop\"