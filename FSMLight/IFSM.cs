using System;
using System.Collections.Generic;
namespace FSMLight
{
    public interface IFSM
    {
        void Reset();
        bool SingleStep();
        IEnumerable<State> States { get; }
        bool Stopped { get; }
    }
}
