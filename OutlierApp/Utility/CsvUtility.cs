using Microsoft.VisualBasic.FileIO;
using OutlierApp.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlierApp.Utility
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : CsvUtility is used for reading csv file</para>
    /// </summary>
    public class CsvUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataTable Read(string path)
        {
            var dt = new DataTable();
            var filename = Path.GetFileNameWithoutExtension(path);
            dt.TableName = filename;

            using (TextFieldParser csvReader = new TextFieldParser(path))
            {
                csvReader.CommentTokens = new string[] { "#" };
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;

                // Read Csv Title and set it to column name
                var title = csvReader.ReadLine().Split(',');

                foreach(var item in title)
                {
                    dt.Columns.Add(new DataColumn(item));
                }

                while (!csvReader.EndOfData)
                {
                    var index = 0;
                    DataRow dr = dt.NewRow();
                    // Read current line fields, pointer moves to the next line.
                    string[] fields = csvReader.ReadFields();

                    foreach(var field in fields)
                    {
                        dr[index++] = field;
                    }

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }
    }
}
