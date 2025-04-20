using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Gg
{
    [Serializable]
    public class CountryData
    {
        public CountryDetails data;
    }
    
    [Serializable]
    public class CountryDetails
    {
        public string name;
        public string introduction;
        public string continent_code;
        
        // List of maps

        public List<CountryLanguages> languages;
    }

    public class CountryLanguages
    {
        public string name;
        public string iso;
        public List<CountryBibles> bibles;
    }

    public class CountryBibles
    {
        
    }

    public class CountryCodes
    {
        public string fips;
        public string iso_a3;
        public string iso_a2;
    }
    
}
