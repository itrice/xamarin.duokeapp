using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

[assembly:Xamarin.Forms.Dependency(typeof(JZXY.Duoke.iOS.NetworkServer))]
namespace JZXY.Duoke.iOS
{
    public class NetworkServer : Interface.INetworkServer
    {
        public bool IsMobileConnected()
        {
            throw new NotImplementedException();
        }

        public bool IsNetWrokConnected()
        {
            throw new NotImplementedException();
        }

        public bool IsWifiConnected()
        {
            throw new NotImplementedException();
        }
    }
}