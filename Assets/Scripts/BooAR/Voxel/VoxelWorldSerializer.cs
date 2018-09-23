using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Utils;

namespace BooAR.Voxel
{
	public class VoxelWorldSerializer
	{
		const string Extension = "chunk";

		public void Serialize(string dirPath, IDictionary<Vector3i, Chunk> chunks)
		{
			Directory.Delete(dirPath, true);
			Directory.CreateDirectory(dirPath);

			foreach ((Vector3i p, Chunk c) in chunks.ToTuples())
			{
				string filePath = GetFilePath(dirPath, p);
				using (FileStream o = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
				{
					c.Save(o);
				}
			}
		}

		public void Deserialize(string dirPath, IDictionary<Vector3i, Chunk> chunks, Func<Vector3i, Chunk> init)
		{
			IEnumerable<string> filePaths = Directory.EnumerateFiles(dirPath, $"*.{Extension}");
			if (!filePaths.Any()) // no chunks found
			{
				Debug.LogWarning("No chunks found in the disk");
				return;
			}
			
			foreach (string filePath in filePaths)
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

		string GetFilePath(string dirPath, Vector3i p)
		{
			string fileName = string.Join(".", p.x, p.y, p.z, Extension);
			return Path.Combine(dirPath, fileName);
		}
	}
}