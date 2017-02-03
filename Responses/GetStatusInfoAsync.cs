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
    public class InitiateResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public InitiateResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IEnumerable<ChannelInfo> GetInfo(Stream stream, IJsonSerializer json,ILogger logger)
        {
	    logger.Info("[VDR] Start GetInfo");
            var root = json.DeserializeFromStream<RootObject>(stream);
	    logger.Info("[VDR] got Root Object");
//	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
            if (root != null && root.Channels != null)
            {
		logger.Info("[VDR] Parse Channel Response");
                UtilsHelper.DebugInformation(logger,string.Format("[VDR] InitiateResponse: {0}", json.SerializeToString(root)));
	 	return root.Channels.Select(i => new ChannelInfo
 	           {
                    Name = i.name,
		    ChannelType = i.is_radio ? ChannelType.Radio : ChannelType.TV,
                    Number = i.number.ToString(_usCulture),
                    Id = i.channel_id,
                    ImageUrl = i.image ? string.Format("{0}/channels/image/{1}", _baseUrl, i.channel_id) : null, 
                    HasImage = i.image
                });
	    }
	    else
		{
		logger.Info("[VDR] Parse Channel Response failed");
                logger.Info(string.Format("[VDR] InitiateResponse: {0}", json.SerializeToString(root)));
		}
            return new List<ChannelInfo>();
        }

        // Classes created with http://json2csharp.com/
public class Service
{
    public string path { get; set; }
    public int version { get; set; }
}

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
    public List<Service> services { get; set; }
    public Diskusage diskusage { get; set; }
    public Vdr vdr { get; set; }
}

    }
}
