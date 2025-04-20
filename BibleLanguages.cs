using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Gg
{
    [Serializable]
    public class BibleLanguages
    {
        public List<LanguageDetails> data;
        public LanguageMeta meta;
    }
    
    
    
    [Serializable]
    public class LanguageDetails
    {
        public int id;
        [CanBeNull] public string glotto_id;
        [CanBeNull] public string iso;
        [CanBeNull] public string name;
        [CanBeNull] public string autonym;
        public int bibles;
        public int filesets; 
        public int rolv_code;
    }
    
    

    [Serializable]
    public class LanguageMeta
    {
        [CanBeNull] public LanguagePagination pagination;
    }

    [Serializable]
    public class LanguagePagination
    {
        public int total;
        public int count;
        public int per_page;
        public int current_page;
        public int total_pages;
        //[CanBeNull] public LanguageLinks links;
    }

    [Serializable]
    public class LanguageLinks // if these are ever needed, create a copy of this class and call it BibleLanguagesSearch ; then uncomment this for THIS CLASS (BibleLanguages.cs); use the new class for Searching ; Pagination.Links was commented out because, when finding languages via Search, if there is only one page, links are not provided and the JSON Converter fails to produce a usable class (no data available)
    {
        [CanBeNull] public string previous;
        [CanBeNull] public string next;
    }
}
