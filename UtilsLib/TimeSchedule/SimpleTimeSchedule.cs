using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UtilsLib.TimeSchedule
{
    public class SimpleTimeSchedule
    {
        public static void StartAll(List<SimpleTimeSchedule> SimpleTimeSchedules)
        {
            foreach (var item in SimpleTimeSchedules)
            {
                item.Start();
            }
        }

        /// <summary>
        /// 时间间隔，单位：毫秒
        /// </summary>
        private int interval;
        /// <summary>
        /// 动作
        /// </summary>
        private Action act;
        /// <summary>
        /// 名字
        /// </summary>
        private string name;

        /// <summary>
        /// wait
        /// </summary>
        private ManualResetEvent mre = new ManualResetEvent(false);
        /// <summary>
        /// 停止
        /// </summary>
        private bool isStopping = false;

        public SimpleTimeSchedule(string name, int interval, Action act)
        {
            this.name = name;
            this.interval = interval;
            this.act = act;
        }

        public void Start()
        {
            isStopping = false;

            //LogFile.WriteLogMessage("启动定时任务:" + name);

            Thread th = new Thread(() =>
            {
                while (!isStopping)
                {
                    if (!mre.WaitOne(interval))
                    {
                        DoWork();
                    }
                    else
                    {
                        break;
                    }
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        public void Stop()
        {
            isStopping = true;
            mre.Set();
        }

        private void DoWork()
        {
            try
            {
                if (act != null)
                {
                    act.Invoke();
                }
            }
            catch (Exception ex)
            {
                //LogFile.WriteErrorLogMessage(name + "调度异常" + ex);
            }
        }
    }
}
