using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Player> players;
    [SerializeField] List<GameObject> characters;
    [SerializeField] float slideModifier;
    [SerializeField] float KickForce;
    [SerializeField] float BaseJumpForce;
    [SerializeField] float BaseGravity;
    [SerializeField] float AirGravity;
    [SerializeField] float RegionGravity;
    [SerializeField] float RegionJumpForce;
    [SerializeField] float topSpeed;
    [SerializeField] float drag;
    [SerializeField] Animator backgroundAnimator;
    [SerializeField] float health;
    float crownHealth = 0;
    Player crownPlayer;
    [SerializeField] GameObject missParticle;
    [SerializeField] float kickPunishment;
    [SerializeField] float minimunSlidingSpeed = 0;
    [SerializeField] float kickCooldown;




    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("PlayerCount"));

        for (int i = 0; i < PlayerPrefs.GetInt("PlayerCount"); i++)
        {
            GetComponent<PlayerInputManager>().playerPrefab = characters[PlayerPrefs.GetInt("Player"+i.ToString()+"Char")];
            var newPlayer = GetComponent<PlayerInputManager>().JoinPlayer(i, -1, null, Gamepad.all.ToList().Find(gp => gp.deviceId == PlayerPrefs.GetInt("Player" + i.ToString() + "Gamepad")));
            newPlayer.gameObject.transform.position = spawnPoints[i].transform.position;
            newPlayer.GetComponent<Player>().health = health;
            players.Add(newPlayer.GetComponent<Player>());

        }
        
        if (players.Count == 0) return;

        foreach (var player in players)
        {
            player.rb = player.GetComponent<Rigidbody2D>();
            player.gravityDirection = Vector2.down;
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var player in players)
        {
            player.body.transform.rotation = Quaternion.Lerp(player.body.transform.rotation, Quaternion.FromToRotation(Vector2.up, -player.gravityDirection), 0.25f);

            player.health = Mathf.Clamp(player.health, 0, health);

            if (player.health <= 0)
            {
                
                player.gameObject.SetActive(false);
            }

            if (player.kick && player.grounded && !SongManager.instance.canKick)
            {
                player.kickPunishment = kickPunishment;
                if (missParticle != null) 
                {
                    Instantiate(missParticle, player.transform.position, Quaternion.identity, player.gameObject.transform);
                }
                player.kicksMissed += 1;
            }
        }

        if (players.Count > 1 && players.Count(player => player.gameObject.activeSelf == true) == 1)
        {
            LevelManager.levelManager.phase = LevelManager.Phase.Result;
        }

    }

    private void FixedUpdate()
    {
        if (players.Count == 0) return;

        if(LevelManager.levelManager.phase == LevelManager.Phase.Starting)
        {
            foreach (var player in players)
            {
                if (player.aimDir != Player.AimDirection.none) player.RotationReady = true;
            }
            if (players.All(player => player.RotationReady == true && player.kick && SongManager.instance.canKick && player.kickPunishment <= 0 && player.kickCooldown <= 0))
            {
                LevelManager.levelManager.phase = LevelManager.Phase.Playing;
                backgroundAnimator.Play("Background Opening");
            }
        }


        if(LevelManager.levelManager.phase == LevelManager.Phase.Playing)
        {
            foreach (var player in players)
            {

                if (SongManager.instance.canKick && player.kick && player.aimDir != Player.AimDirection.none && player.grounded && player.kickPunishment <= 0 && player.kickCooldown <= 0)
                {
                    HandleKick(player);
                    HandleAnim(player);
                }

                HandleVelocity(player);
                
                player.rb.AddForce(player.gravityDirection * player.rb.mass * (!player.grounded ? AirGravity : player.jumpRegion ? RegionGravity : BaseGravity));
            }
        }
    }

    private void LateUpdate()
    {
        if (players.Count == 0) return;

        foreach (var player in players)
        {
            if (player.grounded)
            {
                player.rb.velocity = player.rb.velocity.normalized * player.constantVelocity;
            }
        }
    }

    public void HandleAnim(Player player)
    {
        switch (player.aimDir)
        {
            case Player.AimDirection.outside:
                if (player.ani != null) player.ani.SetTrigger("Brake");
                break;
            case Player.AimDirection.inside:
                if (player.ani != null)
                {
                    player.ani.SetBool("OnGround", false);
                    player.ani.SetTrigger("Jump");
                }
                break;
            case Player.AimDirection.front:
                if (player.isSliding && player.ani != null)
                {
                    player.ani.SetBool("Sliding", false);
                    player.ani.SetBool("DashSwitch", !player.ani.GetBool("DashSwitch"));
                    player.ani.SetTrigger("Kick");
                }

                if (!player.isSliding && player.ani!= null)
                {
                    player.ani.SetBool("DashSwitch", !player.ani.GetBool("DashSwitch"));
                    player.ani.SetTrigger("Kick");
                }
                break;
            case Player.AimDirection.back:
                if (player.constantVelocity <= minimunSlidingSpeed)
                {
                    if (player.ani != null)
                    {
                        player.ani.SetBool("DashSwitch", !player.ani.GetBool("DashSwitch"));
                        player.ani.SetTrigger("Kick");
                    }
                }
                else if (!player.isSliding)
                {
                    if (player.ani != null)
                    {
                        player.ani.SetBool("Sliding", true);
                        player.ani.SetTrigger("SlidingTrigger");
                    }
                }
                else
                {
                    if (player.ani != null) player.ani.SetBool("Sliding", false);
                    player.ani.SetBool("DashSwitch", !player.ani.GetBool("DashSwitch"));
                    player.ani.SetTrigger("Kick");
                }
                break;
            case Player.AimDirection.none:
                break;
            default:
                break;
        }
    }

    public void HandleVelocity(Player player)
    {
        if (player.grounded) //second check is here so that after a jump the velocity is not checked
        {

            if (player.rb.velocity.magnitude > topSpeed) player.rb.velocity = Vector2.ClampMagnitude(player.rb.velocity, topSpeed);

            player.constantVelocity -= drag * Time.fixedDeltaTime;
            if (player.constantVelocity < 0)
            {
                player.constantVelocity = 0;
            }
            if (player.constantVelocity > topSpeed)
            {
                player.constantVelocity = topSpeed;
            }

        }
    }

    public void HandleKick(Player player)
    {
        player.kicksHit += 1;
        player.kick = false;
        player.kickCooldown += kickCooldown;
        switch (player.aimDir)
        {
            case Player.AimDirection.outside:
                player.rb.velocity = Vector2.zero;
                player.constantVelocity = 0;
                break;
            case Player.AimDirection.inside:
                player.grounded = false;
                player.rb.AddForce(player.body.transform.up * (player.jumpRegion ? RegionJumpForce : BaseJumpForce) * player.rb.mass, ForceMode2D.Impulse);
                break;
            case Player.AimDirection.front:
                if (player.isSliding)
                {
                    player.isSliding = false;
                    player.constantVelocity /= slideModifier;
                    player.rb.velocity = player.rb.velocity.normalized * player.constantVelocity;
                    
                }
                else
                {
                    player.rb.AddForce((player.antiClockFace ? player.body.transform.right : -player.body.transform.right) * KickForce);
                    player.constantVelocity += KickForce;
                }
                break;
            case Player.AimDirection.back:
                if (player.constantVelocity <= minimunSlidingSpeed)
                {
                    player.antiClockFace = !player.antiClockFace;
                    player.rb.AddForce((player.antiClockFace ? player.body.transform.right : -player.body.transform.right) * KickForce);
                    player.constantVelocity += KickForce;

                    
                }
                else if (!player.isSliding)
                {
                    player.isSliding = true;
                    player.constantVelocity *= slideModifier;
                    
                }
                else
                {
                    player.isSliding = false;
                    player.constantVelocity /= slideModifier;
                    player.rb.velocity = -player.rb.velocity.normalized * player.constantVelocity;
                }
                break;
            case Player.AimDirection.none:
                break;
            default:
                break;
        }
    }

    public void HandleCrown()
    {
        foreach(Player player in players)
        {
            
            if (crownHealth < player.health)
            {
                if (crownPlayer != null)
                {
                    crownPlayer.Crown(false);
                }
                crownPlayer = player;
                crownHealth = player.health;
            }
            else if (crownHealth == player.health)
            {
                if (crownPlayer != null)
                {
                    crownPlayer.Crown(false);
                }
                crownPlayer = null;
            }
            
        }
        if (crownPlayer != null)
        {
            crownPlayer.Crown(true);
        }
    }
}
