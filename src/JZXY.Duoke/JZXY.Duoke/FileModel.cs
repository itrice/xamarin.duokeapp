using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace JZXY.Duoke
{
    /// <summary>
    /// 定义一个文件模型
    /// 目录也是文件
    /// </summary>
    public class FileModel
    {
        public FileModel()
        {
            Children = new List<FileModel>();
        }

        public string Id { get; set; }
        public string ParentId { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        /// <summary>
        /// 0:folder 1:file
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParentPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }

        public ImageSource ImgSrouce{get;set;}

        public List<FileModel> Children { get; set; }
    }
}
