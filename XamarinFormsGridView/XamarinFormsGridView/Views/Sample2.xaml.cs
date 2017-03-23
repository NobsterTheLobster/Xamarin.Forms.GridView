using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamarinFormsGridView.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinFormsGridView.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Sample2 : ContentPage, IDisplayActionSheet
    {
        public Sample2()
        {
            InitializeComponent();
        }

        public Sample2(ViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}
