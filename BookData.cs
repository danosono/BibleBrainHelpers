using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Gg
{
    [Serializable]
    public class BookData
    {
        public List<BookDetails> data;
    }

    [Serializable]
    public class BookDetails
    {
        public string book_id;
        public string book_id_usfx;
        public string book_id_osis;
        public string name;
        public string testament;
        [CanBeNull] public int testament_order;
        public string book_order;
        public string book_group;
        public string name_short;
        public int[] chapters;
    }
}