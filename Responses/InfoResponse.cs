using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.VDR.Helpers;

namespace MediaBrowser.Plugins.VDR.Responses
{
    public class InfoResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public InfoResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public LiveTvServiceStatusInfo GetInfo(Stream stream, IJsonSerializer json,ILogger logger)
        {
	    logger.Info("[VDR] Start GetChannels");
            var root = json.DeserializeFromStream<RootObject>(stream);
	    logger.Info("[VDR] got Root Object");
//	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
	    LiveTvServiceStatusInfo StatusInfo = new LiveTvServiceStatusInfo();
            if (root != null && root.version != null)
            {
		logger.Info("[VDR] Parse Channel Response");
                UtilsHelper.DebugInformation(logger,string.Format("[VDR] InfoResponse: {0}", json.SerializeToString(root)));
		StatusInfo.Status = LiveTvServiceStatus.Ok;
		StatusInfo.Version = root.vdr.version;
		StatusInfo.Tuners = new List<LiveTvTunerInfo>();
	    }
	    else
		{
		logger.Info("[VDR] Parse Channel Response failed");
                logger.Info(string.Format("[VDR] InfoResponse: {0}", json.SerializeToString(root)));
		}
            return StatusInfo;
        }

        // Classes created with http://json2csharp.com/
	
	public class Diskusage
	{
    	public int free_mb { get; set; }
    	public int used_percent { get; set; }
    	public int free_minutes { get; set; }
    	public string description_localized { get; set; }
	}
	
	public class Plugin
	{
    	public string name { get; set; }
    	public string version { get; set; }
	}
	
	public class Device
	{
    	public string name { get; set; }
    	public bool dvb_c { get; set; }
    	public bool dvb_s { get; set; }
    	public bool dvb_t { get; set; }
    	public bool atsc { get; set; }
    	public bool primary { get; set; }
    	public bool has_decoder { get; set; }
    	public int number { get; set; }
    	public string channel_id { get; set; }
    	public string channel_name { get; set; }
    	public int channel_nr { get; set; }
    	public bool live { get; set; }
    	public bool has_ci { get; set; }
    	public int signal_strength { get; set; }
    	public int signal_quality { get; set; }
    	public int str { get; set; }
    	public int snr { get; set; }
    	public int ber { get; set; }
    	public int unc { get; set; }
    	public string status { get; set; }
    	public int adapter { get; set; }
    	public int frontend { get; set; }
    	public string type { get; set; }
	}
	
	public class Vdr
	{
    	public string version { get; set; }
    	public List<Plugin> plugins { get; set; }
    	public List<Device> devices { get; set; }
	}
	
	public class RootObject
	{
    	public string version { get; set; }
    	public int time { get; set; }
    //	public List<Service> services { get; set; }
    	public Diskusage diskusage { get; set; }
    	public Vdr vdr { get; set; }
	}

    }
}
