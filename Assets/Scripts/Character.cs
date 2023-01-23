using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{

    Rigidbody2D rb;
    PlayerInput playerInput;
    
    [SerializeField] Animator ani;
    [SerializeField] float min_velocity_to_start_skate;
    [SerializeField] GameObject MissParticle;

    public float health = 100;
    float jumpForce;
    float kickForce;
    float gravity;

    bool grounded;
    bool jumpRegion;

    public bool RotationReady = false;

    public Gun gun;

    [Header("Gravity")]
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
    [HideInInspector] public Transform spawnPoint;

    [Tooltip("The speed limit for the character")]
    [SerializeField] float topSpeed;

    [SerializeField] float drag = 1;
    [SerializeField] float slideModifier = 0.5f;

    float constantVelocity = 0;
    public bool kick;
    bool isSliding;

    public bool keep_velocity = true;
    public int framecount = 0;

    Vector2 gravityDirection;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawnPoint.position;
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        gravity = baseGravity;
        gravityDirection = Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        ToggleFace();
        GetStickInput();
        CalculateAim();
        SetForceValues();

        if (constantVelocity > min_velocity_to_start_skate)
        {
            if (ani != null) ani.SetBool("HasMomentum", true);
        }
        else
        {
            if (ani != null) ani.SetBool("HasMomentum", false);
        }


        Debug.DrawRay(transform.position, rb.velocity.normalized * 2f, Color.green);
    }


    private void FixedUpdate()
    {
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, Quaternion.FromToRotation(Vector2.up, -gravityDirection), 0.25f);

        if (GameManager.instance.phase == GameManager.Phase.Starting)
        {

            if (stickValue.magnitude != 0)
            {
                RotationReady= true;
            }
        }

        if (GameManager.instance.phase == GameManager.Phase.Playing)
        {
            if (kick && grounded && GameManager.instance.canKick)
            {
                kick = false;
                if (aimDir == AimDirection.inside)
                {
                    rb.AddForce(body.transform.up * jumpForce);
                    grounded = false;
                    if (ani != null)
                    {
                        ani.SetBool("OnGround", false);
                        ani.SetTrigger("Jump");
                    }
                    gravity = airGravity;
                    //Debug.Log("Jump " + framecount.ToString());
                }
                if (aimDir == AimDirection.back)
                {
                    if (constantVelocity == 0)
                    {
                        antiClockFace = !antiClockFace;
                        rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
                        constantVelocity += kickForce;

                        if (ani != null)
                        {
                            ani.SetBool("DashSwitch", !ani.GetBool("DashSwitch"));
                            ani.SetTrigger("Kick");
                        }
                    }
                    else if (!isSliding)
                    {
                        isSliding = true;
                        constantVelocity *= slideModifier;
                        if (ani != null)
                        {
                            ani.SetBool("Sliding", true);
                            ani.SetTrigger("SlidingTrigger");
                            ani.SetBool("DashSwitch", !ani.GetBool("DashSwitch"));
                            ani.SetTrigger("Kick");
                        }
                    }
                    else
                    {
                        isSliding = false;
                        constantVelocity /= slideModifier;
                        rb.velocity = -rb.velocity.normalized * constantVelocity;
                        if (ani != null) ani.SetBool("Sliding", false);
                    }
                }
                if (aimDir == AimDirection.front)
                {
                    if (isSliding)
                    {
                        isSliding = false;
                        constantVelocity /= slideModifier;
                        rb.velocity = rb.velocity.normalized * constantVelocity;
                        if (ani != null)
                        {
                            ani.SetBool("Sliding", false);
                            ani.SetBool("DashSwitch", !ani.GetBool("DashSwitch"));
                            ani.SetTrigger("Kick");
                        }
                    }
                    else
                    {
                        rb.AddForce((antiClockFace ? body.transform.right : -body.transform.right) * kickForce);
                        constantVelocity += kickForce;

                        if (ani != null)
                        {
                            ani.SetBool("DashSwitch", !ani.GetBool("DashSwitch"));
                            ani.SetTrigger("Kick");
                        }
                    }

                }
                if (aimDir == AimDirection.outside)
                {
                    rb.velocity = Vector2.zero;
                    constantVelocity = 0;
                    if (ani != null) ani.SetTrigger("Brake");
                }
            }
        }

        if (kick && grounded && !GameManager.instance.canKick)
        {
            if (MissParticle != null) Instantiate(MissParticle, transform);
            Debug.Log(transform.position);
        }

        if (grounded) //second check is here so that after a jump the velocity is not checked
        {
            
            if (rb.velocity.magnitude > topSpeed) rb.velocity = Vector2.ClampMagnitude(rb.velocity, topSpeed);
            
            constantVelocity -= drag * Time.fixedDeltaTime;
            if (constantVelocity < 0)
            {
                constantVelocity = 0;
            }
            if (constantVelocity > topSpeed)
            {
                constantVelocity = topSpeed;
            }

        }
        
        rb.AddForce(gravityDirection * rb.mass * gravity);
        framecount += 1;
    }

    private void LateUpdate()
    {
        if (grounded)
        {
            rb.velocity = rb.velocity.normalized * constantVelocity;
        }       
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

        aimPivot.transform.rotation = Quaternion.Lerp(aimPivot.transform.rotation, Quaternion.Euler(0, 0, stickValue.magnitude == 0 ? transform.rotation.eulerAngles.z : stickAngle), aimSmooth);
    }

    #endregion


    public void Stick(InputAction.CallbackContext ctx)
    {
        stickValue = ctx.ReadValue<Vector2>();
    }

    public void Kick(InputAction.CallbackContext ctx)
    {
        kick = ctx.ReadValueAsButton();
    }

    private void SetForceValues()
    {
        if (jumpRegion)
        {
            jumpForce = jumpJumpForce;
            kickForce = jumpKickForce;
            gravity = jumpGravity;
        }
        else if (grounded)
        {
            jumpForce = baseJumpForce;
            kickForce = baseKickForce;
            gravity = baseGravity;
        }
        else
        {
            gravity = airGravity;
        }
    }

    void ToggleFace()
    {
        if (constantVelocity != 0) antiClockFace = Vector2.SignedAngle(body.transform.up, rb.velocity) < 0 ? true : false;
        body.transform.localScale = new Vector3(antiClockFace ? 1 : -1, 1, 1);
    }

    #region CollisionStuff

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = true;
            if (ani != null)
                ani.SetBool("OnGround", true);
            gravityDirection = -collision.GetContact(0).normal;
            Debug.Log("Enter");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            //grounded = true;
            if (grounded)
            {
                gravityDirection = -collision.GetContact(0).normal;
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = false;
            if (ani != null)
                ani.SetBool("OnGround", false);
            gravityDirection = Vector2.down;
            //Debug.Log("Exit " + framecount.ToString());
            Debug.Log("Exit");
        }
    }
    #endregion

    #region TriggerStuff
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Weapon":
                var weapon = collision.GetComponent<Gun>();
                if (weapon.owner == null && gun == null)
                {
                    Debug.Log("Weapon");
                    gun = weapon;
                    weapon.owner = this;
                    weapon.transform.parent = aimPivot.transform;
                    weapon.transform.localRotation = Quaternion.identity;
                    weapon.transform.localPosition = new Vector3(0.8f, 0, 0);
                }
                else
                {
                    break;
                }
                break;
            case "Jump":
                jumpRegion = true;
                Debug.Log("Jump In");
                break;
            case "Coin":
                Destroy(collision.gameObject);
                break;
        }

        //Debug.Log("Enter " + framecount.ToString());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Jump")
        {
            jumpRegion = false;
            Debug.Log("Jump Out");
        }
    }
    #endregion
}
