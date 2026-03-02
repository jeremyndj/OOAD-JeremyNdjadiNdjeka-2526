using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfControlsOefenblad.Helpers;

namespace WpfControlsOefenblad.Exercises
{
    [NavPage(Title = "Volume Control", Description = "Slider gebruiken met ValueChanged en property-aanpassing.", Order = 7, IsVisible = true)]
    public partial class VolumeControl : Page
    {
        public VolumeControl()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtVolume.Text = $"Volume: {sldWidth.Value}%";
        }
    }
}
