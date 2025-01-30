using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class atPlayerC : MonoBehaviour
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
    private Transform p1StartPos;

    public GameObject powerBar;
    public GameObject rig;

    public AudioSource catchingS;
    public AudioSource throwS;
    public AudioSource swishS;

    private Rigidbody rbBall;
    private Rigidbody rb;
    private Collider mainCol;
    private Collider colBall;
    float dirX, dirY;

    public float dribSpeed;
    public float moveSpeed;
    float T = 0f;
    public float p1points = 0;
    float addPoints = 0;
    int LorR;
    int SorM;

    public bool ballInDrib = false;
    public bool ballRight = true;
    public bool ballLeft = false;
    public bool spacePressed = false;
    public bool inZone;
    public bool blocked;
    public bool isBlocking;
    public bool fallen;
    bool blockedShot;
    bool staminaDecreasing;
    
    bool ballIn1Hands = true;
    bool ballFlying = false;
    bool ballEnemy = true;

    public float stamina;
    float maxStamina;
    
    public float power;
    float maxPower = 100;
    bool powerIncreasing = true;
    bool powerBarON = false;

    public TextMeshProUGUI p1PointC;
    public Slider staminaBar;
    public Slider throwBar;
    public float dValue;

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
        GetRagdollComps();
        RagdollOff();
        p1StartPos = attPos;
        transform.position = new Vector3(p1StartPos.position.x, p1StartPos.position.y,p1StartPos.position.z);
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

    IEnumerator Lying()
    {
        yield return new WaitForSeconds(3f);
        RagdollOff();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        blocked = blockChecker1.blocked1;
        isBlocking = blockChecker2.blocked2;

        //blocking stand
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

        //looking
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

        Movement();

        Sprint();

        if (stamina <= 0)
        {
            //RagdollOn();
            //StartCoroutine(Lying());
        }

        if (ballFlying == false && ballIn1Hands == false && rbBall.constraints != RigidbodyConstraints.None)
        {
            ballEnemy = true;
        }
        else
        {
            ballEnemy = false;
        }

        if (ballIn1Hands)
        {
            colBall.isTrigger = true;
            rbBall.constraints = RigidbodyConstraints.FreezePosition;
            //pressing space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                power = 1;
                powerIncreasing = true;
                powerBarON = true;
                powerBar.SetActive(true);
                StartCoroutine(UpdatePowerBar());
            }
            if (Input.GetKey(KeyCode.Space))
            {
                spacePressed = true;
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

            //releasing space
            if (Input.GetKeyUp(KeyCode.Space))
            {
                powerBarON = false;
                throwS.Play();
                SorM = Random.Range(20, 100);
                LorR = Random.Range(0, 2);
                hands.localEulerAngles = Vector3.left * 0;
                rHand.localEulerAngles = Vector3.left * 0;
                spacePressed = false;
                ballFlying = true;
                ballIn1Hands = false;
                T = 0;
                dribSpeed = 6;
                moveSpeed = 6;
                if (inZone)
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

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(dirX, 0, dirY);
    }

    private void Movement()
    {
        dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;
        dirY = Input.GetAxisRaw("Vertical") * moveSpeed;
        if (dirX != 0 && dirY != 0)
        {
            dirX = dirX / 1.4f;
            dirY = dirY / 1.4f;
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


    Collider[] ragdollCols;
    Rigidbody[] ragdollRigids;
    void GetRagdollComps()
    {
        ragdollCols = rig.GetComponentsInChildren<Collider>();
        ragdollRigids = rig.GetComponentsInChildren<Rigidbody>();
    }

    void RagdollOn()
    {
        foreach (Collider col in ragdollCols)
        {
            col.enabled = true;
        }
        foreach (Rigidbody rigid in ragdollRigids)
        {
            rigid.isKinematic = false;
        }
        mainCol.enabled = false;
        rb.isKinematic = true;
        fallen = true;
    }

    void RagdollOff()
    {
        foreach (Collider col in ragdollCols)
        {
            col.enabled = false;
        }
        foreach (Rigidbody rigid in ragdollRigids)
        {
            rigid.isKinematic = true;
        }
        mainCol.enabled = true;
        rb.isKinematic = false;
        fallen = false;
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