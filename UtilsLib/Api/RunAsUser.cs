using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace UtilsLib.Api
{
    public class RunAsUser
    {
        /// <summary>
        /// 创建用户进程
        /// </summary>
        /// <param name="appFullName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CreateProcess(string appFullName, out string msg)
        {
            return CreateProcess(appFullName, "", out msg);
        }
        /// <summary>
        /// 创建用户进程（带参数）
        /// </summary>
        /// <param name="appFullName"></param>
        /// <param name="args"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CreateProcess(string appFullName, string args, out string msg)
        {
            msg = "";
            int? sessonId = GetActiveSessionId(out msg);
            //如果已有用户登陆（
            if (sessonId.HasValue)
            {
                return CreateProcess(sessonId.Value, appFullName, args, out msg);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 在指定用户的会话ID下创建进程
        /// </summary>
        /// <param name="sessionId">用户的会话ID，默认为当前活动用户</param>
        /// <param name="appFullName">程序完整路径</param>
        /// <param name="args">启动参数</param>
        /// <param name="msg">返回运行信息</param>
        /// <returns></returns>
        public static bool CreateProcess(int sessionId, string appFullName, string args, out string msg)
        {
            if (!System.IO.File.Exists(appFullName))
            {
                throw new System.IO.FileNotFoundException(appFullName);
            }

            bool success = false;
            msg = string.Empty;

            IntPtr hToken = IntPtr.Zero,
                   hDupedToken = IntPtr.Zero,
                   lpEnvironment = IntPtr.Zero;
            Mapper.Process_Information pi = new Mapper.Process_Information();
            Mapper.SecurityAttributes sa;
            try
            {
                //获取指定会话的用户令牌，须具备System权限
                success = Mapper.WTSQueryUserToken(sessionId, out hToken);
                if (!success)
                {
                    //服务程序中，获取指定会话的桌面进程
                    var explorer = Process.GetProcesses()
                        .FirstOrDefault(p => p.SessionId == sessionId && string.Equals(p.ProcessName, Setting.ExplorerProcess, StringComparison.OrdinalIgnoreCase));
                    if (explorer == null)
                    {
                        return false;
                    }
                    success = Mapper.OpenProcessToken(explorer.Handle, TokenAccessLevels.AllAccess, ref hToken);
                    if (!success)
                    {
                        msg = GetMsg_TraceWin32Error(sessionId.ToString(), "WTSQueryUserToken");
                        return false;
                    }
                }
                //复制桌面进程的句柄
                sa = new Mapper.SecurityAttributes();
                sa.Length = Marshal.SizeOf(sa);
                var si = new Mapper.StartUpInfo();
                si.cb = Marshal.SizeOf(si);
                success = Mapper.DuplicateTokenEx(
                      hToken,
                      Mapper.GENERIC_ALL_ACCESS,
                      ref sa,
                      (int)Mapper.SecurityImpersonationLevel.SecurityIdentification,
                      (int)Mapper.TokenType.TokenPrimary,
                      ref hDupedToken
                );
                if (!success)
                {
                    msg = GetMsg_TraceWin32Error(sessionId.ToString(), "DuplicateTokenEx");
                    return false;
                }
                //利用复制的句柄在指定的会话中初始化运行环境                
                success = Mapper.CreateEnvironmentBlock(out lpEnvironment, hDupedToken, false);
                if (!success)
                {
                    msg = GetMsg_TraceWin32Error(sessionId.ToString(), "CreateEnvironmentBlock");
                    return false;
                }
                //在指定会话中开启进程
                if (!string.IsNullOrEmpty(args))
                {
                    args = string.Format("\"{0}\" {1}", appFullName, args);
                    appFullName = null;
                }
                success = Mapper.CreateProcessAsUser(
                    hDupedToken,
                    appFullName,
                    args,
                    ref sa, ref sa,
                    false, 0, IntPtr.Zero,
                    null,
                    ref si,
                    ref pi
                );
                if (!success)
                {
                    msg = GetMsg_TraceWin32Error(sessionId.ToString(), "CreateProcessAsUser");
                }

                return success;
            }
            catch (Exception exp)
            {
                msg = GetMsg_TraceWin32Error(sessionId.ToString(), exp.Message);
                return false;
            }
            finally
            {
                if (hDupedToken != IntPtr.Zero) Mapper.CloseHandle(hDupedToken);
                if (lpEnvironment != IntPtr.Zero) Mapper.DestroyEnvironmentBlock(lpEnvironment);
                if (pi.hProcess != IntPtr.Zero) Mapper.CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero) Mapper.CloseHandle(pi.hThread);
            }
        }
        public static int? GetActiveSessionId(out string msg)
        {
            msg = string.Empty;
            int? id = null;
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero, ppSessionInfo = IntPtr.Zero;

            try
            {
                int count = 0;
                bool success = Mapper.WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1, ref ppSessionInfo, ref count);
                if (success)
                {
                    int dataSize = Marshal.SizeOf(typeof(Mapper.WTS_SESSION_INFO));
                    var current = ppSessionInfo.ToInt64();  //数组起始地址
                    for (int i = 0; i < count; i++)
                    {
                        var si = (Mapper.WTS_SESSION_INFO)Marshal.PtrToStructure((IntPtr)current, typeof(Mapper.WTS_SESSION_INFO));
                        if (si.State == Mapper.WTS_CONNECTSTATE_CLASS.WTSActive)
                        {
                            id = si.SessionID;
                            break;
                        }
                        current += dataSize;
                    }

                    if (!id.HasValue)
                    {
                        msg = "Find WTSActive From Sessions Fail";
                    }
                }
                else
                {
                    msg = "Find ActiveSessionId From WTSEnumerateSessions Fail";
                }
            }
            finally
            {
                if (ppSessionInfo != IntPtr.Zero) Mapper.WTSFreeMemory(ppSessionInfo);
            }

            return id;
        }
        private static string GetMsg_TraceWin32Error(string sessionIdStr, string error)
        {
            string msg = string.Empty;
            try
            {
                msg = string.Format("CreateProcessAs Session:{0},step:{1},error code:{2}", sessionIdStr, error, Marshal.GetLastWin32Error().ToString());
            }
            catch
            {
                msg = "log error";
            }
            return msg;
        }
    }
}
