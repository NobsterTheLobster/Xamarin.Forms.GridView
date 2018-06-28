using Plugin.GridViewControl.UWP.Renderers;
using System;
using Xamarin.Forms.Platform.UWP;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using System.ComponentModel;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Plugin.GridViewControl.Common;

[assembly: ExportRenderer(typeof(Plugin.GridViewControl.Common.GridView), typeof(GridViewRenderer))]
namespace Plugin.GridViewControl.UWP.Renderers
{
    public class GridViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            var list = e.NewElement;

            if (Control != null)
            {
                var baseList = Control as ListViewBase;

                //Retrieve the min item width property.
                var itemWidth = (double)list.GetValue(Common.GridView.MinItemWidthProperty);

                //If the property is set.
                if (itemWidth > 0)
                {
                    //Build the new items panel template.
                    string template =
                    "<ItemsPanelTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
                        "<ItemsWrapGrid VerticalAlignment = \"Top\" ItemWidth = \"" + itemWidth + "\" Orientation = \"Horizontal\"/>" +
                    "</ItemsPanelTemplate> ";

                    baseList.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(template);
                }

                baseList.RightTapped += OnRightTapped;
            }
        }

        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {

            if (Element is Common.GridView gridView)
            {
                var source = (FrameworkElement)e.OriginalSource;
                //var item = (GridViewXamlCell)source.DataContext;
                //var context = item.BindingContext;
                gridView.RaiseOnItemHold(source.DataContext);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
          
            //If the property is width.
            if (e.PropertyName == "Width")
            {
                //Unbox the xamarin host control.
                var list = sender as Xamarin.Forms.ListView;

                //Get the Minimum item size.
                var itemMinSize = (double)list.GetValue(Common.GridView.MinItemWidthProperty);

                //If the property is set and the control is of expected type.
                if (itemMinSize > 0 && Control is ListViewBase itemsControl && itemsControl.ItemsPanelRoot is ItemsWrapGrid itemsPanel)
                {
                    //Get total size (leave room for scrolling.)
                    var total = list.Width - 10;

                    //How many items can be fit whole.
                    var canBeFit = Math.Floor(total / itemMinSize);

                    //Set the items Panel item width appropriately.
                    //Note you will need your container to stretch
                    //along with the items panel or it will look 
                    //strange. 
                    itemsPanel.ItemWidth = total / canBeFit;
                }
            }
        }
    }

  
}