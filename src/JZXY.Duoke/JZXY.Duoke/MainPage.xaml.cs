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
                       
            //_currentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _userModel.LoginId);
            _currentPath = Path.Combine("/storage/emulated/0/jzxy/", _userModel.LoginId);
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
            try
            {
                var files = Directory.GetFiles(path, ".", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fi = new FileInfo(file);
                    _source.Add(new FileModel
                    {
                        Name = fi.Name,
                        FilePath = file,
                        Size = fi.Length + " byte",
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
            catch (Exception)
            {
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
            if (cell != null)
            {
                if (cell.Detail == "0")
                {
                    _currentPath = Path.Combine(_currentPath, cell.Text);
                    LoadData(_currentPath);
                    BindData();
                }
                else
                {
                    var filePath = cell.Text;
                    var mimeType = GetMIMEType(Path.GetExtension(filePath).ToLower());
                    //"application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                    //switch (Path.GetExtension(filePath).ToLower())
                    //{
                    //    case ".bmp":
                    //        mimeType = "image/bmp";
                    //        break;
                    //    case ".pdf":
                    //        mimeType = "application/pdf";
                    //        break;
                    //    case ".png":
                    //    case ".jpg":
                    //        mimeType = "image/jpeg";
                    //        break;
                    //    default:
                    //        mimeType = "*/*";
                    //        break;
                    //}

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

        public string[,] MIME_MapTable = new string[,] {

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
    }
}
