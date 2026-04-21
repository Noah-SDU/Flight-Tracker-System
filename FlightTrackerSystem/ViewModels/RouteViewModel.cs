using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlightTrackerSystem.Models;
using NetTopologySuite.IO;

namespace FlightTrackerSystem.ViewModels;

public partial class RouteViewModel : ViewModelBase
{
    [ObservableProperty]
    private FlightData? flightData;
    
    [ObservableProperty]
    private Airport?  selectedAirport;

    [ObservableProperty] private ObservableCollection<Airport> allAirports = new();

    [ObservableProperty] private ObservableCollection<Flight> routeFlights = new();

    public void Initialize(FlightData flightData)
    {
        FlightData = flightData;
        AllAirports = new ObservableCollection<Airport>(flightData.Airports);
    }

    [RelayCommand]
    public void SelectAirport(Airport airport)
    {
        SelectedAirport = airport;
        UpdateRoutes();
    }

    private void UpdateRoutes()
    {
        if (SelectedAirport == null || FlightData == null)
        {
            RouteFlights.Clear();
        }
        
        var flights = FlightData.Flights
            .Where(f => f.DepartureAirport == SelectedAirport.IataCode)
            .ToList();
        
        RouteFlights =  new ObservableCollection<Flight>(flights);
    }

    [RelayCommand]
    public void ClearSelection()
    {
        SelectedAirport = null;
        RouteFlights.Clear();
    }
}