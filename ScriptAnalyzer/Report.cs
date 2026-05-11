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
			return from item in Directory.EnumerateDirectories(Path.Combine(gameRoot, "DATA/ENV/PRODUCTION/"), "*", SearchOption.AllDirectories)
				   where File.Exists(Path.Combine(item, "WORLD/REDS.BIN")) // arbitrary path to see if it's a level
				   select item;
		}

		private static Dictionary<string, XElement> cachedLevelReports = [];

		private static XElement GetLevelReport(GameRootContext gameRootContext, string levelPath)
		{
			if (DisableMemoization || !cachedLevelReports.TryGetValue(levelPath, out XElement? levelReport))
			{
				levelReport = LevelReport.GetLevelReport(gameRootContext, levelPath);

				if (!DisableMemoization)
					cachedLevelReports.TryAdd(levelPath, levelReport);
			}

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);

			return levelReport;
		}

		private static XElement GetLevelReports(GameRootContext gameRootContext, string gameRoot)
		{
			return new XElement("LevelReports",
					from level in GetLevels(gameRoot).AsParallel().WithDegreeOfParallelism(LevelParallelism)
					where LevelFilter.Count == 0 || LevelFilter.Contains(Path.GetFileNameWithoutExtension(level))
					select GetLevelReport(gameRootContext, level));
		}

		public static void GenerateLevelReports(GameRootContext gameRootContext, string gameRoot)
		{
			Directory.CreateDirectory($"Report/{ConvertPathToSafePath(gameRoot)}/LevelReports");

			GetLevels(gameRoot).AsParallel().WithDegreeOfParallelism(LevelParallelism).Where((level) => LevelFilter.Count == 0 || LevelFilter.Contains(Path.GetFileNameWithoutExtension(level))).ForAll((level) =>
			{
				{
					XElement levelReport = GetLevelReport(gameRootContext, level);
					levelReport.SetAttributeValue("Path", gameRoot);
					levelReport.SetAttributeValue("SafePath", ConvertPathToSafePath(gameRoot));

					new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"../../LevelReport.xslt\""),
						levelReport).Save($"Report/{ConvertPathToSafePath(gameRoot)}/LevelReports/{Path.GetFileNameWithoutExtension(level)}.xml");
				}

				GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
			});
		}

		private static XElement GetMiscGameReport(string gameRoot)
		{
			// PLACEHOLDER
			return new XElement("Misc");
		}

		private static Dictionary<string, XElement> cachedGameReports = [];

		public static bool DisableMemoization { get; internal set; } = true;
		public static ISet<string> LevelFilter { get; internal set; } = new HashSet<string>();
		public static int GameRootParallelism { get; set; } = 1;
		public static int LevelParallelism { get; set; } = 1;

		private static XElement GetGameReport(GameRootContext gameRootContext, string gameRoot)
		{
			if (DisableMemoization || !cachedGameReports.TryGetValue(gameRoot, out XElement? gameReport))
			{
				gameReport = new XElement("GameReport",
				new XAttribute("Path", gameRoot),
				new XAttribute("SafePath", gameRoot),
				GetLevelReports(gameRootContext, gameRoot),
				GetMiscGameReport(gameRoot));

				if (!DisableMemoization)
					cachedGameReports.TryAdd(gameRoot, gameReport);
			}

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);

			return gameReport;
		}

		public static void GenerateGameReport(GameRootContext gameRootContext, string gameRoot)
		{
			Directory.CreateDirectory($"Report/{ConvertPathToSafePath(gameRoot)}");

			{
				new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"../GameReport.xslt\""),
						GetGameReport(gameRootContext, gameRoot)).Save($"Report/{ConvertPathToSafePath(gameRoot)}/GameReport.xml");
			}

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
		}

		public static void GenerateReport(IList<string> gameRoots)
		{
			//Console.WriteLine($"Processing {gameRoots}");

			/*List<XElement> LevelReports = [];
			LevelReports.AddRange(from item in Directory.EnumerateDirectories(Path.Combine(gameRoots, "DATA/ENV/PRODUCTION/"))
								  select LevelReport.GetLevelReport(item));*/

			{
				new XDocument(new XProcessingInstruction("xml-stylesheet", "type=\"text/xml\" href=\"MasterReport.xslt\""),
					new XElement("GameReports",
						from gameRoot in gameRoots.AsParallel().WithDegreeOfParallelism(GameRootParallelism)
						select GetGameReport(new GameRootContext(gameRoot), gameRoot))).Save($"Report/MasterReport.xml");
			}

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
		}
	}
}
