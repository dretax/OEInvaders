// <copyright file="IniParser.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace OEInvaders.Library
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    /// <summary>
    /// Handles ini file.
    /// </summary>
    public class IniParser
    {
        private string iniFilePath;
        private Hashtable keyPairs = new Hashtable();
        private System.Collections.Generic.List<SectionPair> tmpList = new System.Collections.Generic.List<SectionPair>();
        private Thread _t;

        /// <summary>
        /// Returns the name of the ini.
        /// </summary>
        public string Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="IniParser"/> class.
        /// </summary>
        /// <param name="iniPath">Path.</param>
        /// <exception cref="FileNotFoundException">Ex.</exception>
        public IniParser(string iniPath)
        {
            string str2 = null;
            this.iniFilePath = iniPath;
            this.Name = Path.GetFileNameWithoutExtension(iniPath);

            if (!File.Exists(iniPath))
            {
                throw new FileNotFoundException("Unable to locate " + iniPath);
            }

            try
            {
                using (TextReader reader = new StreamReader(iniPath))
                {
                    for (string str = reader.ReadLine(); str != null; str = reader.ReadLine())
                    {
                        str = str.Trim();
                        if (str == string.Empty)
                        {
                            continue;
                        }

                        if (str.StartsWith("[") && str.EndsWith("]"))
                        {
                            str2 = str.Substring(1, str.Length - 2);
                        }
                        else
                        {
                            SectionPair pair;

                            if (str.StartsWith(";"))
                            {
                                str = str.Replace("=", "%eq%") + @"=%comment%";
                            }

                            string[] strArray = str.Split(new char[] { '=' }, 2);
                            string str3 = null;
                            if (str2 == null)
                            {
                                str2 = "ROOT";
                            }

                            pair.Section = str2;
                            pair.Key = strArray[0];
                            if (strArray.Length > 1)
                            {
                                str3 = strArray[1];
                            }

                            try
                            {
                                this.keyPairs.Add(pair, str3);
                                this.tmpList.Add(pair);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string IniPath
        {
            get { return this.iniFilePath; }
        }

        /// <summary>
        /// Adds a setting.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <param name="settingName">Key.</param>
        public void AddSetting(string sectionName, string settingName)
        {
            this.AddSetting(sectionName, settingName, string.Empty);
        }

        /// <summary>
        /// Adds a setting.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <param name="settingName">Key.</param>
        /// <param name="settingValue">Value.</param>
        public void AddSetting(string sectionName, string settingName, string settingValue)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (settingValue == null)
            {
                settingValue = string.Empty;
            }

            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs.Remove(pair);
            }

            if (this.tmpList.Contains(pair))
            {
                this.tmpList.Remove(pair);
            }

            this.keyPairs.Add(pair, settingValue);
            this.tmpList.Add(pair);
        }

        /// <summary>
        /// Counts the sections.
        /// </summary>
        /// <returns>Number.</returns>
        public int Count()
        {
            return this.Sections.Length;
        }

        /// <summary>
        /// Deletes data by key.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <param name="settingName">Key.</param>
        public void DeleteSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs.Remove(pair);
                this.tmpList.Remove(pair);
            }
        }

        /// <summary>
        /// Enums the section by keys.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <returns>Ret.</returns>
        public string[] EnumSection(string sectionName)
        {
            List<string> list = new List<string>();
            foreach (SectionPair pair in this.tmpList)
            {
                if (pair.Key.StartsWith(";"))
                    continue;

                if (pair.Section == sectionName)
                {
                    list.Add(pair.Key);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets Section names.
        /// </summary>
        public string[] Sections
        {
            get
            {
                List<string> list = new List<string>();
                foreach (SectionPair pair in this.tmpList)
                {
                    if (!list.Contains(pair.Section))
                    {
                        list.Add(pair.Section);
                    }
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// Returns the value by key and section.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <param name="settingName">Key.</param>
        /// <returns></returns>
        public string GetSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            return (string)this.keyPairs[pair];
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        public void Save()
        {
            var fi = new FileInfo(this.iniFilePath);
            float mega = (fi.Length / 1024f) / 1024f;
            if (mega <= 0.6)
            {
                this.SaveSettings(this.iniFilePath);
                return;
            }

            _t = new Thread(() => this.SaveSettings(this.iniFilePath));
            _t.Start();
        }

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="newFilePath">Path.</param>
        public void SaveSettings(string newFilePath)
        {
            ArrayList list = new ArrayList();
            string str = string.Empty;
            string str2 = string.Empty;
            foreach (SectionPair pair in this.tmpList)
            {
                if (!list.Contains(pair.Section))
                {
                    list.Add(pair.Section);
                }
            }

            foreach (string str3 in list)
            {
                str2 = str2 + "[" + str3 + "]\r\n";
                foreach (SectionPair pair2 in this.tmpList)
                {
                    if (pair2.Section == str3)
                    {
                        str = (string)this.keyPairs[pair2];
                        if (str != null)
                        {
                            if (str == "%comment%")
                            {
                                str = string.Empty;
                            }
                            else
                            {
                                str = "=" + str;
                            }
                        }

                        str2 = str2 + pair2.Key.Replace("%eq%", "=") + str + "\r\n";
                    }
                }

                str2 = str2 + "\r\n";
            }

            using (TextWriter writer = new StreamWriter(newFilePath))
            {
                writer.Write(str2);
            }
        }

        /// <summary>
        /// Sets the value by keys.
        /// </summary>
        /// <param name="sectionName">Section</param>
        /// <param name="settingName">Key.</param>
        /// <param name="value">Value.</param>
        public void SetSetting(string sectionName, string settingName, string value)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            if (string.IsNullOrEmpty(value))
            {
                value = string.Empty;
            }

            if (this.keyPairs.ContainsKey(pair))
            {
                this.keyPairs[pair] = value;
            }
        }

        /// <summary>
        /// Checks if a key is contained in the ini.
        /// </summary>
        /// <param name="sectionName">Section.</param>
        /// <param name="settingName">Key.</param>
        /// <returns>Bool.</returns>
        public bool ContainsSetting(string sectionName, string settingName)
        {
            SectionPair pair;
            pair.Section = sectionName;
            pair.Key = settingName;
            return this.keyPairs.Contains(pair);
        }

        /// <summary>
        /// Checks if the value exists.
        /// </summary>
        /// <param name="valueName">Value.</param>
        /// <returns>Bool.</returns>
        public bool ContainsValue(string valueName)
        {
            return this.keyPairs.ContainsValue(valueName);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SectionPair
        {
            public string Section;
            public string Key;
        }
    }
}