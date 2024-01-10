using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

public class Planet : MonoBehaviour
{
	[Range(2, 256)]
	public int resolution = 10;
	public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
	public FaceRenderMask faceRenderMask;

	public ShapeSettings shapeSettings;

	[SerializeField]
	GameObject camObj; // The camera that will be used to determine which face is being looked at

	[SerializeField, HideInInspector]
	MeshFilter[] meshFilters;
	TerrainFace[] terrainFaces;
	private Material originalMaterial;
    private Material highlightMaterial;
	
	
	void Start()
    {
        InitializeMaterials();
    }

	void FixedUpdate() {
        HighlightLookedAtFace();
    }

    private void OnValidate() {
		Initialize();
		GenerateMesh();

		// Parse countries.xml
		var xmlDocument = XDocument.Load("Assets/countries.xml");
		var xmlCountries = xmlDocument.Descendants("country").Select(x => x.Attribute("name_en").Value).ToList(); 
		//print(xmlCountries.Count); //253

		// Parse Countries (medium res).txt
		var jsonText = File.ReadAllText("Assets/myAssets/Countries (medium res).txt");
		var jsonDocument = JObject.Parse(jsonText);
		var jsonCountries = jsonDocument["features"].Children().ToList();
		//print(jsonCountries.Count); //241

		var i=0;
		foreach (var xmlCountry in xmlCountries)
		{
			i++;
			var jsonCountry = jsonCountries.FirstOrDefault(jc => jc["properties"]["NAME"].Value<string>() == xmlCountry);
			if (jsonCountry != null)
			{
				print(i + " " + xmlCountry); 
				
				if(jsonCountry["geometry"]["type"].Value<string>() == "MultiPolygon")
				{
					print("MultiPolygon");
					var coordinates = jsonCountry["geometry"]["coordinates"]
					.Select(polygon => polygon.Children() // Get the second level (array of coordinates)
						.Children() // Get the first level (array of polygons)
						.Select(coordinate => coordinate.Values<double>().ToList())
						.ToList())
    				.ToList();

					/*foreach (var polygon in coordinates)
					{
						print("New Polygon:" + polygon.Count); 
						foreach (var coordinate in polygon) 
						{
							print($"Longitude: {coordinate[0]}, Latitude: {coordinate[1]}");
						}
					}*/
				}else{
					print("Polygon");
					var coordinates = jsonCountry["geometry"]["coordinates"]
					.Children() // Get the first level (array of polygons)
					.Children() // Get the second level (array of coordinates)
					.Select(coordinate => coordinate.Values<double>().ToList())
    				.ToList();

					/*foreach (var coordinate in coordinates)
					{
						print($"Longitude: {coordinate[0]}, Latitude: {coordinate[1]}");
					}*/
				}
			} 
		}
	}

	void Initialize()
	{
		this.tag = "Planet";

		if(shapeSettings == null)
		{
			shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape");

			if (shapeSettings == null)
            {
                Debug.LogError("ShapeSettings not found. Make sure it exists at 'Assets/Resources/Settings/Shape'");
            }
		}

		if(camObj == null)
		{
			camObj = Camera.main.gameObject;
		}
		
		if (meshFilters == null || meshFilters.Length == 0)
		{
			meshFilters = new MeshFilter[6];
		}
		terrainFaces = new TerrainFace[6];

		Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

		for (int i = 0; i < 6; i++)
		{
			if (meshFilters[i] == null)
			{
				GameObject meshObj = new GameObject("mesh");
				meshObj.transform.parent = transform;

				// Add MeshRenderer
				MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = Resources.Load<Material>("Earth");

				// Add MeshFilter
				meshFilters[i] = meshObj.AddComponent<MeshFilter>();
				meshFilters[i].sharedMesh = new Mesh();

				// Add MeshCollider
				MeshCollider meshCollider = meshObj.AddComponent<MeshCollider>();
				meshCollider.convex = true;
				meshCollider.isTrigger = true;
				meshCollider.providesContacts = true;
			}

			terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, directions[i], shapeSettings.planetRadius);
			bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
			meshFilters[i].gameObject.SetActive(renderFace);
		}
	}

	private void InitializeMaterials()
    {
        originalMaterial = Resources.Load<Material>("Earth");
        highlightMaterial = new Material(Shader.Find("Standard"));
        highlightMaterial.color = Color.yellow;
    }

	public void OnShapeSettingsUpdated()
	{
		Initialize();
		GenerateMesh();
	}

	void GenerateMesh()
	{
		for(int i = 0; i < 6; i++)
		{
			if (meshFilters[i].gameObject.activeSelf)
			{
				terrainFaces[i].ConstructMesh();
			}
		}
	}


	private void HighlightLookedAtFace()
	{
		if (camObj == null)
		{
			Debug.LogError("Camera is not assigned in the inspector.");
			return;
		}

		Camera cam = camObj.GetComponent<Camera>();
		if (cam == null)
		{
			Debug.LogError("The assigned camera GameObject does not have a Camera component.");
			return;
		}

		Ray ray = new Ray(cam.transform.position, cam.transform.forward);
		
		// Draw the ray in the Scene view
		Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform.CompareTag("Planet") || hit.transform.parent.CompareTag("Planet"))
			{
				// Highlight the face by passing the hit object's transform
				HighlightFace(hit.transform);
			}
		}else{
			// Reset all materials to originalMaterial
			HighlightFace(null);
		}
	}

    private void HighlightFace(Transform hitTransform)
    {
        // Reset all materials to originalMaterial
        foreach (var meshFilter in meshFilters)
        {
            meshFilter.GetComponent<MeshRenderer>().material = originalMaterial;
        }

		// Highlight the specific face
		if (hitTransform != null){
			hitTransform.GetComponent<MeshRenderer>().material = highlightMaterial;
		}
    }
}
