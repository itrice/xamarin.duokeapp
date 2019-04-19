using JZXY.Duoke.Servers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace JZXY.Duoke.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        private string[,] MIME_MapTable = new string[,] {
            {".3gp",    "video/3gpp"},
            {".apk",    "application/vnd.android.package-archive"},
            {".asf",    "video/x-ms-asf"},
            {".avi",    "video/x-msvideo"},
            {".bin",    "application/octet-stream"},
            {".bmp",      "image/bmp"},
            {".c",        "text/plain"},
            {".class",    "application/octet-stream"},
            {".conf",    "text/plain"},
            {".cpp",    "text/plain"},
            {".doc",    "application/msword"},
            {".exe",    "application/octet-stream"},
            {".gif",    "image/gif"},
            {".gtar",    "application/x-gtar"},
            {".gz",        "application/x-gzip"},
            {".h",        "text/plain"},
            {".htm",    "text/html"},
            {".html",    "text/html"},
            {".jar",    "application/java-archive"},
            {".java",    "text/plain"},
            {".jpeg",    "image/jpeg"},
            {".jpg",    "image/jpeg"},
            {".js",        "application/x-javascript"},
            {".log",    "text/plain"},
            {".m3u",    "audio/x-mpegurl"},
            {".m4a",    "audio/mp4a-latm"},
            {".m4b",    "audio/mp4a-latm"},
            {".m4p",    "audio/mp4a-latm"},
            {".m4u",    "video/vnd.mpegurl"},
            {".m4v",    "video/x-m4v"},
            {".mov",    "video/quicktime"},
            {".mp2",    "audio/x-mpeg"},
            {".mp3",    "audio/x-mpeg"},
            {".mp4",    "video/mp4"},
            {".mpc",    "application/vnd.mpohun.certificate"},
            {".mpe",    "video/mpeg"},
            {".mpeg",    "video/mpeg"},
            {".mpg",    "video/mpeg"},
            {".mpg4",    "video/mp4"},
            {".mpga",    "audio/mpeg"},
            {".msg",    "application/vnd.ms-outlook"},
            {".ogg",    "audio/ogg"},
            {".pdf",    "application/pdf"},
            {".png",    "image/png"},
            {".pps",    "application/vnd.ms-powerpoint"},
            {".ppt",    "application/vnd.ms-powerpoint"},
            {".prop",    "text/plain"},
            {".rar",    "application/x-rar-compressed"},
            {".rc",        "text/plain"},
            {".rmvb",    "audio/x-pn-realaudio"},
            {".rtf",    "application/rtf"},
            {".sh",        "text/plain"},
            {".tar",    "application/x-tar"},
            {".tgz",    "application/x-compressed"},
            {".txt",    "text/plain"},
            {".wav",    "audio/x-wav"},
            {".wma",    "audio/x-ms-wma"},
            {".wmv",    "audio/x-ms-wmv"},
            {".wps",    "application/vnd.ms-works"},  
            //{".xml",    "text/xml"},  
            {".xml",    "text/plain"},
            {".z",        "application/x-compress"},
            {".zip",    "application/zip"},
            {"",        "*/*"}
    };

        /// <summary>
        /// 所有文件的集合，用于检索
        /// </summary>
        private List<FileModel> _allFiles;

        private UserModel _userModel;

        private JZXY.Duoke.Interface.IDocumentViewer _docViewer = DependencyService.Get<Interface.IDocumentViewer>();

        private List<FileModel> _viewFiles;

        private string _currentPath;

        private string _firstPath;

        public ItemsPage()
        {
            InitializeComponent();
            _userModel = (App.Current as App).CurrentUser;

            //_currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _userModel.LoginId);
            var _firstPath = Path.Combine("/storage/emulated/0/jzxy/", _userModel.LoginId);
            var source = GetFiles(_firstPath);
            _allFiles = GetAllFiles(_firstPath);
            _viewFiles = source;
            BindData(source);
        }

        private void BindData(List<FileModel> source)
        {
            var lst = FindByName("listView") as ListView;
            lst.ItemsSource = source;
        }

        protected override bool OnBackButtonPressed()
        {
            if (string.IsNullOrEmpty(_currentPath))
            {
                DisplayAlert("提示", "已经是根目录", "确定");
                return true;
            }
            var index = _currentPath.LastIndexOf("/");
            if (index <= Path.Combine("/storage/emulated/0/jzxy/", _userModel.LoginId).Length - 1)
            {
                DisplayAlert("提示", "已经是根目录", "确定");
                return true;
            }
            _currentPath = _currentPath.Substring(0, index);
            LoadFileModel(_currentPath);

            return true;
        }

        /// <summary>
        /// 获取当前显示的列表
        /// </summary>
        /// <returns></returns>
        private List<FileModel> GetViewSource()
        {
            var lst = FindByName("listView") as ListView;
            return lst.ItemsSource as List<FileModel>;
        }

        private List<FileModel> GetFiles(string path)
        {
            var files = Directory.GetFiles(path, ".", SearchOption.TopDirectoryOnly);
            var rst = new List<FileModel>();
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                rst.Add(new FileModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentPath = path,
                    Name = fi.Name,
                    FilePath = file,
                    Size = fi.Length + " byte",
                    Type = 1,
                    TypeName = fi.Extension
                });
            }
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                var fi = new DirectoryInfo(folder);
                rst.Add(new FileModel
                {
                    Id = Guid.NewGuid().ToString(),
                    ParentPath = path,
                    Name = fi.Name,
                    FilePath = folder,
                    Size = (fi.GetFiles().Length + fi.GetDirectories().Length).ToString(),
                    Type = 0,
                    TypeName = "文件夹"
                });
            }
            return rst;
        }

        /// <summary>
        /// 获取所有文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<FileModel> GetAllFiles(string path)
        {
            var files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
            var fileList = new List<FileModel>();
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                fileList.Add(new FileModel
                {
                    Name = fi.Name,
                    FilePath = file,
                    Size = fi.Length + " byte",
                    Type = 1
                });
            }
            return fileList;
        }

        private string GetMIMEType(string filePath)
        {
            string type = "*/*";
            if (filePath == "") return type;
            //from MIME_MapTable to get the respond type  
            for (int i = 0; i < MIME_MapTable.Length / 2; i++)
            {
                if (filePath.Equals(MIME_MapTable[i, 0]))
                    type = MIME_MapTable[i, 1];
            }
            return type;
        }

        #region 私有方法

        // 当点击查询按钮时候执行
        private void SearchBtn_Clicked(object sender, EventArgs e)
        {
            var keyWords = (FindByName("keywords") as Entry).Text.Trim();
            if (_allFiles == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(keyWords))
            {
                BindData(_viewFiles);
            }
            else
            {
                _viewFiles = GetViewSource();
                var source = _allFiles.Where(o => o.Name.Contains(keyWords) || keyWords.Contains(o.Name)).ToList();
                BindData(source);
            }
        }

        private void TextCell_Tapped(object sender, EventArgs e)
        {
            var cell = sender as ViewCell;
            if (cell != null)
            {
                var fileModel = cell.BindingContext as FileModel;
                _currentPath = fileModel.FilePath;
                LoadFileModel(fileModel);
            }
        }

        private void LoadFileModel(FileModel fileModel)
        {
            if (fileModel != null)
            {
                if (fileModel.Type == 0)
                {
                    var source = GetFiles(fileModel.FilePath);
                    BindData(source);
                }
                else
                {
                    var mimeType = GetMIMEType(fileModel.TypeName.ToLower());
                    try
                    {
                        _docViewer.ShowDocumentFile(fileModel.FilePath, mimeType);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            }
        }

        private void LoadFileModel(string path)
        {
            var source = GetFiles(path);
            BindData(source);
        }

        #endregion
    }
}