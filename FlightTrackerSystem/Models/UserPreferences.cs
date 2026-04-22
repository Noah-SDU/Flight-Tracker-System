namespace FlightTrackerSystem.Models;

public class UserPreferences
{
    public string? LastSelectedAirport { get; set; }
    public string? LastSearchQuery { get; set; }
    public string? LastSelectedStatus { get; set; }
    public int? LastSelectedTab { get; set; }
}
