
set target="D:\Code\MusicBrowser2\Resource\plugins"

copy D:\Code\MusicBrowser2\PlugIns\BasicViews\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\EchoNest\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\HTBackdrop\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\LastFM\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\MediaBrowserXMLReader\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\Billboard\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\FanArtTV\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\FileSystem\bin\Release\plugin.*.dll %target%
copy D:\Code\MusicBrowser2\PlugIns\TheTVDB\bin\Release\plugin.*.dll %target%

copy %target%\*.dll "C:\ProgramData\MusicBrowser\PlugIn"



pause


