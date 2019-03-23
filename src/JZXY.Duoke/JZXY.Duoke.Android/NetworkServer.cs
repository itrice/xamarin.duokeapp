using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly:Dependency(typeof(JZXY.Duoke.Droid.NetworkServer))]
namespace JZXY.Duoke.Droid
{
    public class NetworkServer : Interface.INetworkServer
    {
        public bool IsMobileConnected()
        {
            ConnectivityManager manager = ConnectivityManager.FromContext(MainActivity.AppContext);
            NetworkInfo info = manager.GetNetworkInfo(ConnectivityType.Mobile);
            if (info != null)
            {
                return info.IsAvailable;
            }
            return false;
        }

        public bool IsNetWrokConnected()
        {
            ConnectivityManager manager = ConnectivityManager.FromContext(MainActivity.AppContext);
            var info = manager.ActiveNetworkInfo;
            if (info == null)
            {
                return false;
            }
            return info.IsAvailable;
        }

        public bool IsWifiConnected()
        {
            ConnectivityManager manager = ConnectivityManager.FromContext(MainActivity.AppContext);
            NetworkInfo info = manager.GetNetworkInfo(ConnectivityType.Wifi);
            if (info != null)
            {
                return info.IsAvailable;
            }
            return false;
        }
    }
}