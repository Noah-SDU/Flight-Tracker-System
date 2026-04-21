using System.Collections.Generic;

namespace FlightTrackerSystem.Models;

public class FlightData
{
    public List<Airport> Airports { get; set; } = new();
    public List<Flight> Flights { get; set; } = new();

    public override string ToString()
    {
        return $"FlightData: {Airports?.Count ?? 0} airports, {Flights?.Count ?? 0} flights";
    }
}
