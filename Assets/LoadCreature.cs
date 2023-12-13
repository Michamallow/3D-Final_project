using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCreature : MonoBehaviour
{
    public int id;
    public string nom;
    private static LoadCreature load;

    private void Awake()
    {
        if (load == null)
        {

            DontDestroyOnLoad(this.gameObject);
            load = this;
        }
        else if (load != this)
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setpath(int id, string nom)
    {
        this.nom = nom;
        this.id = id;
    }
}
