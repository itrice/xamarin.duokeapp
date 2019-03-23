using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JZXY.Duoke.Server;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateDataPage : ContentPage
    {
        public UpdateDataPage()
        {
            InitializeComponent();
            new Task(() =>
            {
                DuokeServer.Instance.GetDoucuments();
                App.Current.MainPage = new MainPage();
            }).Start();
        }
    }
}