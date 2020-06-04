using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace UtilsLib
{
    public class NetHelper
    {

        public static void GetIPs()
        {
            NetworkInterface[] NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface NetworkIntf in NetworkInterfaces)
            {
                IPInterfaceProperties IPInterfaceProperties = NetworkIntf.GetIPProperties();
                UnicastIPAddressInformationCollection UnicastIPAddressInformationCollection = IPInterfaceProperties.UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicastIPAddressInformation in UnicastIPAddressInformationCollection)
                {
                    if (UnicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                    }
                }
            }

            var i = Dns.GetHostEntry(Dns.GetHostName());
            var c = Dns.Resolve(Dns.GetHostName());

            System.Diagnostics.Process[] ss = System.Diagnostics.Process.GetProcessesByName("UpdateService");
            var s = ss[0].MainModule.FileVersionInfo;

        }
    }
}

