using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AATool.Tests
{
    public class TestAppDomain : ITest
    {
        private static int abc = 99;

        public void Test()
        {
            //进程下加载的模块
            var moduleList = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule module in moduleList)
                Console.WriteLine(string.Format("{0}\n  URL:{1}\n  Version:{2}",
                    module.ModuleName, module.FileName, module.FileVersionInfo.FileVersion));

            var appDomain = AppDomain.CreateDomain("NewAppDomain");
            appDomain.Load("UtilsLib");
            appDomain.DomainUnload += (s, e) =>
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " " + AppDomain.CurrentDomain.Id + AppDomain.CurrentDomain.FriendlyName + " unload");
            };

            foreach (var assembly in appDomain.GetAssemblies())
            {
                Console.WriteLine(string.Format("{0}\n----------------------------", assembly.FullName));
            }

            CrossAppDomainDelegate crossAppDomainDelegate = DoAppDomainCallBack;
            appDomain.DoCallBack(crossAppDomainDelegate);
            AppDomain.Unload(appDomain);

            //ContextBound contextBound = new ContextBound();
            //Task.Factory.StartNew(() =>
            //{
            //    contextBound.GetMoney(600);
            //});
            //Task.Factory.StartNew(() =>
            //{
            //    contextBound.GetMoney(600);
            //});

            NormalMoney normalMoney = new NormalMoney();
            Task.Factory.StartNew(() =>
            {
                normalMoney.GetMoney(600);
            });
            Task.Factory.StartNew(() =>
            {
                normalMoney.GetMoney(600);
            });
        }

        public static void DoAppDomainCallBack()
        {
            string name = AppDomain.CurrentDomain.FriendlyName;
            for (int n = 0; n < 4; n++)
                Console.WriteLine(string.Format(abc + "  Do work in {0}........", name));
        }
    }

    public class NormalMoney
    {
        private decimal currentMoney = 1000;

        [MethodImpl(methodImplOptions: MethodImplOptions.Synchronized)]
        public void GetMoney(decimal money)
        {
            if (currentMoney < money)
            {
                Console.WriteLine("money less than " + money);
                return;
            }
            Thread.Sleep(2000);
            currentMoney -= money;
            Console.WriteLine("currentMoney=" + currentMoney);
        }
    }

    [Synchronization]
    public class ContextBound : ContextBoundObject
    {
        private decimal currentMoney = 1000;

        public void GetMoney(decimal money)
        {
            if (currentMoney < money)
            {
                Console.WriteLine("money less than " + money);
                return;
            }
            Thread.Sleep(2000);
            currentMoney -= money;
            Console.WriteLine("currentMoney=" + currentMoney);
        }
    }


}
