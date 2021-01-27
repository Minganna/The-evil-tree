using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonMovements : MonoBehaviour
{

    [SerializeField]Transform child;
    public float shoulderPos=1.5f;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
            Vector3 newPosition = new Vector3(child.localPosition.x, child.localPosition.y + shoulderPos, child.localPosition.z);
            this.transform.position = newPosition;
            child.transform.rotation = this.transform.rotation;
        
 
       

    }
}
