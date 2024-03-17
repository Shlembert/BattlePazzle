using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Text resultText, scoreText, timeText, totalScoreText;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private GridPlacement left, right;
    [SerializeField] private RectTransform leftOffsetSceen, rightOffsetScreen;
    [SerializeField] private Transform leftAnchor, rightAnchor, winPanel;
    [SerializeField] private SpriteRenderer bgSprite;
    [SerializeField] private Image screen;
    [SerializeField] private float alfaPicture;
    [SerializeField] private int niceScore, goodScore, nobadScore;

    private Vector3 _textPos;
    private Color _textColor;
    private int _score = 0;
    private int _count;
    private int _totalScore = 0;
    private int _winCount;
    private bool _check;
    private float _currentTime;

    private List<Vector2> points = new List<Vector2>();

    public async void StartGame()
    {
        DOTween.Init();
        DOTween.SetTweensCapacity(1000, 50);

        _textPos = resultText.transform.position;
        _textColor = resultText.color;

        if (!_check) _check = true;
        else return;

        GridAdaptation();

        _count = gridGenerator.GetSizeGrid();
        _winCount = _count * _count;

        int halfCount = _count * _count / 2 + 1;

        points = left.PlacePoints(halfCount);
        points.AddRange(right.PlacePoints(halfCount));

        int delay = _count * 1000;

        await UniTask.Delay(delay);
        bgSprite.DOFade(alfaPicture, 1f);
        RandomMovePazzles();
        ClickHandler.OnDrag += HandleDrag;
        PazzleController.ItemPlacedEvent += HandleItemPlaced;
    }

    private void OnDestroy()
    {
        ClickHandler.OnDrag -= HandleDrag;
        PazzleController.ItemPlacedEvent -= HandleItemPlaced;
        DOTween.KillAll();
    }

    private void HandleDrag(bool isPressed)
    {
        float duration = 0.5f;

        bgSprite.DOKill();

        if (isPressed)
        {
            bgSprite.DOFade(1f, duration);
        }
        else
        {
            bgSprite.DOFade(alfaPicture, duration);
        }
    }

    bool check = true;

    private async void RandomMovePazzles()
    {
        if (check)
        {
            check = false;
            List<PazzleController> pazzles = gridGenerator.Pazzles;

            List<int> usedIndexes = new List<int>();

            foreach (PazzleController pazzle in pazzles)
            {
                int index;
                do
                {
                    index = UnityEngine.Random.Range(0, points.Count);
                }
                while (usedIndexes.Contains(index));

                usedIndexes.Add(index);

                Vector3 go = points[index];

                if (pazzle)
                {
                    pazzle.RandomPos = go;
                    pazzle.MoveToPoint(go, 0.5f);
                    pazzle.ZoomDownHandler(0.5f);
                }
            }

          await  ReadySend();
        }
    }

    private void GridAdaptation()
    {
        leftAnchor.position = leftOffsetSceen.position;
        rightAnchor.position = rightOffsetScreen.position;
    }

    private void HandleItemPlaced(float distance)
    {
        CheckDistance(distance);

        if (_winCount > 1) _winCount--;
        else WinGame();
    }

    private bool stop;

    private async void WinGame()
    {
        stop = true;

        StopCoroutine(TrackTime());
        await UniTask.Delay(3000);
        DOTween.KillAll();

        _winCount = _count * _count; // кол-во элементов
        int timeRatio = Mathf.RoundToInt(1000f / Mathf.Sqrt(_currentTime)); // время сессии
        _totalScore = _score * _count + _winCount + timeRatio;

        TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.CeilToInt(_currentTime));
        string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

        Debug.Log(
            "score: " + _score + 
            " || Time: " + formattedTime + 
            " || timeRatio: " + timeRatio + 
            " || winCount: " + _winCount + 
            " || Total: " + _totalScore);

        winPanel.gameObject.SetActive(true);
        winPanel.DOMoveY(-15f, 0.5f, false).From().OnComplete(() =>
        {
            DOTween.To(() => 0, x => totalScoreText.text = x.ToString(), _totalScore, 1f)
                .SetEase(Ease.OutQuad);
        });
    }

    private void CheckDistance(float distance)
    {
        string result;
       
        if (distance < 0.1)
        {
            result = "Nice!";
            _score += niceScore;
        }
        else if (distance < 0.5)
        {
            result = "Good!";
            _score += goodScore;
        }
        else
        {
            result = "No bad";
            _score += nobadScore;
        }

        scoreText.text = "Score: " + _score.ToString();
        resultText.text = result;
        resultText.DOKill();
        resultText.transform.position = _textPos;
        resultText.color = _textColor;

        resultText.DOFade(1f, 0.1f);
        // Создаем последовательность анимаций
        Sequence sequence = DOTween.Sequence();

        // Анимация 1: текст становится видимым и увеличивается в размере
        sequence.Append(resultText.DOFade(0f, 0.5f).From()); // Плавное появление текста
        sequence.Join(resultText.transform.DOScale(Vector3.zero, 0.5f).From()); // Увеличение размера текста

        // Анимация 2: текст поднимается наверх и становится прозрачным
        sequence.Append(resultText.transform.DOMoveY(15f, 0.8f).SetEase(Ease.InBack)); // Поднятие текста
        sequence.Join(resultText.DOFade(0f, 1f)); // Исчезновение текста

        // Запускаем последовательность анимаций
        sequence.Play();
    }

    private async UniTask ReadySend()
    {
        resultText.color = Color.green;
        resultText.text = "Ready!";
        resultText.DOFade(0f, 1.0f);
        resultText.transform.DOMoveY(10f, 1f).SetEase(Ease.InBack);
        await UniTask.Delay(900);
        StartCoroutine(TrackTime());
    }
    private IEnumerator TrackTime()
    {
        while (!stop)
        {
            yield return new WaitForSeconds(1);
            _currentTime ++;

            TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.CeilToInt(_currentTime));
            string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);

            timeText.text = formattedTime;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

