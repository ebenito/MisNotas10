﻿using Newtonsoft.Json;

namespace OneDriveSimple.Response
{
    public class AudioResponseInfo
    {
        public string Album
        {
            get;
            set;
        }

        public string AlbumArtist
        {
            get;
            set;
        }

        public string Artist
        {
            get;
            set;
        }

        public int BitRate
        {
            get;
            set;
        }

        public string Composers
        {
            get;
            set;
        }

        public string Copyright
        {
            get;
            set;
        }

        public int Disc
        {
            get;
            set;
        }

        public int DiscCount
        {
            get;
            set;
        }

        [JsonProperty("duration")]
        public int DurationMilliSeconds
        {
            get;
            set;
        }

        public string Genre
        {
            get;
            set;
        }

        public bool HasDrm
        {
            get;
            set;
        }

        public bool IsVariableBitrate
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public int Track
        {
            get;
            set;
        }

        public int TrackCount
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }
    }
}