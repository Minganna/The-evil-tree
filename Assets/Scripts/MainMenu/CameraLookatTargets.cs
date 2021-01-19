using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookatTargets : MonoBehaviour
{

    public Transform[] targets;
    public int currentTarget;
    private FollowPath fp;
    float rotSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectOfType<FollowPath>())
        {
            fp = FindObjectOfType<FollowPath>();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fp.GetWP()<2)
        {
            currentTarget = 0;
        }
        else
        {
            currentTarget = 1;
        }
        Vector3 direction = targets[currentTarget].position - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

    }
}
