using System;

namespace log4net
{
    internal interface ILog
    {
        void Info(object value);

        void Debug(object value);

        void Error(object value);

        void Error(object value, Exception ex);

        void Warn(object value);

        void ErrorFormat(object value);
    }
}
