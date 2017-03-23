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
using XamarinFormsGridView.UWP.Extensions;

[assembly: ExportRenderer(typeof(Xamarin.Forms.MasterDetailPage), typeof(CustomMasterDetailPageRenderer))]
namespace XamarinFormsGridView.UWP.Renderers
{
    public class CustomMasterDetailPageRenderer : MasterDetailPageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.MasterDetailPage> e)
        {
            base.OnElementChanged(e);
            var page = e.NewElement;

            if (Control != null && page != null)
            {
                ConfigureSplitView(Control, page);
                //page.SizeChanged += OnSizeChanged;
            }
            
        }

        //private void OnSizeChanged(object sender, EventArgs e)
        //{
        //    var page = (Xamarin.Forms.MasterDetailPage)sender;

        //    Control.ApplyTemplate();

        //    ConfigureSplitView(Control, page);

        //    page.SizeChanged -= OnSizeChanged;
        //}

        static void ConfigureSplitView(MasterDetailControl control, MasterDetailPage page)
        {
            try
            {

                if ((bool)page.GetValue(XamarinFormsGridView.Behaviours.MasterDetailPageBehaviour.IsDynamicMasterBehaviourEnabledProperty))
                {
                    var threshold = (double)page.GetValue(XamarinFormsGridView.Behaviours.MasterDetailPageBehaviour.DynamicMasterBehaviorThresholdProperty);

                    if (page.Width <= threshold)
                    {
                        //control.CollapseStyle = Xamarin.Forms.PlatformConfiguration.WindowsSpecific.CollapseStyle.Partial;
                        control.ShouldShowSplitMode = false;
                        control.IsPaneOpen = false;
                    }
                    else
                    {
                        control.ShouldShowSplitMode = true;
                        control.IsPaneOpen = true;
                    }
                }
            }
            catch (Exception)
            {
                //Not sure why this is getting thrown.
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var page = (Xamarin.Forms.MasterDetailPage)sender;

            if (e.PropertyName == "Detail" || e.PropertyName == "Width")
            {
                ConfigureSplitView(Control, page);
            }

        }
    }
}