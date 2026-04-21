namespace FlightTrackerSystem.Models;

public class Airport
{
    public string IataCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public override string ToString()
    {
        return $"{IataCode} - {Name}";
    }
}