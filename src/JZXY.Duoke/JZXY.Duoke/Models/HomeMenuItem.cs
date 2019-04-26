using System;
using System.Collections.Generic;
using System.Text;

namespace JZXY.Duoke.Models
{
    public enum MenuItemType
    {
        Browse,
        Resent,
        Config,
        Exit
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
