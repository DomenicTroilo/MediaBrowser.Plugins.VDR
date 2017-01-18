xbuild
cp bin/Debug/MediaBrowser.Plugins.VDR.dll /var/lib/emby-server/plugins/.
rm /var/lib/emby-server/logs/*
/etc/init.d/emby-server restart
