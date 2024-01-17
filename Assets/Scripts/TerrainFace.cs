using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointComparer : IComparer<Vector3>
{
    private Vector3 referencePoint;

    public PointComparer(Vector3 reference)
    {
        referencePoint = reference;
    }

    public int Compare(Vector3 a, Vector3 b)
    {
        float angleA = Mathf.Atan2(a.y - referencePoint.y, a.x - referencePoint.x);
        float angleB = Mathf.Atan2(b.y - referencePoint.y, b.x - referencePoint.x);

        if (angleA < angleB)
            return -1;
        if (angleA > angleB)
            return 1;
        return 0;
    }
}

public class TerrainFace : MonoBehaviour
{
	Mesh mesh;
    int resolution;
    List<Vector2> coords;
    float radius;
    List<Vector3> vertices;

	public TerrainFace(Mesh mesh, int resolution, List<Vector2> coords, float radius)
	{
		this.mesh = mesh;
		this.resolution = resolution;
		this.coords = coords;
		this.radius = radius;

		//axisA = new Vector3(coords.y, coords.z, coords.x);
		//axisB = Vector3.Cross(coords, axisA);
	}

	public void ConstructMesh()
	{
		// Convertir les coordonnées du plan 2D à des coordonnées sphériques
        List<Vector3> planeVertices = ConvertToPlaneCoordinates(coords);

        // Trouver un point de référence (par exemple, le centroïde)
        Vector3 referencePoint = FindReferencePoint(planeVertices);

        // Trier les vertices en fonction de l'angle polaire
        planeVertices.Sort(new PointComparer(referencePoint));

        // Trianguler les vertices pour créer des faces
        List<int> triangles = Triangulate(planeVertices.Count);

        // Assigner les vertices et les triangles au mesh
        mesh.Clear();
        mesh.vertices = planeVertices.ToArray();
        mesh.triangles = triangles.ToArray();
		//mesh.RecalculateNormals();
	}

	

	void OnDrawGizmos()
    {
        if (vertices != null)
        {
            Gizmos.color = Color.red;
            foreach (var vertex in vertices)
            {
                Gizmos.DrawSphere(vertex, 0.1f);
            }
        }
    }

	private Vector3 FindReferencePoint(List<Vector3> points)
	{
		// You can choose any point as a reference. Here, we use the centroid.
		float sumX = 0f;
		float sumY = 0f;
		float sumZ = 0f;

		foreach (Vector3 point in points)
		{
			sumX += point.x;
			sumY += point.y;
			sumZ += point.z;
		}

		float centerX = sumX / points.Count;
		float centerY = sumY / points.Count;
		float centerZ = sumZ / points.Count;

		return new Vector3(centerX, centerY, centerZ);
	}

	// Triangulation function for a convex polygon
	List<int> Triangulate(int vertexCount)
    {
        List<int> triangles = new List<int>();

        for (int i = 1; i < vertexCount - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        return triangles;
    }

	private List<Vector3> ConvertToPlaneCoordinates(List<Vector2> coords)
    {
        List<Vector3> planeVertices = new List<Vector3>();

        foreach (var coord in coords)
        {
            // Les coordonnées du plan 2D restent inchangées
            Vector3 planeVertex = new Vector3(coord.x, 0f, coord.y);
            planeVertices.Add(planeVertex);
        }

        return planeVertices;
    }

	private List<Vector3> ConvertToSphereCoordinates(List<Vector2> coords)
	{
		List<Vector3> sphereVertices = new List<Vector3>();

		foreach (var coord in coords)
		{
			// Convertir les coordonnées du plan 2D à des coordonnées sphériques
			Coordinate c = new Coordinate(coord.x * Mathf.Deg2Rad, coord.y * Mathf.Deg2Rad);
			Vector3 sphereVertex = SphericalToCartesian(c, radius);
			sphereVertices.Add(sphereVertex);

			//Debug.Log("Transformed Coord: " + sphereVertex);
		}

		return sphereVertices;
	}

	private Vector3 SphericalToCartesian(Coordinate sphericalCoord, float radius)
	{
		// Les coordonnées sphériques définissent un point sur une sphère en termes de longitude et de latitude
		float x = radius * Mathf.Sin(sphericalCoord.latitude) * Mathf.Cos(sphericalCoord.longitude);
		float z = radius * Mathf.Cos(sphericalCoord.latitude); // Utilisation de Cos pour la composante y
		float y = radius * Mathf.Sin(sphericalCoord.latitude) * Mathf.Sin(sphericalCoord.longitude);

		return new Vector3(x, y, z);
	}
}