using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class atPlayerC : MonoBehaviour
{
    public PlayersControls controls;

    public Transform enemy;
    public Transform target;
    public Transform miss1;
    public Transform miss2;
    public Transform hoop;
    public Transform ball;
    public Transform hands;
    public Transform rHand;
    public Transform lHand;
    public Transform aboveHeadPos;
    public Transform RdribblePos;
    public Transform LdribblePos;
    public Transform attPos;
    public Transform defPos;
    
    public GameObject powerBar;
    public GameObject rig;

    public AudioSource catchingS;
    public AudioSource throwS;
    public AudioSource swishS;

    private Transform p1StartPos;
    private Rigidbody rbBall;
    private Rigidbody rb;
    private Collider mainCol;
    private Collider colBall;
    float dirX, dirY;

    public float dribSpeed;
    public float moveSpeed;
    private Vector3 moveDir = Vector3.zero;
    float T = 0f;
    public float p1points = 0;
    float addPoints = 0;
    int LeftOrRight;
    int ScoreOrMiss;

    public bool ballInDrib;
    public bool ballRight = true;
    public bool ballLeft;
    public bool spacePressed;
    public bool inZone;
    public bool blocked;
    public bool isBlocking;
    public bool fallen;
    bool blockedShot;
    bool staminaDecreasing;
    
    bool ballIn1Hands;
    bool ballFlying;
    bool ballEnemy;

    public float stamina;
    float maxStamina;
    
    public float power;
    float maxPower = 100;
    bool powerIncreasing = true;
    bool powerBarON;

    public TextMeshProUGUI p1PointC;
    public Slider staminaBar;
    public Slider throwBar;
    public float dValue;

    private void Awake()
    {
        controls = new PlayersControls();
    }

    private void OnEnable()
    {
        controls.MainPlayer.Enable();

        controls.MainPlayer.Shoot.performed += OnAim;
        controls.MainPlayer.Shoot.canceled += OnShoot;
    }

    private void OnDisable()
    {
        controls.MainPlayer.Disable();
    }

    void Start()
    {
        colBall = ball.GetComponent<Collider> ();
        rbBall = ball.GetComponent<Rigidbody>();
        mainCol = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -15, 0);
        maxStamina = stamina;
        staminaBar.maxValue = maxStamina;
        powerBar.SetActive(false);
        p1StartPos = attPos;
        transform.position = new Vector3(p1StartPos.position.x, p1StartPos.position.y, p1StartPos.position.z);
    }

    void Update()
    {
        blocked = blockChecker1.blocked1;
        isBlocking = blockChecker2.blocked2;

        BlockingPose();

        Looking();

        Movement();

        Sprint();

        Ballin();

        BallFlying();

        if (ballFlying == false && ballIn1Hands == false && rbBall.constraints != RigidbodyConstraints.None)
        {
            ballEnemy = true;
        }
        else
        {
            ballEnemy = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, 0, moveDir.y * moveSpeed);
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        //start aiming
        power = 1;
        powerIncreasing = true;
        powerBarON = true;
        powerBar.SetActive(true);
        StartCoroutine(UpdatePowerBar());
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        //shooting anim
        powerBarON = false;
        throwS.Play();
        ScoreOrMiss = Random.Range(20, 100);
        LeftOrRight = Random.Range(0, 2);
        spacePressed = false;
        ballFlying = true;
        ballIn1Hands = false;
        T = 0;
        dribSpeed = 6;
        moveSpeed = 6;

        if (inZone)
            addPoints = 1;
        else
            addPoints = 2;

        if (blocked)
            blockedShot = true;
        else
            blockedShot = false;

        rbBall.constraints = RigidbodyConstraints.None;
        colBall.isTrigger = false;

        controls.MainPlayer.Shoot.Disable();
    }

    private void Movement()
    {
        //dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        //dirY = Input.GetAxisRaw("Vertical") * moveSpeed;

        moveDir = controls.MainPlayer.Move.ReadValue<Vector2>();
    }

    IEnumerator UpdatePowerBar()
    {
        while (powerBarON)
        {
            if (powerIncreasing)
            {
                power *= 1.3f;
                if (power >= maxPower)
                {
                    powerIncreasing = false;
                }
            }
            else
            {
                power /= 1.3f;
                if (power <= 1)
                {
                    powerIncreasing = true;
                }
            }
            
            throwBar.value = power;
            yield return new WaitForSeconds(0.02f);
        }
        yield return null;
    }


    private void BallFlying()
    {
        if (ballFlying)
        {
            T += Time.deltaTime;
            float duration = 0.8f;
            float t01 = T / duration;

            Vector3 A = aboveHeadPos.position;
            Vector3 B;

            if (power >= ScoreOrMiss && !blockedShot)
            {
                B = target.position;
            }
            else
            {
                if (LeftOrRight == 0)
                {
                    B = miss1.position;
                }
                else
                {
                    B = miss2.position;
                }
            }

            Vector3 pos = Vector3.Lerp(A, B, t01);

            Vector3 arc = Vector3.up * Mathf.Sin(t01 * 3.2f) * 5;
            ball.position = pos + arc;

            if (t01 >= 1)
            {
                ballFlying = false;
                if (B == target.position && !blockedShot)
                {
                    p1points += addPoints;
                    p1PointC.text = p1points.ToString();
                    swishS.Play();
                    roundSystem.Winner = 1;
                    roundSystem.RoundEnd = true;
                }
                ball.GetComponent<Rigidbody>().isKinematic = false;
                powerBar.SetActive(false);
            }
        }
    }

    private void Ballin()
    {
        if (ballIn1Hands)
        {
            controls.MainPlayer.Shoot.Enable();

            colBall.isTrigger = true;
            rbBall.constraints = RigidbodyConstraints.FreezePosition;
            
            if (controls.MainPlayer.Shoot.IsPressed())
            {
                //aiming anim
                spacePressed = true;
                ball.position = aboveHeadPos.position;
                moveSpeed = 2;

            }
            else
            {
                ball.position = RdribblePos.position;
                //dribbling
            }
        }
    }

    private void BlockingPose()
    {
        if (isBlocking && !ballIn1Hands && !ballFlying)
        {
            lHand.localEulerAngles = Vector3.forward * -80;
            rHand.localEulerAngles = Vector3.forward * 80;
        }
        else if (!isBlocking)
        {
            lHand.localEulerAngles = Vector3.forward * 0;
            rHand.localEulerAngles = Vector3.forward * 0;
        }
    }

    private void Looking()
    {
        if (ballIn1Hands)
        {
            transform.LookAt(new Vector3(hoop.position.x, transform.position.y, hoop.position.z));
        }
        else if (ballIn1Hands == false && ballEnemy == true)
        {
            transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
        }
        else
        {
            transform.LookAt(new Vector3(ball.position.x, transform.position.y, ball.position.z));
        }
    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                if (((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.D))) && spacePressed == false)
                {
                    stamina -= 2;
                }
            }
        }
        if (((Input.GetKeyDown(KeyCode.W)) || (Input.GetKeyDown(KeyCode.A)) || (Input.GetKeyDown(KeyCode.S)) || (Input.GetKeyDown(KeyCode.D))) && (Input.GetKey(KeyCode.LeftShift)) && !staminaDecreasing)
        {
            if (stamina > 0)
            {
                stamina -= 2;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (stamina > 0)
            {
                if (((Input.GetKey(KeyCode.W)) || (Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S)) || (Input.GetKey(KeyCode.D))) && spacePressed == false)
                {
                    moveSpeed = 12;
                    dribSpeed = 12;
                    DecreaseStam();
                    staminaDecreasing = true;
                }
                else
                {
                    if (stamina < maxStamina)
                    {
                        IncreaseStam();
                        staminaDecreasing = false;
                    }
                }
            }
            else
            {
                dribSpeed = 6;
                moveSpeed = 6;
            }

        }
        else
        {
            dribSpeed = 6;
            moveSpeed = 6;
            if (stamina < maxStamina)
            {
                IncreaseStam();
                staminaDecreasing = false;
            }
        }

        staminaBar.value = stamina;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ballT" && !ballIn1Hands && !ballFlying)
        {
            catchingS.Play();
            ballIn1Hands = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "zoneT")
        {
            inZone = true;
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "zoneT")
        {
            inZone = false;
        }
        
    }

    private void DecreaseStam()
    {
        if (stamina != 0)
        { 
            stamina -= dValue * Time.deltaTime;
        }
    }

    private void IncreaseStam()
    {
        stamina += dValue / 2 * Time.deltaTime;
    }
}