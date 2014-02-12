using System;
using System.Globalization;
using System.IO;
using System.Linq;
using TZ4Net;    //  library from http://www.babiej.demon.nl/Tz4Net/main.htm
                 //  they request a small donation. 

namespace Timezone {
    class OlsonData {


        public void GetData() {

            // Original code from: http://blogs.msdn.com/b/sqlprogrammability/archive/2008/03/18/using-time-zone-data-in-sql-server-2008.aspx

            //  Change the file path to suit your needs
            StreamWriter sr = File.CreateText(@"c:\Code\Timezone\OlsonData\TZMapping.txt");
            StreamWriter wsr = File.CreateText(@"c:\Code\Timezone\OlsonData\WindowsTZMapping.txt");
            StreamWriter tr = File.CreateText(@"c:\Code\Timezone\OlsonData\TZZones.txt");

            //  write the headers
            sr.WriteLine("ID\tDaylightName\tStandardName\tRawUtcOffset\tOffsetSeconds\tStandardAbbreviation\tWin32Id");
            wsr.WriteLine("ID\tDaylightName\tStandardName\tRawUtcOffset\tOffsetSeconds\tStandardAbbreviation\tWin32Id");
            tr.WriteLine("ID\tTransitionStart\tTransitionEnd\tDeltaSeconds\tDST");

            string[] zoneNames = OlsonTimeZone.AllNames;

            for (int i = 0; i < zoneNames.Length; i++) {
                OlsonTimeZone tz = OlsonTimeZone.GetInstanceFromOlsonName(zoneNames[i].ToString());

                //  Write out the raw Olson data.  There are many...
                sr.Write(i.ToString() + "\t");
                sr.Write(tz.DaylightName.Trim() + "\t");
                sr.Write(tz.StandardName.Trim() + "\t");
                sr.Write(tz.RawUtcOffset.ToString() + "\t");
                sr.Write(tz.RawUtcOffset.TotalSeconds.ToString() + "\t");
                sr.Write(tz.StandardAbbreviation.Trim() + "\t");
                sr.WriteLine(tz.Win32Id == null ? "" : tz.Win32Id.Trim());
                
                //  Dump the data for all records with a Windows Timezone ID
                if (tz.Win32Id != null) {
                    wsr.Write(i.ToString() + "\t");
                    wsr.Write(tz.DaylightName.Trim() + "\t");
                    wsr.Write(tz.StandardName.Trim() + "\t");
                    wsr.Write(tz.RawUtcOffset.ToString() + "\t");
                    wsr.Write(tz.RawUtcOffset.TotalSeconds.ToString() + "\t");
                    wsr.Write(tz.StandardAbbreviation.Trim() + "\t");
                    wsr.WriteLine(tz.Win32Id.Trim());
                }

                DaylightTime[] times = tz.AllTimeChanges;
                for (int j = 0; j < times.Length; j++) {
                    //  I don't want all of the historical data, just the current and future rules.
                    if (times[j].End.Year.CompareTo(2012) >= 0) {
                        tr.Write(i.ToString() + "\t");
                        tr.Write(times[j].Start.ToString("yyyy-MM-dd HH:mm:ss") + "\t");
                        tr.Write(times[j].End.ToString("yyyy-MM-dd HH:mm:ss") + "\t");
                        tr.Write(times[j] is StandardTime ? "0\t" : times[j].Delta.TotalSeconds.ToString() + "\t");
                        tr.WriteLine(times[j] is StandardTime ? false.ToString() : true.ToString());
                    }
                }
            }

            // clean up
            tr.WriteLine();
            sr.WriteLine();
            wsr.WriteLine();
            tr.Close();
            sr.Close();
            wsr.Close();
        }
    }
}
