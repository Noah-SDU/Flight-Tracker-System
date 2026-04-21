using Avalonia.Controls;
using Mapsui;
using Mapsui.Tiling;

namespace FlightTrackerSystem.Views;

public partial class RouteView : UserControl
{
    public RouteView()
    {
        InitializeComponent();

        // Le Map
        var map = MyMapControl.Map;
        if (map is null) return;

        map.Layers.Add(OpenStreetMap.CreateTileLayer());

        // Set background color for out-of-bounds areas
        map.BackColor = new Mapsui.Styles.Color(170, 211, 223);

        // Restrict zoom range
        map.Navigator.OverrideZoomBounds = new MMinMax(0.3, 60000);
    }
}