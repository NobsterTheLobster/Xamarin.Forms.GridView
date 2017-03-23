using System;
using Xamarin.Forms;
using XamarinFormsGridView.Interfaces;

namespace XamarinFormsGridView.Views
{
    public partial class MainMasterDetailPage : MasterDetailPage, IDisplayActionSheet
    {
        /// <summary>
        /// The view model for the application.
        /// </summary>
        ViewModel vm;

        /// <summary>
        /// Default contstructor.
        /// </summary>
        public MainMasterDetailPage()
        {
            InitializeComponent();

            //Initialize the view model(s)
            vm = new ViewModel();
            vm.ActionSheetDisplayer = this;

            //Attach handling for property changes in the navigation view model.
            vm.PropertyChanged += Vm_PropertyChanged;

            //Set the data context for the master and the root page.
            Master.BindingContext = vm;
            BindingContext = vm;
        }

        /// <summary>
        /// Property changes occuring in the immediate binding context.
        /// </summary>
        /// <param name="sender">System.Object reperesenting the source of the event.</param>
        /// <param name="e">The arguments for the event.</param>
        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentPage")
            {
                try
                {
                    //Set the detail page. Creating a new navigation page wrapper with no navigation history.
                    Detail = new NavigationPage((Page)Activator.CreateInstance(System.Type.GetType("XamarinFormsGridView.Views." + vm.CurrentPage.ToString()), vm));
                }
                catch (Exception ex)
                {
                    //Check for specific plaform bug.
                    if (!ex.ToString().Contains("Android.Content.Res.Resources+NotFoundException: Resource ID"))
                    {
                        //#if __ANDROID__ && DEBUG
                        //#else
                        //#endif

                        throw;
                    }
                }
            }
        }
    }
}
