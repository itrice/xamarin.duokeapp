using System;
using System.Collections.Generic;
using System.Text;

namespace JZXY.Duoke.Interface
{
    /// <summary>
    /// 定义一个网络工作服务接口
    /// </summary>
    public interface INetworkServer
    {
        bool IsNetWrokConnected();

        bool IsWifiConnected();

        bool IsMobileConnected();
    }
}
