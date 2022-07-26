using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRCToyController
{
    class VRCLogReader
    {
        string folderPath;
        string logPath;
        bool folderFound;
        StreamReader logStream;
        string recentWorld;
        public VRCLogReader()
        {
            folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+ "Low/VRChat/VRChat";
            folderFound = Directory.Exists(folderPath);
            if (folderFound)
            {
                UpdateLogPath();
            }
        }

        void UpdateLogPath()
        {
            logPath = Directory.GetFiles(folderPath).Where(p => Path.GetFileName(p).StartsWith("output_log")).OrderByDescending(p => File.GetLastWriteTime(p)).FirstOrDefault();
            if (logStream != null) 
            { 
                logStream.Close();
                logStream = null;
            }
            if(logPath != null)
            {
                var fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                logStream = new StreamReader(fs);
            }
        }

        public string GetRecentWorld()
        {
            if (logStream == null) return recentWorld;

            string line;
            while ((line = logStream.ReadLine()) != null){
                if (line.Contains("[Behaviour] Joining w"))
                {
                    //Console.WriteLine(line);
                    recentWorld = Regex.Match(line, @"wr.*(?=:)").Value;
                }
            }

            return recentWorld;
        }

    }
}
