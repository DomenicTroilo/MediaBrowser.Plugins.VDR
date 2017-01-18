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
    public class ChannelResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public ChannelResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IEnumerable<ChannelInfo> GetChannels(Stream stream, IJsonSerializer json,ILogger logger)
        {
	    logger.Info("[VDR] Start GetChannels");
            var root = json.DeserializeFromStream<RootObject>(stream);
	    logger.Info("[VDR] got Root Object");
//	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
// 	    List<ChannelInfo> channels = new List<ChannelInfo>();
            if (root != null && root.Channels != null)
            {
		logger.Info("[VDR] Parse Channel Response");
                UtilsHelper.DebugInformation(logger,string.Format("[VDR] ChannelResponse: {0}", json.SerializeToString(root)));
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
                logger.Info(string.Format("[VDR] ChannelResponse: {0}", json.SerializeToString(root)));
		}
            return new List<ChannelInfo>();
        }

        // Classes created with http://json2csharp.com/
        public class Channel
        {
	public string name { get; set; }
	public int number { get; set; }
	public string channel_id { get; set; }
	public bool image { get; set; }
	public string group { get; set; }
	public int tansponder { get; set; }
	public string stream { get; set; }
	public bool is_atsc { get; set; }
	public bool is_cable { get; set; }
	public bool is_terr { get; set; }
	public bool is_sat { get; set; }
	public bool is_radio { get; set; }
	public int index { get; set; }

        }

        public class RootObject
        {
            public List<Channel> Channels { get; set; }
	    public int count { get; set; }
	    public int total { get; set; }
        }

    }
}
