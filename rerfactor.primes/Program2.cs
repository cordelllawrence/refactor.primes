using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rerfactor.primes
{
    class Program2
    {
        static void Main(string[] args)
        {
            var limit = 16000000;

            Console.WriteLine($"Time to Execute: {TimeIt(() => Console.WriteLine($"Sync: # of primes under {limit} is {SyncCountPrimesBelow(limit)}"))}");
            Console.WriteLine($"Time to Execute: {TimeIt(() => Console.WriteLine($"Parallel: # of primes under {limit} is {ParallelCountPrimesBelow(limit)}"))}");
            Console.WriteLine($"Time to Execute: {TimeIt(() => Console.WriteLine($"LINQ Sync: # of primes under {limit} is {LinqSyncCountPrimesBelow(limit)}"))}");
            Console.WriteLine($"Time to Execute: {TimeIt(() => Console.WriteLine($"LINQ Parallel: # of primes under {limit} is {LinqParallelCountPrimesBelow(limit)}"))}");

            Console.WriteLine("< press any key to terminate >");
            Console.ReadKey();
        }

        static TimeSpan TimeIt(Action method)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            method();

            return watch.Elapsed;
        }

        // Creates an enumerable [lazy] sequence
        static IEnumerable<int> Sequence(int fromInclusive, int toExclusive, int increment = 1)
        {
            for (int i = fromInclusive; i < toExclusive; i += increment)
                yield return i;
        }

        // Determine if a number is prime or not
        static bool isPrime(int n)
        {
            if (n == 2) return true;

            if (n % 2 == 0) return false;

            for (int i = 3; i <= Math.Sqrt(n); i += 2)
                if (n % i == 0) return false;

            return true;
        }

        // Synchronously calulates the number of primes under the supplied limit
        static int SyncCountPrimesBelow(int n)
        {
            int primeCount = 1;

            foreach(var j in Sequence(3, n, 2))
                if (isPrime(j)) primeCount++;

            return primeCount;
        }

        // Calulates the number of primes under the supplied limit in Parallel
        static int ParallelCountPrimesBelow(int n)
        {
            var lockObject = new Object();
            var primeCount = 1;

            Parallel.ForEach<int>(Sequence(3, n, 2), (j) => { if (isPrime(j)) lock (lockObject) { primeCount++; } });

            return primeCount;
        }

        // Using LINQ, synchronously calulates the number of primes under the supplied limit
        static int LinqSyncCountPrimesBelow(int n)
        {
            return Sequence(3, n, 2).Where(i => isPrime(i)).Count() + 1;
        }

        // Calulates the number of primes under the supplied limit in Parallel using LINQ
        static int LinqParallelCountPrimesBelow(int n)
        {
            return Sequence(3, n, 2).AsParallel().Where(i => isPrime(i)).Count() + 1;
        }
    }
}
