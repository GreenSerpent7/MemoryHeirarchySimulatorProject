﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


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
        public int physicalPageNumber = -1;
        public override string ToString() => $"({_perm},{_hex})";
        public static bool operator ==(Data obj1, Data obj2)
        {
            if (ReferenceEquals(obj1, obj2)) return true;
            if (ReferenceEquals(obj2, null)) return false;
            if (ReferenceEquals(obj1, null)) return false;
            return obj1.Equals(obj2);
        }
        public static bool operator !=(Data obj1, Data obj2) => !(obj1 == obj2);
    }




    static void Main()
    {
        string filepath = @"C:\Users\willi\Downloads\real_tr.dat";

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
        leastRecentUse(virtualMemory);
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
     static void leastRecentUse(Data[] virtualMemory)
    {
        Console.WriteLine("LRU Algorithm:\n");
        Console.WriteLine("Virtual Address\tVirtual Pg #\tPage Offset\tTable Result\tPhysical Page #");
        Console.WriteLine("---------------|-------------|-------------|-------------|-----------------|");

        int cacheSize = 20;
        Data[] cache = new Data[cacheSize];
        int[] accessTime = new int[cacheSize];
        int totalHit = 0;
        int totalMiss = 0;


        int[] physicalPages = new int[cacheSize];
        for (int i = 0; i < cacheSize; i++)
        {
            physicalPages[i] = i;
        }

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
            int cacheIndex = -1;

            // Check if current data is in cache
            for (int i = 0; i < cache.Length; i++)
            {
                if (cache[i]._hex == currentData._hex)
                {
                    inCache = true;
                    cacheIndex = i;
                    break;
                }
            }

            if (inCache)
            {
                // Update access time of the page
                accessTime[cacheIndex] = index;
                totalHit++;
            }
            else
            {
                totalMiss++;

                // Find the least recently used page in the cache
                int leastUsedIndex = Array.IndexOf(accessTime, accessTime.Min());
                cache[leastUsedIndex] = currentData;
                accessTime[leastUsedIndex] = index;


                int physicalPageNumber = physicalPages[leastUsedIndex];
                Console.WriteLine($"{virtualAddress}\t\t{virtualPage}\t{pageOffset}\t\tmiss\t{physicalPageNumber}");


                physicalPages[leastUsedIndex] = (index % cacheSize);
            }

            if (inCache)
            {
                Console.WriteLine($"{virtualAddress}\t\t{virtualPage}\t{pageOffset}\t\thit\t{physicalPages[cacheIndex]}");
            }
        }

        Console.WriteLine("Simulation Statistics");
        Console.WriteLine($"Total Hit: {totalHit}");
        Console.WriteLine($"Total Miss: {totalMiss}");
        double hitRatio = totalMiss != 0 ? (double)totalHit / totalMiss : 0;
        Console.WriteLine($"Hit Ratio: {hitRatio}");
    }

    private static void greedy(Data[] virtualMemory)
{
    // delcare a variable that you will use to increment 
    int i = 0; // I use this for the while loop
    int miss = 0; // I use this to calculate misses
    int counter = 0; // I use this to count to n - 1 
   
    int cache = 4; 
    
    //initialize physical memory array size 
    Data[] physicalMemory = new Data[4];
    for(int k = 0; k < 5; k++)
    {
        physicalMemory[k] = new Data(); 
    }
    //while array is not full, add data and calculate compensatory misses
    while(physicalMemory.Length < 4)
    {
        //copies the virtualMemory to phsyicalMemory at index j
        virtualMemory.CopyTo(physicalMemory, i);
        counter++;
        i++;
        miss++;
    }
    if(physicalMemory.Length == 4)
    {
        //determine which value to remove by incrementing through the virtual memory array
        //set j = 4 in order to start a index [4] because we have already added the indexes [0] to [3] from virtualMemory
        //use for loop
        for (int j = 4; j < virtualMemory.Length; j++)
        {
            //Is index [0] the same as virtualmemory?
            if (physicalMemory[0] == virtualMemory[j])
            {
                //If yes, then increment counter as a relative hit
                if( counter < cache)
                {
                    counter++;
                   
                }
                //If counter = 4 then we need to record miss and update data in physical memory
                else if (counter >= 4)
                {
                    // remove item from cache
                    //a add next item from virtual memory to cache 
                    physicalMemory[0] = virtualMemory[j];   

                    // set counter to 0 again
                    counter = 0; 
                    break; 
                }
            }
            //Is index [2] the same as virtualMemory? 
            if (physicalMemory[1] == virtualMemory[j])
            {
                if( counter < cache)
                {
                    counter++;
                   
                }
                else if( counter >= 4)
                {
                    // remove item from cache
                    // add next item from virtual memory to cache
                    physicalMemory[1] = virtualMemory[j];   

                    // set counter to 0 again

                    counter = 0; 
                    break; 
                }
            }
            if (physicalMemory[2] == virtualMemory[j])
            {
                if (counter < cache)
                {
                    counter++;
   
                }
                else if (counter >= 4)
                {
                    // remove item from cache
                    // add next item from virtual memory to cache }
                    physicalMemory[2] = virtualMemory[j];

                    // set counter to 0 again
                    counter = 0; 
                    break;
                }
            }
            if (physicalMemory[3] == virtualMemory[j])
            { 
                if(counter < cache)
                {
                    counter++;
                  
                }
                else if(counter >= 4)
                {
                     // remove item from cache
                     //a add next item from virtual memory to cache
                     physicalMemory[3] = virtualMemory[j];


                     // set counter to 0 again
                    counter = 0; 
                    break; 
                }
            } 
            if (physicalMemory[0] != virtualMemory[j] | physicalMemory[1] != virtualMemory[j] | physicalMemory[2] != virtualMemory[j]
                | physicalMemory[3] != virtualMemory[j])
            {
                //if yes, then we need to record a miss, since there is no match in cache
                if (counter < cache)
                {
                    //record miss
                    miss++; 
                    break;
                }
                else if(counter == 4)
                {
                    //record miss
                    miss++;
                    // remove item from cache
                    //a add next item from virtual memory to cache
                    physicalMemory[counter] = virtualMemory[j];
                    counter = 0;
                    break; 
                }                                   
            }
        }
    }

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
            var virt = Regex.Match(stringData, @"(.{9})\s*$");
            string virtualAddressP = virt.ToString();
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
