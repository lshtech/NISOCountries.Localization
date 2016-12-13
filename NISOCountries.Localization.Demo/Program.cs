using System;
using System.Collections.Generic;
using NISOCountries.Core;
using NISOCountries.GeoNames;

namespace NISOCountries.Localization.Demo
{
  internal class Program
  {
    private const string CountryCode = "AE";
    private const string ListLanguage = "fr-CA";
    private const string SingleLanguage = "sr-Latn";

    private static void Main(string[] args)
    {
      var countries = GetCountryList();
      var enLookup = new ISOCountryLookup<IISOCountry>(countries);

      var frenchCountries = GetLocalizedCountryList(countries);
      var frLookup = new ISOCountryLookup<IISOCountry>(frenchCountries);

      var srCountry = GetLocalizedCountrySingle(enLookup.GetByAlpha2(CountryCode));

      Console.WriteLine($"AE : {enLookup.GetByAlpha2("AE").CountryName} : {frLookup.GetByAlpha2("AE").CountryName} : {srCountry.CountryName}");
      Console.ReadLine();
    }

    private static IEnumerable<IISOCountry> GetCountryList()
    {
      return new GeonamesISOCountryReader().GetDefault();
    }

    /// <summary>
    /// Gets list of all localized country objects
    /// </summary>
    /// <param name="countries"></param>
    /// <returns></returns>
    private static IEnumerable<IISOCountry> GetLocalizedCountryList(IEnumerable<IISOCountry> countries)
    {
      return countries.Localize(ListLanguage, new LocalizationSettings()
        { CacheLength = new TimeSpan(0, 0, 1, 0), CacheMode = Enums.CacheMode.Cache });
    }


    /// <summary>
    /// Gets single localized country object
    /// </summary>
    /// <param name="country"></param>
    /// <returns></returns>
    private static IISOCountry GetLocalizedCountrySingle(IISOCountry country)
    {
      return country.Localize(SingleLanguage, new LocalizationSettings()
        { CacheLength = new TimeSpan(0, 0, 1, 0), CacheMode = Enums.CacheMode.Cache });
    }
  }
}