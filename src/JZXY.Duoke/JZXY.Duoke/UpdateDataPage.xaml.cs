using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using JZXY.Duoke.Server;
using JZXY.Duoke.ViewModel;

namespace JZXY.Duoke
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateDataPage : ContentPage
    {
        private DuokeServer _duokeServer;

        private UpdatingViewModel _viewModel;

        private Interface.IMessage _messager = DependencyService.Get<Interface.IMessage>();

        public UpdateDataPage()
        {
            InitializeComponent();
            Init();

            new Task(() =>
            {
                try
                {
                    _duokeServer.GetDoucuments();
                    App.Current.MainPage = new MainPage();
                }
                catch (Exception e)
                {
                    _messager.ShortAlert($"获取文档信息异常:{e.Message}");
                }
            }).Start();
        }

        private void Init()
        {
            _viewModel = new UpdatingViewModel();
            _duokeServer = new DuokeServer();
            _duokeServer.ProcessOn += _duokeServer_ProcessOn;
            _duokeServer.ProcessDone += _duokeServer_ProcessDone;
            this.BindingContext = _viewModel;
        }

        private void _duokeServer_ProcessDone()
        {
        }

        private void _duokeServer_ProcessOn(string arg1, int arg2)
        {
            _viewModel.Detial = arg1;
            _viewModel.Process = arg2;
        }
    }
}