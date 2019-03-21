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
            var path = AUrl + "/ufInterface?opr=getHash&name=" + loginId.Text.Trim() + "&password=" + loginPwd.Text;
            string resptxt = GetHttp(path, 3000);
            if (string.IsNullOrEmpty(resptxt) || resptxt.Contains("错误"))
            {
                DisplayAlert("认证失败", resptxt, "确定");
                return;
            }
            DuokeServer.HashKey = resptxt;
            App.Current.MainPage = new MainPage();
        }


        private string GetHttp(string a_strUrl, int timeout)
        {
            try
            {
                string AUrl = a_strUrl;
                System.Net.HttpWebRequest myReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(AUrl);
                myReq.Timeout = timeout;
                System.Net.HttpWebResponse HttpWResp = (System.Net.HttpWebResponse)myReq.GetResponse();
                System.IO.Stream myStream = HttpWResp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(myStream, Encoding.UTF8);
                StringBuilder sb = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    sb.AppendLine(sr.ReadLine());
                }
                return sb.ToString();


            }
            catch (Exception exp)
            {
                return "错误：" + exp.Message;
            }
        }

        private void DownloadHttpFile(string savepath, string AUrl, int timeout)
        {
            Uri downUri = new Uri(AUrl);

            System.Net.HttpWebRequest myReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(downUri);
            System.Net.HttpWebResponse myResp = (System.Net.HttpWebResponse)myReq.GetResponse();

            string head = myResp.GetResponseHeader("Content-Disposition");
            if (head.Trim() == "")
            {
                return;
            }

            int headst = head.IndexOf("filename=\"");
            int headend = head.IndexOf("\"", headst + 10);
            string filename = head.Substring(headst + 10, headend - (headst + 10));
            filename = System.Web.HttpUtility.UrlDecode(filename);

            if (savepath[savepath.Length - 1] != '\\')
            {
                savepath = savepath + '\\';
            }

            string filepath = savepath + filename;

            using (Stream stream = myReq.GetResponse().GetResponseStream())
            {
                using (FileStream fs = File.Create(@filepath))
                {
                    byte[] bytes = new byte[102400];

                    int n = 1;

                    while (n > 0)
                    {
                        n = stream.Read(bytes, 0, 10240);

                        fs.Write(bytes, 0, n);
                    }
                }
            }

            //LogResponse(savepath + filename + "下载完成");
        }

    }
}