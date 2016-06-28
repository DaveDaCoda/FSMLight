using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMLight
{
    public class FSMException : Exception
    {
        public FSMException(string message)
            : base(message)
        {

        }
    }
}
