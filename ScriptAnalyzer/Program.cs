using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ScriptAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter semicolon-delimited game roots...");
            //Console.ReadLine();

            string[] gameRoots = Console.ReadLine().Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            /*XDocument document = new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"GameReport.xslt\""),
                new XElement("GameReports",
                    from path in gameRoots
                    select Report.GenerateReport(path)));*/

            gameRoots.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).ForAll(x => {
                Report.GenerateLevelReports(x);
                Report.GenerateGameReport(x);
            });
				//select Report.GenerateGameReport(gameRoot);

			Report.GenerateReport(gameRoots);

            //GC.Collect();

            //Console.WriteLine("Saving report to GameReport.xml...");
			//document.Save("./GameReport/GameReport.xml");

			//Console.WriteLine("Saving transformed report to GameReport.html...");
			//XslCompiledTransform xslt = new();
			//xslt.Load("./GameReport.xslt");
			//xslt.Transform("./GameReport.xml", "./GameReport.html");
		}
    }
}
