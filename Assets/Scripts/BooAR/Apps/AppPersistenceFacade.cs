using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BooAR.ARs;
using BooAR.Games;
using BooAR.Voxel;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	/* Access point to disk serialization of whatever in the app
	 * 
	 * 
	 */
	public class AppPersistenceFacade : IGameList, IAppPersistence
	{
#pragma warning disable 649
		[Inject]
		IVoxelPersistence _voxel;

		[Inject]
		IArWorldPersistence _ar;

		[Inject]
		IGamePersistence _game;
#pragma warning restore 649

		string RootPath => Path.Combine(Application.persistentDataPath, "data");

		public async UniTask SaveAll(string id)
		{
			try
			{
				string path = GetPath(id);
				Directory.CreateDirectory(path); // ensure directory exists

				_voxel.Save(path);
				_game.Save(path);
				await _ar.Save(path);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
			finally
			{
				Debug.Log($"AppPersistenceFacade.SaveAll({id})");
			}
		}

		public async UniTask LoadAll(string id)
		{
			try
			{
				string path = GetPath(id);
				Debug.Assert(Directory.Exists(path));

				_voxel.Load(path);
				_game.Load(path);
				await _ar.Load(path);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				throw;
			}
			finally
			{
				Debug.Log($"AppPersistenceFacade.LoadAll({id})");
			}
		}

		public IEnumerable<string> GetGameIDs()
		{
			if (!Directory.Exists(RootPath)) // root doesnt exist in disk
			{
				Debug.LogWarning("Root directory not found");
				return Enumerable.Empty<string>();
			}

			return Directory
			       .EnumerateDirectories(RootPath)
			       .Select(f => Path.GetFileName(f));
		}

		string GetPath(string id)
		{
			return Path.Combine(RootPath, id);
		}
	}
}