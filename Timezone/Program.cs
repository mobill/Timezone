using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

/// <summary>
/// This program dumps timezone info to text files.
/// It uses an opensource library to dump Olson Timezone Data to set of tab separated files.
/// It also dumps the Windows Timezone data to a tab sep file.
/// Perhaps the code could be prettier but it served my purpose. May it help you as well.
/// </summary>
namespace Timezone {

    class Program {
        static void Main(string[] args) {

            //  Uncomment the next line to look at SystemTimeZones in the debugger.
            //  IReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();

            GetTimeZones();

            var olson = new OlsonData();
            olson.GetData();

            // Give you a chance to see any console output
            Console.WriteLine("{0}Press Ctrl^C to close the window. {0}", Environment.NewLine);

            Thread.Sleep(10000 * 100000);
        }

        /// <summary>
        /// Retrieve and write out Windows Timezone info to a file.
        /// </summary>
        static void GetTimeZones() {

            StreamWriter wsr = File.CreateText(@"c:\Code\Timezone\WindowsData\WindowsTZ.txt");

            // Write the header info, tab separated.
            wsr.WriteLine("ID\tDaylightName\tStandardName\tRawUtcOffset\tDisplayName\tSupportsDST");

            foreach (TimeZoneInfo z in TimeZoneInfo.GetSystemTimeZones()) {
                Console.WriteLine(z.Id);
                wsr.Write(z.Id.Trim() + "\t");
                wsr.Write(z.DaylightName.Trim() + "\t");
                wsr.Write(z.StandardName.Trim() + "\t");
                wsr.Write(z.BaseUtcOffset.ToString() + "\t");
                wsr.Write(z.DisplayName.Trim() + "\t");
                wsr.WriteLine(z.SupportsDaylightSavingTime.ToString() + "\t");
                
                //  I wanted to see the transition data in the debugger to determine usefulness.
                //  Windows adjustment rules seem only to include
                //  the time of day that transitions occur and not the date.
                if (z.SupportsDaylightSavingTime) {
                    var rules = z.GetAdjustmentRules();
                }
            }
            
            //  clean up
            wsr.WriteLine();
            wsr.Close();
        }
    }
}
