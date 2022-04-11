using System;
using System.Runtime.Serialization;

namespace Suretom.Client.Common
{
    /// <summary>
    /// 客户端通用异常类
    /// 主动抛出的异常
    /// </summary>
    [Serializable]
    public class CtbException : Exception
    {
        public CtbException(string message)
            : base(message)
        {
        }

        public CtbException(string messageFormat, params object[] args)
            : base(string.Format(messageFormat, args))
        {
        }

        protected CtbException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CtbException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}