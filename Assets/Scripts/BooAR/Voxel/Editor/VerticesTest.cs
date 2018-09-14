using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BooAR.Voxel.Editor
{
	public class VerticesTest
	{
		[Test]
		public void VerticesTestSimplePasses()
		{
			Mesh mesh = new Mesh();
			List<Vector3> vertices = new List<Vector3>();
			mesh.SetVertices(vertices);
			
			Assert.AreEqual(mesh.vertexCount, 0);

			vertices.Add(new Vector3());
			
			Assert.AreEqual(mesh.vertexCount, 0);
		}
	}
}