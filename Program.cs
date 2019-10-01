using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ringba_test
{
    class Program
    {
        public static string fileUrl = "https://ringba-test-html.s3-us-west-1.amazonaws.com/TestQuestions/output.txt";
        
        static void Main(string[] args)
        {
            Console.WriteLine("Statistics of the file!\n");
            bool error = false;
            String fileContent = string.Empty;

            #region File read

            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(fileUrl);
                StreamReader reader = new StreamReader(stream);
                fileContent = reader.ReadToEnd();
            }
            catch (Exception)
            {
                error = true;
            }

            #endregion

            if (!error)
            {
                if (!string.IsNullOrEmpty(fileContent))
                {
                    #region Each letter total count

                    var letterList = fileContent
                                     .GroupBy(c => c.ToString().ToLower())
                                     .OrderBy(c => c.Key)
                                     .Where(c => c.Key != " ")
                                     .Select(c => new { Letter = c.Key.Trim(), Count = c.Count() });

                    foreach (var letter in letterList)
                    {
                        Console.WriteLine("Letter '" + letter.Letter + "' total is " + letter.Count + " ");
                    }

                    #endregion

                    #region Total capital letters

                    var capitalList = fileContent
                                      .Where(char.IsUpper)
                                      .Count();

                    Console.WriteLine("\nTotal capital letters are " + capitalList);

                    #endregion

                    #region Most common word with count

                    var commonList = string.Concat(
                                                    fileContent
                                                    .Select(w => Char.IsUpper(w) ? " " + w : w.ToString())
                                                  ).TrimStart(' ');

                    var wordsCountList = Regex.Split(commonList, "[ ]")
                                         .Select(w => w.Trim())
                                         .Where(w => !string.IsNullOrEmpty(w))
                                         .GroupBy(w => w, StringComparer.OrdinalIgnoreCase)
                                         .Select(d => new { key = d.Key, count = d.Count() }).ToList();

                    if (wordsCountList.Count > 0)
                    {
                        var maxwordcount = wordsCountList.OrderByDescending(c => c.count).First().count;

                        var maxwordList = wordsCountList
                                      .Where(m => m.count == maxwordcount)
                                      .Select(w => new { Word = w.key, Count = w.count });

                        foreach (var item in maxwordList)
                        {
                            Console.WriteLine("\nMost common word is '" + item.Word + "' and " + item.Count + " times it has been seen");
                        }
                    }
                    else
                        Console.WriteLine("\nMost common word not found!");

                    #endregion

                    #region Most common 2 character prefix with occurrance

                    var prefixList = wordsCountList
                                     .Where(o => o.key.Length >= 2)
                                     .GroupBy(x => x.key.Substring(0, 2))
                                     .Select(o => new { Prefix = o.Key.Substring(0, 2), Count = o.Count() })
                                     .OrderByDescending(g => g.Count).ToList();

                    if (prefixList.Count() > 0)
                        Console.WriteLine("\nMost common 2 character prefix is '" + prefixList.First().Prefix + "' and " + prefixList.First().Count + " occurrences in the text file. ");
                    else
                        Console.WriteLine("\nMost common 2 character prefix not found!");

                    #endregion
                }
                else Console.WriteLine("File is empty!");
            }
            else Console.WriteLine("File not found!");
        }
    }
}
