using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMLight
{
    [AttributeUsage(System.AttributeTargets.Method)]
    public class Inputs : Attribute
    {
        public int[] Transitions
        {
            get;
            set;
        }

        public Inputs(params int[] transitions)
        {
            Transitions = transitions;
        }
    }

    [AttributeUsage(System.AttributeTargets.Method)]
    public class Outputs : Attribute
    {
        public int[] Transitions
        {
            get;
            set;
        }

        public Outputs(params int[] transitions)
        {
            Transitions = transitions;
        }
    }
}
