using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class DbException
    {

        [Serializable]
        public class OtherSyntaxError : Exception
        {
            public OtherSyntaxError() : base("Other Syntax Error") { }
            public OtherSyntaxError(string message) : base(message) { }
            public OtherSyntaxError(string message, Exception inner) : base(message, inner) { }
        }

        [Serializable]
        public class InvalidKeyword : Exception
        {
            public InvalidKeyword() : base("Invalid Keyword Error") { }
            public InvalidKeyword(string message) : base(message) { }
            public InvalidKeyword(string message, Exception inner) : base(message, inner) { }
        }

        [Serializable]
        public class UnkownKeyword : Exception
        {
            public UnkownKeyword() : base("Unkown Keyword Error") { }
            public UnkownKeyword(string message) : base(message) { }
            public UnkownKeyword(string message, Exception inner) : base(message, inner) { }
        }

        [Serializable]
        public class MismatchingArguments: Exception
        {
            public MismatchingArguments() : base("Unkown Keyword Error") { }
            public MismatchingArguments(string message) : base(message) { }
            public MismatchingArguments(string message, Exception inner) : base(message, inner) { }
        }

    }
}
