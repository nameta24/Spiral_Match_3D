using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Voodoo.Sauce.Internal;
using Voodoo.Sauce.Internal.EnvironmentSettings;

// ReSharper disable once CheckNamespace
namespace Voodoo.Analytics
{
    internal class Tracker
    {
        private const string TAG = "Analytics - Tracker";
        private const string FilePrefix = "VoodooAnalytics-FileInProcess-";
        private static readonly string PersistenceFolder = Application.persistentDataPath + "/VoodooAnalyticsSDK/";
        private readonly int[] _backOffDelays = {0, 4, 8, 15, 30, 60, 90};

        private readonly ConcurrentDictionary<string, FileProcessInfo> _queue = new ConcurrentDictionary<string, FileProcessInfo>();
        private static readonly object FileAccess = new object();

        private string _lastFilePath;
        private int _eventNumberInFile;
        private IConfig _config;

        private static Tracker _instance;
        public static Tracker Instance => _instance ?? (_instance = new Tracker());

        internal void Init(IConfig config, string proxyServer)
        {
            _config = config;

            UpdateCurrentEventsFileName();

            var rootFolder = new DirectoryInfo(PersistenceFolder);
            if (!rootFolder.Exists) {
                rootFolder.Create();
            }

            // GetURL relies on UnityEngine.PlayerPrefs.GetInt 
            // This cannot be run outside the main thread 
            // So it needs to be called here 
            AnalyticsApi.SetAnalyticsGatewayUrl(
                EnvironmentSettings.GetURL("https://vs-api.voodoo-{0}.io/push-analytics",
                    EnvironmentSettings.Api.Analytics));
            AnalyticsApi.ProxyServer = proxyServer;

            var flushEventsTimer = new System.Timers.Timer(config.GetSenderWaitIntervalSeconds() * 1000);
            flushEventsTimer.Elapsed += (sender, args) => {
                try {
                    OnTimedEvent();
                } catch (Exception exception) {
                    // Unfortunately C# Timers catch every exception and make them silent.
                    // That's why I am catching and logging here any exceptions. We shouldn't miss them.
                    VoodooLog.LogE("ANALYTICS", exception.Message);
                }
            };
            flushEventsTimer.AutoReset = true;
            flushEventsTimer.Enabled = true;
        }

        private void OnTimedEvent()
        {
            RetrieveAndSendEvents();
        }

        private void UpdateCurrentEventsFileName()
        {
            AnalyticsLog.Log(TAG, "Events file name updated");
            _lastFilePath = PersistenceFolder + "events_" + new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + ".jsonl";
            _eventNumberInFile = 0;
        }

        private void RetrieveAndSendEvents()
        {
            AnalyticsLog.Log(TAG, "Retrieve events");

            var info = new DirectoryInfo(PersistenceFolder);
            FileInfo[] files;
            try
            {
                files = info.GetFiles().OrderBy(p => p.CreationTimeUtc).ToArray();
            }
            catch (UnauthorizedAccessException)
            {
                VoodooLog.LogE("ANALYTICS", "The user doesn't have access to " + PersistenceFolder);
                return;
            }

            if (files.Length > 0) {
                UpdateCurrentEventsFileName();
            }

            foreach (FileInfo file in files) {
                if (!SaveFileToSend(file)) {
                    continue;
                }

                var events = new List<string>();
                lock (FileAccess) {
                    AnalyticsLog.Log(TAG, "Found '" + file.Name + "' File");
                    try {
                        using (StreamReader streamReader = File.OpenText(file.FullName)) {
                            string jsonString = streamReader.ReadToEnd();
                            string[] jsonStringArray = jsonString.Split('\n');
                            events.AddRange(jsonStringArray.Where(value => value != ""));
                        }
                        
                    } 
                    catch (Exception e) {
                        //Only log in crashlytics if its not FileNotFoundException
                        if (!(e is FileNotFoundException || e is IOException || e is UnauthorizedAccessException)) {
                        }

                        VoodooLog.LogE("ANALYTICS", $"Error when reading file: {e.Message}");
                    }
                }

                SendAndDeleteFiles(events, file);
            }
        }

