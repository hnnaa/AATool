using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace UtilsLib.IO.MemoryFile
{
    public class MemoryFileReader
    {
        string mapName;

        MemoryMappedFile mmf = null;

        public MemoryFileReader(string mapName)
        {
            this.mapName = mapName;
        }

        public bool Init()
        {
            bool isOk = false;
            try
            {
                if (!string.IsNullOrEmpty(mapName))
                {
                    mmf = MemoryMappedFile.OpenExisting(mapName);
                }
            }
            catch
            {
                isOk = false;
            }
            return isOk;
        }

        public string ReadStr()
        {
            string retStr = string.Empty;
            if (mmf != null)
            {
                using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                {
                    //try
                    //{
                    //    var reader = new StreamReader (stream);
                    //    reader.WriteLine(writeString);

                    //    isOk = true;
                    //}
                    //catch
                    //{
                    //    isOk = false;
                    //}
                }
            }
            return retStr;
        }

        ~MemoryFileReader()
        {
            if (mmf != null)
                mmf.Dispose();
        }
    }
}
