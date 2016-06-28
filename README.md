# FSMLight
Unobtrusive Finite State Machine 


##Code Example

Here's a complete example that shows how easy it is to get started:
(Please see the Wiki for more usage and project information.)

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
