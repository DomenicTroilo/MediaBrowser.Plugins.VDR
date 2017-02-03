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
    public class TimerResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public TimerResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IEnumerable<TimerInfo> GetTimers(Stream stream, IJsonSerializer json,ILogger logger)
        {
	    logger.Info("[VDR] Start GetTimers");
            var root = json.DeserializeFromStream<RootObject>(stream);
	    logger.Info("[VDR] got Root Object");
//	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
            if (root != null && root.timers != null)
            {
		logger.Info("[VDR] Parse Recording Response");
                //UtilsHelper.DebugInformation(logger,string.Format("[VDR] ChannelResponse: {0}", json.SerializeToString(root)));
	 	return root.timers.Select(i => new TimerInfo
 	           {
                Name = i.filename,
                ChannelId = i.channel,
                Id = i.id,
                StartDate = DateTime.Parse(i.start_timestamp),
                EndDate = DateTime.Parse(i.stop_timestamp),
                Priority = i.priority
                });
	    }
	    else
		{
		logger.Info("[VDR] Parse Timer Response failed");
                logger.Info(string.Format("[VDR] TimerResponse: {0}", json.SerializeToString(root)));
		}
            return new List<TimerInfo>();
        }

	public class Timer
	{
    	public string id { get; set; }
    	public int index { get; set; }
    	public int flags { get; set; }
    	public int start { get; set; }
    	public string start_timestamp { get; set; }
    	public string stop_timestamp { get; set; }
    	public int stop { get; set; }
    	public int priority { get; set; }
    	public int lifetime { get; set; }
    	public int event_id { get; set; }
    	public string weekdays { get; set; }
    	public string day { get; set; }
    	public string channel { get; set; }
    	public string filename { get; set; }
    	public string channel_name { get; set; }
    	public bool is_pending { get; set; }
    	public bool is_recording { get; set; }
    	public bool is_active { get; set; }
    	public string aux { get; set; }
	}

	public class RootObject
	{
    	public List<Timer> timers { get; set; }
    	public int count { get; set; }
    	public int total { get; set; }
	}
    }
}
