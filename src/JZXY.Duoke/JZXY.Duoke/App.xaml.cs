using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke
{
    public partial class App : Application
    {
        /// <summary>
        /// 根节点路径
        /// </summary>
        public string Root => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public App()
        {
            InitializeComponent();
            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
