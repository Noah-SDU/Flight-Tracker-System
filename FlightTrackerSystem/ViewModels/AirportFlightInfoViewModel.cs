using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.ViewModels;

public partial class AirportFlightInfoViewModel : ViewModelBase
{
    [ObservableProperty]
    private FlightData? flightData;
    
    [ObservableProperty]
    private Airport?  selectedAirport;

    [ObservableProperty] private ObservableCollection<Airport> allAirports = new();
    
    [ObservableProperty] private ObservableCollection<Flight> displayedFlights = new();

    [ObservableProperty] private ObservableCollection<string> statusOption = new()
    {
        "All",
        "On time",
        "Delayed",
        "Cancelled"
    };
    
    [ObservableProperty] private string selectedStatus = "All";

    private ObservableCollection<Flight> _allFlightsForAirport = new();

    public void Initialize(FlightData flightData)
    {
        FlightData = flightData;
        AllAirports = new ObservableCollection<Airport>(flightData.Airports);
    }
    
    [RelayCommand]
    public void SelectAirport(Airport airport)
    {
        SelectedAirport = airport;
        LoadFlightsForAirport(airport);
    }

    [RelayCommand]
    public void FilterByStatys(Airport airport)
    {
        SelectedAirport =  airport;
        ApplyFilter();
    }
    
    private void LoadFlightsForAirport(Airport airport)
    {
        if(SelectedAirport == null || FlightData == null)
        {
            _allFlightsForAirport.Clear();
            DisplayedFlights.Clear();
            return;
        }
        
        var flights = FlightData.Flights
            .Where(f => f.DepartureAirport == SelectedAirport.IataCode)
            .OrderBy(f => f.ScheduledDeparture)
            .ToList();
        
        _allFlightsForAirport = new ObservableCollection<Flight>(flights);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (SelectedStatus == "All")
        {
            DisplayedFlights =  _allFlightsForAirport;
        }
        else
        {
            var filtered = _allFlightsForAirport
                .Where(f => f.Status == SelectedStatus)
                .ToList();
            DisplayedFlights = new ObservableCollection<Flight>(filtered);
        }
    }
}