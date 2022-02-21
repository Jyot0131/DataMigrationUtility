using DataMigrationUtility.Data;
using DataMigrationUtility.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataMigrationUtility.UI
{
    public class Program
    {
        private static ConcurrentDictionary<string,string> batchstats = new();
        static void Main(string[] args)
        {
            var _context = new DatabaseContext();
            _context.Database.EnsureCreated();
            while (true)
            {
                try
                {
                    Method1();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }


        public static void Method1()
        {
            int lower, higher;
            Console.WriteLine("Give lower of a batch : ");
            bool success1 = Int32.TryParse(Console.ReadLine(), out lower);
            Console.WriteLine("Give higher of a batch : ");
            bool success2 = Int32.TryParse(Console.ReadLine(), out higher);

            if (!(success1 && success2))
                throw new Exception("Please enter number only");

            if (lower < 0 || higher < 0 || (higher - lower) < 1 || lower > 1000000 || higher > 1000000)
                throw new Exception("Range is not valid");


            var operation = new Operation();

            try
            {
                operation.Migration(lower, higher);
            }
            catch(TaskCanceledException)
            {
                Console.WriteLine("A Task was cancelled");
            }
            

            Console.WriteLine("Task has been completed, press Enter to continue or press ctrl+c to abort.");
            Console.ReadKey();
        }

    }

}


//truncate table BatchStats;
//truncate table DestinationTable;