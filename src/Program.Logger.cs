using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace surveillance_system
{
    public partial class Program
    {
        public class Logger
        {
            private string loc;

            public Logger(string sLoc)
            {
                loc = sLoc;
            }

            public string getLogTime()
            {
                return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            }

            public void info(string msg)
            {
                string logTime = this.getLogTime();
                Console.WriteLine(logTime + "@" + loc + " [info] " + msg + "\n");
            }

            public void debug(string msg)
            {
                string logTime = this.getLogTime();
                Console.WriteLine(logTime + "@" + loc + " [debug] " + msg + "\n");
            }

            public void warn(string msg)
            {
                string logTime = this.getLogTime();
                Console.WriteLine(logTime + "@" + loc + " [warn] " + msg + "\n");
            }
        }
    }
}
