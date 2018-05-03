using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	static class LoadSaveSystem
	{
		public static string SavesDir = "SavedGames";

		public static void Save(GameSessionData data)
		{
			EnsureSavesDir();

			var fileName = DateTime.Now.ToString("yyyy M d hh mm ss") + ".save";
			var pathToFile = Path.Combine(SavesDir, fileName);

			File.WriteAllText(pathToFile, JsonUtility.ToJson(data, true));
		}

		public static GameSessionData Load(string path)
		{
			var p = Path.Combine(SavesDir, path);
			return File.Exists(p)
				? JsonUtility.FromJson<GameSessionData>(File.ReadAllText(p))
				: null;
		}

		public static List<string> GetSavedGames()
		{
			return Directory.GetFiles(SavesDir, "*.save").Select(Path.GetFileName).ToList();
		}

		private static void EnsureSavesDir()
		{
			if (!Directory.Exists(SavesDir)) {
				Directory.CreateDirectory(SavesDir);
			}
		}
	}
}
