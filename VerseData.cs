using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gg
{
    
    [Serializable]
    public class VerseData
    {
        public List<VerseDetails> data;
    }
    
    [Serializable]
    public class VerseDetails
    {
        public string book_id;
        public string book_name;
        public string book_name_alt;
        public int chapter;
        public string chapter_alt;
        public int verse_start;
        public string verse_start_alt;
        public int verse_end;
        public string verse_end_alt;
        public string verse_text;
        
    }
    
}
