using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMovement : MonoBehaviour
{
    [Header("Initialisations")]
    [SerializeField] private float initialRunSpeed;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private bool isLeftMoveDirection;

    [Header("Movement values")]
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float wallGravityModifier = .4f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float initialWallJumpPower = 3f;
    [SerializeField] private float walljumpPowerGain = 1f;
    [SerializeField] private float maxWallJumpPower = 10f;
    [SerializeField] private float wallBurstRunSpeed = 3f;
    [SerializeField] private float maxInifinteVolocity = 4f;
    [SerializeField] private float hitWallVelocityTransfer = .25f;

    [Header("Relevant character to world info")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private float checkDistance;
    [SerializeField] private LayerMask groundCheckMask;
    [SerializeField] private LayerMask wallCheckMask;
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;

    private CharacterController controller;
    private const float swapDirectionCooldown = 0.3f; //to prevent it from getting stuck on a wall
    private float speed;
    private float wallPower;
    private Vector3 playerVelocity;
    [SerializeField] private bool playerGrounded;
    [SerializeField] private bool wasLastWallLeft;

    private bool isDirectionSwapAvailable = true;
    private Coroutine lastRoutine;
    private Coroutine walljumpRoutine;
    private bool hitAWall;
    private bool startedWallJump;
    private bool wallStateChange;
    private bool oldHitAWall;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        speed = initialRunSpeed;
        wallStateChange = false;
        oldHitAWall = false;
    }

    private void Update()
    {
        hitAWall = WallChecks(); //determine which side was hit last
        wallStateChange = oldHitAWall == hitAWall; //will both get the boost when wall is hit and when wall is left but that is fine for now
        Move(); //move towards the side that wasn't hit last
        ApplyGravity();
        oldHitAWall = hitAWall;
    }

    private void Move()
    {
        Vector3 movement = Vector3.zero;
        if (isLeftMoveDirection)
        {
            movement = new Vector3(-speed * Time.deltaTime, 0,0);

        }
        else
        {
            movement = new Vector3(speed * Time.deltaTime, 0, 0);
        }
        Jump();

        if(hitAWall && !wallStateChange)
        {
            float velocityTransfer = playerVelocity.magnitude * hitWallVelocityTransfer;
            playerVelocity.y += velocityTransfer;
        }

        if (playerGrounded)
        {
            if (isDirectionSwapAvailable)
            {
                if (hitAWall)
                {
                    isLeftMoveDirection = !isLeftMoveDirection;
                    StartCoroutine(SwapDirectionCooldown(swapDirectionCooldown));
                }
            }
        }
        controller.Move(movement);
        
    }

    private void Jump()
    {
        
        if (playerGrounded)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                playerVelocity.y += jumpPower;
            }
        }
        if (hitAWall)
        {
            if (Input.GetButton("Fire1") && !startedWallJump)
            {
                startedWallJump = true;
                walljumpRoutine = StartCoroutine(WallPowerCounter());
            }
            if (Input.GetButtonUp("Fire1"))
            {
                StopCoroutine(walljumpRoutine);
                //excute wall jump
                if (wasLastWallLeft)
                {
                    playerVelocity.x += jumpPower;
                }
                else
                {
                    playerVelocity.x -= jumpPower;
                }

                //Debug.Log("Jumped off wall with  " + wallPower + "  power");
                startedWallJump = false;

            }
        }
    }

    private bool WallChecks()
    {
        if(Physics.CheckSphere(leftWallCheck.position, checkDistance, wallCheckMask))
        {
            wasLastWallLeft = true;
            return true;
        }
        if(Physics.CheckSphere(rightWallCheck.position, checkDistance, wallCheckMask))
        {
            wasLastWallLeft = false;
            return true;
        }
        return false;
    }

    private void ApplyGravity()
    {
        if (hitAWall)
        {
            playerVelocity.y += gravity * Time.deltaTime * wallGravityModifier;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        playerGrounded = Physics.CheckSphere(groundCheckPos.position, checkDistance, groundCheckMask);
        if (playerGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -1f;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void VelocityDeterioration()
    {
        //return if velocity shoulnd't deteriorate
        if(playerVelocity.magnitude < maxInifinteVolocity)
        {
            return;
        }
        float newMagnitude = playerVelocity.magnitude / 4 * Time.deltaTime; //find some better equation for this in the future
        playerVelocity = playerVelocity.normalized * newMagnitude;

    }

    IEnumerator SwapDirectionCooldown(float seconds)
    {
        isDirectionSwapAvailable = false;
        yield return new WaitForSeconds(seconds);
        isDirectionSwapAvailable = true;
    }

    IEnumerator WallPowerCounter()
    {
        wallPower = initialWallJumpPower;
        playerVelocity.y += wallBurstRunSpeed; //only applied once when a wall is first hit while holding down the jump button
        while (true)
        {
            yield return new WaitForFixedUpdate();

            wallPower += walljumpPowerGain * Time.deltaTime;
            if (wallPower > maxWallJumpPower) wallPower = maxWallJumpPower;
        }
    }
}
