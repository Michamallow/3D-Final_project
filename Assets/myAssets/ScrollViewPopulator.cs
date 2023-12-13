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
    public Transform centerImageTransform; // Transform de l'image au centre du canvas
    public int flagsToSelect; // Nombre de drapeaux à sélectionner
    public Button toggleButtonText; // Nouveau bouton pour activer/désactiver le texte


    private Image highlightedImage; // Image actuellement en surbrillance
    private List<Sprite> flags = new List<Sprite>(); // Liste des sprites chargés depuis le dossier
    private List<int> usedIndices = new List<int>(); // Liste des indices déjà utilisés
    private List<Text> textComponents = new List<Text>(); // Liste des composants de texte créés
    private Dictionary<string, string> countryNames = new Dictionary<string, string>(); // Dictionnaire pour stocker les noms des pays

    void Start()
    {
        // Charger tous les sprites depuis le dossier spécifié
        LoadSpritesFromFolder("Assets/myAssets/Flags");

        // Charger les noms des pays depuis le fichier XML
        LoadCountryNamesFromXML("Assets/myAssets/countries.xml");

        float yOffset = -50f; // Position initiale en y
        float totalWidth = 100f;

        for (int i = 0; i < flagsToSelect; i++)
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

                // Ajuster la position en y
                RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(totalWidth, yOffset);

                // Définir l'ancrage sur le coin supérieur gauche
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);

                // Créer un objet de texte pour afficher le nom de l'image au-dessus de l'image
                GameObject textGO = Instantiate(textPrefab, content);
                Text textComponent = textGO.GetComponent<Text>();

                if (textComponent != null)
                {
                    // Utilisez la fonction GetCountryName pour obtenir le nom du pays
                    string countryName = GetCountryName(flags[randomIndex].name.ToUpper());
                    textComponent.text = countryName;

                    textComponent.rectTransform.anchoredPosition = new Vector2(totalWidth, -145 + 30f); // Ajustez la position du texte au-dessus de l'image

                    textComponent.rectTransform.anchorMin = new Vector2(0, 1);
                    textComponent.rectTransform.anchorMax = new Vector2(0, 1);

                    // Ajouter le composant de texte à la liste
                    textComponents.Add(textComponent);
                }

                // Mettre à jour la position pour le prochain drapeau
                totalWidth += rectTransform.rect.width + 10; // Utilisez la largeur du RectTransform comme décalage
            }
        }

        // Ajuster la largeur du content en fonction de la largeur totale
        RectTransform contentRectTransform = content.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(totalWidth, contentRectTransform.sizeDelta.y);

        // Désactiver le défilement vers le bas
        ScrollRect scrollRect = content.parent.GetComponent<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.vertical = false;
        }

        // Associer la méthode ToggleTextVisibility au clic du bouton
        toggleButtonText.onClick.AddListener(ToggleTextVisibility);
    }

    // Méthode pour charger tous les sprites depuis un dossier
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

    // Méthode pour obtenir un indice aléatoire non utilisé
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

    // Méthode appelée lorsqu'une image est cliquée
    void OnImageClicked(Image clickedImage)
    {
        // Réinitialiser la couleur de l'image précédemment en surbrillance (si elle existe)
        if (highlightedImage != null)
        {
            highlightedImage.color = Color.white; // Remettez la couleur à celle d'origine (ou une autre couleur de votre choix)
        }

        // Mettre en surbrillance la nouvelle image
        clickedImage.color = Color.black;

        // Mettez à jour l'image actuellement en surbrillance
        highlightedImage = clickedImage;

        // Afficher l'image au centre du canvas
        centerImageTransform.GetComponent<Image>().sprite = clickedImage.sprite;
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


    void ToggleTextVisibility()
    {
        foreach (Text textComponent in textComponents)
        {
            textComponent.enabled = !textComponent.enabled; // Inverser l'état actuel de la visibilité du texte
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
