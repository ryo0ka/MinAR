using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;

namespace BooAR.Voxel
{
	[Serializable]
	public class ChunksSerializer
	{
		[SerializeField]
		string _rootPath;

		[SerializeField]
		string _extension;

		public void Serialize(string id, IDictionary<Vector3i, Chunk> chunks)
		{
			string dirPath = GetDirPath(id);
			Directory.Delete(dirPath, true);
			Directory.CreateDirectory(dirPath);

			foreach ((Vector3i p, Chunk c) in chunks.ToTuples())
			{
				string filePath = GetFilePath(id, p);
				using (FileStream o = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
				{
					c.Save(o);
				}
			}
		}

		public void Deserialize(string id, IDictionary<Vector3i, Chunk> chunks, Func<Vector3i, Chunk> init)
		{
			string dirPath = GetDirPath(id);
			Directory.CreateDirectory(dirPath);

			foreach (string filePath in Directory.EnumerateFiles(dirPath, $"*.{_extension}"))
			{
				string[] ps = Path.GetFileName(filePath).Split('.');
				Vector3i p = (int.Parse(ps[0]), int.Parse(ps[1]), int.Parse(ps[2]));
				Chunk chunk = chunks.GetOrAddValue(p, init);
				using (FileStream o = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					chunk.Load(o);
				}
			}
		}

		string GetFilePath(string id, Vector3i p)
		{
			string fileName = string.Join(".", p.x, p.y, p.z, _extension);
			return Path.Combine(GetDirPath(id), fileName);
		}

		string GetDirPath(string id)
		{
			return Path.Combine(UnityIOBridge.persistentDataPath, _rootPath, id);
		}
	}
}