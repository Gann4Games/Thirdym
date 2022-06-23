using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gann4Games.Thirdym.Utility;
public class RagdollController : MonoBehaviour {

    [Tooltip("Object used to look at camera's direction")]
    public Transform facerTransform;
    public GameObject spawnParticle;
    public CheckGround enviroment;
    public bool isRagdollState;
    public bool invertBalanceDetection;
    public bool inverseVelocity;
    Transform bodyGuide;
    Vector3 axisMovement;

    [Header("Ragdoll Components")]
    [HideInInspector] public Transform guide;
    public Transform body;
    public Transform head;
    float AngleX
    {
        get
        {
            float ang = body.localEulerAngles.x;
            ang = (ang > 180) ? ang - 360 : ang;
            return ang;
        }
    }
    float bodyRotation;
    float bodySpring;
    float bodyDamp = 10;
    public Rigidbody[] bodyParts;
    public List<HingeJoint> LegsMotion;
    public HingeJoint[] rootBalancer;

    CharacterCustomization _character;
    [HideInInspector] public Rigidbody HeadRB, BodyRB;
    void Start () {
        _character = GetComponent<CharacterCustomization>();
        bodyParts = GetComponentsInChildren<Rigidbody>();
        HeadRB = head.GetComponent<Rigidbody>();
        BodyRB = body.GetComponent<Rigidbody>();

        guide = new GameObject("guide").transform;
        guide.rotation = body.rotation;
        bodyGuide = rootBalancer[0].transform;
        rootBalancer[0].autoConfigureConnectedAnchor = false;
    }
    private void FixedUpdate()
    {
        if(!_character.isNPC && !IngameMenuHandler.instance.paused && !_character.HealthController.IsDead)
        {
            if(enviroment.IsSwimming)
            {
                SwimInput();
            }
            else if (!enviroment.IsGrounded && !enviroment.IsSwimming)
            {
                AirInput();
            }
        }
    }
    float wallJumpTime;
    readonly float walljumpDelay = .1f;
    void Update () {
        axisMovement = new Vector3(PlayerInputHandler.instance.movementAxis.x, 0, PlayerInputHandler.instance.movementAxis.y); // Player movement

        _character.Animator.SetBool("Grounded", enviroment.IsGrounded || enviroment.IsSwimming);
        if (PlayerInputHandler.instance.firing)
            _character.Animator.SetFloat("Z_Velocity", 2);
        else
            _character.Animator.SetFloat("Z_Velocity", -ZVelocity);

        foreach (HingeJoint rootHinge in rootBalancer) //Hips HingeJoint parameters
        {
            JointSpring hingeSpring = rootHinge.spring;
            hingeSpring.spring = bodySpring;
            hingeSpring.damper = bodyDamp;
            hingeSpring.targetPosition = bodyRotation;
            rootHinge.spring = hingeSpring;
            rootHinge.useSpring = _character.HealthController.IsFullyAlive;
        }

        RagdollMode(!isRagdollState, isRagdollState);

        if (!_character.isNPC && !IngameMenuHandler.instance.paused)//If not using AI and is not paused
        {
            if (!_character.HealthController.IsDead)
            {
                if (PlayerInputHandler.instance.ragdolling) //Be a ragdoll
                {
                    isRagdollState = true;
                    RagdollInput();
                    MouseInput();
                }
                else if (_character.HealthController.IsFullyAlive && _character.HealthController.Unconcious == false)
                {
                    isRagdollState = false;
                    if (PlayerInputHandler.instance.ragdolling)
                        RagdollMode(true, false);
                    MouseInput();
                }
                if (PlayerInputHandler.instance.jumping) // Wall jumping
                {
                    if (wallJumpTime <= 0) PerfomWalljump();
                    else wallJumpTime -= Time.deltaTime;
                }
                else if (wallJumpTime != walljumpDelay)
                {
                    wallJumpTime = walljumpDelay;
                }

                if (enviroment.IsGrounded)
                {
                    if (PlayerInputHandler.instance.gameplayControls.Player.Jump.triggered && !PlayerInputHandler.instance.ragdolling && !jumping) // Basic jump
                        StartCoroutine(Jump());
                    bodyDamp = 10;

                    _character.Animator.SetBool("Crouch", PlayerInputHandler.instance.crouching);
                    _character.Animator.SetBool("Kicking", PlayerInputHandler.instance.kicking);

                    if (axisMovement != Vector3.zero && !PlayerInputHandler.instance.aiming && !isRagdollState)
                    {
                        if (enviroment.IsDraggingBody)
                        {
                            axisMovement = -axisMovement;
                            _character.Animator.SetFloat("Y", -.5f * axisMovement.magnitude);
                            _character.Animator.SetBool("Crouch", true);
                            bodyRotation = 0; //15;

                            //arms.RightShoulder[0].useSpring = false;
                            //arms.RightShoulder[1].useSpring = false;
                            //arms.RightBicep.useSpring = false;
                            //arms.RightElbow.useSpring = false;
                        }
                        else
                        {
                            _character.Animator.SetFloat("Y", PlayerInputHandler.instance.walking
                                ?
                                .25f * axisMovement.magnitude // Animation speed multiplied by left gamepad stick
                                :
                                .5f * axisMovement.magnitude
                                );
                            bodyRotation = PlayerInputHandler.instance.walking ? 0 : -15 * axisMovement.magnitude; // Radial movement
                        }

                        if (_character.Animator.GetFloat("X") != 0)
                            _character.Animator.SetFloat("X", 0);
                        guide.forward = Vector3.Lerp(guide.forward, guide.position + new Vector3(_character.CameraController.activeCamera.transform.TransformDirection(axisMovement).x, 0, _character.CameraController.activeCamera.transform.TransformDirection(axisMovement).z), Time.deltaTime * 10);

                        SimpleBalance();
                    }
                    else if (PlayerInputHandler.instance.aiming)
                    {
                        Vector3 camDir = _character.CameraController.activeCamera.transform.forward;
                        camDir.y = 0;
                        guide.forward = Vector3.Lerp(guide.forward, camDir, Time.deltaTime * 10);

                        _character.Animator.SetFloat("Y", (PlayerInputHandler.instance.walking && axisMovement.z > 0)
                            ?
                            (axisMovement.z / 2) / 2 * axisMovement.magnitude
                            :
                            axisMovement.z / 2 * axisMovement.magnitude
                            );
                        _character.Animator.SetFloat("X", axisMovement.x / 2);
                        bodyRotation = PlayerInputHandler.instance.walking ? 0 : 15 * (-axisMovement.z); // Forward and backwards movement

                        SimpleBalance();
                    }
                    else
                    {
                        if (AngleX < 45 && AngleX > -45 && isRagdollState == false)
                            Balance();
                        else
                        {/*
                            if (Input.GetKey(KeyCode.X)) // Pointless code
                                RagdollMode(false, true);
                            else
                                RagdollMode(false, true);*/
                            RagdollMode(false, true);
                        }
                    }
                }
            }
        }
        else if (IngameMenuHandler.instance.paused)
        {
            _character.Animator.SetFloat("Y", 0);
            _character.Animator.SetFloat("X", 0);
            Balance();
        }
        else if (_character.isNPC)
        {
            _character.NPC.stateMachine.RunStateMachine();
        }
    }

