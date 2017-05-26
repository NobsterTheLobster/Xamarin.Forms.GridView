using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFormsGridView.Interfaces;

namespace XamarinFormsGridView.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sample3 : ContentPage, IDisplayActionSheet
    {
        public Sample3()
        {
            InitializeComponent();
        }

        public Sample3(ViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}
