using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehavious : MonoBehaviour
{
    GameObject Player;
    PlayerController pc;
    public Shader ExtrudeShader;
    public Material[] treeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        pc = Player.GetComponent<PlayerController>();
        foreach (Material mt in treeMaterial)
        {
            mt.shader = Shader.Find("Legacy Shaders/Bumped Diffuse");
        }
    }

    public void TreeDeath()
    {
        foreach(Material mt in treeMaterial)
        {
            mt.shader = ExtrudeShader;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(pc.size<5)
        {
            if (Vector3.Distance(this.transform.position, Player.transform.position) < 80 && !pc.iscrouching && pc.canHit)
            {
                pc.Damage();
            }
        }
        
        
    }
}
