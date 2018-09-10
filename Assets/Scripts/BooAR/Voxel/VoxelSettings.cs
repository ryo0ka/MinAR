using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Utils;

namespace BooAR.Voxel
{
	public class VoxelSettings : BaseScriptableObject, ITerrainGenerator, IBlockAttributeTable
	{
		[Serializable]
		struct Param
		{
#pragma warning disable 649
			[SerializeField]
			public string Key;

			[SerializeField]
			public float Scale;

			[SerializeField]
			public int Octave;

			[SerializeField]
			public float Threshold;

			[SerializeField]
			public Visibilities Visibility;

			[SerializeField]
			public int Durability;
#pragma warning restore 649
		}

		// need a key for each block type
		[SerializeField]
		List<Param> _params;

		Perlin[] _perlins;

		public void Initialize()
		{
			UpdatePerlins();
		}

		void UpdatePerlins()
		{
			_perlins = new Perlin[BlocksUtils.All.Length];

			for (int i = 0; i < BlocksUtils.All.Length; i++)
			{
				string key = _params[i].Key;
				_perlins[i] = new Perlin(GeneratePerm(key));
			}
		}

		int[] GeneratePerm(string key)
		{
			byte[] c = new byte[1];
			int[] ps = new int[512];
			for (int i = 0; i < 64; i++)
			{
				using (SHA512 sha256hash = SHA512.Create())
				{
					c[0] = (byte) key[i % key.Length];
					byte[] cs = sha256hash.ComputeHash(c);

					for (int j = 0; j < 64; j++)
					{
						int p = cs[j];
						for (int k = 0; k < 512 / 64; k++)
						{
							ps[j + k * 64] = p;
						}
					}
				}
			}

			return ps;
		}

		public Blocks GenerateBlock(Vector3i position)
		{
			using (UnityUtils.Sample("TerrainGenerator.Ask()"))
			{
				return AskBlock(position, Blocks.Iron)
				       ?? AskBlock(position, Blocks.Coal)
				       ?? Blocks.Stone;
			}
		}

		Blocks? AskBlock(Vector3i position, Blocks block)
		{
			Param p = _params[(int) block];
			float x = MathUtils.Map(position.x, -p.Scale, p.Scale, -1f, 1f);
			float y = MathUtils.Map(position.y, -p.Scale, p.Scale, -1f, 1f);
			float z = MathUtils.Map(position.z, -p.Scale, p.Scale, -1f, 1f);
			float n = _perlins[(int) block].Fbm(new Vector3(x, y, z), p.Octave);

			return (n > p.Threshold) ? block : (Blocks?) null;
		}

		public Visibilities GetVisibility(Blocks block)
		{
			return _params[(int) block].Visibility;
		}

		public int GetDurability(Blocks block)
		{
			return _params[(int) block].Durability;
		}
	}
}