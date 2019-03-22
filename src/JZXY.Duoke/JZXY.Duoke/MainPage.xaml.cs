using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using JZXY.Duoke.server;

namespace JZXY.Duoke
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private List<FileModel> _source;

        public MainPage()
        {
            InitializeComponent();
            LoadData();
            BindData();
        }

        private void BindData()
        {
            var lst = FindByName("listView") as ListView;
            lst.ItemsSource = _source;
        }

        private void LoadData()
        {
            var files = DuokeServer.Instance.GetDoucuments();
            _source = new List<FileModel>();
            // 重组文件集合
            foreach (var item in files)
            {
                _source.AddRange(GetFiles(item));
            }
            //_source.Add(new FileModel
            //{
            //    Name = "测试文件.doc", Size = "100kb"               
            //});
        }

        private List<FileModel> GetFiles(FileModel file)
        {
            var rst = new List<FileModel>();
            var files = file.Children.Where(o => o.Type == 1);
            foreach (var item in files)
            {
                item.Name = $"{file.Name}\\{item.Name}";
                rst.Add(item);
            }
            var folders = file.Children.Where(o => o.Type == 0);
            foreach (var item in folders)
            {
                item.Name = $"{file.Name}\\{item.Name}";
                rst.AddRange(GetFiles(item));
            }
            return rst;
        }

        private void TextCell_Tapped(object sender, EventArgs e)
        {
            var cell = sender as TextCell;
            DisplayAlert(cell.Text, cell.Detail, "确定");
        }
    }
}
