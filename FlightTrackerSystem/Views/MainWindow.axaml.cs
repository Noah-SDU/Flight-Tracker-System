using Avalonia.Controls;
using Mapsui.Tiling;

namespace FlightTrackerSystem.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MyMapControl.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());
    }
}