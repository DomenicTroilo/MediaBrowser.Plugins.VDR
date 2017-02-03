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
    public class RecordingResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public RecordingResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IEnumerable<RecordingInfo> GetRecordings(Stream stream, IJsonSerializer json,ILogger logger)
        {
	    logger.Info("[VDR] Start GetRecordings");
            var root = json.DeserializeFromStream<RootObject>(stream);
	    logger.Info("[VDR] got Root Object");
//	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
// 	    List<RecordingInfo> channels = new List<RecordingInfo>();
            if (root != null && root.recordings != null)
            {
		logger.Info("[VDR] Parse Recording Response");
                //UtilsHelper.DebugInformation(logger,string.Format("[VDR] ChannelResponse: {0}", json.SerializeToString(root)));
	 	return root.recordings.Select(i => new RecordingInfo
 	           {
                Id = i.number.ToString(),
                ChannelId = i.channel_id,
                Name = i.event_title,
                EpisodeTitle = i.event_short_text,
                Overview = i.event_description,
                StartDate = ApiHelper.DateTimeFromUnixTimestampSeconds(i.event_start_time).ToUniversalTime(),
                EndDate = ApiHelper.DateTimeFromUnixTimestampSeconds(i.event_start_time).ToUniversalTime().AddSeconds(i.event_duration)
                });
	    }
	    else
		{
		logger.Info("[VDR] Parse Recording Response failed");
                logger.Info(string.Format("[VDR] RecordingResponse: {0}", json.SerializeToString(root)));
		}
            return new List<RecordingInfo>();
        }

	public class Recording
	{
    	public int number { get; set; }
    	public string name { get; set; }
    	public string file_name { get; set; }
    	public string relative_file_name { get; set; }
    	public string inode { get; set; }
    	public bool is_new { get; set; }
    	public bool is_edited { get; set; }
    	public bool is_pes_recording { get; set; }
    	public int duration { get; set; }
    	public int filesize_mb { get; set; }
    	public string channel_id { get; set; }
    	public double frames_per_second { get; set; }
    	public List<object> marks { get; set; }
    	public string event_title { get; set; }
    	public string event_short_text { get; set; }
    	public string event_description { get; set; }
    	public int event_start_time { get; set; }
    	public int event_duration { get; set; }
    	public object additional_media { get; set; }
    	public string aux { get; set; }
    	public string sync_action { get; set; }
    	public string hash { get; set; }
	}

	public class RootObject
	{
    	public List<Recording> recordings { get; set; }
    	public int count { get; set; }
    	public int total { get; set; }
	}
    }
}
