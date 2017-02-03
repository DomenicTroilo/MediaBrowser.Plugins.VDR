using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.VDR.Helpers;

namespace MediaBrowser.Plugins.VDR.Responses
{
    class ListingsResponse
    {
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly string _baseUrl;

        public ListingsResponse(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public IEnumerable<ProgramInfo> GetPrograms(Stream stream, IJsonSerializer json, string channelId, ILogger logger)
        {
            var root = json.DeserializeFromStream<RootObject>(stream);
            UtilsHelper.DebugInformation(logger,string.Format("[VDR] GetPrograms Response: {0}",json.SerializeToString(root)));
//  	    logger.Info(string.Format("[VDR] Display Root Object: {0}", json.SerializeToString(root)));
	    if (root != null && root.events != null)
            {
		return root.events.Select(epg => new ProgramInfo()
                {
                ChannelId = channelId,
                Id = epg.channel + epg.id.ToString(),
                Overview = epg.description,
                StartDate = ApiHelper.DateTimeFromUnixTimestampSeconds(epg.start_time).ToUniversalTime(),
                EndDate =  ApiHelper.DateTimeFromUnixTimestampSeconds(epg.start_time).ToUniversalTime().AddSeconds(epg.duration),
                Genres = new List<string>(),
                OriginalAirDate = null,
                //OriginalAirDate = parse infom from Description
                Name = epg.title,
                OfficialRating = null,
                //OfficialRating = parse infom from Description
                //OfficialRating = "G",
                //CommunityRating = null, // not provided
                //CommunityRating = 10,
                EpisodeTitle = epg.short_text,
                Audio = ParseAudio(epg.description),
                IsHD = epg.description.ToLower().Contains(" hd ") || epg.channel_name.ToLower().Contains("hd"),
                IsRepeat = false,
                IsSeries = epg.description.Contains("series", StringComparison.OrdinalIgnoreCase),
                HasImage = (epg.images > 0),
                ImageUrl = (epg.images > 0) ?  (_baseUrl + "/events/image/1/" + epg.id.ToString(_usCulture)) : null,
                IsNews = epg.description.Contains("news.", StringComparison.OrdinalIgnoreCase) ||
		    epg.contents.FindAll(str => str.ToLower().Contains("news")).Count > 0,
                IsMovie = epg.description.Contains("movie.", StringComparison.OrdinalIgnoreCase) ||
		    epg.contents.FindAll(str => str.ToLower().Contains("movie")).Count > 0,
		IsKids = epg.description.Contains("children.", StringComparison.OrdinalIgnoreCase) ||
		    epg.contents.FindAll(str => str.ToLower().Contains("kid")).Count > 0,

                IsSports = epg.description.Contains("sports", StringComparison.OrdinalIgnoreCase) ||
                    epg.description.Contains("Sports non-event", StringComparison.OrdinalIgnoreCase) ||
                    epg.description.Contains("Sports event", StringComparison.OrdinalIgnoreCase) ||
                    epg.description.Contains("Sports talk", StringComparison.OrdinalIgnoreCase) ||
                    epg.description.Contains("Sports news", StringComparison.OrdinalIgnoreCase) ||
		    epg.contents.FindAll(str => str.ToLower().Contains("sport")).Count > 0
                });
	      }
		return new List<ProgramInfo>();
	    }

        public static float? ParseCommunityRating(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var hasPlus = value.IndexOf('+') != -1;

                var rating = value.Replace("+", string.Empty).Length + (hasPlus ? .5 : 0);

                return (float)rating;
            }

            return null;
        }

        public static ProgramAudio? ParseAudio(string value)
        {
            if (value.Contains("stereo", StringComparison.OrdinalIgnoreCase))
            {
                return ProgramAudio.Stereo;
            }

            return null;
        }


        // Classes created with http://json2csharp.com/

        private class Event
        {
            public int id { get; set; } //OID
            public string title { get; set; } //Title
	    public string short_text { get; set; } // ShortText
            public string description { get; set; } //Desc
            public int start_time { get; set; } //StartTime
            public string channel { get; set; } //ChannelOID
	    public string channel_name { get; set; }//Channel_name
            public int duration { get; set; } // EndTime
	    public int table_id { get; set; }
	    public int version { get; set; }
            public int images { get; set; } // FanArt
            public bool timer_exists { get; set; } // HasSchedule
            public bool timer_active { get; set; } // HasActiveSchedule
            public string timer_id { get; set; } // Schedule_id
            public int parental_rating { get; set; } //Parental_rating
            public int vps { get; set; }
            public List<object> components { get; set; }
            public List<string> contents { get; set; }
            public List<int> raw_contents { get; set; }
            public object additional_media { get; set; }
        }

        private class RootObject
        {
           public int count { get; set; }
    	   public int total { get; set; }
    	   public List<Event> events { get; set; }
        }

    }
}
