using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Spawner : MonoBehaviour
{
	public GameObject point;                //Prefab qui spawn
	public GameObject lineRend;             //Prefab rendu du spring
	public Transform parent;

	private GameObject objTemp;             //Objet temporaire enregistrer pour springJoint
	private bool objclk = false;            //Boolean surveillant si un objet à été cliqué

	public Material estSelectionne;         //Materiau couleur pour objet selectionné
	public Material nonSelectionne;         //Materiau couleur pour objet non selectionné

	private SpringJoint2D spring;           //Spring utilisé pour creer SpringJoint

	public List<GameObject> tabSommets = new List<GameObject>();
	public List<SpringJoint2D> tabSpring = new List<SpringJoint2D>();
	public List<GameObject> tabLineRends = new List<GameObject>();


	public GameObject Spawn(Vector3 position)     //Methode qui instancie le prefab a partir d'une coordonnée.
	{
		Quaternion q = new Quaternion();
		GameObject go = Instantiate(point, position, q, parent);
		tabSommets.Add(point);                     //rajout du point dans une list pour les ordonnées

		point.GetComponent<MeshRenderer>().material = nonSelectionne;   //Change la couleur en objet selectionné
		return go;
	}
	public void AddSpring(GameObject sommet, GameObject cible)      //Methode qui ajoute uun spring entre deux objet
	{
		spring = sommet.AddComponent<SpringJoint2D>();                  //création du springjoint2D
		spring.connectedBody = cible.GetComponent<Rigidbody2D>();       //Attribution de la connection au springjoint2D
		tabSpring.Add(spring);

		GameObject line = Instantiate(lineRend, parent, true);
		line.GetComponent<LiensRenderer>().sommet1 = sommet;
		line.GetComponent<LiensRenderer>().sommet2 = cible;
		tabLineRends.Add(line);
	}

	public SpringJoint2D AddSpring(GameObject sommet, GameObject cible, int frequence)      //Methode qui ajoute uun spring entre deux objet
	{
		spring = sommet.AddComponent<SpringJoint2D>();                  //création du springjoint2D
		spring.connectedBody = cible.GetComponent<Rigidbody2D>();       //Attribution de la connection au springjoint2D
		spring.frequency = frequence;
		tabSpring.Add(spring);

		GameObject line = Instantiate(lineRend, parent, true);
		line.GetComponent<LiensRenderer>().sommet1 = sommet;
		line.GetComponent<LiensRenderer>().sommet2 = cible;
		tabLineRends.Add(line);

		return spring;
	}

	public bool VerifSpringDoublon(GameObject A, GameObject B)      //methode évitant les doublons de springjoint2D, renvoie false si pas de doublon
	{
		SpringJoint2D[] jointsA = A.GetComponents<SpringJoint2D>();     //récupération de tout les springjoint2D de l'objet A
		SpringJoint2D[] jointsB = B.GetComponents<SpringJoint2D>();     //récupération de tout les springjoint2D de l'objet B

		bool bOk = false;

		foreach (SpringJoint2D joint in jointsA)
		{
			if (joint.connectedBody.gameObject == B) { bOk = true; }    //verifie qu'il n'y a aucune connection deja existante dans les springjoint2D de A
		}
		foreach (SpringJoint2D joint in jointsB)
		{
			if (joint.connectedBody.gameObject == A) { bOk = true; }    //verifie qu'il n'y a aucune connection deja existante dans les springjoint2D de B
		}
		return bOk;
	}

	// Update is called once per frame
	public void Update()
	{
		if (Input.GetMouseButtonDown(0))            //detection du clique gauche
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);        //mise au point du rayon pour detecter un collider2D

			if (hit.collider != null)                                               //test si il rencontre un collider
			{
				if (hit.transform.gameObject.tag == "Sommet")
				{


					bool memeObj = (objTemp == hit.transform.gameObject);

					if (objclk && !memeObj)                                         //verifie qu'il y a deja un objet cliqué, et qu'il ne s'agit pas du deuxieme même
					{
						if (!VerifSpringDoublon(objTemp, hit.transform.gameObject))     //verifie qu'il n'y a pas deja de spring entre les objet
						{
							AddSpring(objTemp, hit.transform.gameObject);               //ajoute le spring
						}
					}
					else
					{
						objTemp = hit.transform.gameObject;                             //Place dans l'objet temporaire l'objet detecter
						objclk = true;                                                  //Change objclk à il y a un objet cliqué
						objTemp.GetComponent<MeshRenderer>().material = estSelectionne; //Change la couleur en objet selectionné
					}
				}
				else if (hit.transform.gameObject.tag == "Lien")
				{

				}
			}
			else
			{                                   //transforme le clique de la souris en vector3 pour connaitre la position du clique
				Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
				worldPoint.z = 0.0f;            //place le z afin de mettre tout les point sur le même plan

				Spawn(worldPoint);              //Fait apparaitre le point
			}
		}
		else if (Input.GetMouseButtonDown(1))   //detecte le clique droit
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);    //rayon pour detecter un collider2D

			if (hit.collider != null)                                           //détecte le clique sur un objet
			{
				if (hit.transform.gameObject == objTemp)
				{
					objclk = false;
				}

				GameObject obj = hit.transform.gameObject;
				bool objSom = obj.tag == "Sommet";

				for (int i = tabSpring.Count - 1; i >= 0; i--)
				{
					if (tabSpring[i].connectedBody.gameObject == obj || tabSpring[i].transform.gameObject == obj || tabLineRends[i] == obj)
					{
						SpringJoint2D jDel = tabSpring[i];
						tabSpring.RemoveAt(i);
						Destroy(jDel);

						GameObject lDel = tabLineRends[i];
						tabLineRends.RemoveAt(i);
						Destroy(lDel);
					}
				}
				if (objSom)
				{
					Destroy(obj);
				}

			}
			else if (objclk)
			{
				if (objTemp.tag == "Sommet")
				{
					objTemp.GetComponent<MeshRenderer>().material = nonSelectionne; //Change la couleur en objet non selectionné
					objclk = false;                                                 //deselctionne l'objet cliqué si clique dans le vide
				}
				else if (objTemp.tag == "Lien")
				{

				}

			}
		}
	}


}
