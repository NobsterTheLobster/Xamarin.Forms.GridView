using XamarinFormsGridView.UWP.Renderers;
using System;
using System.Linq;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Media;
using XamarinFormsGridView.UWP.Extensions;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Button), typeof(CustomButtonRenderer))]
namespace XamarinFormsGridView.UWP.Renderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            var button = e.NewElement;

            if (Control != null)
            {
                button.SizeChanged += OnSizeChanged;
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            var button = (Xamarin.Forms.Button)sender;

            Control.ApplyTemplate();
            var grid = Control.GetVisuals<Windows.UI.Xaml.Controls.Grid>();

            grid.First().Background = Windows.UI.Xaml.Application.Current.Resources["ButtonBackground"] as SolidColorBrush;
            button.SizeChanged -= OnSizeChanged;
        }

        //protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    base.OnElementPropertyChanged(sender, e);

        //    if (e.PropertyName == "BorderRadius")
        //    {
        //        var borders = Control.GetVisuals<Border>();

        //        foreach (var border in borders)
        //        {
        //            border.CornerRadius = new CornerRadius(((Xamarin.Forms.Button)sender).BorderRadius);
        //        }
        //    }
        //}
    }

  
}