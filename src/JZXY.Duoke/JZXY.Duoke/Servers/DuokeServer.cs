﻿using JZXY.Duoke.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Xamarin.Forms;

namespace JZXY.Duoke.Servers
{
    public class DuokeServer
    {
        #region 变量

        private readonly int _outtime = 30000;

        private static string _loginId;

        private IDocumentViewer _documentViewer;

        /// <summary>
        /// the temp path of current
        /// </summary>
        private static string _currentPath;

        #endregion

        public DuokeServer()
        {
            _documentViewer = DependencyService.Get<Interface.IDocumentViewer>();
        }

        #region 属性

        private static string HashKey { get; set; }

        /// <summary>
        /// 服务器地址
        /// </summary>
        private static string IPAddress { get; set; }

        private static string _rootPath;

        //private string Root
        //{
        //    get
        //    {
        //        return "/storage/emulated/0/jzxy";//Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);// "/storage/emulated/0/Download/"; //
        //    }
        //}

        #endregion

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event Action<string, int> ProcessOn;

        public event Action ProcessDone;

        #endregion

        #region 方法

        /// <summary>
        ///
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Login(string address, string loginId, string password)
        {
            IPAddress = address;
            var url = IPAddress + "/ufInterface?opr=getHash&name=" + loginId + "&password=" + password;
            var resptxt = GetHttp(url, _outtime);
            if (string.IsNullOrEmpty(resptxt) || resptxt.Contains("错误"))
            {
                return false;
            }
            HashKey = resptxt;
            _loginId = loginId;
            _rootPath = Path.Combine("/storage/emulated/0/jzxy", _loginId);
            return true;
        }

        /// <summary>
        /// 获取组织结构
        /// </summary>
        /// <returns></returns>
        private string GetOrg()
        {
            var url = $"{IPAddress}/ufInterface?opr=org&hash2={HashKey}";
            return GetHttp(url, _outtime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<FileModel> GetDoucuments()
        {
            var orgXmlStr = GetOrg();
            ProcessOn?.BeginInvoke("获取组织结构", 0, null, null);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(orgXmlStr);
            var groupItems = xmlDoc.SelectNodes("//GroupItem");
            var fileModels = new List<FileModel>();
            foreach (XmlNode item in groupItems)
            {
                var id = item.Attributes["Id"].Value;
                if (id == "0")
                {
                    continue;
                }
                _currentPath = Path.Combine(_rootPath, item.Attributes["Name"].Value);
                var files = GetFiles(ownderid: id, item.Attributes["Name"].Value);
                fileModels.AddRange(files);
            }
            return fileModels;
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="ownderid"></param>
        /// <param name="floderid"></param>
        /// <param name="floderName"></param>
        /// <returns></returns>
        public List<FileModel> GetFiles(string ownderid, string ownderName, string floderid = "0")
        {
            #region 创建目录

            var path = CreateFolder(ownderName);

            #endregion

            #region 获取文件

            var fileModels = new List<FileModel>();
            if (ownderid == "0")
            {
                return fileModels;
            }
            var url = $"{IPAddress}/ufInterface?opr=folderfiles&ownerid={ownderid}" +
                $"&folderid={floderid}&count=2048&sortid=1&sortstyle=1&isfiletx=0" +
                $"&hash2={HashKey}";
            var rst = GetHttp(url, _outtime);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rst);
            var dataNode = xmlDoc.SelectSingleNode("Data");
            if (dataNode != null)// 这里是第一次请求获取所有目录，不会涉及文件
            {
                foreach (XmlNode item in dataNode.ChildNodes)
                {
                    var id = item.Attributes["Id"].Value;
                    var pid = item.Attributes["FatherId"].Value;
                    var name = item.FirstChild.InnerText;
                    var bitem = new FileModel
                    {
                        Id = id,
                        ParentId = pid,
                        Name = name,
                        Type = 0
                    };
                    if (id != "0") // 获取下当前目的文件
                    {
                        bitem.Children.AddRange(GetFiles(ownderid, name, id));
                    }
                    var child = item.SelectSingleNode("Childs");
                    var children = child.SelectNodes("Item");
                    if (children != null)
                    {
                        foreach (XmlNode citem in children)
                        {
                            var cid = citem.Attributes["Id"].Value;
                            if (cid != "0")
                            {
                                var on = Path.Combine(ownderName, citem.SelectSingleNode("Path").InnerText);
                                bitem.Children.AddRange(GetFiles(ownderid, on,cid));
                            }
                        }
                    }
                    fileModels.Add(bitem);
                }
            }
            else
            {
                var folders = xmlDoc.SelectNodes("//Folder");
                foreach (XmlNode folder in folders)
                {
                    var name = folder.Attributes["Name"].Value;
                    var p = new FileModel()
                    {
                        Name = name,
                        Type = 0,
                    };
                    var fileItems = folder.SelectSingleNode("FileItems");
                    foreach (XmlNode item in fileItems.ChildNodes)
                    {
                        ProcessOn?.BeginInvoke($"获取文件:{item.Attributes["Name"].Value}", 0, null, null);
                        var fileModel = new FileModel
                        {
                            Id = item.Attributes["FileKey"].Value,
                            Name = item.Attributes["Name"].Value,
                            Type = 1,
                            Size = item.Attributes["Size"].Value,
                        };

                        System.Diagnostics.Debugger.Log(0, "dwonload", item.Attributes["Name"].Value);
                        fileModel.FilePath = Download(fileModel.Id, path);
                        p.Children.Add(fileModel);
                    }
                    // subfolder 
                    var subfolders = folder.SelectSingleNode("SubFolders");
                    foreach (XmlNode item in subfolders)
                    {
                        var sid = item.Attributes["Id"].Value;
                        var on = Path.Combine(ownderName, item.InnerText);
                        p.Children.AddRange(GetFiles(ownderid,on, sid));
                    }
                    fileModels.Add(p);
                }

            }
            #endregion

            return fileModels;
        }

        private string Download(string fileId, string relatePath)
        {
            string url = IPAddress + "/ufInterface?opr=download&filekey=" + fileId + "&hash2=" + HashKey;
            Uri downUri = new Uri(url);

            System.Net.HttpWebRequest myReq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(downUri);
            System.Net.HttpWebResponse myResp = (System.Net.HttpWebResponse)myReq.GetResponse();
            string head = myResp.GetResponseHeader("Content-Disposition");
            if (head.Trim() == "")
            {
                return "";
            }

            int headst = head.IndexOf("filename=\"");
            int headend = head.IndexOf("\"", headst + 10);
            string filename = head.Substring(headst + 10, headend - (headst + 10));
            filename = System.Web.HttpUtility.UrlDecode(filename);

            var filepath = Path.Combine(relatePath, filename);

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

                    fs.Flush();
                    fs.Close();
                }
            }
            System.Diagnostics.Debugger.Log(0, fileId, filepath);
            return filepath;
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        private string CreateFolder(string folderName)
        {
            var path = Path.Combine(_rootPath, folderName);
            Directory.CreateDirectory(path);
            return path;
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

        #endregion
    }
}
