using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BooAR.Voxel
{
	[Serializable]
	public class VoxelMeshSource
	{
		[SerializeField]
		Material[] _materials;

		public Material[] BlockMaterials => _materials;
	}
}