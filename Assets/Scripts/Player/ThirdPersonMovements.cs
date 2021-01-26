using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonMovements : MonoBehaviour
{

    [SerializeField]Transform child;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
            Vector3 newPosition = new Vector3(child.localPosition.x, child.localPosition.y + 1.5f, child.localPosition.z);
            this.transform.position = newPosition;
            child.transform.rotation = this.transform.rotation;
        
 
       

    }
}
