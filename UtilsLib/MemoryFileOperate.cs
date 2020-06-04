using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace UtilsLib
{
    public class MemoryFileOperate
    {
        long capacity = 1024;

        MemoryMappedFile mmf = null;

        public MemoryFileOperate(string mapName, long capacity)
        {
            this.capacity = capacity;
            if (mmf != null)
            {
                mmf.Dispose();
            }
            mmf = MemoryMappedFile.CreateOrOpen(mapName, capacity, MemoryMappedFileAccess.ReadWrite);
        }
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="writeString"></param>
        /// <returns></returns>
        public bool WriteStr(string writeString)
        {
            bool isOk = false;
            //if (mmf == null)
            //{
            //    mmf = MemoryMappedFile.CreateOrOpen(mapName, capacity, MemoryMappedFileAccess.ReadWrite);
            //}
            //    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            //    {
            //        try
            //        {
            //            var writer = new StreamWriter(stream);
            //            writer.WriteLine(writeString);

            //            isOk = true;
            //        }
            //        catch
            //        {
            //            isOk = false;
            //        }
            //    }
            return isOk;
        }

        public string ReadStr()
        {
            string retStr = string.Empty;
            if (mmf != null)
            {
                //using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                //{
                //    try
                //    {
                //        var writer = new StreamWriter(stream);
                //        writer.WriteLine(writeString);

                //        isOk = true;
                //    }
                //    catch
                //    {
                //        isOk = false;
                //    }
                //}
            }
            return retStr;
        }

        ~MemoryFileOperate()
        {
            if (mmf != null)
                mmf.Dispose();
        }
    }
}
