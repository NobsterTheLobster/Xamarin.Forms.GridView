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
    public partial class HamburgerMenu : ContentPage, IDisplayActionSheet
    {
        public HamburgerMenu()
        {
            InitializeComponent();
        }
    }
}
