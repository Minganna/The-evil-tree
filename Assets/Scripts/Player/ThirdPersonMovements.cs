using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovements : MonoBehaviour
{
    CharacterController controller;
    [SerializeField] float speed=6.0f;
    [SerializeField] float turnSmoothTime= 0.1f;
    [SerializeField] float gravity = -19.81f;
    [SerializeField] float jumpHeight = 3.0f;

    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundDistance=0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    [SerializeField] Transform cam;

    bool isGrounded;


    // Start is called before the first frame update
    void Start()
    {
        controller = this.GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded&&velocity.y<0)
        {
            velocity.y = -2f;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        if(direction.magnitude>=0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z)*Mathf.Rad2Deg +cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

           
        }
        if(Input.GetButtonDown("Jump")&&isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }
}
