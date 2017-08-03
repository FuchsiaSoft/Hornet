using MsgReader;
using MsgReader.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Pri.LongPath.File;

namespace Hornet.IO.TextParsing.ContentReaders
{
    public class EmailContentReader : IFileFormatReader
    {
        public bool TryGetContent(string filePath, out string result)
        {
            try
            {
                using (Stream stream = File.OpenRead(filePath))
                using(Storage.Message msg = new Storage.Message(stream))
                {
                    Reader reader = new Reader();
                    //TODO: get attachments from looking at code here,
                    //must cast the attachments from a msg object into byte[]
                    //and modify content readers to take a memory stream
                    //https://github.com/Sicos1977/MSGReader/blob/master/MsgReader/Reader.cs#L1718

                    throw new NotImplementedException();
                }
                
            }
            catch (Exception)
            {
                result = string.Empty;
                return false;
            }
        }
    }
}
