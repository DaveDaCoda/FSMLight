using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FSMLight
{
     //Example: A simple state machine which initialzes a random number generator, then chooses to either trigger an error, 
     //terminate itself, or choose another number.
     
     //public class MyMachine : FSM
     //{
     //   const int INITED = 1;
     //   const int RUNNING = 2;
     //   const int ERROR = 3;
     //   const int DONE = 4;
        
     //   Random rand;
        
     //   public MyMachine()
     //   {
     //   }
        
     //   [Outputs(RUNNING)]
     //   public int Init()
     //   {
     //       rand = new Random();
     //       return RUNNING;
     //   }
    
     //   [Inputs(RUNNING), Outputs(RUNNING, ERROR, DONE)]
     //   public int DoIt()
     //   {
     //       var R = rand.NextDouble();
     //       if (R < 0.2)
     //           return ERROR;
     //       else if (R < 0.5)
     //           return DONE;
     //       else
     //       {
     //           Console.WriteLine(R);
     //           return RUNNING;
     //       }
     //   }
        
     //   [Inputs(ERROR)]
     //   public int Err()
     //   {
     //       Console.WriteLine("Err()");
     //       return FINISHED;
     //   }
        
     //   [Inputs(DONE)]
     //   public int Done()
     //   {
     //       Console.WriteLine("Done()");
     //       return FINISHED;
     //   }
     //}
     


    /// <summary>
    /// A simple Finite State Machine.
    /// 
    /// Usage: Derive from this class, define public state methods with "Inputs" and/or "Outputs" attributes, 
    /// then call SingleStep repeatedly, or until it returns false.
    /// 
    /// IMPORTANT: All machines must specify a single ttarting state that has no inputs.
    /// 
    /// Ex:   
    ///         [Outputs(RUNNING)]
    ///         int Init()
    ///         {
    ///             ... do something
    ///             return RUNNING;
    ///         }
    ///         
    /// Machines may have Stop states which causes the machine to stop completely. These must be defined without
    /// Outputs. These states must return either FINISHED or DEFAULT.
    /// Ex:
    ///         [Inputs(ERROR)]
    ///         int Error()
    ///         {
    ///             .. panick ..
    ///             return DEFAULT;
    ///             return FINISH;
    ///         }
    ///         
    /// States can loop back to themselves. 
    /// 
    /// Ex:
    ///        [Inputs(TRYAGAIN,RUN), Outputs(TRYAGAIN,DONE,ERROR)]
    ///        int ReadInput() {
    ///             ...
    ///             ...
    ///             return TRYAGAIN;
    ///        }
    /// </summary>
    public class FSM : IFSM
    {
        /// <summary>
        /// State methods can return to choose DEFAULT when only a single output transition exists.
        /// Stop methods, which are methods with no output transitions may use DEFAULT instead of FINISHED.
        /// </summary>
        public const int DEFAULT = -3;

        /// <summary>
        /// State methods without Outputs are considered Stop states and must return FINISHED. Use
        /// of FINISHED in any other context is forbidden.
        /// </summary>
        public const int FINISHED = -2;

        #region Uninteresting Private State
        /// <summary>
        /// Used internally to represent that no current state has been selected, and the machine should 
        /// find it's starting state.
        /// </summary>
        private const int START = -1;

        /// <summary>
        /// Set to true only when a Stop state method has executed.
        /// </summary>
        private bool _reachedend = false;

        /// <summary>
        /// Maintain a list of states for searching.
        /// </summary>
        private List<State> _states;
        /// <summary>
        /// The current state can only be null when starting for the first time or following a Reset()
        /// </summary>
        private State cur_state;
        #endregion

        /// <summary>
        /// Creates the State list by scanning through the public instance methods that have Inputs or Outputs attributes.
        /// FSM() can throw an exception if methods aren't configured properly.
        /// </summary>
        public FSM()
        {
            _states = null;

            foreach (var x in this.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Public))
            {

                var I = x.GetCustomAttributes(typeof(Inputs), true);
                var O = x.GetCustomAttributes(typeof(Outputs), true);
            
                if((I!=null && I.Length>=1)  || (O!=null && O.Length>=1))
                {
                   
                    var func = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>),this,x);
                    AddMethod(func);
                }
            }
        }

        /// <summary>
        /// User accessible state list, for whatever reason.
        /// </summary>
        public IEnumerable<State> States
        {
            get
            {
                return _states;
            }
        }

        /// <summary>
        /// Determines if a Stop state has been reached.
        /// </summary>
        public bool Stopped
        {
            get
            {
                return _reachedend;
            }
        }

        /// <summary>
        /// Resests the machine so that the next time Run is called, the Start state will be found.
        /// </summary>
        public virtual void Reset()
        {
            _reachedend = false;
            cur_state = null;
        }

        /// <summary>
        /// Called as the state machine's heartbeat. This is entirely syhchronous.
        /// </summary>
        /// <returns>true if machine has not reached a stop state, false if a stop state method has executed.</returns>
        public virtual bool SingleStep()
        {
            if (_reachedend) return false;

            if (_states == null || _states.Count == 0) 
                throw new FSMException("Machine has no states.");

            if (cur_state == null)
            {
                PickState(START);
            }

            PickState(cur_state.Run());

            return !_reachedend;
        }

    
        /// <summary>
        /// Helper to create states from methods.
        /// </summary>
        /// <param name="method"></param>
        private void AddMethod(Func<int> method)
        {
            var ins = (Inputs[])method.Method.GetCustomAttributes(typeof(Inputs), true);
            var outs = (Outputs[])method.Method.GetCustomAttributes(typeof(Outputs), true);

            if ((ins == null || ins.Length == 0) && (outs == null || outs.Length == 0))
            {
                throw new FSMException("Method must have at least one Input or Output attribute");
            }
            else if (ins != null && ins.Length > 1)
                throw new FSMException("Transitions must be specified in a single Input attribute.");
            else if (outs != null && outs.Length > 1)
                throw new FSMException("Transitions must be specified in a single Output attribute.");

            int[] inputs = null;
            int[] outputs = null;
            if (ins != null && ins.Length == 1)
            {
                inputs = ins[0].Transitions;
                foreach (var x in inputs)
                    if (x < 0)
                        throw new FSMException("Transition integers must be greater or equal to zero. Negative numbers are reserved.");
            }

            if (outs != null && outs.Length == 1)
            {
                outputs = outs[0].Transitions;
                foreach (var x in outputs)
                    if (x < 0)
                        throw new FSMException("Transitions integers must be greater or equal to zero. Negative numbers are reserved.");
            }
            if (_states == null)
                _states = new List<State>();

            _states.Add(new State()
            {
                Ins = inputs,
                Outs = outputs,
                Run = method
            });
        }

        /// <summary>
        /// Given a transition id, choose the next appropriate state. 
        /// START is used to find the starting state
        /// DEFAULT can optionally be used to pick the next reasonable state, when no doubts exists.
        /// FINISHED is used on Stop states. These states must have no Outputs.
        /// Otherwise, the id must be a valid output transition for the current state, and the receiving state 
        /// must be the only state that can accept it.
        /// </summary>
        /// <param name="id">The output transition used to find the next state, START, FINISHED or DEFAULT.</param>
        private void PickState(int id = START)
        {
            if (id == START)
            {
                var match = from x in _states where x.Ins == null select x;
                if (match == null || match.Count() != 1)
                    throw new FSMException("Need unique starting state.");
                cur_state = match.First();
            }
            else if (cur_state == null)
            {
                throw new FSMException("Must start at Start state.");
            }
            else
            {
                if (id == FINISHED && (cur_state.Outs == null || cur_state.Outs.Count() == 0))
                {
                    _reachedend = true;
                }
                else if (id == DEFAULT)
                {
                    if (cur_state.Outs == null || cur_state.Outs.Count() == 0)
                    {
                        _reachedend = true;
                    }
                    else if (cur_state.Outs.Length != 1)
                        throw new FSMException("Can't use default when more than one outputs states allowed.");
                    else
                    {
                        id = cur_state.Outs[0];
                        var match = from x in _states where x.Ins != null && x.Ins.Contains(id) select x;
                        if (match == null || match.Count() != 1)
                            throw new FSMException("Input mapped in multiple states.");
                        cur_state = match.First();
                    }
                }
                else if (cur_state.Outs!=null && cur_state.Outs.Contains(id))
                {
                    var match = from x in _states where x.Ins != null && x.Ins.Contains(id) select x;
                    if (match == null || match.Count() != 1)
                        throw new FSMException("Input mapped in multiple states.");
                    cur_state = match.First();
                }
                else throw new FSMException("Unsupported output state: " + id);
            }
        }
    }
}