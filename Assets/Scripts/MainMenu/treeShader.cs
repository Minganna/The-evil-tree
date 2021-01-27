using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeShader : MonoBehaviour
{
    public Material[] treeMaterial;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Material mt in treeMaterial)
        {
            mt.shader = Shader.Find("Legacy Shaders/Bumped Diffuse");
        }
    }


}
