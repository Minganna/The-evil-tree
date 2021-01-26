using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreaturesMaster : MonoBehaviour
{
    int brainrandom, pooprandom;
    List<GameObject> targets=new List<GameObject>();
    List<Bot> creatures= new List<Bot>();
    GameObject Player;
    bool runfromPlayer=false;
    Bot scaredBot;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject targ in GameObject.FindGameObjectsWithTag("wp"))
        {
            targets.Add(targ);
        }
        foreach (Bot creature in FindObjectsOfType<Bot>())
        {
            creatures.Add(creature);
            if(creature.tc==TypeOfCreature.brain)
            {
                creature.target = GetBrainRandom();
            }
            if (creature.tc == TypeOfCreature.pungolo)
            {
                creature.target = GetPungoloRandom();
            }
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        brainrandom = getRandomTarget();
        pooprandom = getRandomTarget();
    }

     int getRandomTarget()
    {
        return Random.Range(0, targets.Count);

    }

    public GameObject GetBrainRandom()
    {
      
        return targets[brainrandom];
    }
    public GameObject GetPungoloRandom()
    {
        return targets[pooprandom];
    }
    void Update()
    {
       
        foreach(Bot creature in creatures)
        {
            float distancefromp = Vector3.Distance(creature.transform.position, Player.transform.position);
            Debug.Log("distance from p" + distancefromp);
            if (creature.target)
            {
                if (!creature.runfromPlayer)
                {
                    if (Vector3.Distance(creature.transform.position, Player.transform.position) > 10)
                    {
                        creature.FindTargetTochase();
                        creature.Seek(creature.target.transform.position);
                    }
                    else if (FindObjectOfType<PlayerController>().ScareMonster)
                    {
                        Debug.Log("Arrived Here");
                        runfromPlayer = true;
                        scaredBot = creature;
                    }
                }
                else
                {
                    if (Vector3.Distance(creature.transform.position, Player.transform.position) < 10)
                    {
                        creature.target = Player;
                        creature.Evade();
                    }
                    else
                    {
                        if(scaredBot)
                        {
                            if (scaredBot.tc == TypeOfCreature.brain)
                            {
                                brainrandom = getRandomTarget();
                            }
                            else
                            {
                                pooprandom = getRandomTarget();
                            }
                        }
                       
                        creature.runfromPlayer = false;
                        scaredBot = null;
                    }

                }
                if(scaredBot)
                {
                    if (runfromPlayer && creature.tc == scaredBot.tc)
                    {
                        creature.runfromPlayer = runfromPlayer;
                    }
                }



            }
        }
       

    }
}
