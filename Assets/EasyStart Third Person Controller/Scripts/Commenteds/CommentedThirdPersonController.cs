
using UnityEngine;

public class CommentedThirdPersonController : MonoBehaviour
{

    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    bool isJumping = false;
    bool isSprinting = false;
    bool isCrouching = false;

    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputCrouch;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    float jumpElapsedTime = 0;

    

    void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        inputJump = Input.GetAxis("Jump") == 1f;
        inputSprint = Input.GetAxis("Fire3") == 1f;
        inputCrouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.JoystickButton1);

        if ( inputCrouch == true )
        {
            isCrouching = !isCrouching; // Use the ! in a boolean is a way to toggle it!
        }

        if ( cc.isGrounded )
        {
            if ( isCrouching == true )
                animator.SetBool( "crouch", true );
            else
                animator.SetBool( "crouch", false );

            float minimumSpeed = 0.9f; // You can test with other values
            if ( cc.velocity.magnitude > minimumSpeed) 
                animator.SetBool( "run", true );
            else
                animator.SetBool( "run", false );

            if ( cc.velocity.magnitude > 0.9f && inputSprint )
                isSprinting = true;
            else
                isSprinting = false;
            
            animator.SetBool("sprint", isSprinting );
        }

        if( cc.isGrounded == true )
            animator.SetBool( "air", false );
        else
            animator.SetBool( "air", true );

        if ( inputJump && cc.isGrounded )
            isJumping = true;

        HeadHittingDetect();
    }


    private void FixedUpdate()
    {

        float velocityAdittion = 0;
        if ( isSprinting )
            velocityAdittion = sprintAdittion;
        if (isCrouching)
            velocityAdittion = - (velocity * 0.50f); // -50% velocity

        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
        float directionY = 0;

        if ( isJumping ) {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

            jumpElapsedTime += Time.deltaTime;
            if ( jumpElapsedTime >= jumpTime )
            {
                isJumping = false; // It's not jumping anymore
                jumpElapsedTime = 0; // We reset the time so that the player can jump again later
            }
        }

        directionY = directionY - gravity * Time.deltaTime;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        forward = forward * directionZ;
        right = right * directionX;

        if ( directionX != 0 || directionZ != 0 )
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 verticalDirection = Vector3.up * directionY;
        Vector3 horizontalDirection = forward + right;

        Vector3 moviment = verticalDirection + horizontalDirection;
        cc.Move( moviment );
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }

}