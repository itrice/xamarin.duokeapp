using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
            var address = FindByName("entryAddress") as Entry;
            address.Text = Server.LocalConfigManager.Instance.ServerAddress;
        }

        private void BtnOnClick(object sender, EventArgs e)
        {
            var address = FindByName("entryAddress") as Entry;
            Server.LocalConfigManager.Instance.ServerAddress = address.Text;

        }
    }
}