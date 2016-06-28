using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMLight
{
    public class State
    {
        public State()
        {
            allowed_in = null;
            allowed_out = null;
        }
        public void SetInputs(params int[] ins)
        {
            Ins = ins;
        }
        public void SetOutputs(params int[] outs)
        {
            Outs = outs;
        }
        public bool input_set;
        public bool output_set;
        private int[] allowed_in;
        private int[] allowed_out;

        public int[] Ins
        {
            get
            {
                return allowed_in;
            }
            set
            {
                if (input_set)
                {
                    throw new Exception("Can't set inputs twice.");
                }
                else
                {
                    input_set = true;
                    allowed_in = value;
                }
            }
        }
        public int[] Outs
        {
            get
            {
                return allowed_out;
            }
            set
            {
                if (output_set)
                {
                    throw new Exception("Can't set outputs twice.");
                }
                else
                {
                    output_set = true;
                    allowed_out = value;
                }
            }
        }
        public Func<int> Run;


    }
}
