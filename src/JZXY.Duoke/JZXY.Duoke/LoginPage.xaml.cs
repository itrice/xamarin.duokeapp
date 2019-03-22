using JZXY.Duoke.server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
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
            string AUrl = "http://" + ip.Text.Trim() + "";
            bool rst = DuokeServer.Instance.Login(AUrl, loginId.Text.Trim(), loginPwd.Text.Trim());
            if (rst)
            {
                App.Current.MainPage = new MainPage();
            }
            else
            {
                DisplayAlert("认证失败", "请确认信息填写正确", "确定");
            }
        }
    }
}