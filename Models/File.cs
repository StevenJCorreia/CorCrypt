using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorCrypt.Models
{
    public class File
    {
        public string fileName { get { return fullFilePath.Split('\\').Last(); } set { } }
        public string fullFilePath;

        public File() { }
        public File(string fullFilePath) { this.fullFilePath = fullFilePath; }

        public static File[] StringToFile_Array(string[] files)
        {
            File[] fileList = new File[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                fileList[i] = new File(files[i]);
            }

            return fileList;
        }

        public static Boolean DuplicateFileName(File[] files, string fileName)
        {
            foreach (File file in files)
            {
                if (file.fileName.Equals(fileName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
