using System;
using System.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.VDR.Helpers;

namespace MediaBrowser.Plugins.VDR.Responses
{
    public class InstantiateResponse
    {
        public ClientKeys GetClientKeys(Stream stream, IJsonSerializer json,ILogger logger)
        {
            var root = json.DeserializeFromStream<RootObject>(stream);

            logger.Error("[VDR] Initialize Version {0}", root.clientKeys.version);
            if (root.clientKeys != null && root.clientKeys.version != null)
            {
                UtilsHelper.DebugInformation(logger,string.Format("[VDR] ClientKeys: {0}", json.SerializeToString(root)));
                return root.clientKeys;
            }
            logger.Error("[VDR] Failed to load the ClientKeys from VDR.");
            throw new ApplicationException("Failed to load the ClientKeys from VDR.");
        }

        public class ClientKeys
        {
            public string version { get; set; }
        }

        private class RootObject
        {
            public ClientKeys clientKeys { get; set; }
        }
    }
}
