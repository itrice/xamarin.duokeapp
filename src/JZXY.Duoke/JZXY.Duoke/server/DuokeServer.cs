using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JZXY.Duoke.server
{
    public class DuokeServer
    {
        static DuokeServer()
        {
            Instance = new DuokeServer();
        }
        private readonly int _outtime = 3000;

        private string HashKey { get; set; }


        /// <summary>
        /// 服务器地址
        /// </summary>
        private string IPAddress { get; set; }

        public static DuokeServer Instance { get; private set; }

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
            var resptxt = GetHttp(url, 3000);
            if (string.IsNullOrEmpty(resptxt) || resptxt.Contains("错误"))
            {
                return false;
            }
            HashKey = resptxt;
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
                var files = GetFiles(id);
                fileModels.AddRange(files);
            }
            return fileModels;
        }


        public List<FileModel> GetFiles(string ownderid, string floderid = "0")
        {
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
            if (dataNode != null)// 这里是获取所有目录，不会涉及文件
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
                    if (id != "0")
                    {
                        bitem.Children.AddRange(GetFiles(ownderid, id)); // 获取下当前目的文件                    
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
                                bitem.Children.AddRange(GetFiles(ownderid, cid));
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
                    var fileItems = folder.FirstChild;
                    foreach (XmlNode item in fileItems.ChildNodes)
                    {
                        p.Children.Add(new FileModel
                        {
                            Name = item.Attributes["Name"].Value,
                            Type = 1,
                            Size = item.Attributes["Size"].Value,
                        });
                    }
                    // subfolder
                    var subfolders = folder.LastChild;
                    foreach (XmlNode item in subfolders)
                    {
                        var sid = item.Attributes["Id"].Value;
                        p.Children.AddRange(GetFiles(ownderid,sid));
                    }
                    fileModels.Add(p);
                }
            }
            return fileModels;
        }

        public void Download(string filepath)
        {
            throw new NotImplementedException();
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

    }
}
