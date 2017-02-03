using MediaBrowser.Common.Extensions;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.VDR.Helpers;
using MediaBrowser.Plugins.VDR.Responses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.VDR
{
    /// <summary>
    /// Class LiveTvService
    /// </summary>
    public class LiveTvService : ILiveTvService
    {
        private readonly IHttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly CultureInfo _usCulture = new CultureInfo("en-US");
        private readonly ILogger _logger;
        private int _liveStreams;
        private readonly Dictionary<int, int> _heartBeat = new Dictionary<int, int>();

        private string Sid { get; set; }

        public LiveTvService(IHttpClient httpClient, IJsonSerializer jsonSerializer, ILogger logger)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }


        /// <summary>
        /// Gets the channels async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{IEnumerable{ChannelInfo}}.</returns>
        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
        {
            _logger.Info("[VDR] Start GetChannels Async, retrieve all channels");
            //await EnsureConnectionAsync(cancellationToken).ConfigureAwait(false);

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/channels.json", baseUrl)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new ChannelResponse(Plugin.Instance.Configuration.WebServiceUrl).GetChannels(stream, _jsonSerializer, _logger).ToList();
            }
        }

        /// <summary>
        /// Gets the Recordings async
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{IEnumerable{RecordingInfo}}</returns>
        public async Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
        {
            //_logger.Info("[VDR] Start GetRecordings Async, retrieve all 'Pending', 'Inprogress' and 'Completed' recordings ");

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/recordings.json", baseUrl)
            };


            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new RecordingResponse(Plugin.Instance.Configuration.WebServiceUrl).GetRecordings(stream, _jsonSerializer, _logger).ToList();
            }
        }

        /// <summary>
        /// Delete the Recording async from the disk
        /// </summary>
        /// <param name="recordingId">The recordingId</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        public async Task DeleteRecordingAsync(string recordingId, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start Delete Recording Async for recordingId: {0}", recordingId));
            _logger.Info("[VDR] Start Delete Recording NOT IMPLIMENTED");
            throw new NotImplementedException();

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/Delete/{1}?sid={2}", baseUrl, recordingId, Sid)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                bool? error = new CancelDeleteRecordingResponse().RecordingError(stream, _jsonSerializer, _logger);

                if (error == null || error == true)
                {
                    _logger.Error(string.Format("[VDR] Failed to delete the recording for recordingId: {0}", recordingId));
                    throw new ApplicationException(string.Format("Failed to delete the recording for recordingId: {0}", recordingId));
                }
                _logger.Info("[VDR] Deleted Recording with recordingId: {0}", recordingId);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return "VDR"; }
        }

        /// <summary>
        /// Cancel pending scheduled Recording 
        /// </summary>
        /// <param name="timerId">The timerId</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        public async Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start Cancel Recording Async for recordingId: {0}", timerId));
            _logger.Info("[VDR] Start Cancel Recording Async NOT IMPLIMENTED");
            throw new NotImplementedException();

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/CancelRec/{1}?sid={2}", baseUrl, timerId, Sid)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                bool? error = new CancelDeleteRecordingResponse().RecordingError(stream, _jsonSerializer, _logger);

                if (error == null || error == true)
                {
                    _logger.Error(string.Format("[VDR] Failed to cancel the recording for recordingId: {0}", timerId));
                    throw new ApplicationException(string.Format("Failed to cancel the recording for recordingId: {0}", timerId));
                }
                _logger.Info(string.Format("[VDR] Cancelled Recording for recordingId: {0}", timerId));
            }
        }

        /// <summary>
        /// Create a new recording
        /// </summary>
        /// <param name="info">The TimerInfo</param>
        /// <param name="cancellationToken">The cancellationToken</param>
        /// <returns></returns>
        public async Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start CreateTimer Async for ChannelId: {0} & Name: {1}", info.ChannelId, info.Name));
            _logger.Info("[VDR] Start CreateTimer Async for ChannelId NOT IMPLIMENTED");
            throw new NotImplementedException();

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/Record?sid={1}", baseUrl, Sid)
            };

            var timerSettings = await GetDefaultScheduleSettings(cancellationToken).ConfigureAwait(false);

            timerSettings.allChannels = false;
            timerSettings.ChannelOID = int.Parse(info.ChannelId, _usCulture);

            if (!string.IsNullOrEmpty(info.ProgramId))
            {
                timerSettings.epgeventOID = int.Parse(info.ProgramId, _usCulture);
            }

            timerSettings.post_padding_min = info.PostPaddingSeconds / 60;
            timerSettings.pre_padding_min = info.PrePaddingSeconds / 60;

            var postContent = _jsonSerializer.SerializeToString(timerSettings);
            UtilsHelper.DebugInformation(_logger, string.Format("[VDR] TimerSettings CreateTimer: {0} for ChannelId: {1} & Name: {2}", postContent, info.ChannelId, info.Name));

            options.RequestContent = postContent;
            options.RequestContentType = "application/json";

            try
            {
                await _httpClient.Post(options).ConfigureAwait((false));
            }
            catch (HttpException ex)
            {
                _logger.Error(string.Format("[VDR] CreateTimer async with exception: {0}", ex.Message));
                throw new LiveTvConflictException();
            }
        }

        /// <summary>
        /// Get the pending Recordings.
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
        {
            //_logger.Info("[VDR] Start GetTimers Async, retrieve all 'Pending', 'Inprogress' and 'Completed' recordings ");
            _logger.Info("[VDR] Start GetTimers Async, NOT IMPLIMENTED");

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/timers.json", baseUrl)
            };


            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new TimerResponse(Plugin.Instance.Configuration.WebServiceUrl).GetTimers(stream, _jsonSerializer, _logger).ToList();
            }

        }

        /// <summary>
        /// Get the recurrent recordings
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
        {
            //_logger.Info("[VDR] Start GetSeriesTimer Async, retrieve the recurring recordings");
            _logger.Info("[VDR] Start GetSeriesTimer Async, retrieve the recurring recordings NOT IMPLIMENTED");
            throw new NotImplementedException();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ManageService/Get/SortedFilteredList?sid={1}", baseUrl, Sid)
            };

            var filterOptions = new
            {
                resultLimit = -1,
                All = false,
                None = false,
                Pending = false,
                InProgress = false,
                Completed = false,
                Failed = false,
                Conflict = false,
                Recurring = true,
                Deleted = false
            };

            var postContent = _jsonSerializer.SerializeToString(filterOptions);

            options.RequestContent = postContent;
            options.RequestContentType = "application/json";

            var response = await _httpClient.Post(options).ConfigureAwait(false);

            using (var stream = response.Content)
            {
                //return new RecordingResponse(baseUrl).GetSeriesTimers(stream, _jsonSerializer, _logger);
            }
        }

        /// <summary>
        /// Create a recurrent recording
        /// </summary>
        /// <param name="info">The recurrend program info</param>
        /// <param name="cancellationToken">The CancelationToken</param>
        /// <returns></returns>
        public async Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start CreateSeriesTimer Async for ChannelId: {0} & Name: {1}", info.ChannelId, info.Name));
            _logger.Info("[VDR] Start CreateSeriesTimer Async for ChannelId: NOT IMPLIMENTED");
            throw new NotImplementedException();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/Record?sid={1}", baseUrl, Sid)
            };

            var timerSettings = await GetDefaultScheduleSettings(cancellationToken).ConfigureAwait(false);

            timerSettings.allChannels = info.RecordAnyChannel;
            timerSettings.onlyNew = info.RecordNewOnly;
            timerSettings.recurringName = info.Name;
            timerSettings.recordAnyTimeslot = info.RecordAnyTime;

            if (!info.RecordAnyTime)
            {
                timerSettings.startDate = info.StartDate.ToString(_usCulture);
                timerSettings.endDate = info.EndDate.ToString(_usCulture);
                timerSettings.recordThisTimeslot = true;
            }

            if (info.Days.Count == 1)
            {
                timerSettings.recordThisDay = true;
            }

            if (info.Days.Count > 1 && info.Days.Count < 7)
            {
                timerSettings.recordSpecificdays = true;
            }

            timerSettings.recordAnyDay = info.Days.Count == 7;
            timerSettings.daySunday = info.Days.Contains(DayOfWeek.Sunday);
            timerSettings.dayMonday = info.Days.Contains(DayOfWeek.Monday);
            timerSettings.dayTuesday = info.Days.Contains(DayOfWeek.Tuesday);
            timerSettings.dayWednesday = info.Days.Contains(DayOfWeek.Wednesday);
            timerSettings.dayThursday = info.Days.Contains(DayOfWeek.Thursday);
            timerSettings.dayFriday = info.Days.Contains(DayOfWeek.Friday);
            timerSettings.daySaturday = info.Days.Contains(DayOfWeek.Saturday);

            if (!info.RecordAnyChannel)
            {
                timerSettings.ChannelOID = int.Parse(info.ChannelId, _usCulture);
            }

            if (!string.IsNullOrEmpty(info.ProgramId))
            {
                timerSettings.epgeventOID = int.Parse(info.ProgramId, _usCulture);
            }

            timerSettings.post_padding_min = info.PostPaddingSeconds / 60;
            timerSettings.pre_padding_min = info.PrePaddingSeconds / 60;

            var postContent = _jsonSerializer.SerializeToString(timerSettings);
            UtilsHelper.DebugInformation(_logger, string.Format("[VDR] TimerSettings CreateSeriesTimer: {0} for ChannelId: {1} & Name: {2}", postContent, info.ChannelId, info.Name));

            options.RequestContent = postContent;
            options.RequestContentType = "application/json";

            try
            {
                await _httpClient.Post(options).ConfigureAwait((false));
            }
            catch (HttpException ex)
            {
                _logger.Error(string.Format("[VDR] CreateSeries async with exception: {0} ", ex.Message));
                throw new LiveTvConflictException();
            }
        }

        /// <summary>
        /// Update the series Timer
        /// </summary>
        /// <param name="info">The series program info</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start UpdateSeriesTimer Async for ChannelId: {0} & Name: {1}", info.ChannelId, info.Name));
            _logger.Info("[VDR] Start UpdateSeriesTimer Async for ChannelId: NOT IMPLIMENTED");
            throw new NotImplementedException();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/UpdateRecurr?sid={1}", baseUrl, Sid)
            };

            var timerSettings = await GetDefaultScheduleSettings(cancellationToken).ConfigureAwait(false);

            timerSettings.recurrOID = int.Parse(info.Id);
            timerSettings.post_padding_min = info.PostPaddingSeconds / 60;
            timerSettings.pre_padding_min = info.PrePaddingSeconds / 60;
            timerSettings.recurringName = info.Name;
            timerSettings.keep_all_days = true;
            timerSettings.days_to_keep = 0;
            timerSettings.extend_end_time_min = 0;

            var postContent = _jsonSerializer.SerializeToString(timerSettings);
            UtilsHelper.DebugInformation(_logger, string.Format("[VDR] TimerSettings UpdateSeriesTimer: {0} for ChannelId: {1} & Name: {2}", postContent, info.ChannelId, info.Name));

            options.RequestContent = postContent;
            options.RequestContentType = "application/json";

            try
            {
                await _httpClient.Post(options).ConfigureAwait((false));
            }
            catch (HttpException ex)
            {
                _logger.Error(string.Format("[VDR] UpdateSeries async with exception: {0}", ex.Message));
                throw new LiveTvConflictException();
            }
        }

        /// <summary>
        /// Update a single Timer
        /// </summary>
        /// <param name="info">The program info</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task UpdateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start UpdateTimer Async for ChannelId: {0} & Name: {1}", info.ChannelId, info.Name));
            _logger.Info("[VDR] Start UpdateTimer Async for ChannelId: NOT IMPLIMENTED");
            throw new NotImplementedException();

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/UpdateRec?sid={1}", baseUrl, Sid)
            };

            var timerSettings = await GetDefaultScheduleSettings(cancellationToken).ConfigureAwait(false);

            timerSettings.scheduleOID = int.Parse(info.Id);
            timerSettings.post_padding_min = info.PostPaddingSeconds / 60;
            timerSettings.pre_padding_min = info.PrePaddingSeconds / 60;

            var postContent = _jsonSerializer.SerializeToString(timerSettings);
            UtilsHelper.DebugInformation(_logger, string.Format("[VDR] TimerSettings UpdateTimer: {0} for ChannelId: {1} & Name: {2}", postContent, info.ChannelId, info.Name));

            options.RequestContent = postContent;
            options.RequestContentType = "application/json";

            try
            {
                await _httpClient.Post(options).ConfigureAwait((false));
            }
            catch (HttpException ex)
            {
                _logger.Error(string.Format("[VDR] UpdateTimer Async with exception: {0}", ex.Message));
                throw new LiveTvConflictException();
            }
        }

        /// <summary>
        /// Cancel the Series Timer
        /// </summary>
        /// <param name="timerId">The Timer Id</param>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        public async Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            //_logger.Info(string.Format("[VDR] Start Cancel SeriesRecording Async for recordingId: {0}", timerId));
            _logger.Info("[VDR] Start Cancel SeriesRecording Async for recordingId: NOT IMPLIMENTED");
            throw new NotImplementedException();

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/CancelRecurr/{1}?sid={2}", baseUrl, timerId, Sid)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                bool? error = new CancelDeleteRecordingResponse().RecordingError(stream, _jsonSerializer, _logger);

                if (error == null || error == true)
                {
                    _logger.Error(string.Format("[VDR] Failed to cancel the recording with recordingId: {0}", timerId));
                    throw new ApplicationException(string.Format("Failed to cancel the recording with recordingId: {0}", timerId));
                }
                _logger.Info("[VDR] Cancelled Recording for recordingId: {0}", timerId);
            }
        }

        /// <summary>
        /// Get the DefaultScheduleSettings
        /// </summary>
        /// <param name="cancellationToken">The CancellationToken</param>
        /// <returns></returns>
        private async Task<ScheduleSettings> GetDefaultScheduleSettings(CancellationToken cancellationToken)
        {
            //_logger.Info("[VDR] Start GetDefaultScheduleSettings");
            _logger.Info("[VDR] Start GetDefaultScheduleSettings NOT IMPLIMENTED");
            throw new NotImplementedException();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/public/ScheduleService/Get/SchedSettingsObj?sid={1}", baseUrl, Sid)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new TimerDefaultsResponse().GetScheduleSettings(stream, _jsonSerializer);
            }
        }

        public Task<List<MediaSourceInfo>> GetChannelStreamMediaSources(string channelId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<MediaSourceInfo>> GetRecordingStreamMediaSources(string recordingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<MediaSourceInfo> GetChannelStream(string channelOid, string mediaSourceId, CancellationToken cancellationToken)
        {
            _logger.Info("[VDR] Start ChannelStream");
            var config = Plugin.Instance.Configuration;
	    //var strb = StringBuilder();
            //strb = Plugin.Instance.Configuration.WebServiceUrl.ToString();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl.Substring(0,Plugin.Instance.Configuration.WebServiceUrl.IndexOf(":", Plugin.Instance.Configuration.WebServiceUrl.IndexOf(":") + 1)) + ":" + Plugin.Instance.Configuration.StreamPort;
            _liveStreams++;

            string streamUrl = string.Format("{0}/TS/{1}", baseUrl, channelOid);
            _logger.Info("[VDR] Streaming " + streamUrl);
            return new MediaSourceInfo
            {
                Id = _liveStreams.ToString(CultureInfo.InvariantCulture),
                Path = streamUrl,
                Protocol = MediaProtocol.Http,
                MediaStreams = new List<MediaStream>
                        {
                            new MediaStream
                            {
                                Type = MediaStreamType.Video,
                                IsInterlaced = true,
                                // Set the index to -1 because we don't know the exact index of the video stream within the container
                                Index = -1,
                            },
                            new MediaStream
                            {
                                Type = MediaStreamType.Audio,
                                IsInterlaced = true,
                                // Set the index to -1 because we don't know the exact index of the audio stream within the container
                                Index = -1
                            }
                        },
                // This takes too long
                SupportsProbing = false
            };
        }

        public async Task<MediaSourceInfo> GetRecordingStream(string recordingId, string mediaSourceId, CancellationToken cancellationToken)
        {
            //_logger.Info("[VDR] Start GetRecordingStream");
            var config = Plugin.Instance.Configuration;
            //var strb = StringBuilder();
            //strb = Plugin.Instance.Configuration.WebServiceUrl.ToString();
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl.Substring(0,Plugin.Instance.Configuration.WebServiceUrl.IndexOf(":", Plugin.Instance.Configuration.WebServiceUrl.IndexOf(":") + 1)) + ":" + Plugin.Instance.Configuration.StreamPort;
            _liveStreams++;
	    int recordingNo;
	    if (int.TryParse(recordingId, out recordingNo))
	    {
            string streamUrl = string.Format("{0}/{1}.rec.ts", baseUrl, ++recordingNo);
            _logger.Info("[VDR] Streaming " + streamUrl);
            return new MediaSourceInfo
            {
                Id = _liveStreams.ToString(CultureInfo.InvariantCulture),
                Path = streamUrl,
                Protocol = MediaProtocol.Http,
                MediaStreams = new List<MediaStream>
                        {
                            new MediaStream
                            {
                                Type = MediaStreamType.Video,
                                IsInterlaced = true,
                                // Set the index to -1 because we don't know the exact index of the video stream within the container
                                Index = -1,
                            },
                            new MediaStream
                            {
                                Type = MediaStreamType.Audio,
                                IsInterlaced = true,
                                // Set the index to -1 because we don't know the exact index of the audio stream within the container
                                Index = -1
                            }
                        },
                // This takes too long
                SupportsProbing = false
            };
	  }
  	  else
            {
                _logger.Info("[LiveTV.Vdr] Parsing RecordingID failed, recordingId={0}", recordingId);
                return null;
	    }

        }

        public async Task CloseLiveStream(string id, CancellationToken cancellationToken)
        {
            _logger.Info("[VDR] Closing " + id);
            var config = Plugin.Instance.Configuration;
            _liveStreams--;

        }

        public async Task CopyFilesAsync(StreamReader source, StreamWriter destination)
        {
            _logger.Info("[VDR] Start CopyFiles Async");
            char[] buffer = new char[0x1000];
            int numRead;
            while ((numRead = await source.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await destination.WriteAsync(buffer, 0, numRead);
            }
        }

        public async Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo program = null)
        {
            //_logger.Info("[VDR] Start GetNewTimerDefault Async");
  	     return await Task.Factory.StartNew(() =>               
             {
                 return new SeriesTimerInfo
                 {
                     PostPaddingSeconds = 120, //TODO: if it can't be extracted via Restful api, move to config or extend restful api
                     PrePaddingSeconds = 120,
                     RecordAnyChannel = false, // TODO (clarify): from my understanding: important for series timer (let seriestimer look on any channel for creating timers)
                     RecordAnyTime = true,
                     RecordNewOnly = false
                 };
             });
        }

        public async Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
        {
            _logger.Info("[VDR] Start GetPrograms Async, retrieve all Programs");
            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;
            TimeSpan timespan = endDateUtc - startDateUtc;
            var options = new HttpRequestOptions()
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/events/{1}.json?timespan={2}", 
		baseUrl,
                channelId,
                timespan.TotalSeconds.ToString())
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new ListingsResponse(baseUrl).GetPrograms(stream, _jsonSerializer, channelId, _logger).ToList();
            }
        }

        public Task RecordLiveStream(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public event EventHandler DataSourceChanged;

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;

        public async Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(CancellationToken cancellationToken)
        {
            _logger.Info("[VDR LiveTV] GetStatusInfoAsync ...");

        {
            _logger.Info("[VDR] Start GetStatusInfo Async, retrieve all channels");

            var baseUrl = Plugin.Instance.Configuration.WebServiceUrl;

            var options = new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = string.Format("{0}/info.json", baseUrl)
            };

            using (var stream = await _httpClient.Get(options).ConfigureAwait(false))
            {
                return new InfoResponse(Plugin.Instance.Configuration.WebServiceUrl).GetInfo(stream, _jsonSerializer, _logger);
            }
        }

        }

        public string HomePageUrl
        {
            get { return "http://www.VDR.com/"; }
        }

        public Task ResetTuner(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ImageStream> GetChannelImageAsync(string channelId, CancellationToken cancellationToken)
        {
            // Leave as is. This is handled by supplying image url to ChannelInfo
            throw new NotImplementedException();
        }

        public Task<ImageStream> GetProgramImageAsync(string programId, string channelId, CancellationToken cancellationToken)
        {
            // Leave as is. This is handled by supplying image url to ProgramInfo
            throw new NotImplementedException();
        }

        public Task<ImageStream> GetRecordingImageAsync(string recordingId, CancellationToken cancellationToken)
        {
            // Leave as is. This is handled by supplying image url to RecordingInfo
            throw new NotImplementedException();
        }
    }
}
