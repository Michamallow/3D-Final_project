using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class LiensRenderer : MonoBehaviour
{
    public GameObject sommet1, sommet2;
    private LineRenderer render;
    private EdgeCollider2D collid;
    public float ColliderEdge = 0.5f;
    public float ColliderOfset = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        render = gameObject.GetComponent<LineRenderer>();
        render.positionCount = 2;
        collid = gameObject.GetComponent<EdgeCollider2D>();
        collid.edgeRadius = ColliderEdge + ColliderOfset; ;
        render.widthMultiplier=2*ColliderEdge;
    }

    // Update is called once per frame
    void Update()
    {
        collid.edgeRadius = ColliderEdge + ColliderOfset; ;
        render.widthMultiplier = 2 * ColliderEdge;

        Vector2 vect = (sommet1.transform.position+ - sommet2.transform.position);
        vect.Normalize();
        Vector2 vect1 = sommet1.transform.position;
        Vector2 vect2 = sommet2.transform.position;
        Vector2 vect1S = sommet1.transform.localScale;
        Vector2 vect2S = sommet2.transform.localScale;
       

        render.SetPosition(0, vect1);
        render.SetPosition(1, vect2);
        collid.points = new Vector2[2] { vect1 - vect*(vect1S/2f)-vect* collid.edgeRadius, vect2 + vect*(vect2S / 2f)+vect* collid.edgeRadius};
    }
}
