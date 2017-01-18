using System;
using System.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.VDR.Helpers;

namespace MediaBrowser.Plugins.VDR.Responses
{
    public class InitializeResponse
    {
        public bool LoggedIn(Stream stream, IJsonSerializer json, ILogger logger)
        {
            var root = json.DeserializeFromStream<RootObject>(stream);

            if (root.SIDValidation != null)
            {
                UtilsHelper.DebugInformation(logger,string.Format("[VDR] Connection validation: {0}", json.SerializeToString(root)));
                return root.SIDValidation.validated;
            }
            logger.Error("[VDR] Failed to validate your connection with VDR.");
            throw new ApplicationException("Failed to validate your connection with VDR.");
        }
        
        public class SIDValidation
        {
            public bool validated { get; set; }
        }

        public class RootObject
        {
            public SIDValidation SIDValidation { get; set; }
        }
    }
}
