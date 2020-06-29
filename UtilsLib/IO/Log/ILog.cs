using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib.IO.Log
{
    public interface ILog
    {
        void WriteLogMessage(string msg);

        void WriteErrorLogMessage(string msg);
    }
}
