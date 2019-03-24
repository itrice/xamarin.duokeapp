using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JZXY.Duoke.ViewModel
{
    public class UpdatingViewModel: INotifyPropertyChanged
    {
        private string title;

        private string detial;

        private int process;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public string Detial
        {
            get { return detial; }
            set
            {
                detial = value;
                NotifyPropertyChanged("Detial");
            }
        }

        /// <summary>
        /// 进度百分比
        /// </summary>
        public int Process { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
