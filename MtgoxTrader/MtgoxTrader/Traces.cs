//****************************************************************************             
//
// @File: Traces.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MtGoxTrader.Trader
{
    public class LogTraceListener : TextWriterTraceListener
    {
        public LogTraceListener(System.IO.Stream stream, string name) : base(stream, name) { }
        public LogTraceListener(System.IO.Stream stream) : base(stream) { }
        public LogTraceListener(string fileName, string name) : base(ModifyFileName(fileName), name) { }
        public LogTraceListener(string fileName) : base(ModifyFileName(fileName)) { }
        public LogTraceListener(System.IO.TextWriter writer, string name) : base(writer, name) { }
        public LogTraceListener(System.IO.TextWriter writer) : base(writer) { }

        static string ModifyFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            string parentDirectory = Path.GetDirectoryName(fileName);
            return string.Format(@"{0}\{1}_{2}{3}", parentDirectory, fileNameWithoutExtension, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), extension);
        }

        public override void WriteLine(object o)
        {
            base.WriteLine(o);
        }
    }
}
