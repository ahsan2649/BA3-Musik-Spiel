using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

public class Player : MonoBehaviour
{
    public float health;
    public bool kick;
    public bool grounded;
    public bool jumpRegion;
    public bool isSliding;
    public bool antiClockFace = true;
    Gun gun;

    public float damageDealt = 0;
    public int finalBlows = 0;
    public int kicksMissed = 0;
    public int kicksHit = 0;
    public int healthPacksCollected = 0;
    public float avgSpeed = 0;
    public bool hasGotTheSolo = false;

    public float constantVelocity = 0;
    public float slideModifier = 0.35f;

    public Rigidbody2D rb;

    public GameObject body;
    public Animator ani;

    public bool RotationReady;
    public Vector2 gravityDirection;

    public float kickPunishment = 0;
    public float kickCooldown = 0;
    PauseMenu pauseMenu;

    [SerializeField] float min_velocity_to_start_skate = 0;

    private Gun originalGun;
    [SerializeField] Gun soloGun;
    private GameObject soloGunInstance;
    private bool soloActivated;
    private EventInstance grindingSound;


    [SerializeField] GameObject crown;
    // Start is called before the first frame update
    void Start()
    {
        Crown(false);

        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {

        ToggleFace();
        GetStickInput();
        CalculateAim();

        kickPunishment -= Time.deltaTime;
        kickCooldown -= Time.deltaTime;

        if (kickPunishment < 0) kickPunishment = 0;
        if (kickCooldown < 0) kickCooldown= 0;

        //Check for Solo
        if (!SongManager.instance.timelineInfo.soloActive && soloActivated) { StopSolo(); }


        aimPivot.transform.rotation = Quaternion.Lerp(aimPivot.transform.rotation, Quaternion.Euler(0, 0, stickValue.magnitude == 0 ? transform.rotation.eulerAngles.z : stickAngle), aimSmooth);
    }

    private void FixedUpdate()
    {
        if (constantVelocity > min_velocity_to_start_skate)
        {
            if (ani != null) ani.SetBool("HasMomentum", true);
        }
        else
        {
            if (ani != null) ani.SetBool("HasMomentum", false);
        }
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
                    //Sound
                    SoundManager.instance.PlayOneShot(FMODEvents.instance.weaponPickup, transform.position);

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
            //Start Grinding Sound
            grindingSound = SoundManager.instance.CreateEventInstance(FMODEvents.instance.grinding);
            grindingSound.start();

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
            //Stop Grinding Sound
            grindingSound.stop(STOP_MODE.IMMEDIATE);

            gravityDirection = Vector2.down;
            grounded = false;
            if (ani != null) ani.SetBool("OnGround", false);
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

    #region SOLO
    public void StartSolo()
    {
        soloActivated = true;
        Debug.Log("Solo");
        //Save old gun
        originalGun = gun;
        originalGun.gameObject.SetActive(false);
        
        soloGunInstance = Instantiate(soloGun.gameObject);
        gun = soloGunInstance.GetComponent<Gun>();
        soloGunInstance.GetComponent<Gun>().shooter = this;
        soloGunInstance.GetComponent<Collider2D>().enabled = false;
        soloGunInstance.transform.parent = aimPivot.transform;
        soloGunInstance.transform.localRotation = Quaternion.identity;
        soloGunInstance.transform.localPosition = new Vector3(0.8f, 0, 0);
    }

    public void StopSolo()
    {
        originalGun.gameObject.SetActive(true);
        gun = originalGun;
        soloGunInstance.SetActive(false);
    }
    #endregion

    #region Pause Menu

    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        if (pauseMenu.active)
        {
            GetComponent<PlayerInput>().currentActionMap = GetComponent<PlayerInput>().actions.FindActionMap(nameOrId: "Player");
            pauseMenu.active = false;
            pauseMenu.GetComponent<Animator>().SetTrigger("closeMenu");
            Time.timeScale = 1;
        }
        else
        {
            GetComponent<PlayerInput>().currentActionMap = GetComponent<PlayerInput>().actions.FindActionMap(nameOrId: "PauseMenu");
            pauseMenu.active = true;
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.GetComponent<Animator>().SetTrigger("openMenu");
        }
        
    }
    



    public void PauseMoveUp(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        pauseMenu.MoveSelectionUp();
    }
    public void PauseMoveDown(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        pauseMenu.MoveSelectionDown();
    }
    public void PauseSelect(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        pauseMenu.Select();
    }
    public void PauseBack(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        pauseMenu.Back();
    }




    #endregion



    public void Crown(bool hasCrown)
    {
        if (hasCrown) { crown.SetActive(true); }
        else { crown.SetActive(false); }
    }
}
