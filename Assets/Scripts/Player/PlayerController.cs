using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDir;
    float jumpDir;
    float CrouchDir;
    float FightTreebutton;
    Vector2 CameraDir;
    public float moveSpeed = 0;
    [SerializeField] float turnSpeed = 1000f;
    [SerializeField] float maxForwardSpeed = 8;
    float desiredSpeed;
    float forwardSpeed;
    float jumpSpeed=1f;
    float groundRayDist = 1.7f;
    bool GameWon = false;

    public GameObject root;

    const float groundAccel = 5;
    const float groundDecel = 25;
    public bool ScareMonster = false;
    public UnityEngine.UI.Slider slider;
    
    string monstertype;
    int pinheadeaten = 0;
    int braineaten = 0;
    int spikeeaten = 0;
    int maxspike = 3;
    int maxbrain = 4;
    int maxpin = 3;
    TMP_Text brainText;
    TMP_Text PungoloText;
    TMP_Text PinheadText;

    Animator anim;

    Rigidbody rb;
    bool readyJump = false;

    bool OnGround = true;

    public bool iscrouching = false;
    public bool canHit = true;
    public GameObject Death;
    public GameObject Fight;
    GameObject TreeBoss;

    public int size = 0;

    bool isMoveInput
    {
        get { return !Mathf.Approximately(moveDir.sqrMagnitude, 0f); }
    }



    public void OnMove(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
        
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        jumpDir = context.ReadValue<float>();

    }
    public void OnCrouch(InputAction.CallbackContext context)
    {
        CrouchDir = context.ReadValue<float>();

    }
    public void OnFight(InputAction.CallbackContext context)
    {
        FightTreebutton = context.ReadValue<float>();

    }

    public void OnCameraMove(InputAction.CallbackContext context)
    {
        CameraDir = context.ReadValue<Vector2>();

    }

    void Move(Vector2 direction)
    {
        float turnAmount = direction.x;
        float fDirection = direction.y;
        if(direction.sqrMagnitude>1f)
        {
            direction.Normalize();
            
        }
        desiredSpeed = direction.magnitude * maxForwardSpeed * Mathf.Sign(fDirection);
        float acceleration = isMoveInput ? groundAccel : groundDecel;

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, acceleration * Time.deltaTime);
        anim.SetFloat("ForwardSpeed", forwardSpeed);

        GameObject.Find("ThirdPersonPlayer").transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);

    }

    void Jump(float direction)
    {
        if(direction>0&&OnGround)
        {
            anim.SetBool("Jump", true);
            readyJump = true;
        }
        else if(readyJump)
        {
            anim.SetBool("Jump", false);
            readyJump = false;
        }

    }

    void Interact(float direction)
    {
        Debug.Log(ScareMonster);
        if (direction > 0)
        {
            ScareMonster = true;
        }
        else
        {
            ScareMonster = false;
        }

       

    }    

    public void stopeating()
    {
        anim.SetBool("EatMonster", false);
        this.transform.localScale = new Vector3(this.transform.localScale.x + 1, this.transform.localScale.y + 1, this.transform.localScale.z + 1);
        size += 1;
        if(size>=5)
        {
            Fight.SetActive(true);
        }
        FindObjectOfType<ThirdPersonMovements>().shoulderPos += 1;
        slider.value += 1;
        FindObjectOfType<CreaturesMaster>().sendDataToPlayer = true;
        if (monstertype == "brain")
        {
            braineaten += 1;
            brainText.text = braineaten + "-" + maxbrain;

        }
        if (monstertype == "pinhead")
        {
            pinheadeaten += 1;
            PinheadText.text = pinheadeaten + "-" + maxpin;
        }
        if (monstertype == "pungolo")
        {
            spikeeaten += 1;
            PungoloText.text = spikeeaten + "-" + maxspike;
        }

    }

    public void stopLookingAround()
    {
        anim.SetBool("LookAround", false);
    }

    public void reactionToInteract(bool reaction,string creatureType)
    {
        monstertype = creatureType;
       
        if (creatureType== "brain")
        {
            if(braineaten<maxbrain)
            {
                reaction = true;
            }
            else
            {
                reaction = false;
            }
        }
        else if (creatureType == "pinhead")
        {
            if (pinheadeaten < maxpin)
            {
                reaction = true;
            }
            else
            {
                reaction = false;
            }
        }
        else if (creatureType == "pungolo")
        {
            if (spikeeaten < maxspike)
            {
                reaction = true;
            }
            else
            {
                reaction = false;
            }
        }
        if (reaction)
        {
            anim.SetBool("EatMonster", true);
        }
        else
        {
            anim.SetBool("LookAround", true);
        }
    }
    void Crouch(float direction)
    {
        if(direction>0)
        {
            anim.SetBool("Crouching", true);
            iscrouching = true;
        }
        else
        {
            anim.SetBool("Crouching", false);
            iscrouching = false;
        }
    }

    public void Launch()
    {
        Debug.Log("Here");
        anim.applyRootMotion = false;
        rb.AddForce(0, jumpSpeed, 0);
        

    }

    public void Land()
    {
        anim.SetBool("Land", false);
        anim.applyRootMotion = true;
    }



    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        TreeBoss = GameObject.Find("TreeBoss");
        brainText=GameObject.Find("BrainText").GetComponent<TMP_Text>();
        PungoloText=GameObject.Find("pungoloText").GetComponent<TMP_Text>();
        PinheadText= GameObject.Find("PinheadText").GetComponent<TMP_Text>(); 


    }

    public void Damage()
    {
        root.transform.position = this.transform.position;
        canHit = false;
        anim.SetBool("Hit", true);
    }

    public void startCountforDamage()
    {
       if(size>0)
        {
            anim.SetBool("Hit", false);
            root.transform.position = new Vector3(root.transform.position.x, root.transform.position.y - 3, root.transform.position.z);
            size -= 1;
            slider.value -= 1;
            this.transform.localScale = new Vector3(this.transform.localScale.x - 1, this.transform.localScale.y - 1, this.transform.localScale.z - 1);
            StartCoroutine(hittableagain());
        }
       else
        {
            anim.SetBool("Dead", true);
            
        }
       
    }

    IEnumerator hittableagain()
    {
        yield return new WaitForSeconds(3.0f);
        canHit = true;
    }

    public void DeathandRestart()
    {
        Death.SetActive(true);
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(3.0f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    void FinishGame()
    {
        if(size>=5&& Vector3.Distance(this.transform.position, TreeBoss.transform.position)<40)
        {
           if(FightTreebutton>0&&!GameWon)
            {
                GameWon = true;
                anim.SetBool("GameWon", true);
            }
            
        }
    }

    public void DestroyTree()
    {
        TreeBehavious tb = TreeBoss.GetComponent<TreeBehavious>();
        tb.TreeDeath();
        TMP_Text textfinal=Fight.GetComponentInChildren<TMP_Text>();
        textfinal.text = "You Won!";
        StartCoroutine(ReturnToMenu());

    }

    IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(3f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        Move(moveDir);
       // Jump(jumpDir);
        Crouch(CrouchDir);
        Interact(jumpDir);
        FinishGame();
        RaycastHit hit;
        Vector3 ThisRayPos = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        Ray ray = new Ray(ThisRayPos + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
        Debug.DrawRay(ThisRayPos + Vector3.up * groundRayDist * 0.5f, -Vector3.up * groundRayDist, Color.red);
        if (Physics.Raycast(ray,out hit,groundRayDist))
        {
            
            if(!OnGround)
            {
                OnGround = true;
                anim.SetBool("Land", true);
            }
            else
            {
                OnGround = false;
            }
        }
        
    }
}
