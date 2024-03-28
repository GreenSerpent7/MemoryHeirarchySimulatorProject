using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;


class RWData
{


    public struct Data
    {
        public Data(char permission, string hex)
        {
            this._perm = permission;
            this._hex = hex;
        }

        public char _perm { get; init; }
        public string _hex { get; init; }
        public int physicalPageNumber { get; set; }
        public override string ToString() => $"({_perm},{_hex})";
    }




    static void Main()
    {
        string filepath = @"C:\Users\rayce\Downloads\real_tr.dat";

        int arraysize = File.ReadLines(filepath).Count();

        Data[] virtualMemory = new Data[arraysize];
        int i = 0;

        using StreamReader reader = new StreamReader(filepath);
        {

            string line;
            while ((line = reader.ReadLine()) != null)
            {

                Data data = new Data(line[0], (line.Substring(2)));  
                virtualMemory[i] = data;
                i++;
            }
        }

        printTraceConfig();
        printTraceData(virtualMemory);
        leastReventUse(virtualMemory);
        greedy(virtualMemory);
        firstInFirstOut(virtualMemory);
    }

    private static void printTraceData(Data[]virttualMemory)
    {
        Console.WriteLine("==============\nVirtual Memory\n==============\n");
        foreach (Data data in virttualMemory)
        {
            Console.WriteLine(data.ToString());
        }

    }

    private static void printTraceConfig()
    {
       

        Console.WriteLine("trace.config");
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------");
        Console.WriteLine("Page Table Configuration: ");
        Console.WriteLine("Number of virtual pages: 100");
        Console.WriteLine("Number of physical pages: 108296");
        Console.WriteLine("Page size: 256");
        Console.WriteLine(" \n");
    }
    private static void leastReventUse(Data[]virtualMemory)
    {
        //todo
        //we can print all output within the function, or change void to string and print as its called.
    }

    private static void greedy(Data[] virtualMemory)
    {
        //todo
        //we can print all output within the function, or change void to string and print as its called.
    }

   static void firstInFirstOut(Data[] virtualMemory)
    {
        Console.WriteLine("FIFO Algorithm:\n");
        Console.WriteLine("Virtual Address\tVirtual Pg #\tPage Off\tTable Result\tPhysical Page #");
        Console.WriteLine("---------------|-------------|----------|----------------------|-------------------|");
        int cacheSize = 20;
        Data[] cache = new Data[cacheSize];
        int physicalPageNumber = -1;
        int totalHit = 0;
        int totalMiss = 0;

        for (int index = 0; index < virtualMemory.Length; index++)
        {
            Data currentData = virtualMemory[index];
            string stringData = currentData.ToString();
            var last3 = Regex.Match(stringData, @"(.{4})\s*$");
            string virtualAddressP = $"00000{last3}";
            string virtualAddress = virtualAddressP.Replace(")", "");
            char virtualPage = stringData[^4];
            var pageOffsetPV = Regex.Match(stringData, @"(.{3})\s*$");
            string pageOffsetP = pageOffsetPV.ToString();
            string pageOffset = pageOffsetP.Replace(")", "");
            bool inCache = false;
            bool tableResult = false;

            for (int i = 0; i < cache.Length; i++)
            {
                if (cache[i]._hex == currentData._hex)
                {
                    inCache = true;
                    tableResult = true;
                    physicalPageNumber = i;
                    break;
                }
            }

            if (inCache)
            {
                totalHit++;
            }
            else
            {
                totalMiss++;

                for (int i = 0; i < cache.Length - 1; i++)
                {
                    cache[i] = cache[i + 1];
                }
                cache[cache.Length - 1] = currentData;
                cache[cache.Length - 1].physicalPageNumber = physicalPageNumber;
                physicalPageNumber = (physicalPageNumber + 1) % cache.Length;
            }

            string tableResultString = tableResult ? "hit" : "miss";
            Console.WriteLine($"{virtualAddress}\t\t{virtualPage}\t    {pageOffset}\t\t\t{tableResultString}\t\t\t{physicalPageNumber}");
        }

        Console.WriteLine("Simulation Statistics");
        Console.WriteLine($"Total Hit: {totalHit}");
        Console.WriteLine($"Total Miss: {totalMiss}");
        double hitRatio = totalMiss != 0 ? (double)totalHit / totalMiss : 0;
        Console.WriteLine($"Hit Ratio: {hitRatio}");
    }
}
