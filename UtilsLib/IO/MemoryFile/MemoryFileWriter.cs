using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace UtilsLib.IO.MemoryFile
{
    public class MemoryFileWriter
    {
        string mapName;
        long capacity;

        long minCapacity = 1024;

        MemoryMappedFile mmf = null;

        public MemoryFileWriter(string mapName, long capacity)
        {
            this.mapName = mapName;
            this.capacity = capacity > minCapacity ? capacity : minCapacity;
        }

        public bool Init()
        {
            bool isOk = false;
            try
            {
                if (!string.IsNullOrEmpty(mapName))
                {
                    mmf = MemoryMappedFile.CreateNew(mapName, capacity, MemoryMappedFileAccess.ReadWrite);
                }
            }
            catch 
            {
                isOk = false;
            }
            return isOk;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="writeString"></param>
        /// <returns></returns>
        public bool WriteStr(string writeString)
        {
            bool isOk = false;
            if (mmf != null)
            {
                //using (MemoryMappedViewStream stream = mmf.CreateViewStream(0,this.capacity))
                //{
                //    try
                //    {
                //        var writer = new StreamWriter(stream,false);
                //        writer.
                //        writer.WriteLine(writeString);

                //        isOk = true;
                //    }
                //    catch
                //    {
                //        isOk = false;
                //    }
                //}
            }
            return isOk;
        }

        private byte[] BuildWriteBytes(string str)
        {
            byte[] writebytes = new byte[str.Length + 4];
            //byte[] strLength= str.Length
            return writebytes;
        }

        ~MemoryFileWriter()
        {
            if (mmf != null)
                mmf.Dispose();
        }
    }
}
