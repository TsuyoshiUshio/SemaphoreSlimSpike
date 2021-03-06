﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SemaphoreSlimSpike
{
    class Program
    {
        private static SemaphoreSlim semaphore;
        private static int count;
        static void Main(string[] args)
        {
            semaphore = new SemaphoreSlim(0, 3);
            Console.WriteLine($"{semaphore.CurrentCount} taks can enter the semaphore.");

            Task[] tasks = new Task[5];

            foreach(var i in Enumerable.Range(0, 5))
            {

                //// Async
                tasks[i] = Task.Run(async () =>
                {
                    Console.WriteLine($"Task {Task.CurrentId}: {i} begins and wait for the semaphore. {DateTime.Now.ToString()}");
                    await semaphore.WaitAsync();
                    Interlocked.Add(ref count, 1);
                    Console.WriteLine($"Task {Task.CurrentId}: {i} enters the semaphore. {DateTime.Now.ToString()}");
                    await Task.Delay(TimeSpan.FromSeconds(3 + count)); // It will change the end time for the concurrently started thread.
                    Console.WriteLine($"Task {Task.CurrentId}: {i} releases the semaphore; previous count {semaphore.Release()}");
                    Console.WriteLine($"Task {Task.CurrentId}: {i} ends the semaphore. {DateTime.Now.ToString()}");

                });

                // Sync
                //tasks[i] = Task.Run(() =>
                //{
                //    Console.WriteLine($"Task {Task.CurrentId}: {i} begins and wait for the semaphore. {DateTime.Now.ToString()}");
                //    semaphore.Wait();
                //    Interlocked.Add(ref count, 1);
                //    Console.WriteLine($"Task {Task.CurrentId}: {i} enters the semaphore. {DateTime.Now.ToString()}");
                //    Thread.Sleep((3 + count) * 1000); // It will change the end time for the concurrently started thread.
                //    Console.WriteLine($"Task {Task.CurrentId}: {i} releases the semaphore; previous count {semaphore.Release()}");
                //    Console.WriteLine($"Task {Task.CurrentId}: {i} ends the semaphore. {DateTime.Now.ToString()}");

                //});
            }

            // Wait for half a second to allow all the tasks to start and block .
            Thread.Sleep(500);

            Console.WriteLine("Main thread call Release(3)");
            // increase avairable Semaphore
            semaphore.Release(3);
            Console.WriteLine($"{semaphore.CurrentCount} tasks can enter the semaphore.");
            Task.WaitAll(tasks);
            Console.WriteLine("Main thread exits.");
            Console.ReadLine();

        }
    }
}
