using OutlierApp.Model;
using SupplementaryAPI.Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;

namespace OutlierApp
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : Function Extension</para>
    /// </summary>
    public static class Extensions
    {
        #region Variables
        private static readonly string _DEFAULT_SMTP_HOST = "smtp.xxxxx.com";
        private static readonly string _DEFAULT_SMTP_PORT = "25";
        private static readonly string _DEFAULT_SMTP_TITLE = "";
        #endregion

        #region Information retrieval
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetIPAddress(this IPAddress[] ip) => ip.Where(w => w.AddressFamily == AddressFamily.InterNetwork)
                                                                  .Select(s => s.ToString()).FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="networkInterfaces"></param>
        /// <returns></returns>
        public static string GetMacAddress(this NetworkInterface[] networkInterfaces) => networkInterfaces.Where(w => w.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                                                                    .Select(s => s.GetPhysicalAddress().ToString()).FirstOrDefault();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetVersion(this Assembly assembly) => $"v{assembly.GetName().Version.Major.ToString()}.{assembly.GetName().Version.Minor.ToString()}{assembly.GetName().Version.Revision.ToString()}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAppName(this Assembly assembly) => assembly.GetName().Name;
        #endregion

        #region Enum Handling
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetEnumString(this Enum enumString) => enumString.ToString();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objEnum"></param>
        /// <returns></returns>
        private static string GetCustomDescription(object objEnum)
        {
            var fi = objEnum.GetType().GetField(objEnum.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : objEnum.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Description(this Enum value) => GetCustomDescription(value);
        #endregion

        #region File Handling
        /// <summary>
        /// Create directory if it is not existed
        /// </summary>
        /// <param name="outputdir"></param>
        public static void CreateDir(this string outputdir)
        {
            if (!Directory.Exists(outputdir))
            {
                Directory.CreateDirectory(outputdir);
            }
        }

        /// <summary>
        /// Move Files to target directory
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destFile"></param>
        public static void MoveFile(this string sourceFile, string destFile)
        {
            if (File.Exists(destFile))
            {
                File.Delete(destFile);
            }

            File.Move(sourceFile, destFile);
        }
        #endregion

        #region Csv Extensions Handling
        /// <summary>
        /// <para>This function is workable with the heading row in EXCEL.</para>
        /// <para>Replacement:</para>
        /// <para>Space -> _</para>
        /// <para>( -> NULL</para>
        /// <para>/ -> Or</para>
        /// <para>) -> NULL</para>
        /// <para>. -> NULL</para>
        /// <para>% -> NULL</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateItemFromRow<T>(this DataRow row) where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="row"></param>
        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column, replace useless symbols in the heading
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName.Trim());

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    var tempstring = row[c].ToString();

                    if (p.PropertyType.Name == nameof(DateTime))
                    {
                        if (!string.IsNullOrEmpty(tempstring))
                        {
                            p.SetValue(item, DateTime.Parse(tempstring), null);
                        }
                        else
                        {
                            p.SetValue(item, DateTime.MinValue, null);
                        }
                    }
                    else if (p.PropertyType.Name == nameof(Int32))
                    {
                        if (!string.IsNullOrEmpty(tempstring))
                        {
                            var isInt = int.TryParse(tempstring, out int n);

                            if (isInt)
                            {
                                p.SetValue(item, int.Parse(tempstring), null);
                            }
                            else
                            {
                                p.SetValue(item, 0, null);
                            }
                        }
                        else
                        {
                            p.SetValue(item, 0, null);
                        }
                    }
                    else if (p.PropertyType.Name == nameof(Int64))
                    {
                        if (!string.IsNullOrEmpty(tempstring))
                        {
                            p.SetValue(item, long.Parse(tempstring), null);
                        }
                        else
                        {
                            p.SetValue(item, 0, null);
                        }
                    }
                    else if (p.PropertyType.Name == nameof(Double))
                    {
                        if (!string.IsNullOrEmpty(tempstring))
                        {
                            var isDouble = double.TryParse(tempstring, out double n);

                            if (isDouble)
                            {
                                p.SetValue(item, double.Parse(tempstring), null);
                            }
                            else
                            {
                                p.SetValue(item, 0.0, null);
                            }
                        }
                        else
                        {
                            p.SetValue(item, 0, null);
                        }
                    }
                    else
                    {
                        p.SetValue(item, row[c], null);
                    }
                }
            }
        }
        #endregion

        #region INI File Handling
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myIni"></param>
        /// <param name="section"></param>
        private static AppConfig.Alert GetEmailAlert(IniReader myIni, string section)
        {
            var to = myIni.Read(EnumIniKey.to.GetEnumString(), section);
            var cc = myIni.Read(EnumIniKey.cc.GetEnumString(), section);
            var tempTo = to.Contains(",") ? to.IndexOf(",") > 0 ? to.Split(',') : new string[] { } : new string[] { to };
            var tempCc = cc.Contains(",") ? cc.IndexOf(",") > 0 ? cc.Split(',') : new string[] { } : new string[] { cc };

            try
            {
                var alert = new AppConfig.Alert()
                {
                    Enable = myIni.Read(EnumIniKey.enable.GetEnumString(), section).ToLower() == "true",
                    Host = myIni.Read(EnumIniKey.host.GetEnumString(), section) ?? _DEFAULT_SMTP_HOST,
                    Port = int.Parse(myIni.Read(EnumIniKey.port.GetEnumString(), section) ?? _DEFAULT_SMTP_PORT),
                    Subject = myIni.Read(EnumIniKey.subject.GetEnumString(), section) ?? _DEFAULT_SMTP_TITLE,
                    From = myIni.Read(EnumIniKey.from.GetEnumString(), section),
                    To = tempTo.ConvertArray2List(),
                    Cc = tempCc.ConvertArray2List()
                };

                return alert;
            }
            catch (Exception ex)
            {
                return new AppConfig.Alert();
            }
        }

        /// <summary>
        /// Read ini configuation for this coding exercise demostration
        /// </summary>
        /// <param name="myIni"></param>
        /// <param name="login"></param>
        /// <returns></returns>
        public static bool ReadIni(this IniReader myIni, ref AppConfig appconfig)
        {
            try
            {
                // Retrieve the normal information in INI
                var mainSection = EnumIniSection.MainSection.Description();
                var sectionAlert = EnumIniSection.AlertSection.Description();

                // Main Section
                var inputPath = myIni.Read(EnumIniKey.inputpath.GetEnumString(), mainSection);
                var outputPath = myIni.Read(EnumIniKey.outputpath.GetEnumString(), mainSection);
                var backupPath = myIni.Read(EnumIniKey.backuppath.GetEnumString(), mainSection);
                var inputfile = myIni.Read(EnumIniKey.inputfile.GetEnumString(), mainSection);
                var outputfile = myIni.Read(EnumIniKey.outputfile.GetEnumString(), mainSection);

                // Email Alert
                var emailAlert = GetEmailAlert(myIni, sectionAlert);

                // Assign to AppConfig
                appconfig._IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.GetIPAddress();
                appconfig._MacAddress = NetworkInterface.GetAllNetworkInterfaces().GetMacAddress();
                appconfig._AppName = Assembly.GetExecutingAssembly().GetAppName();
                appconfig._BackupPath = backupPath;
                appconfig._InputPath = inputPath;
                appconfig._OutputPath= outputPath;
                appconfig._InputFile = inputfile;
                appconfig._OutputFile = outputfile;
                appconfig._AlertInstance = emailAlert;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Other functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static List<string> ConvertArray2List(this string[] temp)
        {
            var list = new List<string>();

            foreach (var t in temp)
            {
                if (!string.IsNullOrEmpty(t))
                {
                    list.Add(t);
                }
            }

            return list;
        }

        /// <summary>
        /// Only predefined buckets are acceptable
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="buckets"></param>
        /// <returns></returns>
        private static bool ValidateBuckets(string sourceName, List<char> buckets)
        {
            var validBuckets = new char[] { '{', '[', '(', '<' };

            // NOT Contain Items in Buckets
            if (!buckets.All(item => sourceName.Contains(item)))
            {
                return false;
            }

            // MUST Use specified buckets
            if (!validBuckets.Any(sourceName.Contains))
            {
                return false;
            }
            else
            {
                // NOT include the open and close buckets
                if (buckets.Count < 2)
                {
                    return false;
                }
                else
                {
                    // MUST Close by same bucket symbol
                    if (buckets[1] - buckets[0] != 2)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Replace the predefined function name to target value
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="buckets"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ReplaceByBuckets(this string sourceName, List<char> buckets, DateTime date)
        {
            if (ValidateBuckets(sourceName, buckets))
            {
                var indexFirstBucket = sourceName.IndexOf($"{buckets[0]}$DATE");
                var indexLastBucket = sourceName.Substring(indexFirstBucket).IndexOf(buckets[1]) + indexFirstBucket;

                var source_1 = sourceName.Substring(0, indexFirstBucket);
                var source_2 = sourceName.Substring(indexFirstBucket + 1, indexLastBucket - indexFirstBucket - 1);
                var source_3 = sourceName.Substring(indexLastBucket + 1, sourceName.Length - (indexLastBucket + 1));

                var source_2_split = source_2.Split(':');

                return $"{source_1}{date.ToString(source_2_split[1])}{source_3}";
            }
            else
            {
                return sourceName;
            }
        }
        #endregion
    }
}
