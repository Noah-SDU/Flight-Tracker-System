using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FlightTrackerSystem.Models;
using FlightTrackerSystem.Services;

namespace FlightTrackerSystem.ViewModels;

public partial class AirportFlightInfoViewModel : ViewModelBase
{
    private readonly PreferencesService _prefsService = new();
    private readonly UserPreferences _preferences;

    public AirportFlightInfoViewModel()
    {
        _preferences = _prefsService.LoadPreferences();
    }

    [ObservableProperty]
    private FlightData? flightData;
    
    [ObservableProperty]
    private Airport?  selectedAirport;

    [ObservableProperty] private ObservableCollection<Airport> allAirports = new();
    
    [ObservableProperty] private ObservableCollection<Flight> displayedFlights = new();

    [ObservableProperty] private ObservableCollection<string> statusOptions = new()
    {
        "All"
    };
    
    [ObservableProperty] private string selectedStatus = "All";

    private ObservableCollection<Flight> _allFlightsForAirport = new();

    public void Initialize(FlightData flightData)
    {
        FlightData = flightData;
        
        if (flightData?.Airports != null)
        {
            AllAirports = new ObservableCollection<Airport>(flightData.Airports.OrderBy(a => a.IataCode));
        }
        
        if (!string.IsNullOrEmpty(_preferences.LastSelectedAirport))
        {
            SelectedAirport = AllAirports.FirstOrDefault(a => a.IataCode == _preferences.LastSelectedAirport);
        }
        
        SelectedAirport ??= AllAirports.FirstOrDefault();
        SelectedStatus = _preferences.LastSelectedStatus ?? "All";

        if (flightData?.Flights != null)
        {
            var statuses = flightData.Flights
                .Select(f => f.Status)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(System.StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .Cast<string>()
                .ToList();

            statuses.Insert(0, "All");
            StatusOptions = new ObservableCollection<string>(statuses);
        }
        
        SelectedStatus = "All";
        SelectedAirport = AllAirports.FirstOrDefault();
    }

    partial void OnSelectedAirportChanged(Airport? value)
    {
        _preferences.LastSelectedAirport = value?.IataCode;
        _prefsService.SavePreferences(_preferences);
        LoadFlightsForAirport();
    }

    partial void OnSelectedStatusChanged(string value)
    {
        _preferences.LastSelectedStatus = value;
        _prefsService.SavePreferences(_preferences);
        ApplyFilter();
    }
    
    private void LoadFlightsForAirport()
    {
        if (SelectedAirport == null || FlightData == null)
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
            DisplayedFlights = new ObservableCollection<Flight>(_allFlightsForAirport);
        }
        else
        {
            var filtered = _allFlightsForAirport
                .Where(f => string.Equals(f.Status, SelectedStatus, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
            DisplayedFlights = new ObservableCollection<Flight>(filtered);
        }
    }
}