        private bool SaveFileToSend(FileSystemInfo file)
        {
            if (!_queue.TryGetValue(FilePrefix + file.Name, out FileProcessInfo fileProcessInfo)) {
                // The file is new.
                fileProcessInfo = new FileProcessInfo {
                    Status = FileProcessStatus.InProcess,
                    NumberOfTries = 0
                };
                return _queue.TryAdd(FilePrefix + file.Name, fileProcessInfo);
            }
            
            // The file is already present in the queue.
            
            if (fileProcessInfo.Status == FileProcessStatus.InProcess) {
                AnalyticsLog.Log(TAG, "The file '" + file.Name + "' is being processed");
                return false;
            }
            if (fileProcessInfo.NextProcessDate != null && fileProcessInfo.NextProcessDate > DateTime.Now) {
                AnalyticsLog.Log(TAG, "Too early to reprocess the file '" + file.Name + "'");
                return false;
            }

            return true;
        }

        private void SendAndDeleteFiles(List<string> events, FileSystemInfo file)
        {
            if (events.Count == 0) {
                return;
            }

            AnalyticsLog.Log(TAG, "Send and delete file '" + file.Name + "' File");

            AnalyticsApi.SendEvents(events, succeeded => {
                if (succeeded) {
                    lock (FileAccess) {
                        AnalyticsLog.Log(TAG, "Delete file: '" + file.Name + "' File");

                        if (_queue.ContainsKey(FilePrefix + file.Name)) {
                            _queue.TryRemove(FilePrefix + file.Name, out _);
                        }

                        file.Delete();
                    }
                } else if (_queue.ContainsKey(FilePrefix + file.Name)) {
                    FileProcessInfo fileProcessInfo = _queue[FilePrefix + file.Name];
                    fileProcessInfo.Status = FileProcessStatus.Waiting;
                    fileProcessInfo.NumberOfTries++;
                    int delay = _backOffDelays.Last();
                    if (fileProcessInfo.NumberOfTries < _backOffDelays.Length) {
                        delay = _backOffDelays[fileProcessInfo.NumberOfTries];
                    }

                    AnalyticsLog.Log(TAG, "Retry pushing '" + file.Name + "' file in " + delay + " seconds");

                    fileProcessInfo.NextProcessDate = DateTime.Now.AddSeconds(delay);
                }
            });
        }

        internal async Task TrackEvent(Event e)
        {
            string[] enabledEvents = _config.EnabledEvents();
            if (enabledEvents.Length > 0 && !enabledEvents.Contains(e.GetName()))
            {
                AnalyticsLog.Log(TAG, $"Event ignored: {e.GetName()}");
                return;
            }

            await Task.Run(() =>
            {
                lock (FileAccess)
                {
                    string jsonString = e.ToJson();
                    AnalyticsLog.Log(TAG, "Track event: " + jsonString);
                    try
                    {
                        using (StreamWriter streamWriter = File.AppendText(_lastFilePath))
                        {
                            streamWriter.Write(jsonString + "\n");
                        }

                        _eventNumberInFile++;

                        if (_eventNumberInFile > _config.GetMaxNumberOfEventsPerFile())
                        {
                            UpdateCurrentEventsFileName();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is IOException))
                        {
                        }
                        else if (ex is UnauthorizedAccessException)
                        {
                            // Since this file access is locked, let's try to write the next events on another file.
                            UpdateCurrentEventsFileName();
                        }

                        VoodooLog.LogE("ANALYTICS", ex.Message);
                    }
                }
            });
        }

        private enum FileProcessStatus
        {
            Waiting = 0,
            InProcess = 1,
        }

        private class FileProcessInfo
        {
            public FileProcessStatus Status;
            public DateTime? NextProcessDate;
            public int NumberOfTries;
        }
    }
}