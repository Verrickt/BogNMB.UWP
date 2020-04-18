using System;
using System.Collections.Generic;
using System.Text;

namespace BogNMB.API.Controllers
{
    [Serializable]
    public class BogException : Exception
    {
        private static readonly Dictionary<int, string> errorPhases = new Dictionary<int, string>()
        {
            {0,"不存在的API" },
            {1,"不存在的串" },
            {2,"不存在的页面" },
            {3,"不存在的串" },
        };
        public BogException() { }

        public BogException(int errorId) :this(errorPhases[errorId]) { }

        public BogException(string message) : base(message) { }
        public BogException(string message, Exception inner) : base(message, inner) { }
        protected BogException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
