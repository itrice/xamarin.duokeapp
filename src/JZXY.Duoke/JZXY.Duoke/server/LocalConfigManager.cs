using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace JZXY.Duoke.Server
{
    /// <summary>
    /// 定义一个本地配置服务
    /// </summary>
     class LocalConfigManager
    {
        private Dictionary<int, string> _dic;

        private static LocalConfigManager _instance;

        static LocalConfigManager()
        {
            _instance = new LocalConfigManager();
        }

        enum DicKey
        {
            ADDRESS, ISAUTOLOGIN, ISSAVELOGININFO,
        }
        
        public static LocalConfigManager Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// is auto login
        /// </summary>
        public bool IsAutoLogin
        {
            get
            {
                var rs = GetValue(DicKey.ISAUTOLOGIN);
                if (rs == null)
                {
                    return false;
                }
                return String2Boolean(rs);
            }
            set
            {
                SetValue(DicKey.ISAUTOLOGIN, value.ToString());
            }
        }

        /// <summary>
        /// get or set issave login infomations
        /// </summary>
        public bool IsSaveLoginInfo
        {
            get
            {
                var rs = GetValue(DicKey.ISSAVELOGININFO);
                if (rs == null)
                {
                    return false;
                }
                return String2Boolean(rs);
            }
            set
            {
                SetValue(DicKey.ISSAVELOGININFO, value.ToString());
            }
        }

        /// <summary>
        /// get or set the server's address
        /// </summary>
        public string ServerAddress
        {
            get
            {
                return GetValue(DicKey.ADDRESS);
            }
            set
            {
                SetValue(DicKey.ADDRESS, value);
            }
        }
        
        #region private methods

        /// <summary>
        /// set value of the key
        /// </summary>
        /// <param name="address"></param>
        private void SetValue(DicKey key, string address)
        {
            var dic = GetKeyValues();
            var ikey = (int)DicKey.ADDRESS;
            if (dic.ContainsKey(ikey))
            {
                dic[ikey] = address;
            }
            else
            {
                dic.Add(ikey, address);
            }
            SetKeyValues(dic);
        }

        /// <summary>
        /// get value of the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(DicKey key)
        {
            var dic = GetKeyValues();
            var ikey = (int)key;            
            if (dic.ContainsKey(ikey))
            {
                return dic[ikey];
            }

            return string.Empty;
        }

        /// <summary>
        /// to convert string to boolean
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool String2Boolean(string value)
        {
            try
            {
                bool rst;
                if (bool.TryParse(value, out rst))
                {
                    return rst;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region local file access

        /// <summary>
        /// get local file content and convert to dictionary
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> GetKeyValues(bool force = false)
        {
            if (force)
            {
                _dic = null;
            }

            if (_dic != null)
            {
                return _dic;
            }
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "my.config");

            if (File.Exists(fullFileName))
            {
                var fileContent = File.ReadAllText(fullFileName);
                if (!string.IsNullOrEmpty(fileContent))
                {
                    _dic = JsonConvert.DeserializeObject<Dictionary<int, string>>(fileContent);
                }
            }
            if(_dic == null)
            {
                return new Dictionary<int, string>();
            }
            return _dic;
        }

        /// <summary>
        /// set and save local config file
        /// </summary>
        /// <param name="dic"></param>
        private void SetKeyValues(Dictionary<int, string> dic)
        {
            var fileContent = JsonConvert.SerializeObject(dic);
            var fullFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "my.config");
            File.WriteAllText(fullFileName, fileContent);            
        }

        #endregion

        #endregion

    }
}
