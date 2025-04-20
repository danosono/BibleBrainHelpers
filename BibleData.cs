using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace Gg
{
    [Serializable]
    public class BibleData
    {
        public List<BibleDetails> data;
        public BibleMeta meta;
    }

    [Serializable]
    public class BibleDetails
    {
        public string abbr;
        public string name;
        public string vname;
        public string language;
        public string autonym;
        public int language_id;
        public string language_rolv_code;
        public string iso;
        public string date;
        public Filesets filesets;
    }

    [Serializable]
    public class Filesets
    {
        [JsonProperty("dbp-prod")]
        public List<Prod> prod;
    
        [JsonProperty("dbp-vid")] 
        public List<Vid> vid;
    }


    [Serializable]
    public class Prod
    {
        public string id;
        public string type;
        public string size;
        public string stock_no;
        public string bitrate;
        public string codec;
        public string container;
        public string timing_est_err;
        public string volume;
    }

    [Serializable]
    public class Vid
    {
        public string id;
        public string type;
        public string size;
        public string stock_no;
        public string volume;
    
        [JsonProperty("youtube_playlist_id:JHN")]
        public string youtube_playlist_id_JHN;
    
        [JsonProperty("youtube_playlist_id:LUK")]
        public string youtube_playlist_id_LUK;
    
        [JsonProperty("youtube_playlist_id:MAT")]
        public string youtube_playlist_id_MAT;
    
        [JsonProperty("youtube_playlist_id:MRK")]
        public string youtube_playlist_id_MRK;

    }

    [Serializable]
    public class BibleMeta
    {
        public BiblePagination pagination;
    }

    [Serializable]
    public class BiblePagination
    {
        public int total;
        public int per_page;
        public int current_page;
        public int last_page;
        public string next_page_url;
        public string prev_page_url;
        public int from;
        public int to;
    }
}