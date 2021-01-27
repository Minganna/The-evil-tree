using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    Transform goal;
    public float speed = 2.0f;
    float accuracy = 1.0f;
    float rotSpeed = 1.0f;
    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    bool startMovements=false;
    // Start is called before the first frame update
    void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        g = wpManager.GetComponent<WPManager>().graph;
        currentNode = wps[0];
    }

    public void GoToPlayer()
    {
        g.AStar(currentNode, wps[4]);
        currentWP = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (startMovements == false)
        {
            Debug.Log("here");
            startMovements = true;
            StartCoroutine(StartMoving());

        }
        if (g.getPathLength()==0||currentWP==g.getPathLength())
        {
            return;
        }
        currentNode = g.getPathPoint(currentWP);
        if(Vector3.Distance(g.getPathPoint(currentWP).transform.position,transform.position)<accuracy)
        {
            currentWP++;
        }

        if(currentWP<g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
                   Vector3 lookAtGoal = new Vector3(goal.position.x, this.transform.position.y, goal.position.z);
        Vector3 direction = lookAtGoal - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

            // if animation move model forward comment this code and tick "apply root motion" in the Animator
            this.transform.Translate(0, 0, speed * Time.deltaTime);

        }
       

    }

    IEnumerator StartMoving()
    {
        yield return new WaitForSeconds(1.0f);
        Debug.Log("Moving");
        GoToPlayer();
    }

    public int GetWP()
    {
        return currentWP;
    }
}
