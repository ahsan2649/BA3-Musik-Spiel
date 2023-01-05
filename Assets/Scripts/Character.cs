using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    BoxCollider2D col;
    Rigidbody2D rb;

    [Tooltip("How smoothly the aim rotates")]
    [SerializeField] [Range(0.1f,0.75f)] float alignSmooth;
    
    [Tooltip("The increase in velocity per kick")]
    [SerializeField] float kickForce;

    [Tooltip("The force in jumping off the ground")]
    [SerializeField]float jumpForce;
    
    [Tooltip("Limit the character's speed to avoid buzzing around the arena")]
    [SerializeField] float maxVelocity;

    [Tooltip("The velocity after which the character can slide up walls")]
    [SerializeField] float climbVelocity;

    [Tooltip("Gravity modifier when jumping up (less than fall gravity)")]
    [SerializeField] float climbGravity;

    [Tooltip("Gravity modifier when falling down (more than climb gravity)")]
    [SerializeField] float fallGravity;



    bool antiClockFace = true;
    float gravity;
    [SerializeField] bool grounded;

    Vector2 velocity;
    
    // Start is called before the first frame update
    void Start()
    {

        inputs = new Inputs();
        inputs.Player.Enable();
        
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = rb.velocity;
        ToggleFace();

        StickInput();
        CalculateAimDirection();

        if (Keyboard.current.spaceKey.wasPressedThisFrame && grounded)
        {
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(0, jumpForce));
        }
        
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            rb.AddForce(new Vector2(kickForce, 0));
            antiClockFace = true;
        }
        
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            rb.AddForce(new Vector2(-kickForce, 0));
            antiClockFace = false;
        }

        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }


        if (rb.velocity.magnitude > climbVelocity) rb.gravityScale = 0;
        else rb.gravityScale = 1;

        if (rb.velocity.y > 0 && !grounded)
        {
            rb.gravityScale = 1 * climbGravity;
        }

        if(rb.velocity.y < 0 && !grounded)
        {
            rb.gravityScale = 1 * fallGravity;
        }
    }

    void ToggleFace()
    {
        transform.localScale = new Vector3(antiClockFace ? 1 : -1, 1, 1);
    }

    #region Aim
    
    Inputs inputs;
    enum AimDirection { outside, inside, front, back, none };
    AimDirection aimDir;
    float stickAngle;
    float relativeStickAngle;
    Vector2 stickValue;
    [SerializeField] float aimSmooth = 0.25f;
    [SerializeField] GameObject aimPivot;

    void StickInput()
    {
        stickValue = inputs.Player.Aim.ReadValue<Vector2>();
        stickAngle = stickValue.magnitude == 0 ? float.NaN : Mathf.Atan2(stickValue.y, stickValue.x) * Mathf.Rad2Deg;
        relativeStickAngle = stickValue.magnitude == 0 ? float.NaN : Vector2.SignedAngle(stickValue, transform.up);
        
        aimPivot.transform.rotation = Quaternion.Lerp(aimPivot.transform.rotation, Quaternion.Euler(0, 0, stickValue.magnitude == 0 ? transform.rotation.eulerAngles.z : (antiClockFace ? stickAngle : stickAngle + 180)), aimSmooth);

    }

    void CalculateAimDirection()
    {
        if (stickValue.magnitude == 0) aimDir = AimDirection.none;
        else if (-45 <= relativeStickAngle && relativeStickAngle <= 45) aimDir = AimDirection.inside;
        else if (135 <= relativeStickAngle || -135 >= relativeStickAngle) aimDir = AimDirection.outside;
        else if (45 < relativeStickAngle && relativeStickAngle < 135) aimDir = antiClockFace ? AimDirection.front : AimDirection.back;
        else if (-45 > relativeStickAngle && relativeStickAngle > -135) aimDir = antiClockFace ? AimDirection.back : AimDirection.front;
    }

    #endregion


    void ResetPosition()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            velocity.y = 0;
            transform.position = new Vector3(0, 5, 0);
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        if(Keyboard.current.enterKey.wasPressedThisFrame)
        {
            antiClockFace = !antiClockFace;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.tag == "Surface") grounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Surface") grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Weapon")
        {
            Debug.Log("Weapon");
            collision.transform.parent = aimPivot.transform;
            collision.transform.position = aimPivot.transform.position + new Vector3(0.8f,0,0);
        }
    }
}
