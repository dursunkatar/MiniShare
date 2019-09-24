using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MiniShare
{
    public class SharedFileInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string SharePath { get; private set; }

        public SharedFileInfo(string path)
        {
            var fileInfo = new FileInfo(path);
            Name = fileInfo.Name;
            SharePath = Utilities.SharedPathFix(Name);
            Path = path;
        }

        public byte[] Bytes
        {
            get
            {
                return File.ReadAllBytes(Path);
            }
        }
    }
}