    public float ZVelocity => inverseVelocity ? -body.InverseTransformDirection(BodyRB.velocity).z : body.InverseTransformDirection(BodyRB.velocity).z;
    public Vector3 bodyVelocity => BodyRB.velocity;

    public bool jumping;
    public IEnumerator Jump()
    {
        if (enviroment.IsGrounded)
        {
            wallJumpTime = walljumpDelay;
            float jumpForce = 120;
            float num = 50 / 15;

            BodyRB.velocity = jumpForce * Vector3.up;
            BodyRB.velocity -= num * bodyRotation * rootBalancer[0].transform.forward;

            jumping = true;
        }
        yield return new WaitForSeconds(1);
        jumping = false;
    }
    void PerfomWalljump()
    {
        if (!(Mathf.Abs(ZVelocity) > 1 && enviroment.IsGrounded) 
            || !_character.WalljumpController.canWallJump) return;

        wallJumpTime = walljumpDelay;

        Vector3 jumpDirection = _character.WalljumpController.bounceDirection.normalized*4;
        jumpDirection.y = 5;
        jumpDirection *= 1 + Time.deltaTime;

        guide.forward = new Vector3(jumpDirection.x, 0, jumpDirection.z);

        SetCharacterVelocity(jumpDirection);
    }

    /// <summary>
    /// Set the velocity of every rigidbody in the ragdoll.
    /// </summary>
    void SetCharacterVelocity(Vector3 direction) 
    {
        foreach(Rigidbody part in bodyParts)
        {
            part.velocity = direction;
        }
    }

    /// <summary>
    /// Sets the ragdoll animation and weight
    /// </summary>
    /// <param name="useAnimations">Checks if bodyparts should be animated</param>
    /// <param name="disableWeight">Checks if body weight should be disabled</param>
    public void RagdollMode(bool useAnimations, bool disableWeight, float transitionSpeed = 1) {
        if (disableWeight)
        {
            bodyDamp = 0;
            bodySpring = 0;
        }
        for(int i = 0; i < LegsMotion.Count; i++)
        {
            if (LegsMotion[i] == null)
                continue;
            float springStrength = useAnimations ? bodySpring : 0;
            float lerpValue = Mathf.Lerp(LegsMotion[i].spring.spring, springStrength, Time.deltaTime*transitionSpeed);
            LegsMotion[i].spring = PhysicsTools.SetHingeJointSpring(LegsMotion[i].spring, lerpValue);
        }
    }
    
