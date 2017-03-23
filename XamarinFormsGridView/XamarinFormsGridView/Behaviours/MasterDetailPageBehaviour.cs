using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace XamarinFormsGridView.Behaviours
{
    public class MasterDetailPageBehaviour
    {
        public static readonly BindableProperty IsDynamicMasterBehaviourEnabledProperty =
       BindableProperty.CreateAttached(
           "IsDynamicMasterBehaviourEnabled",
           typeof(bool),
           typeof(MasterDetailPageBehaviour),
           false);


        /// <summary>
        /// The page width at which the master behaviour changes from popover to split
        /// e.g. If the value is 720 then below 720 the behaviour is popover and above
        /// 720 the behaviour is plit.
        /// </summary>
        public static readonly BindableProperty DynamicMasterBehaviorThresholdProperty =
        BindableProperty.CreateAttached(
            "DynamicMasterBehaviorThreshold",
            typeof(double),
            typeof(MasterDetailPageBehaviour),
            720D);

        public static double GetDynamicMasterBehaviorThreshold(BindableObject view)
        {
            return (double)view.GetValue(DynamicMasterBehaviorThresholdProperty);
        }

        public static void SetDynamicMasterBehaviorThreshold(BindableObject view, double value)
        {
            view.SetValue(DynamicMasterBehaviorThresholdProperty, value);
        }

        public static bool GetIsDynamicMasterBehaviourEnabled(BindableObject view)
        {
            return (bool)view.GetValue(IsDynamicMasterBehaviourEnabledProperty);
        }

        public static void SetIsDynamicMasterBehaviourEnabled(BindableObject view, bool value)
        {
            view.SetValue(IsDynamicMasterBehaviourEnabledProperty, value);
        }
    }
}
