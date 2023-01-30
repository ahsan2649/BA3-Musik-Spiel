using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool kick;
    public bool grounded;
    public bool jumpRegion;
    public bool isSliding;
    public bool antiClockFace = true;
    Gun gun;

    public float constantVelocity = 0;
    public float slideModifier = 0.35f;

    public Rigidbody2D rb;

    public GameObject body;
    public Animator ani;

    public Vector2 gravityDirection;

    [SerializeField] float min_velocity_to_start_skate;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ToggleFace();
        GetStickInput();
        CalculateAim();

        if (constantVelocity > min_velocity_to_start_skate)
        {
            if (ani != null) ani.SetBool("HasMomentum", true);
        }
        else
        {
            if (ani != null) ani.SetBool("HasMomentum", false);
        }


        aimPivot.transform.rotation = Quaternion.Lerp(aimPivot.transform.rotation, Quaternion.Euler(0, 0, stickValue.magnitude == 0 ? transform.rotation.eulerAngles.z : stickAngle), aimSmooth);
    }

    void ToggleFace()
    {
        if (constantVelocity != 0) antiClockFace = Vector2.SignedAngle(body.transform.up, rb.velocity) < 0 ? true : false;
        body.transform.localScale = new Vector3(antiClockFace ? 1 : -1, 1, 1);
    }

    #region Trigger Stuff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Weapon":
                var weapon = collision.GetComponent<Gun>();
                if (weapon.shooter == null && gun == null)
                {
                    Debug.Log("Weapon");
                    gun = weapon;
                    weapon.shooter = this;
                    weapon.GetComponent<Collider2D>().enabled = false;
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

    #region Collision Stuff

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
            grounded = true;
            if (ani != null) ani.SetBool("OnGround", true);
            gravityDirection = -collision.GetContact(0).normal;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Surface")
        {
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
            if (ani != null) ani.SetBool("OnGround", false);
            gravityDirection = Vector2.down;
        }
    }

    #endregion

    #region Input

    public void Stick(InputAction.CallbackContext ctx)
    {
        stickValue = ctx.ReadValue<Vector2>();
    }

    public void Kick(InputAction.CallbackContext ctx)
    {
        kick = ctx.ReadValueAsButton();
    }

    #endregion

    #region Aim

    Vector2 stickValue;
    float stickAngle, relativeStickAngle;
    public enum AimDirection { outside, inside, front, back, none };
    public AimDirection aimDir;
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
    }

    #endregion
}