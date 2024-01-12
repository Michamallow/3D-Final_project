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

	void FixedUpdate()
	{
		HighlightLookedAtFace();
	}

	private void OnValidate()
	{
		Initialize();
		GenerateMesh();
	}

	void Initialize()
	{
		this.tag = "Planet";

		if (shapeSettings == null)
		{
			shapeSettings = Resources.Load<ShapeSettings>("Settings/Shape");

			if (shapeSettings == null)
			{
				Debug.LogError("ShapeSettings not found. Make sure it exists at 'Assets/Resources/Settings/Shape'");
			}
		}

		if (camObj == null)
		{
			camObj = Camera.main.gameObject;
		}

		// Parse countries.xml
		var xmlDocument = XDocument.Load("Assets/countries.xml");
		var xmlCountries = xmlDocument.Descendants("country").Select(x => x.Attribute("name_en").Value).ToList();

		// Parse Countries (medium res).txt
		var jsonText = File.ReadAllText("Assets/myAssets/Countries (medium res).txt");
		var jsonDocument = JObject.Parse(jsonText);
		var jsonCountries = jsonDocument["features"].Children().ToList();

		Dictionary<string, List<Vector2>> countries = new Dictionary<string, List<Vector2>>();
		foreach (var xmlCountry in xmlCountries)
		{
			var jsonCountry = jsonCountries.FirstOrDefault(jc => jc["properties"]["NAME"].Value<string>() == xmlCountry);
			if (jsonCountry != null)
			{
				if (jsonCountry["geometry"]["type"].Value<string>() == "MultiPolygon")
				{
					var coordinates = jsonCountry["geometry"]["coordinates"]
					.Select(polygon => polygon.Children() // Get the second level (array of coordinates)
						.Children() // Get the first level (array of polygons)
						.Select(coordinate => coordinate.Values<float>().ToList())
						.ToList())
					.ToList();

					var numPolygon = 0;
					foreach (var polygon in coordinates)
					{
						numPolygon++;
						List<Vector2> coordTuple = polygon.Select(coord => new Vector2(coord[0], coord[1])).ToList();
						countries.Add(xmlCountry + numPolygon.ToString(), coordTuple);
					}
				}
				else
				{
					var coordinates = jsonCountry["geometry"]["coordinates"]
					.Children() // Get the first level (array of polygons)
					.Children() // Get the second level (array of coordinates)
					.Select(coordinate => coordinate.Values<float>().ToList())
					.ToList();

					List<Vector2> coordTuple = coordinates.Select(coord => new Vector2(coord[0], coord[1])).ToList();
					countries.Add(xmlCountry, coordTuple);
				}
			}
		}

		if (meshFilters == null || meshFilters.Length == 0)
		{
			meshFilters = new MeshFilter[countries.Count]; 
		}
		terrainFaces = new TerrainFace[countries.Count];

		string countryName;
		int i = 0;
		foreach(KeyValuePair<string, List<Vector2>> country in countries){
			countryName = country.Key;
			// TODO : How to put MultiPolygon as a single country?

			if (meshFilters[i] == null){
				GameObject meshObj = new GameObject(countryName);
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

			terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, resolution, country.Value, shapeSettings.planetRadius); 
			i++;
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
		for (int i = 0; i < 6; i++)
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
		}
		else
		{
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
		if (hitTransform != null)
		{
			hitTransform.GetComponent<MeshRenderer>().material = highlightMaterial;
		}
	}
}
