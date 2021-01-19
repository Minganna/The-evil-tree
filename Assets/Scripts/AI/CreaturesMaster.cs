using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CreaturesMaster : MonoBehaviour
{
    int brainrandom, pooprandom;
    List<GameObject> targets=new List<GameObject>();
    List<Bot> PoopMonsters, BrainMonsters;
    GameObject Player;
    bool runfromPlayer=false;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject targ in GameObject.FindGameObjectsWithTag("wp"))
        {
            targets.Add(targ);
        }
        Player = GameObject.FindGameObjectWithTag("Player");
        BrainMonsters = new List<Bot>();
        PoopMonsters = new List<Bot>();
        brainrandom = getRandomTarget();
        pooprandom = getRandomTarget();
    }

     int getRandomTarget()
    {
        int randomT;
        return randomT = Random.Range(0, targets.Count);

    }

    public GameObject GetBrainRandom(Bot brainMonster)
    {
        if(!BrainMonsters.Contains(brainMonster))
        {
            BrainMonsters.Add(brainMonster);
        }
      
        return targets[brainrandom];
    }
    public GameObject GetPoopRandom(Bot poopMonster)
    {
        if(!PoopMonsters.Contains(poopMonster))
        {
            PoopMonsters.Add(poopMonster);
        }
        return targets[pooprandom];
    }
    void Update()
    {
        foreach(Bot creature in PoopMonsters)
        {
            if (creature.target)
            {
                if(!runfromPlayer)
                {
                    if (Vector3.Distance(creature.transform.position, Player.transform.position) > 1 )
                    {
                        creature.agent.speed = 3.5f;
                        creature.FindTargetTochase();
                        creature.Seek(creature.target.transform.position);
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        runfromPlayer = true;
                    }
                }
                else
                {
                    if (Vector3.Distance(creature.transform.position, Player.transform.position) < 10)
                    {
                        creature.target = Player;
                        creature.agent.speed = 7;
                        creature.Evade();
                    }
                    else
                    {
                        brainrandom = getRandomTarget();
                        pooprandom = getRandomTarget();
                        runfromPlayer = false;
                    }
                    
                }


            }
        }
       

    }
}
