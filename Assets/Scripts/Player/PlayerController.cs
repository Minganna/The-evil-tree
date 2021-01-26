using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDir;
    float jumpDir;
    float CrouchDir;
    Vector2 CameraDir;
    public float moveSpeed = 0;
    [SerializeField] float turnSpeed = 1000f;
    [SerializeField] float maxForwardSpeed = 8;
    float desiredSpeed;
    float forwardSpeed;
    float jumpSpeed=1f;
    float groundRayDist = 1.7f;

    const float groundAccel = 5;
    const float groundDecel = 25;
    public bool ScareMonster = false;

     Animator anim;

    Rigidbody rb;
    bool readyJump = false;

    bool OnGround = true;

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
    void Crouch(float direction)
    {
        Debug.Log(direction);
        if(direction>0)
        {
            anim.SetBool("Crouching", true);
        }
        else
        {
            anim.SetBool("Crouching", false);
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

    }



    // Update is called once per frame
    void Update()
    {
        Move(moveDir);
       // Jump(jumpDir);
        Crouch(CrouchDir);
        Interact(jumpDir);
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
