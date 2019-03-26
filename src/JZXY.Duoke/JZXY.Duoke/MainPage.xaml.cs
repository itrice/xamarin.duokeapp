using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using JZXY.Duoke.Server;
using Xamarin.Essentials;
using System.Diagnostics;

namespace JZXY.Duoke
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private List<FileModel> _source;

        private UserModel _userModel;

        private JZXY.Duoke.Interface.IDocumentViewer _docViewer = DependencyService.Get<Interface.IDocumentViewer>();

        private string _currentPath;

        public MainPage()
        {
            InitializeComponent();
            _userModel = (App.Current as App).CurrentUser;

            _currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _userModel.LoginId);
            LoadData(_currentPath);
            BindData();
        }

        private void BindData()
        {
            var lst = FindByName("listView") as ListView;
            lst.ItemsSource = _source;
        }

        /// <summary>
        /// 加载本地文件
        /// </summary>
        private void LoadData(string path)
        {
            _source = new List<FileModel>();
            var files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fi = new FileInfo(file);     
                _source.Add(new FileModel
                {
                    Name = fi.Name,
                    FilePath = file,
                    Size = file,
                    Type = 1
                });
            }
            var folders = Directory.GetDirectories(path);
            foreach (var file in folders)
            {
                _source.Add(new FileModel
                {
                    Name = file,
                    FilePath = file,
                    Type = 0
                });
            }
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
            if(cell != null)
            {
                if(cell.Detail == "0")
                {
                    _currentPath = Path.Combine(_currentPath, cell.Text);
                    LoadData(_currentPath);
                    BindData();
                }
                else
                {
                    var filePath = cell.Text;
                    var mimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                    switch (Path.GetExtension(filePath).ToLower())
                    {
                        case ".pdf":
                            mimeType = "application/pdf";
                            break;
                        //case ".png":
                        //    mimeType = "image/png";
                        //    break;
                        //case ".jpg":
                        //    mimeType = "image/jpg";
                        //    break;
                    }

                    try
                    {
                        _docViewer.ShowDocumentFile(filePath, mimeType);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}
