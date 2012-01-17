%windir%\eHome\McmlVerifier.exe -verbose -assemblyredirect:"G:\Code\MusicBrowser2\MusicBrowser2\bin\Debug" -directory:"G:\Code\MusicBrowser2\PlugIns\WhiteTheme\Markup"

copy bin\release\*.dll %windir%\eHome

%SystemRoot%\ehome\ehshell.exe /entrypoint:{46572B5B-2B75-40B3-B48A-F7A308846CB7}\{49233D7D-3063-4B89-801E-C038ADEE6EF9} /nostartupanimation

