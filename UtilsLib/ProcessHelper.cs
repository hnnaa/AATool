using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace UtilsLib
{
   public class ProcessHelper
    {
        /// <summary>
        /// 执行启动程序
        /// </summary>
        /// <param name="path"></param>
        public static bool StartScriptProcess(string workingDirectory, string path)
        {
            if (File.Exists(path))
            {
                Process startprocess = new Process();
                startprocess.StartInfo.WorkingDirectory = workingDirectory;
                startprocess.StartInfo.FileName = path;
                startprocess.StartInfo.UseShellExecute = false;
                startprocess.StartInfo.RedirectStandardInput = true;
                startprocess.StartInfo.RedirectStandardOutput = true;
                startprocess.StartInfo.RedirectStandardError = true;
                startprocess.StartInfo.CreateNoWindow = true;
                startprocess.Start();

                startprocess.WaitForExit();
                return true;
            }
            return false;
        }

        public static void KillProcessByName(string filenameWithOutExt)
        {
            Process[] processes = Process.GetProcessesByName(filenameWithOutExt);
            if (processes != null && processes.Length > 0)
            {
                foreach (Process p in processes)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {
                        p.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 根据文件名获取文件路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetProcessFilePath(string filename)
        {
            string filepath = "";
            try
            {
                string filenameWithNoExtend = Path.GetFileNameWithoutExtension(filename);
                Process[] processes = Process.GetProcessesByName(filenameWithNoExtend);
                if (processes != null && processes.Length > 0)
                {
                    filepath = processes[0].MainModule.FileName;
                }
            }
            catch (Exception ex)
            {
            }
            return filepath;
        }
        /// <summary>
        /// 根据文件名获取进程
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Process GetFirstProcess(string filename)
        {
            Process p = null;
            try
            {
                string filenameWithNoExtend = Path.GetFileNameWithoutExtension(filename);
                Process[] processes = Process.GetProcessesByName(filenameWithNoExtend);
                if (processes != null && processes.Length > 0)
                {
                    p = processes[0];
                }
            }
            catch (Exception ex)
            {
            }
            return p;
        }
        /// <summary>
        /// 根据文件名获取进程关联的窗体句柄
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static IntPtr GetFirstProcessWindowsHandle(string filename)
        {
            IntPtr handle = IntPtr.Zero;
            Process p = GetFirstProcess(filename);
            if (p != null)
            {
                handle = p.MainWindowHandle;
            }
            return handle;
        }
        /// <summary>
        /// 检测当前路径的程序个数
        /// </summary>
        /// <param name="path"></param>
        /// <param name="totalCounts">返回同名总个数</param>
        /// <returns></returns>
        public static int CurrentPathProcessCounts(string path, out int totalCounts)
        {
            int counts = 0;
            totalCounts = 0;

            string fileProcessName = Path.GetFileNameWithoutExtension(path);
            Process[] processes = Process.GetProcessesByName(fileProcessName);

            if (processes != null && processes.Length > 0)
            {
                totalCounts = processes.Length;
                foreach (Process p in processes)
                {
                    if (p.MainModule.FileName == path)
                    {
                        counts++;
                    }
                }
            }
            return counts;
        }
    }
}
