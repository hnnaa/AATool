using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib
{
    public class RegistryUtil
    {
        /// <summary>
        /// 设置开机启动
        /// </summary>
        /// <param name="key">指定注册表key名</param>
        /// <param name="filepath">指定开机启动的路径，取消设置时可为空</param>
        /// <param name="allow">true:设置，false:取消设置</param>
        public static void SetStartup(string key, string filepath, bool allow)
        {
            if (allow) //设置开机自启动  
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue(key, filepath);
                rk2.Close();
                rk.Close();
            }
            else //取消开机自启动  
            {
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue(key, false);
                rk2.Close();
                rk.Close();
            }
        }
    }
}
