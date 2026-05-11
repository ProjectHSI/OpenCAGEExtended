using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ScriptAnalyzer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			// Compact large objects!
			System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
			System.Runtime.GCSettings.LatencyMode = System.Runtime.GCLatencyMode.Batch;

			bool generateLevelReports = false;
			bool generateGameReports = false;
			bool generateEverything = false;

			IList<string> gameRoots = new List<string>();


			foreach (string arg in args)
			{
				if (arg.StartsWith("--"))
				{
					switch (arg.Substring(2).Split(":")[0])
					{
						case "help":
							Console.WriteLine("Usage: ScriptAnalyzer [options] <gameRoots>");
							Console.WriteLine("Options:");
							Console.WriteLine("  --help              Show this help message");
							Console.WriteLine("  --level             Generates per-level reports");
							Console.WriteLine("  --level:<level>     Generates a report only for that level in the game roots specified. May be specified multiple times.");
							Console.WriteLine("                      '--level' does effectively nothing if this flag is present. Operates on '--game' and '--everything' as well.");
							Console.WriteLine("  --game              Generates whole-game reports (makes massive XMLs!).");
							Console.WriteLine("  --everything        Generates a report of every game root specified (makes massive XMLs!).");
							Console.WriteLine("  --memoize           Enables memoization if creating everything reports, whole-game reports, and per-level reports, improving performance.");
							Console.WriteLine("                      Use on systems where memory is not a constraint.");
							Console.WriteLine("  --parallel:game:N   Enables parallel processing of game roots with a degree of parallelism of N.");
							Console.WriteLine("  --parallel:level:N  Enables parallel processing of levels within game roots with a degree of parallelism of N.");
							return;
						case "level":
							generateLevelReports = true;
							if (arg.Split(":").Length > 1)
							{
								Report.LevelFilter.Add(arg.Split(":")[1]);
							}
							break;
						case "parallel":
							if (arg.Split(":").Length > 2)
							{
								switch (arg.Split(":")[1])
								{
									case "game":
										Report.GameRootParallelism = int.Parse(arg.Split(":")[2]);
										break;
									case "level":
										Report.LevelParallelism = int.Parse(arg.Split(":")[2]);
										break;
								}
							}
							break;
						case "game":
							generateGameReports = true;
							break;
						case "everything":
							generateEverything = true;
							break;
						case "memoize":
							Report.DisableMemoization = false;
							break;
						default:
							Console.WriteLine($"Unknown option: {arg}");
							return;
					}
				} else
				{
					gameRoots.Add(arg);
				}
			}

			if (!generateLevelReports || !generateGameReports || !generateEverything)
			{
				// Assume all three
				generateEverything = true;
				generateGameReports = true;
				generateLevelReports = true;
			}

			if (gameRoots.Count == 0) {
				Console.WriteLine("Enter semicolon-delimited game roots...");
				//Console.ReadLine();

				string? userEnteredGameRoots = Console.ReadLine();

				if (userEnteredGameRoots == null)
				{
					Console.WriteLine("No game roots entered.");
					return;
				}

				gameRoots = userEnteredGameRoots.Split(";", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
			}
			/*XDocument document = new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"GameReport.xslt\""),
				new XElement("GameReports",
					from path in gameRoots
					select Report.GenerateReport(path)));*/

			gameRoots.AsParallel().WithDegreeOfParallelism(1).ForAll(x => {
				GameRootContext? gameRootContext = new GameRootContext(x);
				if (generateLevelReports)
					Report.GenerateLevelReports(gameRootContext, x);
				if (generateGameReports)
					Report.GenerateGameReport(gameRootContext, x);
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
