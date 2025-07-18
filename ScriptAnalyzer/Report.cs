using CATHODE.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScriptAnalyzer
{
	internal static class Report
	{
		public static string ConvertPathToSafePath(string path)
		{
			return path.Replace(":", "_").Replace("\\", "_").Replace("/", "_").Replace(" ", "_");
		}

		private static IEnumerable<string> GetLevels(string gameRoot)
		{
			return from item in Directory.EnumerateDirectories(Path.Combine(gameRoot, "DATA/ENV/PRODUCTION/"))
				   where !item.EndsWith("DLC")
				   select item;
		}

		private static Dictionary<string, XElement> cachedLevelReports = [];

		private static XElement GetLevelReport(string levelPath)
		{
			if (!cachedLevelReports.TryGetValue(levelPath, out XElement? levelReport))
			{
				levelReport = LevelReport.GetLevelReport(levelPath);
				cachedLevelReports.TryAdd(levelPath, levelReport);
			}

			return levelReport;
		}

		private static XElement GetLevelReports(string gameRoot)
		{
			return new XElement("LevelReports",
					from level in GetLevels(gameRoot).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
					select GetLevelReport(level));
		}

		public static void GenerateLevelReports(string gameRoot)
		{
			Directory.CreateDirectory($"Report/{ConvertPathToSafePath(gameRoot)}/LevelReports");

			GetLevels(gameRoot).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).ForAll((level) =>
			{
				XElement levelReport = GetLevelReport(level);
				levelReport.SetAttributeValue("Path", gameRoot);
				levelReport.SetAttributeValue("SafePath", ConvertPathToSafePath(gameRoot));

				new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"../../LevelReport.xslt\""),
					levelReport).Save($"Report/{ConvertPathToSafePath(gameRoot)}/LevelReports/{Path.GetFileNameWithoutExtension(level)}.xml");
			});
		}

		private static XElement GetMiscGameReport(string gameRoot)
		{
			// PLACEHOLDER
			return new XElement("Misc");
		}

		private static Dictionary<string, XElement> cachedGameReports = [];

		private static XElement GetGameReport(string gameRoot)
		{
			if (!cachedLevelReports.TryGetValue(gameRoot, out XElement? gameReport))
			{
				gameReport = new XElement("GameReport",
				new XAttribute("Path", gameRoot),
				new XAttribute("SafePath", gameRoot),
				GetLevelReports(gameRoot),
				GetMiscGameReport(gameRoot));

				cachedGameReports.TryAdd(gameRoot, gameReport);
			}

			return gameReport;
		}

		public static void GenerateGameReport(string gameRoot)
		{
			Directory.CreateDirectory($"Report/{ConvertPathToSafePath(gameRoot)}");

			new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"../GameReport.xslt\""),
					GetGameReport(gameRoot)).Save($"Report/{ConvertPathToSafePath(gameRoot)}/GameReport.xml");
		}

		public static void GenerateReport(string[] gameRoots)
		{
			//Console.WriteLine($"Processing {gameRoots}");

			/*List<XElement> LevelReports = [];
			LevelReports.AddRange(from item in Directory.EnumerateDirectories(Path.Combine(gameRoots, "DATA/ENV/PRODUCTION/"))
								  select LevelReport.GetLevelReport(item));*/

			new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"MasterReport.xslt\""),
				new XElement("GameReports",
					from gameRoot in gameRoots
					select GetGameReport(gameRoot))).Save($"Report/MasterReport.xml");

			
					//from ();
		}
	}
}
