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
    public partial class Sample1 : ContentPage, IDisplayActionSheet
    {
        public Sample1()
        {
            InitializeComponent();
        }

        public Sample1(ViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}
