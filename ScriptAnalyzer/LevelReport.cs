#define CATHODE_FAIL_HARD

using CATHODE;
using CATHODE.Scripting;
using CathodeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ScriptAnalyzer
{
	internal static class LevelReport
	{
		private static IEnumerable<XElement> GetCommandEntryLinks(CATHODE.Commands commands, CATHODE.Scripting.Composite entry, CATHODE.Scripting.Internal.Entity entity)
		{
			return (from link in entity.childLinks
					select new XElement("OutLink",
						new XAttribute("SourceParam", link.thisParamID),
						new XAttribute("TargetID", link.linkedEntityID.ToByteString()),
						new XAttribute("TargetName", commands.Utils.GetEntityName(entry.shortGUID, link.linkedEntityID)),
						new XAttribute("TargetParam", link.linkedParamID)
						)).Concat(
					from entryEntity in entry.GetEntities()
					from entityLink in entryEntity.childLinks
					where entityLink.linkedEntityID == entity.shortGUID
					select new XElement("InLink",
						new XAttribute("SourceParam", entityLink.thisParamID),
						new XAttribute("SourceName", commands.Utils.GetEntityName(entry.shortGUID, entryEntity.shortGUID)),
						new XAttribute("SourceID", entryEntity.shortGUID.ToByteString()),
						new XAttribute("TargetParam", entityLink.linkedParamID)
						)).Concat(
					from option in entity.parameters
					select new XElement("Option",
						new XAttribute("Name", option.name),
						new XAttribute("Type", option.content.dataType.ToString()),
						option.content.dataType != DataType.RESOURCE ?
							(option.content.ToString() != null ?
								new XAttribute("Content", option.content.ToString()) :
								null) :
								from resourceReference in (( cResource ) option.content).value
									select new XElement("Resource",
										new XAttribute("ID", resourceReference.resource_id),
										new XAttribute("Type", resourceReference.resource_id.ToString())),
						new XAttribute("Variant", option.variant.ToString())
						));
		}

		private static XElement GetCommandEntryXElement(CATHODE.Commands commands, CATHODE.Scripting.Composite entry)
		{
			//CATHODE.Scripting.EntityVariant.

			if (Report.EnableSpamLogs)
				Console.WriteLine($"Processing composite {entry.name} ({entry.shortGUID.ToByteString()})");

			return new XElement("Composite",
				new XAttribute("Name", entry.name),
				new XAttribute("GUID", entry.shortGUID.ToString()),

				commands.EntryPoints.Contains(entry) ? new XAttribute("Entrypoint", "") : null,

				from function in entry.functions
				orderby commands.Utils.GetEntityName(entry, function)
				select commands.GetComposite(function.function) == null ? new XElement("Function",
					new XAttribute("Name", commands.Utils.GetEntityName(entry, function)),
					new XAttribute("GUID", function.shortGUID.ToByteString()),
					new XAttribute("Type", function.function.ToString()),
					GetCommandEntryLinks(commands, entry, function)
				) : new XElement("Instance",
					new XAttribute("Name", commands.Utils.GetEntityName(entry, function)),
					new XAttribute("GUID", function.shortGUID.ToByteString()),
					new XAttribute("CompositeName", commands.GetComposite(function.function).name),
					new XAttribute("CompositeGUID", function.function.ToByteString()),
					GetCommandEntryLinks(commands, entry, function)
				),

				from variable in entry.variables
				orderby variable.name
				select new XElement("Variable",
					new XAttribute("Name", variable.name),
					new XAttribute("GUID", variable.shortGUID.ToString()),
					new XAttribute("Type", variable.type.ToString()),
					GetCommandEntryLinks(commands, entry, variable)
				),

				from variable in entry.variables
				from parameter in variable.parameters
				orderby parameter.name
				select new XElement("Parameter",
					new XAttribute("Name", parameter.name),
					new XAttribute("Type", parameter.content.dataType.ToString()),
					parameter.content.ToString() != null ? new XAttribute("Content", parameter.content.ToString()) : null,
					new XAttribute("Variant", parameter.variant.ToString())
				)
				);
				/*from function in entry.functions
				where commands.GetComposite(function.function) == null
				select 
				);*/
				//new XAttribute("Type", entry.),
				//new XAttribute("Parameters", string.Join(", ", entry.Parameters)),
				//new XAttribute("Description", entry.Description ?? "No description available"));
		}

		private static XElement GetLevelScriptingReport(LevelContext levelContext)
		{
			XElement result;

			levelContext.CommandsTask.Wait();

			var commands = levelContext.CommandsTask.Result;

			result = new XElement("Scripting",
				from entrypoint in commands.EntryPoints
				orderby entrypoint.name
				select new XElement("Entrypoint", new XAttribute("Name", entrypoint.name), entrypoint.shortGUID.ToByteString()),

				from commandEntry in commands.Entries
				orderby commandEntry.name
				select GetCommandEntryXElement(commands, commandEntry)
				);

			return result;
			//commands.Entries;
		}

		public static XElement GetLevelReport(GameRootContext gameRootContext, string levelPath)
		{
			Console.WriteLine($"Processing level {levelPath}...");

#pragma warning disable CS8604 // Possible null reference argument.
			XElement levelReportElement = new("LevelReport",
				new XAttribute("LevelName", Path.GetFileNameWithoutExtension(levelPath)),
				GetLevelScriptingReport(new LevelContext(gameRootContext, levelPath))
			);
#pragma warning disable CS8604 // Possible null reference argument.

			Console.WriteLine($"Processed level {levelPath}...");

			return levelReportElement;
		}
	}
}
