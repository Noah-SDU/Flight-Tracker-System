using System;

namespace FlightTrackerSystem.Models;

public class Flight
{
    public string FlightNumber { get; set; } = string.Empty;
    public string AirlineName { get; set; } = string.Empty;
    public string AirlineCode { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime ScheduledDeparture { get; set; }
    public DateTime ScheduledArrival { get; set; }
    public string? Status { get; set; }
    public string? AircraftType { get; set; }

    public override string ToString()
    {
        return $"{FlightNumber} | {AirlineCode} {AirlineName} | {DepartureAirport} -> {ArrivalAirport}";
    }
}