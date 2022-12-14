using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    Rigidbody2D rb;
    PlayerInput playerInput;

    [Tooltip("The force in jumping off the ground")]
    float jumpForce;

    [Tooltip("The forward force per kick")]
    float kickForce;

    bool grounded;
    [SerializeField] float baseGravity;
    float gravity;

    [SerializeField] GameObject body;
    [SerializeField] float topSpeed;
    [SerializeField] float climbVelocity;

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

    // Update is called once per frame
    void Update()
    {
        ToggleFace();
        GetStickInput();
        CalculateAim();

        kick = playerInput.actions.FindAction("Kick").WasPressedThisFrame();

        if (grounded)
        {
            if (kick && aimDir == AimDirection.inside)
            {
                rb.AddForce(body.transform.up * jumpForce);
            }

            if (kick && aimDir == AimDirection.back)
            {
                antiClockFace = !antiClockFace;
                rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
            }

            if (kick && aimDir == AimDirection.front)
            {
                rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
            }
            if (kick && aimDir == AimDirection.outside)
            {
                rb.velocity = Vector2.zero;
            }
        }
        else
        {

        }

        if (rb.velocity.magnitude > topSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, topSpeed);

        rb.AddForce(gravityDirection * rb.mass * rb.mass * gravity);


        Debug.DrawRay(transform.position, rb.velocity.normalized * 2f, Color.green);
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
        relativeStickAngle = stickValue.magnitude == 0 ? float.NaN : Vector2.SignedAngle(stickValue, transform.up);
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

            if (rb.velocity.magnitude > climbVelocity)
            {
                gravityDirection = -collision.GetContact(0).normal;
            }
            else
            {
                gravityDirection = Vector2.down;
            }
            body.transform.rotation = Quaternion.Lerp(body.transform.rotation, Quaternion.FromToRotation(Vector2.up, collision.GetContact(0).normal), 0.25f);

            kickForce = collision.gameObject.GetComponent<Platform>().kickForce;
            jumpForce = collision.gameObject.GetComponent<Platform>().jumpForce;
            gravity = collision.gameObject.GetComponent<Platform>().gravity;
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = false;
            gravityDirection = Vector2.down;
            body.transform.rotation = Quaternion.identity;

            gravity = baseGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Weapon")
        {
            Debug.Log("Weapon");
            collision.transform.parent = aimPivot.transform;
            collision.transform.position = aimPivot.transform.position + new Vector3(0.8f, 0, 0);
        }
    }
}
