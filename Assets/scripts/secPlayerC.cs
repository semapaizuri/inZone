using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class secPlayerC : MonoBehaviour
{

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
    private Transform p2StartPos;

    public GameObject powerBar;

    public AudioSource catchingS;
    public AudioSource throwS;
    public AudioSource swishS;

    private Rigidbody rbBall;
    private Rigidbody rb;
    private Collider colBall;
    float dirX, dirY;

    public float dribSpeed;
    public float moveSpeed;
    float T = 0f;
    public float p2points = 0;
    float addPoints = 0;
    int LorR;
    int SorM;

    public bool altPressed = false;
    public bool inZone;
    public bool blocked;
    public bool isBlocking;
    bool blockedShot;
    bool staminaDecreasing;

    bool ballIn2Hands = false;
    bool ballFlying = false;
    bool ballEnemy = true;

    public float stamina;
    float maxStamina;

    public float power;
    float maxPower = 100;
    bool powerIncreasing = true;
    bool powerBarON = false;

    public TextMeshProUGUI p2PointC;
    public Slider staminaBar;
    public Slider throwBar;
    public float dValue;



    // Start is called before the first frame update
    void Start()
    {

        colBall = ball.GetComponent<Collider>();
        rbBall = ball.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -15, 0);
        maxStamina = stamina;
        staminaBar.maxValue = maxStamina;
        powerBar.SetActive(false);
        p2StartPos = defPos;
        transform.position = new Vector3(p2StartPos.position.x, p2StartPos.position.y, p2StartPos.position.z);
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


    // Update is called once per frame
    void Update()
    {
        blocked = blockChecker2.blocked2;
        isBlocking = blockChecker1.blocked1;

        if (isBlocking && !ballIn2Hands && !ballFlying)
        {
            lHand.localEulerAngles = Vector3.forward * -80;
            rHand.localEulerAngles = Vector3.forward * 80;
        }
        else if (!isBlocking)
        {
            lHand.localEulerAngles = Vector3.forward * 0;
            rHand.localEulerAngles = Vector3.forward * 0;
        }

        if (ballFlying == false && ballIn2Hands == false && rbBall.constraints != RigidbodyConstraints.None)
        {
            ballEnemy = true;
        }
        else
        {
            ballEnemy = false;
        }

        //looking
        if (ballIn2Hands)
        {
            transform.LookAt(new Vector3(hoop.position.x, transform.position.y, hoop.position.z));
        }
        else if (ballIn2Hands == false && ballEnemy == true)
        {
            transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
        }
        else
        {
            transform.LookAt(new Vector3(ball.position.x, transform.position.y, ball.position.z));
        }

        Movement();

        Sprint();

        if (ballIn2Hands)
        {
            colBall.isTrigger = true;
            rbBall.constraints = RigidbodyConstraints.FreezePosition;
            //pressing alt
            if (Input.GetKeyDown(KeyCode.RightAlt))
            {
                power = 1;
                powerIncreasing = true;
                powerBarON = true;
                powerBar.SetActive(true);
                StartCoroutine(UpdatePowerBar());
            }
            if (Input.GetKey(KeyCode.RightAlt))
            {
                altPressed = true;
                ball.position = aboveHeadPos.position;
                hands.localEulerAngles = Vector3.left * 180;
                rHand.localEulerAngles = Vector3.left * 0;
                rHand.localEulerAngles = Vector3.left * 20;
                moveSpeed = 2;
            }
            //dribbling
            else
            {
                ball.position = RdribblePos.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * dribSpeed));
                rHand.localEulerAngles = Vector3.left * Mathf.Abs(Mathf.Sin(Time.time * dribSpeed)) * 20;

            }
            //releasing alt
            if (Input.GetKeyUp(KeyCode.RightAlt))
            {
                powerBarON = false;
                throwS.Play();
                SorM = Random.Range(20, 100);
                LorR = Random.Range(0, 2);
                hands.localEulerAngles = Vector3.left * 0;
                rHand.localEulerAngles = Vector3.left * 0;
                altPressed = false;
                ballFlying = true;
                ballIn2Hands = false;
                T = 0;
                dribSpeed = 6;
                moveSpeed = 6;
                if (inZone == true)
                {
                    addPoints = 1;
                }
                else
                {
                    addPoints = 2;
                }
                if (blocked)
                {
                    blockedShot = true;
                }
                else
                {
                    blockedShot = false;
                }
                rbBall.constraints = RigidbodyConstraints.None;
                colBall.isTrigger = false;
            }
        }

        //fly of the ball
        if (ballFlying)
        {
            T += Time.deltaTime;
            float duration = 0.8f;
            float t01 = T / duration;

            Vector3 A = aboveHeadPos.position;
            Vector3 B;

            if (power >= SorM && !blockedShot)
            {
                B = target.position;
            }
            else
            {
                if (LorR == 0)
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
                    p2points += addPoints;
                    p2PointC.text = p2points.ToString();
                    swishS.Play();
                    roundSystem.Winner = 2;
                    roundSystem.RoundEnd = true;
                }
                ball.GetComponent<Rigidbody>().isKinematic = false;
                powerBar.SetActive(false);
            }

        }




    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(dirX, 0, dirY);
    }

    private void Movement()
    {
        if (Input.anyKey)
        {
            if (Input.GetKey(KeyCode.I))
            {
                dirY = moveSpeed;
            }
            if (Input.GetKey(KeyCode.K))
            {
                dirY = -moveSpeed;
            }
            if (Input.GetKey(KeyCode.L))
            {
                dirX = moveSpeed;
            }
            if (Input.GetKey(KeyCode.J))
            {
                dirX = -moveSpeed;
            }
        }
        else if (!Input.anyKey)
        {
            dirX = 0;
            dirY = 0;
        }

        if (dirX != 0 && dirY != 0)
        {
            dirX = dirX / 1.4f;
            dirY = dirY / 1.4f;
        }

    }

    private void Sprint()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (stamina > 0)
            {
                if (((Input.GetKey(KeyCode.I)) || (Input.GetKey(KeyCode.J)) || (Input.GetKey(KeyCode.K)) || (Input.GetKey(KeyCode.L))) && altPressed == false)
                {
                    stamina -= 2;
                }
            }
        }
        if (((Input.GetKeyDown(KeyCode.I)) || (Input.GetKeyDown(KeyCode.J)) || (Input.GetKeyDown(KeyCode.K)) || (Input.GetKeyDown(KeyCode.L))) && (Input.GetKey(KeyCode.B)) && !staminaDecreasing)
        {
            if (stamina > 0)
            {
                stamina -= 2;
            }
        }
        if (Input.GetKey(KeyCode.B))
        {
            if (stamina > 0)
            {
                if (((Input.GetKey(KeyCode.I)) || (Input.GetKey(KeyCode.J)) || (Input.GetKey(KeyCode.K)) || (Input.GetKey(KeyCode.L))) && altPressed == false)
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

    //when we collide the ball
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ballT" && !ballIn2Hands && !ballFlying)
        {
            ballIn2Hands = true;
            catchingS.Play();
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
}