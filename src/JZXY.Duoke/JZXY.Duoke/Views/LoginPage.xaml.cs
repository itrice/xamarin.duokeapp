using JZXY.Duoke.Servers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private Interface.IMessage _messager = DependencyService.Get<Interface.IMessage>();

        public LoginPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            chk_autologin.Checked = Servers.LocalConfigManager.Instance.IsAutoLogin;
            chk_remember.Checked = Servers.LocalConfigManager.Instance.IsSaveLoginInfo;
            if (Servers.LocalConfigManager.Instance.IsSaveLoginInfo)
            {
                loginID.Text = Servers.LocalConfigManager.Instance.GetValue(LocalConfigManager.DicKey.LOGIN_ID);
                loginPwd.Text = Servers.LocalConfigManager.Instance.GetValue(LocalConfigManager.DicKey.LOGIN_PWD);
            }
            if (Servers.LocalConfigManager.Instance.IsAutoLogin)
            {
                Login();
            }
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
            Login();
        }
               

        private void SettingBtnOnClick(object sender, EventArgs e)
        {            
            Navigation.PushModalAsync(new SettingPage1());
        }

        private void Login()
        {
            // yifei jzxy@502
            string AUrl = Servers.LocalConfigManager.Instance.ServerAddress;
            if (string.IsNullOrEmpty(AUrl))
            {
                DisplayAlert("认证失败", "请先配置服务器地址", "确定");
                return;
            }
            var duokeServer = new DuokeServer();
            var user = new UserModel();
            user.LoginId = loginID.Text.Trim();
            user.LoginPwd = loginPwd.Text.Trim();
            (App.Current as App).CurrentUser = user;
            var isConnected = CheckNetworkConnection();
            //isConnected = false;
            Servers.LocalConfigManager.Instance.IsAutoLogin = chk_autologin.Checked;
            Servers.LocalConfigManager.Instance.IsSaveLoginInfo = chk_remember.Checked;
            Servers.LocalConfigManager.Instance.SetValue(LocalConfigManager.DicKey.LOGIN_ID, loginID.Text);
            Servers.LocalConfigManager.Instance.SetValue(LocalConfigManager.DicKey.LOGIN_PWD, loginPwd.Text);

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
                //_messager.ShortAlert("没有网络，进入离线模式");
                Task.Factory.StartNew(() =>
                {
                    App.Current.MainPage = new MainPage();
                });
            }
        }
    }
}