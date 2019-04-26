using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(true)]
    public partial class SettingPage1 : ContentPage
    {
        public SettingPage1()
        {
            InitializeComponent();

            //BindingContext = this;

            entryAddress1.Text = Servers.LocalConfigManager.Instance.ServerAddress;
            autoSave.IsToggled = Servers.LocalConfigManager.Instance.IsSaveLoginInfo;
            autoLogin.IsToggled = Servers.LocalConfigManager.Instance.IsAutoLogin;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            await Task.Factory.StartNew(()=> {
                Servers.LocalConfigManager.Instance.ServerAddress = entryAddress1.Text;
                Servers.LocalConfigManager.Instance.IsSaveLoginInfo = autoSave.IsToggled;
                Servers.LocalConfigManager.Instance.IsAutoLogin = autoLogin.IsToggled;
            });
        }

        async void GoBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}