using System.Collections.Generic;

namespace FSMLight
{
    public class FiniteStateMachineSet : IEnumerable<IFSM>
    {
        private static FiniteStateMachineSet _default_instance;

        private List<IFSM> machines;

        private object syncobj;

        public FiniteStateMachineSet()
        {
            syncobj = new object();
        }

        public static FiniteStateMachineSet Default
        {
            get
            {
                if (_default_instance == null)
                    _default_instance = new FiniteStateMachineSet();
                return _default_instance;
            }
        }

    
        public void Add(IFSM machine)
        {
            lock (syncobj)
            {
                if (machines == null)
                    machines = new List<IFSM>();
                if (!machines.Contains(machine))
                    machines.Add(machine);
            }
        }

        public void Remove(IFSM machine)
        {
            lock (syncobj)
            {
                if (machines != null)
                {
                    machines.Remove(machine);
                }
            }
        }

        public void RemoveAllStopped()
        {
            lock (syncobj)
            {
                if (machines != null)
                {
                    machines.RemoveAll(x => x.Stopped == true);
                }
            }
        }

        public bool SingleStepAll()
        {
            
            // default return value when there are no machines in set.
            bool ret = false;
            lock (syncobj)
            {
                if (machines != null && machines.Count>0)
                {
                    foreach (var m in machines)
                    {

                       if(m.SingleStep())
                       {
                           // return true if any machine has work pending.
                           ret = true;
                       }
                    }
                }
            }
            return ret;
        }

        public IEnumerator<IFSM> GetEnumerator()
        {
            lock (syncobj)
            {
                if (machines != null)
                {
                    foreach (var x in machines)
                        yield return x;
                }
            }
            yield break;
            
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}