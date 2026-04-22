using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlightTrackerSystem.Models;
using FlightTrackerSystem.Services;

namespace FlightTrackerSystem.ViewModels;

public partial class RouteViewModel : ViewModelBase
{
    private readonly PreferencesService _prefsService = new();
    private readonly UserPreferences _preferences;

    public RouteViewModel()
    {
        _preferences = _prefsService.LoadPreferences();
    }

    [ObservableProperty]
    private FlightData? flightData;

    [ObservableProperty] private ObservableCollection<Flight> filteredRouteFlights = new();

    [ObservableProperty]
    private string routeSearchText = string.Empty;

    [ObservableProperty]
    private Flight? selectedFlight;

    [ObservableProperty]
    private Airport? departureAirportInfo;

    [ObservableProperty]
    private Airport? arrivalAirportInfo;

    public void Initialize(FlightData flightData)
    {
        FlightData = flightData;
        RouteSearchText = _preferences.LastSearchQuery ?? string.Empty;
        ApplyRouteFilter();
    }

    partial void OnRouteSearchTextChanged(string value)
    {
        // When AutoComplete commits a picked item, it writes that item text back to Text.
        // Detecting committed display text here avoids mutating ItemsSource during drop-down close.
        _preferences.LastSearchQuery = value;
        _prefsService.SavePreferences(_preferences);
        
        if (FlightData?.Flights is { Count: > 0 } flights)
        {
            var committedFlight = flights.FirstOrDefault(f =>
                string.Equals(value, f.DisplayName, System.StringComparison.Ordinal));

            if (committedFlight != null)
            {
                if (!ReferenceEquals(SelectedFlight, committedFlight))
                {
                    SelectedFlight = committedFlight;
                }

                return;
            }
        }

        if (SelectedFlight != null
            && string.Equals(value, SelectedFlight.DisplayName, System.StringComparison.Ordinal))
        {
            return;
        }

        ApplyRouteFilter();

        if (SelectedFlight != null && !FilteredRouteFlights.Contains(SelectedFlight))
        {
            SelectedFlight = null;
        }
    }

    partial void OnSelectedFlightChanged(Flight? value)
    {
        UpdateSelectedRoute();
    }

    private void ApplyRouteFilter()
    {
        if (FlightData == null || FlightData.Flights == null || FlightData.Flights.Count == 0)
        {
            FilteredRouteFlights = new ObservableCollection<Flight>();
            return;
        }

        var query = RouteSearchText.Trim();
        if (string.IsNullOrWhiteSpace(query))
        {
            var allFlights = FlightData.Flights
                .OrderBy(f => f.ScheduledDeparture)
                .ToList();

            FilteredRouteFlights = new ObservableCollection<Flight>(allFlights);
            return;
        }

        var filtered = FlightData.Flights
            .Where(f =>
                f.FlightNumber.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || f.AirlineName.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || f.AirlineCode.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || f.DepartureAirport.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || f.ArrivalAirport.Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || (f.Status != null && f.Status.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                || GetAirportName(f.DepartureAirport).Contains(query, System.StringComparison.OrdinalIgnoreCase)
                || GetAirportName(f.ArrivalAirport).Contains(query, System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.ScheduledDeparture)
            .ToList();

        FilteredRouteFlights = new ObservableCollection<Flight>(filtered);
    }

    private string GetAirportName(string iataCode)
    {
        if (FlightData == null || FlightData.Airports == null || string.IsNullOrEmpty(iataCode))
            return string.Empty;

        return FlightData.Airports
                   .FirstOrDefault(a => a.IataCode == iataCode)
                   ?.Name
               ?? string.Empty;
    }

    private void UpdateSelectedRoute()
    {
        if (FlightData == null || FlightData.Airports == null || SelectedFlight == null)
        {
            DepartureAirportInfo = null;
            ArrivalAirportInfo = null;
            return;
        }

        DepartureAirportInfo = FlightData.Airports
            .FirstOrDefault(a => a.IataCode == SelectedFlight.DepartureAirport);

        ArrivalAirportInfo = FlightData.Airports
            .FirstOrDefault(a => a.IataCode == SelectedFlight.ArrivalAirport);
    }

    [RelayCommand]
    public void ClearSelection()
    {
        FilteredRouteFlights.Clear();
        SelectedFlight = null;
        RouteSearchText = string.Empty;
        DepartureAirportInfo = null;
        ArrivalAirportInfo = null;
    }
}