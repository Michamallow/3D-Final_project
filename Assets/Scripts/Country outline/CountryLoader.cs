using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CountryLoader : MonoBehaviour
{
	public bool autoRunOnAwake = true;
	public TextAsset countryFile;
	public TextAsset cityFile;
	public TextAsset capitalsFile;

	[SerializeField] Country[] countries;
	bool loaded;

	[ContextMenu("Run")]
	void Awake()
	{
		if (autoRunOnAwake)
		{
			Load();
		}
	}

	public Country[] GetCountries()
	{
		Load();
		return countries;
	}

	public int NumCountries
	{
		get
		{
			Load();
			return countries.Length;
		}
	}

	public void Load()
	{
		if (!loaded || !Application.isPlaying)
		{
			if (countryFile != null)
			{
				CountryReader countryReader = new CountryReader();
				countries = countryReader.ReadCountries(countryFile);
			}

			if (cityFile != null)
			{
				CityReader cityReader = new CityReader();
				City[] allCities = cityReader.ReadCities(cityFile, capitalsFile);
				AddCitiesToCountries(allCities);
			}
			loaded = true;
		}
	}
	
	/*
	[ContextMenu("Create Country Info Template")]
	void CreateCountryInfoJsonTemplate()
	{
		Load();

		AllCountryInfo allCountriesInfo = new AllCountryInfo();
		allCountriesInfo.countryInfo = new CountryInfo[countries.Length];
		for (int i = 0; i < countries.Length; i++)
		{
			CountryInfo info = new CountryInfo() { countryName = countries[i].name, countryCode = countries[i].alpha3Code };
			allCountriesInfo.countryInfo[i] = info;
		}

		string jsonText = JsonUtility.ToJson(allCountriesInfo, prettyPrint: true);

		System.IO.StreamWriter writer = new System.IO.StreamWriter("./Assets/CountryInfoTemplate.json");
		writer.Write(jsonText);
		Debug.Log(jsonText);
		writer.Dispose();
		Debug.Log("Template file saved to Assets folder");
	}
	*/
}
