takeown /f . /r /d y
icacls . /grant "$env:USERDOMAIN\$env:USERNAME:(OI)(CI)F" /t /c /q
icacls . /grant "SYSTEM:(OI)(CI)F" /t /c /q
icacls . /grant "Administrators:(OI)(CI)F" /t /c /q
icacls . /inheritance:e /t /c /q
