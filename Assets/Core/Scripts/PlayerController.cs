using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.ComponentModel;

public class PlayerController : MonoBehaviour
{
    //public PlayersControls controls;
    public InputActionAsset controlsAsset;
    public string controlMapName;
    private InputActionMap _controlMap;
    private InputAction _moveAction;
    private InputAction _shootAction;
    private InputAction _sprintAction;

    public Transform enemy;
    public Transform target;
    public Transform miss1;
    public Transform miss2;
    public Transform hoop;
    public Transform ball;
    public Transform rHand;
    public Transform lHand;
    public Transform aboveHeadPos;
    public Transform RdribblePos;
    public Transform ballCheckPos;

    [SerializeField] private Transform _attPos;
    [SerializeField] private Transform _defPos;
    
    public GameObject powerBar;
    public GameObject rig;

    public TextMeshProUGUI PointCounter;
    public Slider staminaBar;
    public Slider throwBar;

    public AudioSource catchingSound;
    public AudioSource throwS;
    public AudioSource swishS;
    public AudioSource rimHit;

    private Rigidbody _rbBall;
    private Rigidbody _rb;
    private Collider _colBall;
    
    private Vector3 _moveDir = Vector3.zero;
    private Vector3 A;
    
    public float moveSpeed;
    [SerializeField] private float DEFAULT_MOVESPEED = 5;
    [SerializeField] private float AIMING_MOVESPEED = 1;
    [SerializeField] private float SPRINT_MOVESPEED = 10;
    public float power;
    public float stamina;
    public float points = 0;
    public float staminaUsage;

    private float _maxPower = 100;
    private float _maxStamina;
    private float T = 0f;
    private float _addPoints = 0;
    private int _leftOrRight;
    private int _scoreOrMiss;
    
    public bool inZone;
    public bool blocked;
    public bool isBlocking;
    
    [SerializeField] private bool _ballInPlayerHands;
    
    private bool _isAiming;
    private bool _blockedShot;
    private bool _ballFlying;
    private bool _ballEnemy;
    private bool _powerIncreasing = true;
    private bool _powerBarON;

    private void Awake()
    {
        //controls = new PlayersControls();
    }

    private void OnEnable()
    {
        //controls.Enable();
        _controlMap = controlsAsset.FindActionMap(controlMapName);
        if (_controlMap != null)
        {
            _controlMap.Enable();
            _moveAction = _controlMap.FindAction("Move");
            _sprintAction = _controlMap.FindAction("Sprint");
            _shootAction = _controlMap.FindAction("Shoot");

            _shootAction.performed += OnAim;
            _shootAction.canceled += OnShoot;
            _sprintAction.performed += StartOfSprint;
        }
        //controls.MainPlayer.Shoot.performed += OnAim;
        //controls.MainPlayer.Shoot.canceled += OnShoot;
    }

    private void OnDisable()
    {
        _controlMap.Disable();
        //controls.MainPlayer.Disable();

        _shootAction.performed -= OnAim;
        _shootAction.canceled -= OnShoot;
        _sprintAction.performed -= StartOfSprint;
    }

    void Start()
    {
        _colBall = ball.GetComponent<Collider> ();
        _rbBall = ball.GetComponent<Rigidbody>();
        _rb = GetComponent<Rigidbody>();
        _maxStamina = stamina;
        staminaBar.maxValue = _maxStamina;
        powerBar.SetActive(false);
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
        Checking();

        if (_ballFlying == false && _ballInPlayerHands == false && _rbBall.constraints != RigidbodyConstraints.None)
        {
            _ballEnemy = true;
        }
        else
        {
            _ballEnemy = false;
        }
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector3(_moveDir.x * moveSpeed, 0, _moveDir.y * moveSpeed);
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        if (!roundSystem.IsCheck)
        {
            //start aiming
            power = 1;
            _powerIncreasing = true;
            _powerBarON = true;
            powerBar.SetActive(true);
            StartCoroutine(UpdatePowerBar());
        }
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (!roundSystem.IsCheck)
        {
            //shooting anim
            _powerBarON = false;
            throwS.Play();
            _scoreOrMiss = Random.Range(20, 100);
            _leftOrRight = Random.Range(0, 2);
            _isAiming = false;
            _ballFlying = true;
            _ballInPlayerHands = false;
            T = 0;
            moveSpeed = DEFAULT_MOVESPEED;

            if (inZone)
                _addPoints = 1;
            else
                _addPoints = 2;

            if (blocked)
                _blockedShot = true;
            else
                _blockedShot = false;

            _rbBall.constraints = RigidbodyConstraints.None;
            _colBall.isTrigger = false;

            _shootAction.Disable();

            A = aboveHeadPos.position;
        }
        else
        {
            _ballInPlayerHands = false;
            _rbBall.constraints = RigidbodyConstraints.None;
            _colBall.isTrigger = false;
            _rbBall.isKinematic = false;
            _rbBall.linearVelocity = transform.forward * 20;
            _shootAction.Disable();
        }
    }

