using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TabDeneme
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var label = new Label();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
          tab1.Title += "a";   
        }
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            tab1.Content = new Grid();
        }
        private void Button_Clicked_2(object sender, EventArgs e)
        {
            tab.TabItems.Remove(tab1);
        }
    }
}
