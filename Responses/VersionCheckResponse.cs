using System;
using System.IO;
using MediaBrowser.Model.Serialization;

namespace MediaBrowser.Plugins.VDR.Responses
{
    public class VersionCheckResponse
    {
        private readonly RootObject _root;
        
        public VersionCheckResponse(Stream stream, IJsonSerializer json)
        {
           _root = json.DeserializeFromStream<RootObject>(stream);

        }
        public Boolean UpdateAvailable()
        {

            if (_root.versionCheck != null)
            {
                return _root.versionCheck.upgradeAvailable;
            }
            throw new ApplicationException("Failed to get the Update Status from VDR.");
        }

        public string ServerVersion()
        {
            if (_root.versionCheck != null)
            {
                return _root.versionCheck.serverVer;
            }
            throw new ApplicationException("Failed to get the Server Version from VDR.");
        }


        public class VersionCheck
        {
            public bool upgradeAvailable { get; set; }
            public string onlineVer { get; set; }
            public string serverVer { get; set; }
        }

        public class RootObject
        {
            public VersionCheck versionCheck { get; set; }
        }
    }
}
