using JZXY.Duoke.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private Interface.IMessage _messager = DependencyService.Get<Interface.IMessage>();

        public LoginPage()
        {
            InitializeComponent();
            var imgBackground = FindByName("imgBackground") as Image;
            imgBackground.Source = ImageSource.FromResource("JZXY.Duoke.bg.jpg");
        }

        /// <summary>
        /// 检查网络状态
        /// </summary>
        /// <returns></returns>
        private bool CheckNetworkConnection()
        {
            //var networkServer = DependencyService.Get<Interface.INetworkServer>();
            //return networkServer.IsNetWrokConnected();

            var current = Connectivity.NetworkAccess;
            return current == NetworkAccess.Internet;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // yifei jzxy@502
            var ip = FindByName("address") as Entry;
            var loginId = FindByName("loginID") as Entry;
            var loginPwd = FindByName("loginPwd") as Entry;
            if (ip == null || string.IsNullOrEmpty(ip.Text)
                || loginId == null || string.IsNullOrEmpty(loginId.Text)
                || loginPwd == null || string.IsNullOrEmpty(loginPwd.Text))
            {
                DisplayAlert("提示", "请填入必要信息", "确定");
                return;
            }
            string AUrl = ip.Text.Trim() + "";
            var duokeServer = new DuokeServer();
            var user = new UserModel();
            user.LoginId = loginId.Text.Trim();
            user.LoginPwd = loginPwd.Text.Trim();
            (App.Current as App).CurrentUser = user;
            var isConnected = CheckNetworkConnection();
            isConnected = false;
            if (isConnected)
            {
                bool rst = duokeServer.Login(AUrl, user.LoginId, user.LoginPwd);
                if (rst)
                {
                    App.Current.MainPage = new UpdateDataPage();
                }
                else
                {
                    DisplayAlert("认证失败", "请确认信息填写正确", "确定");
                }
            }
            else
            {
                _messager.ShortAlert("没有网络，进入离线模式");
                App.Current.MainPage = new MainPage();
            }
        }
    }
}