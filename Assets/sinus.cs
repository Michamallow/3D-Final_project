using System;
using UnityEngine;

public class sinus : MonoBehaviour
{
    public SpringJoint2D joint;
    [Range(0, 1)]
    public float ecart = 0.2f; //Ecart : Variation de la distance des spring. Exemple ecart 0.2f : La distance va varier entre 80% et 120% de sa taille.
    [Range(-1, 1)]
    public float offset = 0; //Offset : Décalage entre -1 et 1. Cela permet de désynchroniser les membres des bestioles.
    [Range(0, 1)]
    public float frequence = 0.5f; //Fréquence : Vitesse à laquelle le Spring s'étire puis se rétracte. Entre 0 et 1.
    public bool monte=true; //Choisir si le ressort va commencer par s'étendre ou se rétracter.

    //private float accumulatedTime = 0.0f;
    private float temps;
    private float dist;

    // Start is called before the first frame update
    void Start()
    {
        dist = joint.distance;
        frequence = ecart * frequence;
        temps = 1 + (ecart * offset);
    }


    // Update is called once per frame
    void Update()
    {
        if (temps >= 1+ecart)
        {
            monte = false;
        }
        if (temps <= 1-ecart)
        {
            monte = true;
        }

        //accumulatedTime += Time.deltaTime;
        if (monte) {
            temps += (frequence + Time.deltaTime);
            if (temps >= 1 + ecart)
            {
                temps = 1 + ecart;
            }
        }
        else
        {
            temps -= (frequence + Time.deltaTime);
            if (temps <= 1 + ecart)
            {
                temps = 1 - ecart;
            }
        }

        joint.distance = dist * temps;
    }
}
