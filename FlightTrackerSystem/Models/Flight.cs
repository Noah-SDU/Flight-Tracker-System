using System;

namespace FlightTrackerSystem.Models;

public class Flight
{
    public string FlightNumber { get; set; }
    public string AirlineName { get; set; }
    public string AirlineCode { get; set; }
    public string DepartureAirport { get; set; }
    public string ArrivalAirport { get; set; }
    public DateTime ScheduledDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public string? Status { get; set; }
    public string? AircraftType { get; set; }

    public override string ToString()
    {
        return $"{FlightNumber}: {DepartureAirport} -> {ArrivalAirport}";
    }
}