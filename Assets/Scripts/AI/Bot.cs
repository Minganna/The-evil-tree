using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum TypeOfCreature
{
   brain, bubble, pungolo
}
public class Bot : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public TypeOfCreature tc;
    CreaturesMaster cm;
    public bool runfromPlayer=false;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        cm = FindObjectOfType<CreaturesMaster>();

    }

    internal void FindTargetTochase()
    {
        if (tc == TypeOfCreature.pungolo)
        {
            target = cm.GetPungoloRandom();
            

        }
        else
        {
            target = cm.GetBrainRandom();
        }
    }

    public void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    public void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    public void Pursue()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;

        float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(target.transform.forward));
        float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));

        if((toTarget>90&&relativeHeading<20)|| target.GetComponent<Drive>().currentSpeed<0.01f)
        {
            Seek(target.transform.position);
            return;
        }

        float lookAhed = targetDir.magnitude / (agent.speed + target.GetComponent<Drive>().currentSpeed);
        Seek(target.transform.position + target.transform.forward * lookAhed);
    }

    public void Evade()
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float lookAhed = targetDir.magnitude / (agent.speed + target.GetComponent<PlayerController>().moveSpeed);
        Flee(target.transform.position + target.transform.forward * lookAhed);
    }


    
}
