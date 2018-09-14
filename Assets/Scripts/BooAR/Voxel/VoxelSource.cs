using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BooAR.Voxel
{
	[Serializable]
	public class VoxelSource
	{
		[SerializeField]
		Material[] _materials;

		[SerializeField]
		Material _damageMaterial;

		public Material[] BlockMaterials => _materials;
		public Material DamageMaterial => _damageMaterial;
	}
}