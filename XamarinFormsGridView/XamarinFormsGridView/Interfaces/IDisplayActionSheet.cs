using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinFormsGridView.Interfaces
{
    public interface IDisplayActionSheet
    {
        Task<string> DisplayActionSheet(string title, string cancel, string destruction, string[] buttons);

        Task DisplayAlert(string title, string message, string cancel);

        Task<bool> DisplayAlert(string title, string message, string accept, string cancel);
    }
}
