using System;
using System.Linq;


namespace WeatherLab
{
    class Program
    {
        static string dbfile = @".\data\climate.db";

        static void Main(string[] args)
        {
            var measurements = new WeatherSqliteContext(dbfile).Weather;

            var total_2020_precipitation = measurements.Where(a => a.year == 2020) .Select(a => a.precipitation).Sum();
            Console.WriteLine($"Total precipitation in 2020: {total_2020_precipitation} mm\n");

            //
            // Heating Degree days have a mean temp of < 18C
            //   see: https://en.wikipedia.org/wiki/Heating_degree_day
            // Cooling degree days have a mean temp of >=18C
            //
            var countdays = measurements.GroupBy(y => y.year)
                               .Select(s => new
                               {
                                   year = s.Key,
                                   hdd = s.Where(a => a.meantemp < 18).Count(),
                                   cdd = s.Where(a => a.meantemp >= 18).Count()
                               });
            //
            // Most Variable days are the days with the biggest temperature
            // range. That is, the largest difference between the maximum and
            // minimum temperature
            //
            // Oh: and number formatting to zero pad.
            // 
            // For example, if you want:
            //      var x = 2;
            // To display as "0002" then:
            //      $"{x:d4}"
            //
            Console.WriteLine("Year\tHDD\tCDD");
          
            foreach (var i in countdays)
            { Console.WriteLine($"{ i.year }\t{ i.hdd }\t{ i.cdd }"); }


            Console.WriteLine("\nTop 5 Most Variable Days");
            Console.WriteLine("YYYY-MM-DD\tDelta");

            var data = measurements
                    .Select(s => new
                    {
                        Date = $"{s.year}-{s.month:d2}-{s.day:d2}",
                        Delta = s.maxtemp - s.mintemp
                    })
                    .OrderByDescending(d => d.Delta);
            int j = 0;
            foreach (var i in data)
            {   if ( j < 5)
                {
                    j++;
                    Console.WriteLine($"{i.Date}\t{Math.Round((Double)i.Delta, 2)}");
                }
            }
        }
    }
}
