using UIKit;
using Xamarin.Forms.Platform.iOS;
using XamarinFormsGridView.iOS.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ListView), typeof(XamarinFormsGridView.iOS.Renderers.CustomListViewRenderer))]
namespace XamarinFormsGridView.iOS.Renderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            this.Control.TableFooterView = new UIView();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "ItemsSource")
            {
                var control = (UITableView)Control;

                foreach (var cell in control.VisibleCells)
                {
                    cell.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 0);
                }
            }
        }
    }
}