using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace UtilsLib
{
    public class WindowsServerControl
    {
        ServiceController serviceController;

        public bool IsExisted
        {
            get { return serviceController != null; }
        }

        public WindowsServerControl(string serviceName)
        {
            this.serviceController = GetService(serviceName);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private ServiceController GetService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return s;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取服务所在路径
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public string GetServiceFilePath()
        {
            string filepath = "";
            if (IsExisted)
            {
                RegistryKey _Key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Services\" + serviceController.ServiceName);
                if (_Key != null)
                {
                    object _ObjPath = _Key.GetValue("ImagePath");
                    if (_ObjPath != null)
                        filepath = _ObjPath.ToString();

                    _Key.Close();
                    _Key.Dispose();
                }
            }
            return filepath;
        }
        /// <summary>
        /// 断开此 System.ServiceProcess.ServiceController 实例与服务的连接，并释放此实例分配的所有资源
        /// </summary>
        public void Close()
        {
            if (IsExisted)
                serviceController.Close();
            else
                throw new NullReferenceException("断开此 System.ServiceProcess.ServiceController 实例与服务的连接，并释放此实例分配的所有资源，实例不存在。");
        }
    }
}
