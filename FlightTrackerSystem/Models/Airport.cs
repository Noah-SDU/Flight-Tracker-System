namespace FlightTrackerSystem.Models;

public class Airport
{
    public string IataCode { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public override string ToString()
    {
        return $"{IataCode} - {Name}";
    }
}