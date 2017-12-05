using Hornet.IO.FileManagement;
using Hornet.IO.TextParsing;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hornet.IO
{
    public class ScanResult
    {
        private ConcurrentBag<string> _encryptedFiles = new ConcurrentBag<string>();

        public ScanStatus ClosingStatus { get; internal set; }
        public IEnumerable<HashInfoGroup> HashGroups { get; internal set; } = new HashInfoGroup[0];
        public IEnumerable<RegexInfoGroup> RegexGroups { get; internal set; } = new RegexInfoGroup[0];
        public IEnumerable<string> EncryptedFiles
        {
            get
            {
                return _encryptedFiles.ToArray();
            }
        }

        internal void AddEncryptedFile(string filePath)
        {
            _encryptedFiles.Add(filePath);
        }

        public Stream ToPdf()
        {
            Document doc = new Document();
            SetStyles(doc);

            Section hashSection = doc.AddSection();

            if (HashGroups.Count() > 0)
            {
                Paragraph hashHeaderPara = hashSection.AddParagraph("Hash Matches");
                hashHeaderPara.Format.Font.Bold = true;
                hashHeaderPara.Format.Font.Size = 20;
                hashHeaderPara.Format.SpaceAfter = Unit.FromCentimeter(1);
            }
            

            foreach (HashInfoGroup group in HashGroups)
            {
                hashSection.AddParagraph("Hash Group: " + group.Name).Format.Font.Bold = true;
                hashSection.AddParagraph(group.Description).Format.SpaceAfter = Unit.FromCentimeter(1);

                foreach (HashResult result in group.Matches)
                {
                    Table table = hashSection.AddTable();
                    table.KeepTogether = true;
                    table.Format.Font.Size = 9;

                    Column column;

                    column = table.AddColumn(Unit.FromCentimeter(3));
                    column.Format.Alignment = ParagraphAlignment.Left;
                    column.Format.Font.Bold = true;

                    column = table.AddColumn(Unit.FromCentimeter(14));
                    column.Format.Alignment = ParagraphAlignment.Left;

                    Row row = table.AddRow();
                    row[0].AddParagraph("File Name:");
                    row[1].AddParagraph(result.Name).Format.Font.Size = 7;

                    row = table.AddRow();
                    row[0].AddParagraph("Mime Type:");
                    row[1].AddParagraph(result.MimeType);

                    row = table.AddRow();
                    row[0].AddParagraph("Size:");
                    row[1].AddParagraph($"{result.Length.ToString("#,###")} ({GetFriendlySize(result.Length)})");

                    row = table.AddRow();
                    row[0].AddParagraph("Matches:");
                    row[1].AddParagraph($"{result.MatchedHashInfos.Count()} matche(s) found");

                    Table matchTable = hashSection.AddTable();
                    matchTable.KeepTogether = true;

                    column = matchTable.AddColumn(Unit.FromCentimeter(9));
                    column.Format.Alignment = ParagraphAlignment.Left;
                    column.Format.Font.Size = 7;

                    column = matchTable.AddColumn(Unit.FromCentimeter(2));
                    column.Format.Alignment = ParagraphAlignment.Left;
                    column.Format.Font.Size = 7;

                    column = matchTable.AddColumn(Unit.FromCentimeter(7));
                    column.Format.Alignment = ParagraphAlignment.Left;
                    column.Format.Font.Size = 7;

                    row = matchTable.AddRow();
                    row.Format.Font.Bold = true;
                    row[0].AddParagraph("Hash");
                    row[1].AddParagraph("Hash Type");
                    row[2].AddParagraph("Remarks");

                    foreach (HashInfo matchedInfo in result.MatchedHashInfos)
                    {
                        row = matchTable.AddRow();
                        row[0].AddParagraph(matchedInfo.Hash);
                        row[1].AddParagraph(matchedInfo.HashType.ToString());
                        row[2].AddParagraph(matchedInfo.Remarks);
                    }

                    row.Format.SpaceAfter = Unit.FromCentimeter(1);
                }
            }

            Section regexSection = doc.AddSection();

            if (RegexGroups.Count() > 0)
            {
                Paragraph regexHeaderPara = regexSection.AddParagraph("Regex Matches");
                regexHeaderPara.Format.Font.Bold = true;
                regexHeaderPara.Format.Font.Size = 20;
                regexHeaderPara.Format.SpaceAfter = Unit.FromCentimeter(1);
            }

            foreach (RegexInfoGroup group in RegexGroups)
            {
                regexSection.AddParagraph("Regex Group: " + group.Name).Format.Font.Bold = true;
                regexSection.AddParagraph(group.Description).Format.SpaceAfter = Unit.FromCentimeter(1);

                foreach (RegexResult result in group.Matches)
                {
                    Table table = regexSection.AddTable();
                    table.KeepTogether = true;
                    table.Format.Font.Size = 9;

                    Column column;

                    column = table.AddColumn(Unit.FromCentimeter(3));
                    column.Format.Alignment = ParagraphAlignment.Left;
                    column.Format.Font.Bold = true;

                    column = table.AddColumn(Unit.FromCentimeter(14));
                    column.Format.Alignment = ParagraphAlignment.Left;

                    Row row = table.AddRow();
                    row[0].AddParagraph("File Name:");
                    row[1].AddParagraph(result.Name).Format.Font.Size = 7;

                    row = table.AddRow();
                    row[0].AddParagraph("Mime Type:");
                    row[1].AddParagraph(result.MimeType);

                    row = table.AddRow();
                    row[0].AddParagraph("Size:");
                    row[1].AddParagraph($"{result.Length.ToString("#,###")} ({GetFriendlySize(result.Length)})");

                    List<string> uniquePatterns =
                        result.MatchedRegexInfos.Select(r => r.Item1.Pattern).Distinct().ToList();

                    int matchCount = 0;

                    foreach (string uniquePattern in uniquePatterns)
                    {
                        matchCount += result.MatchedRegexInfos
                            .Where(r => r.Item1.Pattern == uniquePattern)
                            .Select(r => r.Item2).Distinct().Count();
                    }

                    row = table.AddRow();
                    row[0].AddParagraph("Matches:");
                    row[1].AddParagraph($"{matchCount} matche(s) found");

                    foreach (string uniquePattern in uniquePatterns)
                    {
                        Table matchTable = regexSection.AddTable();
                        matchTable.Format.Font.Size = 9;
                        matchTable.KeepTogether = true;

                        column = matchTable.AddColumn(Unit.FromCentimeter(3));
                        column.Format.Alignment = ParagraphAlignment.Left;
                        column.Format.Font.Bold = true;

                        column = matchTable.AddColumn(Unit.FromCentimeter(14));
                        column.Format.Alignment = ParagraphAlignment.Left;

                        row = matchTable.AddRow();
                        row[0].AddParagraph("Pattern:");
                        row[1].AddParagraph($"{uniquePattern}");

                        string remarks = result.MatchedRegexInfos.First(r => r.Item1.Pattern == uniquePattern).Item1.Remarks;

                        row = matchTable.AddRow();
                        row[0].AddParagraph("Remarks:");
                        row[1].AddParagraph($"{remarks}");

                        row = matchTable.AddRow();
                        row[0].AddParagraph("Preview(s):");

                        //TODO: some PDFs duplicate the content loads, need to figure out why
                        HashSet<string> doneContent = new HashSet<string>();

                        

                        foreach (Tuple<RegexInfo,string> matchedInfo in result.MatchedRegexInfos.Where(r=>r.Item1.Pattern == uniquePattern))
                        {
                            string contentMatched = matchedInfo.Item2;

                            if (doneContent.Contains(contentMatched))
                            {
                                continue;
                            }
                            else
                            {
                                doneContent.Add(contentMatched);
                            }

                            Regex thisRegex = matchedInfo.Item1.AsRegex();

                            Paragraph contentPara = regexSection.AddParagraph();
                            contentPara.Format.SpaceAfter = Unit.FromCentimeter(0.1);
                            contentPara.Format.Font.Size = 7;

                            contentPara.AddText("<<...");
                            try
                            {
                                //TODO: This was throwing some exceptions, need to look into it,
                                //for now just made it show the content without any formatting
                                Match firstMatch = thisRegex.Matches(contentMatched)[0];

                                if (firstMatch.Index > 0)
                                {
                                    contentPara.AddText(RemoveLineEndings(contentMatched.Substring(0, firstMatch.Index)));
                                }

                                Font font = new Font();
                                font.Color = Colors.OrangeRed;
                                font.Bold = true;
                                contentPara.AddFormattedText(RemoveLineEndings(firstMatch.Value), font);

                                if (contentMatched.Length > firstMatch.Index + firstMatch.Length)
                                {
                                    contentPara.AddText(RemoveLineEndings(contentMatched.Substring(firstMatch.Index + firstMatch.Length)));
                                }
                            }
                            catch (Exception)
                            {
                                contentPara.AddText(contentMatched);
                            }

                            contentPara.AddText("...>>");
                        }
                        Paragraph endPara = regexSection.AddParagraph($"End of content matches for {uniquePattern}");
                        endPara.Format.SpaceAfter = Unit.FromCentimeter(0.5);
                        endPara.Format.Font.Size = 9;
                    }
                    

                }
            }

            PdfDocumentRenderer renderer = new PdfDocumentRenderer();
            renderer.Document = doc;
            renderer.RenderDocument();
            MemoryStream stream = new MemoryStream();
            renderer.Save(stream, false);

            return stream;
        }

        private static void SetStyles(Document migraDocument)
        {
            migraDocument.DefaultPageSetup.PageFormat = PageFormat.A4;
            migraDocument.DefaultPageSetup.Orientation = Orientation.Portrait;

            migraDocument.DefaultPageSetup.RightMargin = Unit.FromCentimeter(2);
            migraDocument.DefaultPageSetup.LeftMargin = Unit.FromCentimeter(2);
            migraDocument.DefaultPageSetup.TopMargin = Unit.FromCentimeter(3);
            migraDocument.DefaultPageSetup.BottomMargin = Unit.FromCentimeter(4);

            Style style = migraDocument.Styles["Normal"];

            style.Font.Name = "Arial";
            style.Font.Size = 11;
            style.Font.Color = Colors.Black;
        }

        private static double _TB = Math.Pow(1024, 4);
        private static double _GB = Math.Pow(1024, 3);
        private static double _MB = Math.Pow(1024, 2);
        private static double _KB = Math.Pow(1024, 1);

        private string GetFriendlySize(long bytes)
        {
            if (bytes > _TB) return (bytes / _TB).ToString("#,##0.00") + " TB";
            if (bytes > _GB) return (bytes / _GB).ToString("#,##0.00") + " GB";
            if (bytes > _MB) return (bytes / _MB).ToString("#,##0.00") + " MB";
            if (bytes > _KB) return (bytes / _KB).ToString("#,##0.00") + " KB";

            return bytes.ToString("#,##0") + " B";
        }

        public static string RemoveLineEndings(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace(lineSeparator, " ").Replace(paragraphSeparator, " ");
        }
    }
}
