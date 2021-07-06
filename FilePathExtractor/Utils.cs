using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FilePathExtractor
{
    public static class Utils
    {
        public static async Task Extract(string input, Stream output, IProgress<(long max, long value)> progress = null)
        {
            using (var streamWriter = new StreamWriter(output, new UTF8Encoding(false), 1024, true))
            using (var fileStream = new FileStream(input, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { Async = true }))
            {
                while (await xmlReader.ReadAsync())
                {
                    if (xmlReader.NodeType != XmlNodeType.Element || xmlReader.Name != "fileName") continue;

                    await xmlReader.ReadAsync();
                    var filePath = (await xmlReader.GetValueAsync()).Trim();

                    if (filePath.Length == 0) continue;

                    streamWriter.WriteLine(filePath);
                    streamWriter.Flush();

                    progress?.Report((fileStream.Length, fileStream.Position));
                }
            }
        }
    }
}
