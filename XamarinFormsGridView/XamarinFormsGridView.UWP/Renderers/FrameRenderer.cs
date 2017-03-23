using XamarinFormsGridView.UWP.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

[assembly: ExportRenderer(typeof(Xamarin.Forms.Frame), typeof(CustomFrameRenderer))]
namespace XamarinFormsGridView.UWP.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Frame> e)
        {
            base.OnElementChanged(e);
            var frame = e.NewElement;

            if (Control != null)
            {
                frame.SizeChanged += OnSizeChanged;

                
            }
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            //Unbox the sender.
            var frame = sender as Xamarin.Forms.Frame;

            //Set the corner radius of the control to nothing.
            //The native control for frame in windows is simply border.
            Control.CornerRadius = new CornerRadius(0);
            Control.Background = Windows.UI.Xaml.Application.Current.Resources["SystemControlPageBackgroundChromeLowBrush"] as SolidColorBrush;

            //Remove event handler.
            frame.SizeChanged -= OnSizeChanged;
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
