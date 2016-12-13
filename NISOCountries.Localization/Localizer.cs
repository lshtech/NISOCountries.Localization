using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NISOCountries.Core;
using NISOCountries.Localization.Properties;

namespace NISOCountries.Localization
{
  public static class Localizer
  {
    private static IDictionary<string, string> _territoriesDictionary;

    public static LocalizationSettings LocalizationSettings { get; set; }

    public static IEnumerable<IISOCountry> Localize(this IEnumerable<IISOCountry> countries,
      CultureInfo culture, LocalizationSettings localizationSettings)
    {
      LocalizationSettings = localizationSettings;
      return Localize(countries, culture.Name, localizationSettings);
    }

    public static IEnumerable<IISOCountry> Localize(this IEnumerable<IISOCountry> countries,
      string culture, LocalizationSettings localizationSettings)
    {
      LocalizationSettings = localizationSettings;
      return countries.Select(country => Localize(country, culture, localizationSettings)).ToList();
    }

    public static IISOCountry Localize(this IISOCountry country, CultureInfo culture,
      LocalizationSettings localizationSettings)
    {
      LocalizationSettings = localizationSettings;
      return Localize(country, culture.Name, localizationSettings);
    }

    public static IISOCountry Localize(this IISOCountry country, string culture,
      LocalizationSettings localizationSettings)
    {
      LocalizationSettings = localizationSettings;
      return new ISOCountry
      {
        Alpha2 = country.Alpha2,
        Alpha3 = country.Alpha3,
        Numeric = country.Numeric,
        CountryName = GetCountryNameFromIsoCode(country.Alpha2, culture)
      };
    }

    private static string GetCountryNameFromIsoCode(string countryCode, string culture)
    {
      if (_territoriesDictionary == null)
      {
        _territoriesDictionary = new Dictionary<string, string>();
      }
      if (!_territoriesDictionary.ContainsKey(culture))
      {
        GetCountryLocalization(culture);
      }
      var countryName = JsonConvert.DeserializeObject<JObject>(_territoriesDictionary[culture])
        ["main"][culture]["localeDisplayNames"]["territories"][countryCode];
      return countryName?.ToString() ?? "";
    }

    private static void GetCountryLocalization(string culture)
    {
      string territory;
      var urlPath = string.Format(Settings.Default.github, culture);
      territory = LocalizationSettings.CacheMode == Enums.CacheMode.NoCache
        ? GetCountryLocalizationFromStream(urlPath)
        : GetCountryLocalizationFromFile(urlPath, culture);
      if (territory != string.Empty)
      {
        _territoriesDictionary.Add(culture, territory);
      }
    }

    private static string GetCountryLocalizationFromStream(string urlPath)
    {
      using (var client = new WebClient())
      {
        return Encoding.UTF8.GetString(client.DownloadData(urlPath));
      }
    }

    private static string GetCountryLocalizationFromFile(string urlPath, string culture)
    {
      var filePath = LocalizationSettings.CachePath + $"\\{culture}.territories.json";
      if (File.Exists(filePath))
      {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.LastWriteTime.Add(LocalizationSettings.CacheLength) >= DateTime.Now)
        {
          return File.ReadAllText(filePath);
        }
      }
      DownloadFile(urlPath, filePath);
      return File.ReadAllText(filePath);
    }

    private static void DownloadFile(string urlPath, string filePath)
    {
      using (var client = new WebClient())
      {
        if (!Directory.Exists(new FileInfo(filePath).DirectoryName))
        {
          var directoryName = new FileInfo(filePath).DirectoryName;
          if (directoryName != null)
            Directory.CreateDirectory(directoryName);
        }
        client.DownloadFile(urlPath, filePath);
      }
    }
  }
}