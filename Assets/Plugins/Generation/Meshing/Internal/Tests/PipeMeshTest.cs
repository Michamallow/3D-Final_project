using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation.Meshing.Tests
{

	[ExecuteInEditMode]
	public class PipeMeshTest : MonoBehaviour
	{

		public bool closed;
		public int resolution = 4;
		public float radius = 1;

		void Update()
		{
			Vector3[] points = Generation.Helpers.TransformHelper.GetAllChildPositions(transform);
			MeshFilter filter = GetComponent<MeshFilter>();

			SimpleMeshData meshData = PipeMeshGenerator.GenerateMesh(points, closed, radius: radius, resolution: resolution);
			Mesh mesh = filter.sharedMesh;
			MeshHelper.CreateMesh(ref mesh, meshData, true);

			//PipeMeshGenerator.CalculateAxes(points, closed, correctClosedNormals);
		}
	}
}