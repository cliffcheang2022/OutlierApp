using OutlierApp.Model;
using OutlierApp.Utility;
using SupplementaryAPI.Algorithms;
using SupplementaryAPI.Ini;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace OutlierApp
{
    class Program
    {
        /// <summary>
        /// Read Ini File and parse the data into AppConfig object
        /// </summary>
        private static AppConfig ReadIni()
        {
            AppConfig appConfig = new AppConfig();

            try
            {
                IniReader myIni = new IniReader($"{AppDomain.CurrentDomain.BaseDirectory}\\appConfig.ini");
                myIni.ReadIni(ref appConfig);
                Console.WriteLine($"Reading INI file Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reading INI file with exception! , ex : {ex}");
            }

            return appConfig;
        }


        /// <summary>
        /// main entry point
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var calc = new CalcAlgorithm();
            var csv = new CsvUtility();
            var buckets = new List<char>() { '{', '}' };
            var today = DateTime.Now;

            try
            {
                // Load setting and retrieve data from input file
                var appConfig = ReadIni();
                var inputfilename = $"{appConfig._InputFile.ReplaceByBuckets(buckets, today)}";
                var outputfilename = $"{appConfig._OutputFile.ReplaceByBuckets(buckets, today)}";
                var targetfile = $"{appConfig._InputPath}{inputfilename}"; // Normally, all input files should be dated by trade date/settlement date and I use today date for an example (file: Outliers_20220412.csv)
                var backupfile = $"{appConfig._BackupPath}{inputfilename}";
                var outputfile = $"{appConfig._OutputPath}{outputfilename}";
                var dt = csv.Read(targetfile);
                var datasource = dt.AsEnumerable()
                                   .Where(w => w != null)
                                   .Select(dr => dr.CreateItemFromRow<HistoricalPrice>())
                                   .ToList();
                

                List<double> input_list = datasource.Select(s => s.Price).ToList();
                List<HistoricalPrice> normal_list = new List<HistoricalPrice>();
                List<HistoricalPrice> adnormal_list = new List<HistoricalPrice>();

                // calculate the standard deviation and avg value for comparsion
                double standard_deviation = calc.GetStandardDeviation(input_list);
                double avg = input_list.Average();
                Console.WriteLine($"Standard Deviation = {standard_deviation}; avg = {avg}");

                foreach (var data in datasource)
                {
                    var diff = Math.Abs(data.Price - avg);
                    Console.WriteLine($"Difference = {diff}");


                    if (diff > standard_deviation)
                    {
                        // when value is out of standard deviation, I can treat it as outliers
                        adnormal_list.Add(data);
                        Console.WriteLine($"Added in adnormal_list");
                    }
                    else
                    {
                        // otherwise, it is a normal value
                        normal_list.Add(data);
                        Console.WriteLine($"Added in normal_list");
                    }
                }

                // Data Output Not included in this coding exercise
                // Previously, I normally use socket programming and event handler for data uploading to Database
                // Server socket is built as a Windows service running in the server side for listening client requests (GET Data; MANAGE Data; DELETE Data)
                // Client socket is built as a API calling by client programs

                // In this case, I'll use external file output
                string outcontent = "Date,Price\n" + string.Join("", normal_list.Select(s => $"{s.Date},{s.Price}\n").ToArray());
                File.WriteAllText(outputfile, outcontent);
                Console.WriteLine($"Data outputted!!");

                // Move the input file to done folder for backup
                targetfile.MoveFile(backupfile);
                Console.WriteLine($"Input file is moved to {appConfig._OutputPath}!!");
            }
            catch (Exception ex)
            {
                // Exception control Not included in this coding exercise
                // Use Log4net to write a log
                // If it is a critical exception, we can send an alert email via smtp 
            }
        }
    }
}
