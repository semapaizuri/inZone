using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.Serialization;

public class roundSystem : MonoBehaviour
{
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private Transform _ball;
    [Space(20)]
    [SerializeField] private Transform _attPos;
    [SerializeField] private Transform _defPos;
    [Space(20)]
    [SerializeField] private GameObject _endingCountDownUI;
    [SerializeField] private GameObject _mainCam;
    [SerializeField] private GameObject _checkCam;
    [SerializeField] private List<GameObject> _playerUIs;
    [Space(20)]
    [SerializeField] private TextMeshProUGUI _endingCountDown;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private TextMeshProUGUI _roundTimer;
    [Space(20)]
    [SerializeField] private float _timeAfterRound;

    private Canvas _uiCanv;
    
    private PlayerController _player1Script;
    private PlayerController _player2Script;

    private int _round = 1;
    private static int _winner = 1;
    private float _endingTimerRemain;
    private float _roundTimerRemain;
    
    public static int Winner { set => _winner = value; }
    public static bool RoundEnd { get; set; }
    public static bool IsCheck { get; set; }
    public static bool Checked { get; set; }
    public static bool TimeIsOut { get; set; }

    private void Awake()
    {
        _player1Script = _player1.GetComponent<PlayerController>();
        _player2Script = _player2.GetComponent<PlayerController>();
    }

    private void Start()
    {
        IsCheck = true;
        _player1.position = _attPos.position;
        _player2.position = _defPos.position;
        _ball.position = _attPos.position;
        _endingTimerRemain = _timeAfterRound;
    }

    void Update()
    {
        if (RoundEnd)
        {
            EndingRound();
        }

        CameraSwitch();
    }

    private void CameraSwitch()
    {
        if (IsCheck)
        {
            _checkCam.SetActive(true);
            _mainCam.SetActive(false);
            foreach (GameObject ui in _playerUIs)
            {
                ui.SetActive(false);
            }
        }
        else
        {
            _mainCam.SetActive(true);
            _checkCam.SetActive(false);
            foreach (GameObject ui in _playerUIs)
            {
                ui.SetActive(true);
            }
        }
    }

    private void EndingRound()
    {
        _endingCountDownUI.SetActive(true);
        if (_endingTimerRemain > 0f)
        {
            _endingTimerRemain -= Time.deltaTime;
            _endingCountDown.text = _endingTimerRemain.ToString("0.0");
        }
        else
        {
            RoundEnd = false;
            if (_winner == 1)
            {
                _player1.position = _attPos.position;
                _player2.position = _defPos.position;
            }
            else
            {
                _player1.position = _defPos.position;
                _player2.position = _attPos.position;
            }

            _player1Script.BallOut();
            _player2Script.BallOut();
            _ball.position = _attPos.position;

            _endingTimerRemain = _timeAfterRound;
            _endingCountDownUI.SetActive(false);
            _round++;
            _roundText.text = _round.ToString();
            IsCheck = true;
        }
    }

    private void StartRound()
    {
        _roundTimerRemain = 24;
        Invoke(nameof(TimerTick), 1f);
    }

    private void TimerTick()
    {
        _roundTimerRemain--;
        if (_roundTimerRemain > 0)
        {
            Invoke(nameof(TimerTick), 1f);
        }
        else
        {
            TimeIsOut = true;
        }
    }
}