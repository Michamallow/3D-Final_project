#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Xml;
using System;


public class ScrollViewPopulator : MonoBehaviour
{
    public GameObject imagePrefab; // Le prefab de votre Image
    public GameObject textPrefab; // Le prefab de votre Text
    public Transform content; // Le Transform du contenu de votre ScrollView
    
    private int numberOfFlags; // Nombre de drapeaux � s�lectionner
    private bool showFlagNames; // Nouveau bouton pour activer/d�sactiver le texte
    private Image highlightedImage; // Image actuellement en surbrillance
    private List<Sprite> flags = new List<Sprite>(); // Liste des sprites charg�s depuis le dossier
    private List<int> usedIndices = new List<int>(); // Liste des indices d�j� utilis�s
    private List<Text> textComponents = new List<Text>(); // Liste des composants de texte cr��s
    private Dictionary<string, string> countryNames = new Dictionary<string, string>(); // Dictionnaire pour stocker les noms des pays

    void Start()
    {
        // Retrieve options from PlayerPrefs
        numberOfFlags = PlayerPrefs.GetInt("NumberOfFlags", 10);
        showFlagNames = PlayerPrefs.GetInt("ShowFlagNames", 0) == 1;

        Debug.Log(showFlagNames);

        // Charger tous les sprites depuis le dossier spécifié
        LoadSpritesFromFolder("Assets/myAssets/Flags");

        // Charger les noms des pays depuis le fichier XML
        LoadCountryNamesFromXML("Assets/myAssets/countries.xml");

        float xOffset = 100f; // Position initiale en x
        float totalHeight = 80f; // Hauteur totale mise à jour

        for (int i = 0; i < numberOfFlags; i++)
        {
            int randomIndex = GetUniqueRandomIndex();

            GameObject imageGO = Instantiate(imagePrefab, content);
            Image imageComponent = imageGO.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = flags[randomIndex];

                // Ajouter un gestionnaire d'événements de clic à l'image
                EventTrigger trigger = imageGO.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { OnImageClicked(imageComponent); });
                trigger.triggers.Add(entry);

                // Ajuster la position en x
                RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(xOffset, -totalHeight);

                // Définir l'ancrage sur le coin supérieur gauche
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);

                // Créer un objet de texte pour afficher le nom de l'image au-dessus de l'image
                GameObject textGO = Instantiate(textPrefab, content);
                Text textComponent = textGO.GetComponent<Text>();
                
                DragAndDrop dragAndDropScript = imageGO.AddComponent<DragAndDrop>();
                imageGO.name = flags[randomIndex].name;

                if (textComponent != null)
                {
                    // Utiliser la fonction GetCountryName pour obtenir le nom du pays
                    string countryName = GetCountryName(flags[randomIndex].name.ToUpper());
                    textComponent.text = countryName;

                    textComponent.rectTransform.anchoredPosition = new Vector2(textComponent.rectTransform.anchoredPosition.x, - totalHeight - 80); // Ajuster la position du texte au-dessus de l'image

                    // Ajouter le composant de texte à la liste
                    textComponents.Add(textComponent);
                }

                // Mettre à jour la position pour le prochain drapeau
                totalHeight += rectTransform.rect.height + 30; // Utiliser la hauteur du RectTransform comme décalage
            }
        }

        // Ajuster la hauteur du content en fonction de la hauteur totale
        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);

        // Désactiver le défilement vers le bas
        ScrollRect scrollRect = content.parent.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
        }

        ToggleTextVisibility(showFlagNames);
    }

    // M�thode pour charger tous les sprites depuis un dossier
    void LoadSpritesFromFolder(string folderPath)
    {
#if UNITY_EDITOR
        string[] spritePaths = AssetDatabase.FindAssets("t:Sprite", new string[] { folderPath });

        foreach (string spritePath in spritePaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(spritePath);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite != null)
            {
                flags.Add(sprite);
            }
        }
#endif
    }

    // M�thode pour obtenir un indice al�atoire non utilis�
    int GetUniqueRandomIndex()
    {
        int maxIndex = flags.Count;

        if (maxIndex == 0 || usedIndices.Count == maxIndex)
        {
            Debug.LogError("No more unique indices available.");
            return -1;
        }

        int randomIndex;
        do
        {
            randomIndex = UnityEngine.Random.Range(0, maxIndex);
        } while (usedIndices.Contains(randomIndex));

        usedIndices.Add(randomIndex);
        return randomIndex;
    }

    // M�thode appel�e lorsqu'une image est cliqu�e
    void OnImageClicked(Image clickedImage)
    {
        // R�initialiser la couleur de l'image pr�c�demment en surbrillance (si elle existe)
        if (highlightedImage != null)
        {
            highlightedImage.color = Color.white; // Remettez la couleur � celle d'origine (ou une autre couleur de votre choix)
        }

        // Mettre en surbrillance la nouvelle image
        clickedImage.color = Color.black;

        // Mettez � jour l'image actuellement en surbrillance
        highlightedImage = clickedImage;
    }

    void LoadCountryNamesFromXML(string xmlFilePath)
    {
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFilePath);

            XmlNodeList countryNodes = xmlDoc.SelectNodes("/countries/country");

            foreach (XmlNode countryNode in countryNodes)
            {
                string tag = countryNode.Attributes["tag"].Value;
                string name_fr = countryNode.Attributes["name_fr"].Value;

                countryNames[tag] = name_fr;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading country names from XML: " + e.Message);
        }
    }


    public void ToggleTextVisibility(bool isVisible)
    {
        foreach (Text textComponent in textComponents)
        {
            textComponent.enabled = isVisible; // Set the visibility based on the provided argument
        }
    }


    string GetCountryName(string tag)
    {
        if (countryNames.TryGetValue(tag, out string name))
        {
            return name;
        }
        else
        {
            Debug.LogError("Country name not found for tag: " + tag);
            return "Unknown";
        }
    }
}
