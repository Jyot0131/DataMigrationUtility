using DataMigrationUtility.Data;
using DataMigrationUtility.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataMigrationUtility.UI
{
    public class Operation
    {
        private DatabaseContext _context = new DatabaseContext();
        //private static CancellationTokenSource tokenSource = new CancellationTokenSource();
        //private static CancellationToken token = tokenSource.Token;
        private bool _isCompleted = false;


        public void ConsoleInputs(int batchId, CancellationTokenSource tokenSource)
        {
            while (!_isCompleted)
            {
                Console.WriteLine();
                Console.WriteLine("Type \"CANCEL\" to cancle task");
                Console.WriteLine("Type \"STATUS\" to show status");
                Console.WriteLine();
                var inp = Console.ReadLine();
                switch (inp)
                {
                    case "CANCEL":
                        _isCompleted = true;
                        tokenSource.Cancel();
                        var tempbatch = _context.BatchStats.Find(batchId);
                        if (tempbatch != null) tempbatch.Status = "Cancelled";
                        _context.SaveChanges();
                        continue;
                    case "STATUS":
                        ShowStatus();
                        continue;
                    default:
                        if(!_isCompleted)
                            Console.WriteLine("Invalid Input ! Try again please...");
                        continue;
                }
            }
        }


        public void Migration(int lower, int higher)
        {
            int noOfTasksCreated = 0;

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            var batch = new BatchStats { Max = higher, Min = lower, Status = "Ongoing" };
            _context.BatchStats.Add(batch);
            _context.SaveChanges();

            int batchId = batch.Id;

            List<Task> parallelTasks = new List<Task>();

            Task t1 = Task.Run(() => ConsoleInputs(batchId, tokenSource));

            int low=lower-100, high=lower;

            for (var i = 0; i <= (higher-lower)/100; i++)
            {
                if(noOfTasksCreated >= 25)
                {
                    Task.WaitAll(parallelTasks.ToArray());
                    noOfTasksCreated = 0;
                    parallelTasks.Clear();
                }
                low = 100 + low;
                high = higher < low+100 ? higher : low+100;

                if (low >= high)
                    break;
                
                parallelTasks.Add(CreateTask(low, high, batchId, new DatabaseContext(), token));
                noOfTasksCreated += 1;
                //Console.WriteLine($"\r{parallelTasks.Count} tasks has been created !");
                
            }
            //Console.WriteLine("All tasks has been created !!!");
            Task t2 = Task.WhenAll(parallelTasks.ToArray());
            Task.WaitAny(t1,t2);

            _isCompleted = true;

        }

        public async Task CreateTask(int lowtemp, int hightemp, int batchId, DatabaseContext context, CancellationToken token)
        {
            Task t = Task.Run( () =>
            {

                var srcdata = context.SourceTable.Skip(lowtemp).Take(hightemp - lowtemp).ToList();

                foreach (var src in srcdata)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Cacellation is requested !");
                        //context.SaveChanges(); //Uncomment this to save half done work after cancelling the task.

                        return;
                    }
                    context.DestinationTable.Add(new Destination { SourceId = src.Id, Sum = Sum(src.FirstNumber, src.SecondNumber) });
                }
                try
                {
                    var retrivedBatch = context.BatchStats.Find(batchId);
                    retrivedBatch.Status = "Completed";
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine(ex.InnerException.Message);
                    var tempbatch = _context.BatchStats.Find(batchId);
                    tempbatch.Status = "Error";
                    _context.SaveChanges();

                }

            }, token);

            await t;
        }


        public static void ShowStatus()
        {
            Console.WriteLine("Id\t\tBatch\t\tStatus");

            using var context = new DatabaseContext();

            foreach (var batch in context.BatchStats.ToList())
            {
                Console.WriteLine($"{batch.Id}\t\t{batch.Min}-{batch.Max}\t\t{batch.Status}");
            }
        }


        public static int Sum(int fnum, int snum)
        {
            // To mimic Http api call
            Task.Delay(50).Wait();
            return fnum + snum;
        }
    }
}
