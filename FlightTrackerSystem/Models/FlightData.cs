using System.Collections.Generic;

namespace FlightTrackerSystem.Models;

public class FlightData
{
    public List<Airport> Airports { get; set; }
    public List<Flight> Flights { get; set; }
}