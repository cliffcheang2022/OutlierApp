using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SupplementaryAPI.Ini
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : Ini Reader for parsing ini file</para>
    /// </summary>
    public class IniReader
    {
        #region Global Variables
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;
        #endregion

        #region DllImports
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int GetPrivateProfileStringW(string lpAppName, string lpKeyName, string lpDefault, string lpReturnString, int nSize, string lpFilename);

        // Second Method
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string Section, int Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);

        // Third Method
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int Section, string Key, string Value, [MarshalAs(UnmanagedType.LPArray)] byte[] Result, int Size, string FileName);
        #endregion

        #region Constructor
        public IniReader(string IniPath = null)
        {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }
        #endregion

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(1000);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 1000, Path);
            return RetVal.ToString();
        }

        public List<string> ReadAllKeys(string Section = null)
        {
            var tmpString = new string(' ', 1500);
            GetPrivateProfileStringW(Section ?? EXE, null, "", tmpString, 1500, Path);
            List<string> RetVal = new List<string>(tmpString.Split('\0'));
            RetVal.RemoveRange(RetVal.Count - 2, 2);

            return RetVal;
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public string[] GetSectionNames()
        {
            for (int maxsize = 500; true; maxsize *= 2)
            {
                byte[] bytes = new byte[maxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, maxsize, Path);

                if (size < maxsize - 2)
                {
                    string Selected = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    return Selected.Split(new char[] { '\0' });
                }
            }
        }
    }
}
