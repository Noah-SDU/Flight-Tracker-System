using Avalonia.Controls;
using FlightTrackerSystem.ViewModels;
using FlightTrackerSystem.Services;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Tiling;
using System;
using System.ComponentModel;

namespace FlightTrackerSystem.Views;

public partial class RouteView : UserControl
{
    private readonly MemoryLayer _routeLayer = new("Selected route");
    private readonly MapRenderService _mapRenderService = new();
    private RouteViewModel? _viewModel;

    public RouteView()
    {
        InitializeComponent();

        var map = MyMapControl.Map;
        if (map is null) return;

        // Configure the map
        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.BackColor = new Mapsui.Styles.Color(170, 211, 223);
        map.Navigator.OverrideZoomBounds = new MMinMax(0.3, 60000);
        map.Layers.Add(_routeLayer);

        DataContextChanged += OnDataContextChanged;
        RefreshRouteLayer();

        // Handle AutoCompleteBox selection
        if (FlightAutoComplete is not null)
        {
            FlightAutoComplete.SelectionChanged += OnFlightSelected;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = DataContext as RouteViewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        RefreshRouteLayer();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(RouteViewModel.SelectedFlight)
            or nameof(RouteViewModel.DepartureAirportInfo)
            or nameof(RouteViewModel.ArrivalAirportInfo))
        {
            RefreshRouteLayer();
        }
    }

    private void RefreshRouteLayer()
    {
        var map = MyMapControl.Map;
        if (map is null || _viewModel == null)
            return;

        // Use the service to create route features
        var features = _mapRenderService.CreateRouteFeatures(
            _viewModel.DepartureAirportInfo,
            _viewModel.ArrivalAirportInfo);

        _routeLayer.Features = features;

        // If no route is selected, clear the map
        if (features.Length == 0)
        {
            MyMapControl.Refresh();
            return;
        }

        // Calculate and zoom to the bounding box containing both airports
        var boundingBox = _mapRenderService.CalculateBoundingBox(
            _viewModel.DepartureAirportInfo,
            _viewModel.ArrivalAirportInfo);

        map.Navigator.ZoomToBox(boundingBox, MBoxFit.Fit);
        MyMapControl.Refresh();
    }

    private void OnFlightSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count == 0 || _viewModel == null)
            return;

        var selectedFlight = e.AddedItems[0] as Models.Flight;
        if (selectedFlight != null)
        {
            _viewModel.SelectedFlight = selectedFlight;
        }
    }
}
