using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
public class Program
{
    static async Task Main(string[] args)
    {
        var asd = new ActionBlock<object>(async (a) => { await Task.Delay(10000); },
                    new ExecutionDataflowBlockOptions
                    {
                        EnsureOrdered = true,
                        MaxDegreeOfParallelism = 10000,
                        SingleProducerConstrained = true,
                        TaskScheduler = TaskScheduler.Default,
                        BoundedCapacity = 10000
                    });
        int count = 0;
        int numberOfScheduledTaskBeforeBreak = 1000;
        while (++count > 0)
        {
            var g = asd.SendAsync(new object());
            if (g.IsCompleted && g.Result == true)
            {
                Console.WriteLine($"Completed: {count}");
            }
            else if (g.IsCompleted && g.Result == false)
            {
                Console.WriteLine($"Refused: {count}");
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"SendAsync task scheduled for execution: {count}");
                numberOfScheduledTaskBeforeBreak--;
            }
            if (numberOfScheduledTaskBeforeBreak == 0)
            {
                break;
            }
        }
        Console.WriteLine("Press enter to continue...");
        Console.ReadLine();

        var asd2 = new ActionBlock<object>(async (a) => { await Task.Delay(10000); },
                  new ExecutionDataflowBlockOptions
                  {
                      EnsureOrdered = true,
                      MaxDegreeOfParallelism = 10000,
                      SingleProducerConstrained = true,
                      TaskScheduler = TaskScheduler.Default,
                      BoundedCapacity = 10000
                  });

        count = 0;
        int numberOfRefusedItemsBeforeBreak = 10000;
        while (++count > 0)
        {
            var g = asd2.Post(new object());
            if (g)
            {
                Console.WriteLine($"Accepted: {count}");
            }
            else
            {
                Console.WriteLine($"Refused: {count}");
                numberOfRefusedItemsBeforeBreak--;
            }
            if (numberOfRefusedItemsBeforeBreak == 0)
            {
                break;
            }
        }
    }
}