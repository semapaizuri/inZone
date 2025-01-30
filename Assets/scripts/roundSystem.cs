using System.Collections;
using TMPro;
using UnityEngine;

public class roundSystem : MonoBehaviour
{
    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [Space(20)]
    [SerializeField] private Transform _attPos;
    [SerializeField] private Transform _defPos;
    
    [SerializeField] private GameObject _countDownUI;
    [SerializeField] private TextMeshProUGUI _countDown;
    [SerializeField] private TextMeshProUGUI _roundText;
    private int _round = 1;
    private static int winner = 1;
    private static bool roundEnd = false;
    private float _timerRemain = 2f;
    
    public static int Winner { set => winner = value; }
    public static bool RoundEnd { set => roundEnd = value; }

    
    void Update()
    {
        if (roundEnd == true)
        {
            StartingRound();
        }
    }

    private void StartingRound()
    {
        _countDownUI.SetActive(true);
        if (_timerRemain > 0f)
        {
            _timerRemain -= Time.deltaTime;
            _countDown.text = _timerRemain.ToString();
        }
        else
        {
            roundEnd = false;
            if (winner == 1)
            {
                _player1.position = _attPos.position;
                _player2.position = _defPos.position;
            }
            else
            {
                _player1.position = _defPos.position;
                _player2.position = _attPos.position;
            }
            _timerRemain = 2f;
            _countDownUI.SetActive(false);
            _round++;
            _roundText.text = _round.ToString();
        }
    }
}