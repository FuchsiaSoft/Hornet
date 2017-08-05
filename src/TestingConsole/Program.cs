using Hornet.IO;
using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string dirPath = @"C:\Users\Chris\Documents\GitHub\Hornet\src\Hornet.IO.Tests\TestFiles\Text\";
            string fileName = "ExampleTxt.txt";

            string content = File.ReadAllText(dirPath + fileName);

            dirPath = "C:\\tmp\\";

            /*
             *  ASCII = 1,
                UTF7 = 2,
                UTF8 = 4,
                Unicode = 8,
                UnicodeBigEndian = 16,
                UTF32 = 32,
                AutoDetect = 64
             */

            File.WriteAllText(dirPath + "ASCII_Example.txt", content, Encoding.ASCII);
            File.WriteAllText(dirPath + "UTF7_Example.txt", content, Encoding.UTF7);
            File.WriteAllText(dirPath + "UTF8_Example.txt", content, Encoding.UTF8);
            File.WriteAllText(dirPath + "Unicode_Example.txt", content, Encoding.Unicode);
            File.WriteAllText(dirPath + "UnicodeBE_Example.txt", content, Encoding.BigEndianUnicode);
            File.WriteAllText(dirPath + "UTF32_Example.txt", content, Encoding.UTF32);
        }
    }
}
