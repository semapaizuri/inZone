using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class roundSystem : MonoBehaviour
{
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private Transform _ball;
    [Space(20)]
    [SerializeField] private Transform _attPos;
    [SerializeField] private Transform _defPos;
    [Space(20)]
    [SerializeField] private GameObject _countDownUI;
    [SerializeField] private GameObject _mainCam;
    [SerializeField] private GameObject _checkCam;
    [SerializeField] private List<GameObject> _playerUIs;

    [SerializeField] private TextMeshProUGUI _countDown;
    [SerializeField] private TextMeshProUGUI _roundText;
    [Space(20)]
    [SerializeField] private float _timeAfterRound;

    private Canvas _uiCanv;

    private int _round = 1;
    private static int _winner = 1;
    private float _timerRemain;
    
    public static int Winner { set => _winner = value; }
    public static bool RoundEnd { get; set; }
    public static bool IsCheck { get; set; }
    public static bool Checked { get; set; }

    private void Start()
    {
        IsCheck = true;
        _player1.position = _attPos.position;
        _player2.position = _defPos.position;
        _ball.position = _attPos.position;
        _timerRemain = _timeAfterRound;
    }

    void Update()
    {
        if (RoundEnd == true)
        {
            StartingRound();
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

    private void StartingRound()
    {
        _countDownUI.SetActive(true);
        if (_timerRemain > 0f)
        {
            _timerRemain -= Time.deltaTime;
            _countDown.text = _timerRemain.ToString("0.0");
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

            _player1.GetComponent<PlayerController>().BallOut();
            _player2.GetComponent<PlayerController>().BallOut();
            _ball.position = _attPos.position;

            _timerRemain = _timeAfterRound;
            _countDownUI.SetActive(false);
            _round++;
            _roundText.text = _round.ToString();
            IsCheck = true;
        }
    }
}