using CATHODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptAnalyzer
{
	internal class GameRootContext
	{
		public Task<Textures> TexturesTask { get; private set; }
		public Task<PAK2> AnimPAKTask { get; private set; }
		public Task<CATHODE.AnimationStrings> AnimationStringsTask { get; private set; }
		public Task<CATHODE.AnimationStrings> AnimationStringsDebugTask { get; private set; }

		public GameRootContext(string gameRoot)
		{
			TexturesTask = Task.Run(() => new Textures(Path.Combine(gameRoot, "DATA/ENV/GLOBAL/WORLD/GLOBAL_TEXTURES.ALL.PAK")));
			AnimPAKTask = Task.Run(() => new PAK2(Path.Combine(gameRoot, "DATA/GLOBAL/ANIMATION.PAK")));
			AnimationStringsTask = Task.Run(async () => { return new AnimationStrings((await AnimPAKTask).Entries.FirstOrDefault(o => o.Filename.Contains("ANIM_STRING_DB.BIN")).Content); });
			AnimationStringsDebugTask = Task.Run(async () => { return new AnimationStrings((await AnimPAKTask).Entries.FirstOrDefault(o => o.Filename.Contains("ANIM_STRING_DB_DEBUG.BIN")).Content); });
		}
	}
}
