//TODO: Get rid of all of this and replace with standard loggin
using System;

namespace log4net
{
    internal static class LogManager
    {
        public static ILog GetLogger(string @namespace)
        {
            return new Log(@namespace);
        }

        class Log : ILog
        {
            private readonly string _nameSpace;

            public Log(string nameSpace)
            {
                _nameSpace = nameSpace;
            }

            public void Debug(object value)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
            }

            public void Error(object value)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
            }

            public void Error(object value, Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
                Console.WriteLine(ex);
            }

            public void ErrorFormat(object value)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
            }

            public void Info(object value)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
            }

            public void Warn(object value)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now.ToString()}] {value}");
            }
        }
    }
}