    private void Movement()
    {
        _moveDir = _moveAction.ReadValue<Vector2>();
    }

    IEnumerator UpdatePowerBar()
    {
        while (_powerBarON)
        {
            if (_powerIncreasing)
            {
                power *= 1.3f;
                if (power >= _maxPower)
                {
                    _powerIncreasing = false;
                }
            }
            else
            {
                power /= 1.3f;
                if (power <= 1)
                {
                    _powerIncreasing = true;
                }
            }
            
            throwBar.value = power;
            yield return new WaitForSeconds(0.02f);
        }
        yield return null;
    }

    private void Checking()
    {
        if (roundSystem.IsCheck)
        {
            _moveAction.Disable();
            stamina = _maxStamina;
        }
        else
        {
            _moveAction.Enable();
        }
    }


    private void BallFlying()
    {
        if (_ballFlying)
        {
            T += Time.deltaTime;
            float duration = 0.8f;
            float t01 = T / duration;
            
            Vector3 B;

            if (power >= _scoreOrMiss && !_blockedShot)
            {
                B = target.position;
            }
            else
            {
                if (_leftOrRight == 0)
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
                _ballFlying = false;
                if (B == target.position)
                {
                    if (!roundSystem.RoundEnd)
                    {
                        points += _addPoints;
                        PointCounter.text = points.ToString();
                        roundSystem.Winner = controlMapName == "MainPlayer" ? 1 : 2;
                        roundSystem.RoundEnd = true;
                    }
                    swishS.Play();
                }
                else rimHit.Play();
                _rbBall.isKinematic = false;
                powerBar.SetActive(false);
            }
        }
    }

    private void Ballin()
    {
        if (_ballInPlayerHands)
        {
            if (!roundSystem.IsCheck)
            {
                //controls.MainPlayer.Shoot.Enable();
                _shootAction.Enable();
                _colBall.isTrigger = true;
                _rbBall.constraints = RigidbodyConstraints.FreezePosition;
            
                if (_shootAction.IsPressed())
                {
                    //aiming anim
                    _isAiming = true;
                    ball.position = aboveHeadPos.position;
                    moveSpeed = AIMING_MOVESPEED;
                }
                else
                {
                    ball.position = RdribblePos.position;
                    //dribbling
                }
            }
            else
            {
                _shootAction.Enable();
                _colBall.isTrigger = true;
                _rbBall.constraints = RigidbodyConstraints.FreezePosition;
                ball.position = ballCheckPos.position;
            }
        }
    }

    private void BlockingPose()
    {
        if (isBlocking && !_ballInPlayerHands && !_ballFlying)
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
        if (!roundSystem.IsCheck)
        {
            if (_ballInPlayerHands)
            {
                transform.LookAt(new Vector3(hoop.position.x, transform.position.y, hoop.position.z));
            }
            else if (_ballInPlayerHands == false && _ballEnemy == true)
            {
                transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
            }
            else
            {
                transform.LookAt(new Vector3(ball.position.x, transform.position.y, ball.position.z));
            }
        }
        else
        {
            transform.LookAt(new Vector3(enemy.position.x, transform.position.y, enemy.position.z));
        }
    }

    private void StartOfSprint(InputAction.CallbackContext context)
    {
        if (stamina > 0)
        {
            if (_rb.linearVelocity != Vector3.zero && _isAiming == false)
            {
                stamina -= 2;
            }
        }
    }

    private void Sprint()
    {
        if (_sprintAction.IsPressed())
        {
            if (stamina > 0)
            {
                if (_rb.linearVelocity != Vector3.zero && _isAiming == false)
                {
                    moveSpeed = SPRINT_MOVESPEED;
                    DecreaseStam();
                }
                else
                {
                    if (stamina < _maxStamina)
                    {
                        IncreaseStam();
                    }
                }
            }
            else
            {
                moveSpeed = DEFAULT_MOVESPEED;
            }

        }
        else
        {
            moveSpeed = DEFAULT_MOVESPEED;
            if (stamina < _maxStamina)
            {
                IncreaseStam();
            }
        }

        staminaBar.value = stamina;
    }

    public void BallOut()
    {
        _ballFlying = false;
        if (!_ballInPlayerHands) return;
        _ballInPlayerHands = false;
        ball.position = new Vector3(0, 0, 5);
        _rbBall.constraints = RigidbodyConstraints.None;
        _colBall.isTrigger = false;
        _rbBall.isKinematic = false;
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ballT") && !_ballInPlayerHands && !_ballFlying)
        {
            catchingSound.Play();
            _ballInPlayerHands = true;
            if (roundSystem.IsCheck && !roundSystem.Checked && inZone)
            {
                roundSystem.Checked = true;
            }
            if (roundSystem.IsCheck && roundSystem.Checked && !inZone)
            {
                roundSystem.Checked = false;
                roundSystem.IsCheck = false;
            }
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
            stamina -= staminaUsage * Time.deltaTime;
        }
    }

    private void IncreaseStam()
    {
        stamina += staminaUsage / 2 * Time.deltaTime;
    }
}