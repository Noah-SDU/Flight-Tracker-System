using System;
using CommunityToolkit.Mvvm.ComponentModel;
using FlightTrackerSystem.Services;
using CommunityToolkit.Mvvm.Input;
using FlightTrackerSystem.Models;

namespace FlightTrackerSystem.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly FlightDataService _dataService;

    [ObservableProperty]
    private FlightData? flightData;
    
    [ObservableProperty]
    private string loadingMessage = "Loading...";
    
    [ObservableProperty]
    private bool isLoading = true;

    [ObservableProperty] 
    private int selectedTab = 0;
    
    private RouteViewModel? _routeViewModel;
    private AirportFlightInfoViewModel? _airportViewModel;
    private AnalyticsViewModel? _analyticsViewModel;
    private readonly PreferencesService _prefsService = new();
    private readonly UserPreferences _preferences;
    
    public RouteViewModel? RouteViewModel => _routeViewModel ??= new(_prefsService, _preferences);
    public AirportFlightInfoViewModel? AirportViewModel => _airportViewModel ??= new(_prefsService, _preferences);
    public AnalyticsViewModel? AnalyticsViewModel => _analyticsViewModel ??= new();
    
    public MainWindowViewModel()
    {
        _dataService = new FlightDataService();
        _preferences = _prefsService.LoadPreferences();
        SelectedTab = _preferences.LastSelectedTab ?? 0;
        LoadDataAsync();
    }
    
    partial void OnSelectedTabChanged(int value)
    {
        _preferences.LastSelectedTab = value;
        _prefsService.SavePreferences(_preferences);
    }

    private async void LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            LoadingMessage = "Loading...";
            
            FlightData = await _dataService.LoadFlightDataAsync();

            if (FlightData != null)
            {
                RouteViewModel?.Initialize(FlightData);
                AirportViewModel?.Initialize(FlightData);
                AnalyticsViewModel?.Initialize(FlightData);
            }
        }
        catch (Exception ex)
        {
            LoadingMessage = $"Error loading data: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    
}
