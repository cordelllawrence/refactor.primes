using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rerfactor.squreroot
{
    class Program
    {
        static void Main(string[] args)
        {
            var limit = 16000000;

            // Start Synchronous test
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Console.WriteLine($"The number of primes under {limit} is {SyncCountPrimesBelow1(limit)}");

            watch.Stop();
            Console.WriteLine($"--- Sync Time Elapsed: {watch.Elapsed}");

            watch.Restart();
            Console.WriteLine($"The number of primes under {limit} is {ParallelCountPrimesBelow1(limit)}");

            // Start Parallel test
            watch.Stop();
            Console.WriteLine($"--- Parallel Time Elapsed: {watch.Elapsed}");

            watch.Restart();
            Console.WriteLine($"LINQ: The number of primes under {limit} is {SyncCountPrimesBelow2(limit)}");

            // Start Sybchronous LINQ test
            watch.Stop();
            Console.WriteLine($"--- LINQ Sync Time Elapsed: {watch.Elapsed}");

            watch.Restart();
            Console.WriteLine($"LINQ: The number of primes under {limit} is {ParallelCountPrimesBelow2(limit)}");

            // Start Parallel LINQ test
            watch.Stop();
            Console.WriteLine($"--- LINQ Parallel Time Elapsed: {watch.Elapsed}");

            Console.WriteLine();
            Console.WriteLine("press any key to terminate ...");
            Console.ReadKey();
        }

        // Creates an enumerable [lazy] sequence
        static IEnumerable<int> Sequence(int fromInclusive, int toExclusive, int increment = 1)
        {
            for (int i = fromInclusive; i < toExclusive; i += increment)
                yield return i;
        }

        // Synchronously calulates the number of primes under the supplied limit
        static int SyncCountPrimesBelow1(int n)
        {
            int primeCount = 1;

            //for (int j = 3; j < n; j += 2)
            foreach(var j in Sequence(3, n, 2))
            {
                var isPrime = true;
                for (int i = 3; i <= Math.Sqrt(j) && isPrime; i += 2)
                    if (j % i == 0) isPrime = false;

                // Increment count if the number is prime
                if (isPrime) primeCount++;
            }

            return primeCount;
        }

        // Calulates the number of primes under the supplied limit in Parallel
        static int ParallelCountPrimesBelow1(int n)
        {
            var lockObject = new Object();
            var primeCount = 1;

            Parallel.ForEach<int>(Sequence(3, n, 2), (j) => {
                var isPrime = true;
                for (int i = 3; i <= Math.Sqrt(j) && isPrime; i += 2)
                    if (j % i == 0) isPrime = false;

                // Increment count if the number is prime
                if (isPrime)
                    lock (lockObject) { primeCount++; }

            });

            return primeCount;
        }

        // Using LINQ, synchronously calulates the number of primes under the supplied limit
        static int SyncCountPrimesBelow2(int n)
        {
            Func<int, bool> isPrime = (p) => { for (var i = 3; i <= Math.Sqrt(p); i += 2) if (p % i == 0) return false; return true; };

            // Filter the space by the isPrime function and simply count items
            return Sequence(3, n, 2).Where(i => isPrime(i)).Count();
        }

        // Calulates the number of primes under the supplied limit in Parallel using LINQ
        static int ParallelCountPrimesBelow2(int n)
        {
            Func<int, bool> isPrime = (p) => { for (var i = 3; i <= Math.Sqrt(p); i += 2) if (p % i == 0) return false; return true; };

            // Filter the space (in parallel) by the isPrime function and simply count items
            return Sequence(3, n, 2).AsParallel().Where(i => isPrime(i)).Count();
        }
    }
}
