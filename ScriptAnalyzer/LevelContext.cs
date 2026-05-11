using CATHODE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptAnalyzer
{
	internal class LevelContext
	{
		public readonly GameRootContext gameRootContext;

		public readonly Task<Textures> TexturesTask;
		public readonly Task<Shaders> ShadersTask;
		public readonly Task<Materials> MaterialsTask;

		public readonly Task<Collisions> CollisionsTask;
		public readonly Task<MorphTargets> MorphTargetDBTask;
		public readonly Task<Models> ModelsTask;
		public readonly Task<RenderableElements> RenderableElementsTask;

		public readonly Task<MaterialMappings> MaterialMappingsTask;
		public readonly Task<CollisionMaps> CollisionMapsTask;

		public readonly Task<EnvironmentAnimations> EnvironmentAnimationsTask;

		public readonly Task<CATHODE.Commands> CommandsTask;

		public LevelContext(GameRootContext gameRootContext, string levelPath)
		{
			// TODO: Handle patching here.

			/*this.gameRootContext = gameRootContext;
			TexturesTask = Task.Run(() => new Textures(Path.Combine(levelPath, "RENDERABLE/LEVEL_TEXTURES.ALL.PAK")));
			ShadersTask = Task.Run(() => new Shaders(Path.Combine(levelPath, "RENDERABLE/LEVEL_SHADERS_DX11.PAK")));
			MaterialsTask = Task.Run(async () => new Materials(Path.Combine(levelPath, "RENDERABLE/LEVEL_MODELS.MTL"), await gameRootContext.TexturesTask, await TexturesTask, await ShadersTask));

			CollisionsTask = Task.Run(() => new Collisions(Path.Combine(levelPath, "WORLD/COLLISIONS.BIN")));
			MorphTargetDBTask = Task.Run(() => new MorphTargets(Path.Combine(levelPath, "WORLD/MORPH_TARGET_DB.BIN")));
			ModelsTask = Task.Run(async () => new Models(Path.Combine(levelPath, "RENDERABLE/MODELS.PAK"), await MaterialsTask, await CollisionsTask, await MorphTargetDBTask));

			RenderableElementsTask = Task.Run(async () => new RenderableElements(Path.Combine(levelPath, "WORLD/REDS.BIN"), await ModelsTask, await MaterialsTask));

			MaterialMappingsTask = Task.Run(() => new MaterialMappings(Path.Combine(levelPath, "WORLD/MATERIAL_MAPPINGS.PAK")));

			CollisionMapsTask = Task.Run(async () => new CollisionMaps(Path.Combine(levelPath, "WORLD/COLLISION.MAP"), await MaterialsTask, await MaterialMappingsTask));

			EnvironmentAnimationsTask = Task.Run(async () => new EnvironmentAnimations(Path.Combine(levelPath, "WORLD/ENVIRONMENTMAP.BIN"), await gameRootContext.AnimationStringsDebugTask));*/

			// And finally...
			CommandsTask = Task.Run(async () =>
			{
				bool is64Bit = Path.Exists(Path.Combine(levelPath, "WORLD/COMMANDS.BIN"));

				Materials materials = new("nul", new Textures("nul"), new Textures("nul"), new Shaders("nul"));

				if (!is64Bit)
				{
					return new CATHODE.Commands(Path.Combine(levelPath, "WORLD/COMMANDS.PAK"),
						new EnvironmentAnimations("nul", new AnimationStrings("nul")),
						new CollisionMaps("nul", materials, new MaterialMappings("nul")),
						new RenderableElements("nul", new Models("nul", materials, new Collisions("nul"), new MorphTargets("nul")), materials));
				}
				else
				{					
					return new CATHODE.Commands(Path.Combine(levelPath, "WORLD/COMMANDS.BIN"),
						new EnvironmentAnimations("nul", new AnimationStrings("nul")),
						new CollisionMaps("nul", materials, new MaterialMappings("nul")),
						new RenderableElements("nul", new Models("nul", materials, new Collisions("nul"), new MorphTargets("nul")), materials));
				}
			});
		}

		~LevelContext()
		{
			/*MaterialsTask.Result.ClearReferences();
			CollisionMapsTask.Result.ClearReferences();
			ModelsTask.Result.ClearReferences();
			RenderableElementsTask.Result.ClearReferences();
			CollisionMapsTask.Result.ClearReferences();
			EnvironmentAnimationsTask.Result.ClearReferences();*/
			CommandsTask.Result.ClearReferences();
		}
	}
}
