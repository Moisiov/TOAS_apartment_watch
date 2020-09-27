using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TOASApartmentWatch
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("TOAS apartment watch starting");
            ApartmentWatch apartmentWatch = new ApartmentWatch();
            await apartmentWatch.run();
        }
    }
}