    void RagdollInput()
    {
        bodySpring = 100;
        bodyRotation = -PlayerInputHandler.instance.movementAxis.y* 90;
    }
    public void AirInput()
    {
        bodyDamp = 0;
        float maxVel = 30;
        if (Mathf.Abs(ZVelocity) > maxVel /*|| anim.GetFloat("Z_Velocity") < -maxVel*/)
        {
            bodyRotation = -90;
            bodySpring = 10;
        } else
        {
            bodyRotation = ZVelocity;
            bodySpring = 500;
        }
        if (PlayerInputHandler.instance.movementAxis.y > 0)
        {
            HeadRB.AddForce(rootBalancer[0].transform.forward * 500 * Time.deltaTime);
            HeadRB.AddForce(HeadRB.transform.forward * 1000 * Time.deltaTime);
        }
        if (PlayerInputHandler.instance.movementAxis.y < 0)
        {
            HeadRB.AddForce(rootBalancer[0].transform.forward * -500 * Time.deltaTime);
            HeadRB.AddForce(HeadRB.transform.forward * -1000 * Time.deltaTime);
        }
        if (PlayerInputHandler.instance.movementAxis.x < 0)
            HeadRB.AddForce(rootBalancer[0].transform.right * -500 * Time.deltaTime);
        if (PlayerInputHandler.instance.movementAxis.x > 0)
            HeadRB.AddForce(rootBalancer[0].transform.right * 500 * Time.deltaTime);
    }
    public void SwimInput()
    {
        bodySpring = 100;
        bodyDamp = 10;

        //if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        if(PlayerInputHandler.instance.movementAxis == Vector2.zero)
        {
            RagdollMode(false, false);
        } else
        {
            //RagdollMode(true, false);
            isRagdollState = false;
        }

        guide.eulerAngles = Vector3.Lerp(guide.eulerAngles, new Vector3(0, _character.CameraController.activeCamera.transform.eulerAngles.y, 0), Time.deltaTime*2);

        if (PlayerInputHandler.instance.movementAxis.y>0)
        {
            _character.Animator.SetFloat("Y", 0.5f*PlayerInputHandler.instance.movementAxis.magnitude);
            HeadRB.AddForce(rootBalancer[0].transform.forward * 2500 * Time.deltaTime);
        }
        else if (PlayerInputHandler.instance.movementAxis.y<0)
        {
            _character.Animator.SetFloat("Y", 0.5f* PlayerInputHandler.instance.movementAxis.magnitude);
            HeadRB.AddForce(rootBalancer[0].transform.forward * -2500 * Time.deltaTime);
        }
        else
        {
            _character.Animator.SetFloat("Y", 0.25f* PlayerInputHandler.instance.movementAxis.magnitude);
        }
        if (PlayerInputHandler.instance.movementAxis.x<0)
            HeadRB.AddForce(rootBalancer[0].transform.right * -2500 * Time.deltaTime);
        if (PlayerInputHandler.instance.movementAxis.x>0)
            HeadRB.AddForce(rootBalancer[0].transform.right * 2500 * Time.deltaTime);
        if (PlayerInputHandler.instance.jumping)
        {
            HeadRB.AddForce(rootBalancer[0].transform.up * 3000 * Time.deltaTime);
            bodyRotation = 0;
        }
        else if (PlayerInputHandler.instance.crouching)
        {
            HeadRB.AddForce(rootBalancer[0].transform.up * -3000 * Time.deltaTime);
            bodyRotation = -90;
        } else
        {
            if (PlayerInputHandler.instance.movementAxis.y>0)
                bodyRotation = -45;
            else if (PlayerInputHandler.instance.movementAxis.y<0)
                bodyRotation = 45;
            else
                bodyRotation = -15;
        }
    }
    public void Balance()
    {
        bodyRotation = 0;
        if (AngleX > 15)
        {
            bodySpring = 500;

            if (invertBalanceDetection)
                _character.Animator.SetFloat("Y", 0.5f);
            else
                _character.Animator.SetFloat("Y", -0.5f);
        }
        else if (AngleX < -15)
        {
            bodySpring = 500;

            if (invertBalanceDetection)
                _character.Animator.SetFloat("Y", -0.5f);
            else
                _character.Animator.SetFloat("Y", 0.5f);
        }
        else
        {
            bodySpring = 500;

            _character.Animator.SetFloat("Y", 0);
        }
        if (AngleX > 45 || AngleX < -45)
            bodySpring = 0;
    }
    public void SimpleBalance()
    {
        if (AngleX > 45 || AngleX < -45)
            bodySpring = 250;
        else if (isRagdollState == false)
            bodySpring = 500;
    }
    public void AISelfBalance()
    {
        SimpleBalance();
        bodyRotation = -15 * _character.Animator.GetFloat("Y");
    }
    public void MouseInput()
    {
        bodyGuide.rotation = guide.rotation;
        if (PlayerInputHandler.instance.aiming) Zoom(!_character.EquipmentController.disarmed);
        else Zoom(false);
    }
    public void Zoom(bool zoom)
    {
        if(!_character.isNPC) MainHUDHandler.instance.crosshair.gameObject.SetActive(zoom);
    }
    
    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            GameObject spawnedParticle = Instantiate(spawnParticle);
            spawnedParticle.transform.position = transform.position;
        }
    }
    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            GameObject spawnedParticle = Instantiate(spawnParticle);
            spawnedParticle.transform.position = transform.position;
        }
    }
}
