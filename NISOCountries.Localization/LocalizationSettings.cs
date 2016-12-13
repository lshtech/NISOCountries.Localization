using System;

namespace NISOCountries.Localization
{
  public class LocalizationSettings
  {
    public Enums.CacheMode CacheMode { get; set; }

    /// <summary>
    /// Returns the path to the localization cache. Returns .\localizations if not previously set.
    /// </summary>
    public string CachePath { get; set; } = string.Empty;

    /// <summary>
    /// Time period before refetching localization data.
    /// </summary>
    public TimeSpan CacheLength { get; set; }

    public LocalizationSettings()
    {
      CacheMode = Enums.CacheMode.Cache;
      CacheLength = new TimeSpan(7, 0, 0, 0);
      CachePath = ".\\localizations";
    }
  }
}
