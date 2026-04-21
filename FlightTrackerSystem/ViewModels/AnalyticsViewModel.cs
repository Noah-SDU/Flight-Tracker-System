using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.ViewModels;

public partial class AnalyticsViewModel : ViewModelBase
{
    [ObservableProperty]
    private FlightData? flightData;

    [ObservableProperty] private ObservableCollection<(string Label, int Count)> flightsByAirline = new();
    
    [ObservableProperty] private ObservableCollection<(string Label, int Count)> flightsByAirport = new();
    
    [ObservableProperty] private ObservableCollection<(string Label, int Count)> flightsByDate = new();

    public void Initialize(FlightData? flightData)
    {
        FlightData = flightData;
        GenerateAnalytics();
    }

    private void GenerateAnalytics()
    {
        if (flightData == null)
        {
            return;
        }   
        
        var airlineData = FlightData.Flights
            .GroupBy(f => f.AirlineName)
            .Select(g => (g.Key, g.Count()))
            .OrderByDescending(x => x.Item2)
            .ToList();
        FlightsByAirline = new ObservableCollection<(string, int )>(airlineData);
        
        var airportData = FlightData.Flights
            .GroupBy(f => f.DepartureAirport)
            .Select(g => (g.Key, g.Count()))
            .OrderByDescending(x => x.Item2)
            .ToList();
        FlightsByAirport = new ObservableCollection<(string, int)>(airportData);
        
        var dateData = FlightData.Flights
            .GroupBy(f => f.ScheduledDeparture.Date.ToString("yyyy-MM-dd"))
            .Select(g => (g.Key, g.Count()))
            .OrderByDescending(x => x.Item2)
            .ToList();
        FlightsByDate = new ObservableCollection<(string, int)>(dateData);
    }
}