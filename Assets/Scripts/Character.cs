using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    Rigidbody2D rb;
    PlayerInput playerInput;

    float health = 100;
    float jumpForce;
    float kickForce;
    float gravity;

    bool grounded;
    bool jumpRegion;

    public Gun gun;

    [Header("Gravity")]
    [Tooltip("Gravity when not touching the surface")]
    [SerializeField] float airGravity;
    [Tooltip("Gravity when in a trigger tagged 'Jump'")]
    [SerializeField] float jumpGravity;
    [Tooltip("Gravity when touching the surface")]
    [SerializeField] float baseGravity;

    [Header("Kick Force")]
    [Tooltip("Gravity when touching the surface")]
    [SerializeField] float baseKickForce;
    [Tooltip("Jump force when in a trigger tagged 'Jump'")]
    [SerializeField] float jumpKickForce;

    [Header("Jump Force")]
    [Tooltip("Kick force when touching the surface")]
    [SerializeField] float baseJumpForce;
    [Tooltip("Kick force when in a trigger tagged 'Jump'")]
    [SerializeField] float jumpJumpForce;


    [SerializeField] GameObject body;

    [Tooltip("The speed limit for the character")]
    [SerializeField] float topSpeed;


    bool kick;



    Vector2 gravityDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        gravity = baseGravity;
        gravityDirection = Vector2.down;
    }

    public void Stick(InputAction.CallbackContext ctx)
    {
        stickValue = ctx.ReadValue<Vector2>();
    }

    public void Kick(InputAction.CallbackContext ctx)
    {
        kick = ctx.ReadValueAsButton();
    }
    // Update is called once per frame
    void Update()
    {
        ToggleFace();
        GetStickInput();
        CalculateAim();

        //kick = playerInput.actions.FindAction("Kick").WasPerformedThisFrame();

        if (grounded)
        {
            

            if (!jumpRegion)
            {
                jumpForce = baseJumpForce;
                kickForce = baseKickForce;
                gravity = baseGravity;
            }
            else
            {
                jumpForce = jumpJumpForce;
                kickForce = jumpKickForce;
                gravity = jumpGravity;
            }
        }
        else
        {
            gravity = airGravity;
        }




        Debug.DrawRay(transform.position, rb.velocity.normalized * 2f, Color.green);
    }

    private void FixedUpdate()
    {

        if(grounded)
        {
            if (kick)
            {
                kick = false;
                if (aimDir == AimDirection.inside)
                {
                    rb.AddForce(body.transform.up * jumpForce);
                }

                if (aimDir == AimDirection.back)
                {
                    antiClockFace = !antiClockFace;
                    rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
                }
                if (aimDir == AimDirection.front)
                {
                    rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
                }
                if (aimDir == AimDirection.outside)
                {
                    rb.velocity = Vector2.zero;
                }
            }

            if (rb.velocity.magnitude > topSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, topSpeed);
        }
        
        rb.AddForce(gravityDirection * rb.mass * gravity);

    }

    private void FixedUpdate()
    {
        

        rb.AddForce(gravityDirection * rb.mass * rb.mass * gravity);

    }

    #region Aim

    enum AimDirection { outside, inside, front, back, none };
    AimDirection aimDir;

    float stickAngle, relativeStickAngle;
    Vector2 stickValue;
    bool antiClockFace = true;

    [SerializeField] float aimSmooth = 0.25f;
    [SerializeField] GameObject aimPivot;

    void GetStickInput()
    {
        stickAngle = stickValue.magnitude == 0 ? float.NaN : Mathf.Atan2(stickValue.y, stickValue.x) * Mathf.Rad2Deg;
        relativeStickAngle = stickValue.magnitude == 0 ? float.NaN : Vector2.SignedAngle(stickValue, body.transform.up);
    }

    void CalculateAim()
    {
        if (stickValue.magnitude == 0) aimDir = AimDirection.none;
        else if (-45 <= relativeStickAngle && relativeStickAngle <= 45) aimDir = AimDirection.inside;
        else if (135 <= relativeStickAngle || -135 >= relativeStickAngle) aimDir = AimDirection.outside;
        else if (45 < relativeStickAngle && relativeStickAngle < 135) aimDir = antiClockFace ? AimDirection.front : AimDirection.back;
        else if (-45 > relativeStickAngle && relativeStickAngle > -135) aimDir = antiClockFace ? AimDirection.back : AimDirection.front;

        aimPivot.transform.rotation = Quaternion.Lerp(aimPivot.transform.rotation, Quaternion.Euler(0, 0, stickValue.magnitude == 0 ? transform.rotation.eulerAngles.z : (antiClockFace ? stickAngle : stickAngle + 180)), aimSmooth);
    }

    #endregion

    void ToggleFace()
    {
        transform.localScale = new Vector3(antiClockFace ? 1 : -1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = true;

            gravityDirection = -collision.GetContact(0).normal;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = false;
            gravityDirection = Vector2.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Weapon")
        {
            Debug.Log("Weapon");
            gun = collision.GetComponent<Gun>();
            gun.owner = this;
            gun.transform.parent = aimPivot.transform;
            gun.transform.position = aimPivot.transform.position + new Vector3(0.8f, 0, 0);
        }

        if (collision.tag == "Jump")
        {
            jumpRegion = true;
        }

        if (collision.tag == "Bullet")
        {
            if (collision.GetComponent<Bullet>().shooter != this)
            {
                health -= collision.GetComponent<Bullet>().damage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Jump")
        {
            jumpRegion = false;
        }
    }
}
