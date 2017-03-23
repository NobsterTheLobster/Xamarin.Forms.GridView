using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinFormsGridView.Behaviours
{
    public class ButtonBehaviour
    {
        public static readonly BindableProperty IsToggleEnabledProperty =
       BindableProperty.CreateAttached(
           "IsToggleEnabled",
           typeof(bool),
           typeof(ButtonBehaviour),
           false,
           propertyChanged: OnIsToggleEnabledChanged);


        public static readonly BindableProperty IsCheckedProperty =
        BindableProperty.CreateAttached(
            "IsChecked",
            typeof(bool),
            typeof(ButtonBehaviour),
            false);

        public static bool GetIsChecked(BindableObject view)
        {
            return (bool)view.GetValue(IsCheckedProperty);
        }

        public static void SetIsChecked(BindableObject view, bool value)
        {
            view.SetValue(IsCheckedProperty, value);
        }

        public static bool GetIsToggleEnabled(BindableObject view)
        {
            return (bool)view.GetValue(IsToggleEnabledProperty);
        }

        public static void SetIsToggleEnabled(BindableObject view, bool value)
        {
            view.SetValue(IsToggleEnabledProperty, value);
        }

        static void OnIsToggleEnabledChanged(BindableObject view, object oldValue, object newValue)
        {
            var button = view as Button;
            if (button != null)
            {
                if ((bool)newValue)
                {
                    button.Clicked += Button_Clicked;
                }
                else
                {
                    button.Clicked -= Button_Clicked;
                }
                
            }
        }

        private static void Button_Clicked(object sender, System.EventArgs e)
        {
            //Unbox the button.
            var button = sender as Button;

            //Toggle the checkstate.
            button.SetValue(IsCheckedProperty, !(bool)button.GetValue(IsCheckedProperty));
        }
    }
}
