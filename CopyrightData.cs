using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class CopyrightData
{
    [CanBeNull] public string id;
    [CanBeNull] public string asset_id;
    [CanBeNull] public string type;
    [CanBeNull] public string size;
    [CanBeNull] public Copyright copyright;
}

[Serializable]
public class Copyright
{
    [CanBeNull] public string copyright_date;
    [CanBeNull] public string copyright;
    [CanBeNull] public string copyright_description;
    [CanBeNull] public string created_at;
    [CanBeNull] public string updated_at;
    public int open_access;
    [CanBeNull] public List<Organizations> organizations;
}

[Serializable]
public class Organizations
{
    public int id;
    [CanBeNull] public string slug;
    [CanBeNull] public string abbreviation;
    [CanBeNull] public string primaryColor;
    [CanBeNull] public string secondaryColor;
    public int inactive;
    [CanBeNull] public string url_facebook;
    [CanBeNull] public string url_website;
    [CanBeNull] public string url_donate;
    [CanBeNull] public string url_twitter;
    [CanBeNull] public string address;
    [CanBeNull] public string address2;
    [CanBeNull] public string city;
    [CanBeNull] public string state;
    [CanBeNull] public string country;
    public int zip;
    [CanBeNull] public string phone;
    [CanBeNull] public string email;
    [CanBeNull] public string email_director;
    public float latitude;
    public float longitude;
    [CanBeNull] public string laravel_through_key;
    [CanBeNull] public List<Logos> logos;
    [CanBeNull] public List<Translations> translations;
}

[Serializable]
public class Logos
{
    public int language_id;
    [CanBeNull] public string language_iso;
    [CanBeNull] public string url;
    public int icon;
}

[Serializable]
public class Translations
{
    public int language_id;
    public int vernacular;
    public int alt;
    [CanBeNull] public string name;
    [CanBeNull] public string description_short;
}