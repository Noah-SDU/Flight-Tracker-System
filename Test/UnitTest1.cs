using System;
using System.Collections.Generic;
using System.Linq;
using FlightTrackerSystem.Models;
using FlightTrackerSystem.ViewModels;
using Xunit;

namespace Test;

public class UnitTest1
{
    [Fact]
    public void GenerateAnalytics_GroupByAirlines_Correctly()
    {
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "AA001", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T08:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T11:00:00"), Status = "On Time" },
            new Flight { FlightNumber = "AA002", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T09:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T12:00:00"), Status = "On Time"},
            new Flight { FlightNumber = "BB001", AirlineName = "Airline B", AirlineCode = "BB", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T10:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T13:00:00"), Status = "Delayed" }
        };
        
        var flightData = new FlightData()
        {
            Airports = new List<Airport>(),
            Flights = flights
        };

        var viewModel = new AnalyticsViewModel();
        
        viewModel.Initialize(flightData);

        Assert.Equal(2, viewModel.FlightsByAirline.First(x => x.Label == "Airline A").Count);
        Assert.Equal(1, viewModel.FlightsByAirline.First(x => x.Label == "Airline B").Count);
        Assert.Equal(2, viewModel.FlightsByAirline.Max(x => x.Count));
    }

    [Fact]
    public void GenerateAnalytics_GroupByRouteTime_Correctly()
    {
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "AA001", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T08:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T11:00:00"), Status = "On Time" },
            new Flight { FlightNumber = "AA002", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T09:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T12:00:00"), Status = "On Time"},
            new Flight { FlightNumber = "BB001", AirlineName = "Airline B", AirlineCode = "BB", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T10:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T13:00:00"), Status = "Delayed" }
        };
        
        var flightData = new FlightData()
        {
            Airports = new List<Airport>(),
            Flights = flights
        };

        var viewModel = new AnalyticsViewModel();
        
        viewModel.Initialize(flightData);
        
        Assert.Equal(3, viewModel.BusiestRoutesByTimeOfDay.First(x => x.Label == "JFK -> LAX (Morning)").Count);
        Assert.Equal(3, viewModel.BusiestRoutesMax);
    }

    [Fact]
    public void GenerateAnalytics_GroupByCountryMonth_Correctly()
    {
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "AA001", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T08:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T11:00:00"), Status = "On Time" },
            new Flight { FlightNumber = "AA002", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T09:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T12:00:00"), Status = "On Time"},
            new Flight { FlightNumber = "BB001", AirlineName = "Airline B", AirlineCode = "BB", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T10:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T13:00:00"), Status = "Delayed" }
        };
        
        var flightData = new FlightData()
        {
            Airports = new List<Airport>
            {
                new Airport { IataCode = "JFK", Country = "United States" },
                new Airport { IataCode = "LAX", Country = "United States" }
            },
            Flights = flights
        };

        var viewModel = new AnalyticsViewModel();
        
        viewModel.Initialize(flightData);
        
        Assert.Equal(3, viewModel.CountryTrafficTrends.First(x => x.Label == "2024-01 | United States").Count);
        Assert.Equal(3, viewModel.CountryTrafficMax);
    }

    [Fact]
    public void GenerateAnalytics_SortedDescending_Correctly()
    {
        var flights = new List<Flight>
        {
            new Flight { FlightNumber = "AA001", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T08:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T11:00:00"), Status = "On Time" },
            new Flight { FlightNumber = "AA002", AirlineName = "Airline A", AirlineCode = "AA", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T09:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T12:00:00"), Status = "On Time"},
            new Flight { FlightNumber = "BB001", AirlineName = "Airline B", AirlineCode = "BB", DepartureAirport = "JFK", ArrivalAirport = "LAX", ScheduledDeparture = DateTime.Parse("2024-01-01T10:00:00"), ScheduledArrival = DateTime.Parse("2024-01-01T13:00:00"), Status = "Delayed" }
        };
        
        var flightData = new FlightData()
        {
            Airports = new List<Airport>(),
            Flights = flights
        };
        
        var viewModel = new AnalyticsViewModel();
        
        viewModel.Initialize(flightData);
        
        var airlineList = viewModel.FlightsByAirline.ToList();
        Assert.Equal("Airline A", airlineList[0].Label);
        Assert.Equal(2, airlineList.Count);
        Assert.True(airlineList[0].Count >= airlineList[1].Count);
    }
    
    [Fact]
    public void Initialize_NullData()
    {
        var viewModel = new AnalyticsViewModel();
        
        viewModel.Initialize(null);
        
        Assert.Empty(viewModel.FlightsByAirline);
        Assert.Empty(viewModel.BusiestRoutesByTimeOfDay);
        Assert.Empty(viewModel.CountryTrafficTrends);
        Assert.Equal(1, viewModel.BusiestRoutesMax);
        Assert.Equal(1, viewModel.TopAirlinesMax);
        Assert.Equal(1, viewModel.CountryTrafficMax);
    }
    
    [Fact]
    public void GenerateAnalytics_WithEmptyFlights_CreatesEmptyCollections()
    {
        var flightData = new FlightData
        {
            Airports = new List<Airport>(),
            Flights = new List<Flight>()  
        };
        
        var viewModel = new AnalyticsViewModel();

        viewModel.Initialize(flightData);
        
        Assert.Empty(viewModel.FlightsByAirline);
        Assert.Empty(viewModel.BusiestRoutesByTimeOfDay);
        Assert.Empty(viewModel.CountryTrafficTrends);
        Assert.Equal(1, viewModel.BusiestRoutesMax);
        Assert.Equal(1, viewModel.TopAirlinesMax);
        Assert.Equal(1, viewModel.CountryTrafficMax);
    }
}
