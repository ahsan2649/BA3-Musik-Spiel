using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Player> players;
    [SerializeField] float slideModifier;
    [SerializeField] float KickForce;
    [SerializeField] float BaseJumpForce;
    [SerializeField] float BaseGravity;
    [SerializeField] float AirGravity;
    [SerializeField] float RegionGravity;
    [SerializeField] float RegionJumpForce;
    [SerializeField] float topSpeed;
    [SerializeField] float drag;

    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var player in players)
        {
            player.rb = player.GetComponent<Rigidbody2D>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        foreach (var player in players)
        {
            player.body.transform.rotation = Quaternion.Lerp(player.body.transform.rotation, Quaternion.FromToRotation(Vector2.up, -player.gravityDirection), 0.25f);

            if (SongManager.instance.canKick && player.kick && player.aimDir != Player.AimDirection.none && player.grounded)
            {
                HandleKick(player);
                HandleAnim(player);
            }

            HandleVelocity(player);

            player.rb.AddForce(player.gravityDirection * player.rb.mass * (!player.grounded ? AirGravity : player.jumpRegion ? RegionGravity : BaseGravity));
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
                if (player.constantVelocity == 0)
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
                        player.ani.SetBool("DashSwitch", !player.ani.GetBool("DashSwitch"));
                        player.ani.SetTrigger("Kick");
                    }
                }
                else
                {
                    if (player.ani != null) player.ani.SetBool("Sliding", false);
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
        switch (player.aimDir)
        {
            case Player.AimDirection.outside:
                player.rb.velocity = Vector2.zero;
                player.constantVelocity = 0;
                break;
            case Player.AimDirection.inside:
                player.grounded = false;
                player.rb.AddForce(player.body.transform.up * (player.jumpRegion ? RegionJumpForce : BaseJumpForce) * player.rb.mass);
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
                if (player.constantVelocity == 0)
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
}
