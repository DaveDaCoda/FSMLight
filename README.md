# FSMLight
Unobtrusive Finite State Machine 

<b>Why?</b>

Inevitably at some point during a project that handles many states (like a game), we consider using a formal Finite State Automata framework. We search high and low, and all the conventions, extra setup code and retrofitting efforts seem to outweigh the intended benefit. We may find ourselves sticking with our spagethie bowl and dealing with bugs. <b>Not anymore.</b>

<b>So how unobtrusive is it?</b>



## Example
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
