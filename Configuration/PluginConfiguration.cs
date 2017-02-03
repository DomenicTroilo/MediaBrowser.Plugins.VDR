using System;
using MediaBrowser.Model.Plugins;

namespace MediaBrowser.Plugins.VDR.Configuration
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string WebServiceUrl { get; set; }
        public string StreamPort { get; set; }
        public Boolean EnableDebugLogging { get; set; }
        public string VERSION { get; set; }

        public PluginConfiguration()
        {
            StreamPort = "3000";
            WebServiceUrl = "http://localhost:8002";
            EnableDebugLogging = false;
	    VERSION = "0.4.1";
        }
    }
}
