using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.IO.TextParsing.ContentReaders
{
    class HtmlContentReader : IContentReader
    {
        public bool TryGetContent(Stream fileStream, out string result)
        {
            result = string.Empty;
            if (fileStream.Length == 0) return false;

            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.Load(fileStream, true);

                IEnumerable<HtmlNode> nodes = htmlDoc.DocumentNode.DescendantsAndSelf()
                    .Where(n => n.NodeType == HtmlNodeType.Text);

                StringBuilder sb = new StringBuilder();
                foreach (HtmlNode node in nodes)
                {
                    sb.Append(node.InnerText);
                }
                result = sb.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
