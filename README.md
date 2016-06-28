# FSMLight
Unobtrusive Finite State Machine 

##Motivation

Inevitably at some point during a project that handles many states (like a game), we consider using a formal Finite State Automata framework. We search high and low, and all the conventions, extra setup code and retrofitting efforts seem to outweigh the intended benefit. We may find ourselves sticking with our spagethie bowl and dealing with bugs. <b>Not anymore.</b>

FSMLight was created to address the need to have robust Finite State Automata in games, all the while making it enjoyable to use from the prototype stage.  FSMLight does not get in the way or slow you down. Anybody can use it right away and reap some benefit.

##Background Information

There are many resources around the web on Finite State Automata.  It boils down to programs winding up being in 'States' and moving from state to state via 'transitions' or 'events'.  A state can, in a very typical situation be a method that attempts to read a key from the keyboard. Once a particular key is captured, the program handles the key. Handling the key, so to speak, is another 'state'. "Key Received" is the transiton. Anyways, the web is filled with great examples and explanations. Go have at it.

The key is to make sure that the program behaves in a predetermined way.  

##Why is FSMLight better than other approaches?

It isn't. What it 'is' however is the simplest possible approach I can come up with that enables all of us to enjoy pure Finite State Automata in our projects in the most unobtrusive way possible. This makes it easy for game devs to use. 

##Can't a switch statement do it all?

Of course it can. So can nested switch statements, or nested switches loaded with if-else conditions and method calls. Actually that's a key motivation to use FSA. Use of an unobtrusive Finite State Automata system eliminates much of the cludge and most of the time, makes the mess easier to maintain.  


##So how unobtrusive is it?

Very. See Usage.

##Limitations

Thread safe? Not really.  Enterprise-y? Nope. It's not made for that.  It's made to reduce logic mistakes and make complex state driven code easier to maintain, and explain.

##Warning
This is intended for games, and client software. The whole point of "Finite State Machines" is to ensure that the behavior is always predictable. This is NOT intended for multi-threaded use. It's perfect for games.

##Usage

1. Simply derive a new class based on FSM and define a few transition / events as integers:
        
        public class MyMachine : FSM {
     
                public const int INITED = 0;
                public const int RUNNING = 1;
                public const int ERROR = 2;
                ...
                // add your stuff here, like cameras, vectors, reference to the main Game class etc.
                // this is the stuff you will updating with your FSM.
                ...

                public MyMachine() {
                
                }
        }

2. Define some states (methods) like this:

        // define at least a start state
        // A Start state has no Inputs, just Outputs.
        
        [Outputs(INITED)]
        public int Startup() {
                // make sure the method is defined like '<b>public int method()</b>'
                // do some stuff
                return INITED;  // the return value MUST correspond to one of the Outputs.
        }
        
        // here is a state that can be called repeatedly
        
        [Inputs(INITED,RUNNING),Outputs(RUNNING,ERROR)]
        public int DoStuff()
        {
           // do game stuff
           
           return RUNNING; // notice the Inputs and Outputs contain running? This means that DoStuff() can 'loop'.
           
           // return ERROR;
        }
        
        // here is an optional STOP state, which has no outputs.
        [Inputs(ERROR)]
        public int Oops()
        {
                // log error
                return FINISHED;   // FINISHED is a special constant that can only be returned by a STOP state.
        }

##Rules Summary

1. Derive from FSM
2. States are defined as 'public int MyFunc()' and must have at least one [Outputs(...)] [Inputs(...)] or both.
3. Start states only have Outputs. Only a single Start state MUST exist per finite state machine.
4. Stop states have no Outputs and must return FINISHED
5. States can use the same transition integers in both their Inputs and Outputs to show that a state can be called repeatedly. (Like checking for user input from the keyboard.)

##Running the Machine

Just instantiate the machine and step through it, like this:

        MyMachine machine = new MyMachine()
        while(machine.SingleStep())
        {
        
        }

That's it!

In the case that a machine has a Stop state (like the end of a complex game sequene), then SingleStep() will return false. It is perfectly OK to ommit Stop states. Some machines just run forever, like user input processing.



##Full  Example
Here's a complete example that shows how easy it is to get started:


        using FSMLight;
        
        namespace FSMDemo
        {
           
            class Program
            {
              
                static void Main(string[] args)
                {
                    try
                    {
                        var machine = new MyMachine();
        
                        while (machine.SingleStep())
                        {
                            Console.WriteLine("tick");
                        }
        
                    }
                    catch(FSMException e)
                    {
                        Console.WriteLine("FSM Issue: " + e.Message);
                    }
                    Console.ReadLine();
                }
        
        
                public class MyMachine : FSM
                {
                   
                    const int INITED = 1;
                    const int RUNNING = 2;
                    const int ERROR = 3;
                    const int DONE = 4;
        
                    Random rand;
                    
                    public MyMachine()
                    {
                       
                    }
                    
                    [Outputs(RUNNING)]
                    public int Init()
                    {
                        if(rand==null)
                            rand = new Random();
                        return RUNNING;
                    }
        
                    [Inputs(RUNNING), Outputs(RUNNING,ERROR,DONE)]
                    public int DoIt()
                    {
                        if (!Console.KeyAvailable)
                            return RUNNING;
                        else
                        {
                            var key = Console.ReadKey(true);
                            if(key.Key == ConsoleKey.Escape)
                            {
                                
                                return DONE;
                            }
        
                            else if(key.Key == ConsoleKey.E)
                                return ERROR;
                            else
                            {
                                Console.WriteLine("Got: " + key.Key.ToString());
                                return RUNNING;
                            }
        
        
                        }
                  
                    }
        
                    [Inputs(ERROR)]
                    public int Err()
                    {
                        Console.WriteLine("Err()");
                        return DEFAULT;
                    }
        
                    [Inputs(DONE)]
                    public int Done()
                    {
                        Console.WriteLine("Done()");
                        return FINISHED;
                    }
                }
            }
          
        }
