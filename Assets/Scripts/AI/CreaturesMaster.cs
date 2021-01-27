using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreaturesMaster : MonoBehaviour
{
    int brainrandom, pooprandom,pinrandom;
    List<GameObject> targets=new List<GameObject>();
    List<Bot> creatures= new List<Bot>();
    GameObject Player;
    public bool runfromPlayer=false;
    Bot scaredBot;
    public bool sendDataToPlayer = true;
    GameObject brainTarget;
    GameObject PungoloTarget;
    GameObject PinHeadTarget;
    int currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject targ in GameObject.FindGameObjectsWithTag("wp"))
        {
            targets.Add(targ);
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        brainrandom = getRandomTarget();
        brainTarget = targets[brainrandom];
        pooprandom = getRandomTarget();
        PungoloTarget = targets[pooprandom];
        pinrandom = getRandomTarget();
        PinHeadTarget = targets[pinrandom];
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
            if (creature.tc == TypeOfCreature.pinhead)
            {
                creature.target = GetPinheadRandom();
            }
        }


    }

     int getRandomTarget()
    {
        if (currentTarget < targets.Count-1)
        {
            currentTarget += 1;
        }
        else
        {
            currentTarget = 0;
        }

        return currentTarget;

    }

    public GameObject GetBrainRandom()
    {
        return brainTarget;
    }
    public GameObject GetPungoloRandom()
    {
        return PungoloTarget;
    }
    public GameObject GetPinheadRandom()
    {
        return PinHeadTarget;
    }
    void Update()
    {
       
        foreach(Bot creature in creatures)
        {
            float distancefromp = Vector3.Distance(creature.transform.position, Player.transform.position);
          
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
                       
                        runfromPlayer = true;
                        scaredBot = creature;
                    }
                }
                else
                {
                    if (Vector3.Distance(creature.transform.position, Player.transform.position) < 10)
                    {
                        if(sendDataToPlayer)
                        {
                            sendDataToPlayer = false;
                            FindObjectOfType<PlayerController>().reactionToInteract(runfromPlayer, System.Enum.GetName(typeof(TypeOfCreature), creature.tc));
                        }
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
                                brainTarget = targets[brainrandom];
                            }
                            if(scaredBot.tc == TypeOfCreature.pinhead)
                            {
                                pinrandom = getRandomTarget();
                                PinHeadTarget = targets[pinrandom];
                            }
                            if (scaredBot.tc == TypeOfCreature.pungolo)
                            {
                                pooprandom = getRandomTarget();
                                PungoloTarget = targets[pooprandom];
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
