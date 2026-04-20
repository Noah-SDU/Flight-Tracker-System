using Avalonia.Controls;
using Mapsui.Tiling;

namespace FlightTrackerSystem.Views;

public partial class RouteView : UserControl
{
    public RouteView()
    {
        InitializeComponent();

        // Le map
        MyMapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}