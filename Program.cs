using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
//using Xceed.Wpf.Toolkit;

namespace Shredder
{
    class Program

    {

        


        static void Main(string[] args)
        {

            // specify the path to the rtf file
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            string path = currentDirectory;
            path += "/input_document.rtf";

            //pass the file to a function that will shred it and extract the information
            shredDocument(path);
        }

        public static void shredDocument(string documentPath)
        {
            //Create the RichTextBox
            System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();

            // Get the contents of the rtf file and store in a string
            string rtfText = System.IO.File.ReadAllText(documentPath);

            //split document into sections
            string[] arrayOfSections = splitDoucmentIntoSections(rtfText);

            //Now we need to create a data structure to hold each section along with any paragraphys that may be listed
            //This will be <Section> , <Section Paragraphs[]> 

            //assign paragraphs to section
            IDictionary<string, string[]> sections = assignParagraphsToSection(arrayOfSections);

            //export to xml 
            exportToXML(sections);


        }

        public static string[] splitDoucmentIntoSections(string documentText)
        {
            //split the text up into a array of strings by back space , this will help us have the secions , and the paragraphs sepetate
            string[] strs = documentText.Split('\n');

            //remove any empty strings from that array
            strs = strs.Where(val => val != "").ToArray();
            //remove the     XX    at end of pag
            strs = strs.Where(val => val != "‑‑‑‑XX‑‑‑‑\r").ToArray();

            return strs;
        }

        public static IDictionary<string, string[]> assignParagraphsToSection(string[] arrayOfSections)
        {
            //This will assign paragraphs to section

            IDictionary<string, string[]> sections = new Dictionary<string, string[]>();

            //iterate over the array of sections
            string currentSection = "";

           

            foreach (string s in arrayOfSections)
            {
                //if s is the section 
                if (s.Contains("SECTION"))
                {
                    //check if the map does not have the section add it
                    currentSection = s;
                    if (!sections.ContainsKey(s))
                    {
                        //if it does not add it to the map
                        string[] paragraphs = { };
                        sections.Add(s, paragraphs);
                    }
                }
                else
                {
                    //if s is the paragraph
                    //add it to the array
                    string[] paragraphsForSection;

                    sections.TryGetValue(currentSection, out paragraphsForSection);
                    paragraphsForSection = paragraphsForSection.Concat(new string[] { s }).ToArray();
                    sections[currentSection] = paragraphsForSection;


                }

               

            }

            return sections;
        }

        public static void exportToXML(IDictionary<string, string[]> sections)
        {
            //iterate over the dictionary
            foreach (KeyValuePair<string, string[]> entry in sections)
            {
                // do something with entry.Value or entry.Key

                string[] paragraph = entry.Value;
                string firstParagraph = paragraph[0];

                //should not be hard coded numbers
                int indexSpace = firstParagraph.IndexOf(" ");
                int indexFul = firstParagraph.IndexOf(".");

                //get the id
                string id = firstParagraph.Substring(indexSpace, indexFul);
                id = id.Substring(0, 8);
                Console.WriteLine("ID " + id);

                //split based on the - , to get title no , chapter no and 
                string[] ids = id.Split("-");


                string titleNo = ids[0];

                string chapterNo = ids[1];
                string sectionNo = ids[2];

                //now I will need to export these values to an xml file
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(
                            "<?xml version='1.0'?>" +
                            "<section title='" + titleNo + "' chapter='" + chapterNo + "' number='" + sectionNo + "' id='" + id + "'>" +
                            "<body>" +
                            //need to add paragraph 
                            " <paragraph tabs='1'>Whenever the location stablishment and correction of the State line.</paragraph>" +
                            "</body >" +
                            "</section>"


                            );

                doc.Save(id);
            }
        }
        
    }

}
