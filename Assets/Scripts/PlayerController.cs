using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Inputs inputs;

    struct RaySrc { public Vector2 bl, br, tl, tr; }
    struct ColSrc
    {
        public bool a, b, l, r;
        public bool slopeAsc, slopeDesc;
        public float slopeAngle, oldSlopeAngle;
        public Vector3 oldVel;
        public void ResetColInfo()
        {
            a = b = l = r = false;
            slopeAsc = slopeDesc = false;
            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }

    }
    [Header("Collisions")]
    RaySrc raySrc;
    ColSrc colSrc;
    BoxCollider2D col;
    float skin = 0.015f;
    [SerializeField] LayerMask colMask;
    [SerializeField] int horizontalRays;
    [SerializeField] int verticalRays;
    float horizontalRaySpacing;
    float verticalRaySpacing;

    [Header("Jump")]
    [SerializeField] float jumpHeight;
    [SerializeField] float jumpTime;
    float gravity;
    float jumpSpeed;

    [Header("Roll")]
    [SerializeField] float rollSpeed;
    [SerializeField] float maxAscAngle;
    [SerializeField] float maxDescAngle;

    uint momentum;

    [Header("Aim")]
    [SerializeField] GameObject aimNose;
    Vector2 aim;
    float aimAngle;
    enum AimDirs { u,d,l,r,n };
    AimDirs aimDir;
    bool facingRight;
    Vector3 velocity;
    bool sliding;

    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;

        inputs = new Inputs();
        inputs.Player.Enable();

        col = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpTime, 2);
        jumpSpeed = Mathf.Abs(gravity) * jumpTime;

        print("gravity: " + gravity + ", jump speed: " + jumpSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        aim = inputs.Player.Aim.ReadValue<Vector2>();
        aimAngle = aim.magnitude == 0 ? float.NaN : Vector2.SignedAngle(aim, transform.right);
        CalculateAimDirs();

        

        if (colSrc.b) velocity.y = 0;

        if(colSrc.b && inputs.Player.Kick.WasPressedThisFrame())
        {
            switch (aimDir)
            {
                case AimDirs.u:
                    velocity.y = jumpSpeed;
                    break;
                case AimDirs.d:
                    velocity.x = 0;
                    momentum = 0;
                    break;
                case AimDirs.l:
                case AimDirs.r:
                    if (velocity.x == 0) velocity.x = rollSpeed * (aimDir == AimDirs.l ? -1 : 1);
                    bool sameDirectionInput = false;
                    if (aimDir == AimDirs.r && velocity.x > 0) sameDirectionInput = true;
                    if (aimDir == AimDirs.l && velocity.x < 0) sameDirectionInput = true;

                    if (sameDirectionInput)
                    {
                        momentum++;
                        momentum = (uint)Mathf.Clamp(momentum, 0, 3);
                        velocity.x = rollSpeed * momentum * Mathf.Sign(velocity.x);
                    }
                    else
                    {
                        velocity.x = -velocity.x;
                    }
                    break;
            }
        }


        velocity.y += gravity * Time.deltaTime;
        ExecuteMove(velocity * Time.deltaTime);

        
    }



    void CalculateAimDirs()
    {
        if (aim.magnitude == 0) aimDir = AimDirs.n;
        if (45 <= aimAngle && aimAngle < 135) aimDir = AimDirs.d;
        else if (135 <= aimAngle || aimAngle <= -135) aimDir = AimDirs.l;
        else if (45 > aimAngle && aimAngle >= -45) aimDir = AimDirs.r;
        else if (-45 > aimAngle && aimAngle > -135) aimDir = AimDirs.u;
    }

    void ExecuteMove(Vector3 vel)
    {
        UpdateRaySources();
        colSrc.ResetColInfo();

        colSrc.oldVel = vel;

        if (vel.y < 0) SlopeDesc(ref vel);
        if (vel.y != 0) AdjustVertical(ref vel);
        if (vel.x != 0) AdjustHorizontal(ref vel);


        transform.Translate(vel);
    }

    void AdjustVertical(ref Vector3 vel)
    {
        float dirY = Mathf.Sign(vel.y);
        float rayLen = Mathf.Abs(vel.y) + skin;
        for (int i = 0; i < verticalRays; i++)
        {
            Vector2 rayOrg = (dirY == -1) ? raySrc.bl : raySrc.tl;
            rayOrg += Vector2.right * (verticalRaySpacing * i + vel.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrg, Vector2.up * dirY, rayLen, colMask);
            Debug.DrawRay(rayOrg, Vector2.up * dirY * rayLen, Color.green);
            if (hit)
            {
                vel.y = (hit.distance - skin) * dirY;
                rayLen = hit.distance;

                if(colSrc.slopeAsc)
                {
                    vel.x = vel.y / Mathf.Tan(colSrc.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(vel.x);
                }

                colSrc.b = dirY == -1;
                colSrc.a = dirY == 1;
            }
        }

        if(colSrc.slopeAsc)
        {
            float dirX = Mathf.Sign(vel.x);
            rayLen = Mathf.Abs(vel.x) + skin;
            Vector2 rayOrg = ((dirX == -1) ? raySrc.bl : raySrc.br);
            RaycastHit2D hit = Physics2D.Raycast(rayOrg, Vector2.right * dirX, rayLen, colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != colSrc.slopeAngle)
                {
                    vel.x = (hit.distance - skin) * dirX;
                    colSrc.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void AdjustHorizontal(ref Vector3 vel)
    {
        float dirX = Mathf.Sign(vel.x);
        float rayLen = Mathf.Abs(vel.x) + skin;
        for (int i = 0; i < horizontalRays; i++)
        {
            Vector2 rayOrg = (dirX == -1) ? raySrc.bl : raySrc.br;
            rayOrg += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrg, Vector2.right * dirX, rayLen, colMask);
            Debug.DrawRay(rayOrg, Vector2.right * dirX * rayLen, Color.green);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if(i == 0 && slopeAngle <= maxAscAngle)
                {
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != colSrc.oldSlopeAngle)
                    {
                        distanceToSlopeStart = hit.distance - skin;
                        vel.x -= distanceToSlopeStart * dirX;
                    }
                    SlopeAsc(ref vel, slopeAngle);
                    vel.x += distanceToSlopeStart * dirX;
                }

                if(!colSrc.slopeAsc || slopeAngle > maxAscAngle)
                {
                    vel.x = (hit.distance - skin) * dirX;
                    rayLen = hit.distance;

                    if (colSrc.slopeAsc)
                    {
                        vel.y = Mathf.Tan(colSrc.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(vel.x);
                    }

                    colSrc.l = dirX == -1;
                    colSrc.r = dirX == 1;

                }

            }
        }
    }

    void SlopeAsc(ref Vector3 vel, float slopeAngle)
    {
        float horizontalMoveDistance = Mathf.Abs(vel.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * horizontalMoveDistance;

        if(vel.y <= climbVelocityY)
        {
            vel.y = climbVelocityY;
            vel.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * horizontalMoveDistance * Mathf.Sign(vel.x);
            colSrc.b = true;
            colSrc.slopeAsc = true;
            colSrc.slopeAngle = slopeAngle;

        }
    }

    void SlopeDesc(ref Vector3 vel)
    {
        float dirX = Mathf.Sign(vel.x);
        Vector2 rayOrg = (dirX == -1) ? raySrc.br : raySrc.bl;
        RaycastHit2D hit = Physics2D.Raycast(rayOrg, -Vector2.up, Mathf.Infinity, colMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxDescAngle)
            {
                if(Mathf.Sign(hit.normal.x) == dirX)
                {
                    if(hit.distance - skin <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(vel.x))
                    {
                        float moveDist = Mathf.Abs(vel.x);
                        float descVelY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDist;
                        vel.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(vel.x);
                        vel.y -= descVelY;

                        colSrc.slopeAngle = slopeAngle;
                        colSrc.slopeDesc = true;
                        colSrc.b = true;
                    }
                }
            }
        }
    }

    #region Raycast Calculations
    
    void UpdateRaySources()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skin * -2);

        raySrc.bl = new Vector2(bounds.min.x, bounds.min.y);
        raySrc.br = new Vector2(bounds.max.x, bounds.min.y);
        raySrc.tl = new Vector2(bounds.min.x, bounds.max.y);
        raySrc.tr = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = col.bounds;
        bounds.Expand(skin * -2);

        horizontalRays = Mathf.Clamp(horizontalRays, 2, int.MaxValue);
        verticalRays = Mathf.Clamp(verticalRays, 2, int.MaxValue);


        horizontalRaySpacing = (bounds.size.y) / (horizontalRays - 1);
        verticalRaySpacing = (bounds.size.x) / (verticalRays - 1);
    }

    #endregion


}
