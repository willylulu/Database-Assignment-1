using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class DbException
    {
        public class UnknownTypeException : Exception
        {
            public UnknownTypeException() : base("Unkown Type Error") { }
            public UnknownTypeException(string message) : base(message) { }
            public UnknownTypeException(string message, Exception inner) : base(message, inner) { }
        }


        public class InvalidCommaException : Exception
        {
            public InvalidCommaException() : base("Invalid Comma Error") { }
            public InvalidCommaException(string message) : base(message) { }
            public InvalidCommaException(string message, Exception inner) : base(message, inner) { }
        }

        public class InvalidSQLArguments : Exception
        {
            public InvalidSQLArguments() : base("Invalid Sql Arguments") { }
            public InvalidSQLArguments(string message) : base(message) { }
            public InvalidSQLArguments(string message, Exception inner) : base(message, inner) { }
        }

        public class UnkownInstruction : Exception
        {
            public UnkownInstruction() : base("Unkown Instruction") { }
            public UnkownInstruction(string message) : base(message) { }
            public UnkownInstruction(string message, Exception inner) : base(message, inner) { }
        }


    }
}
