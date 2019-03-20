using System;
using System.Collections.Generic;
using System.Text;

namespace JZXY.Duoke.server
{
    public class DocumentServer
    {
        public string Login(string url,string id)
        {
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
            req.Timeout = 3000;
            System.Net.HttpWebResponse HttpWResp = (System.Net.HttpWebResponse)req.GetResponse();
            System.IO.Stream myStream = HttpWResp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(myStream, Encoding.UTF8);
            StringBuilder sb = new StringBuilder();
            while (!sr.EndOfStream)
            {
                sb.AppendLine(sr.ReadLine());
            }
            return sb.ToString();
        }

        public string GetDoucuments()
        {
            throw new NotImplementedException();
        }

        public string GetFiles()
        {
            throw new NotImplementedException();
        }

        public void Download(string filepath)
        {
            throw new NotImplementedException();
        }
    }
}
