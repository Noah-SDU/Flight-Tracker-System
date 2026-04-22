using System.IO;
using System.Text.Json;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.Services;

public class PreferencesService
{
     private const string PrefsFile = "preferences.json";
     private static readonly JsonSerializerOptions JsonOptions = new()
     {
          WriteIndented = true,
          PropertyNameCaseInsensitive = true
     };

     public void SavePreferences(UserPreferences preferences)
     {
          var safePreferences = preferences ?? new UserPreferences();
          var json = JsonSerializer.Serialize(safePreferences, JsonOptions);
          File.WriteAllText(PrefsFile, json);
     }

     public UserPreferences LoadPreferences()
     {
          if (!File.Exists(PrefsFile))
          {
               return new UserPreferences();
          }
          
          var json = File.ReadAllText(PrefsFile);
          return JsonSerializer.Deserialize<UserPreferences>(json, JsonOptions) ?? new UserPreferences();
     }
}
