using DG.Tweening;
using System;
using UnityEngine;

public class PazzleController : MonoBehaviour
{
    [SerializeField] private float moveBackDuration, moveToPointDuration, scaleDuration;
    [SerializeField] private float distanceToOrigPos;

    public static event Action<float> ItemPlacedEvent;

    private int _origOrder;
    private SpriteRenderer _spriteRenderer;

    private int _index;
    private Vector3 _origPosition;
    private Vector3 _origScale;
    private Vector3 _randomPos;
    private ClickHandler _clickHandler;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _clickHandler = GetComponent<ClickHandler>();
        _origOrder = _spriteRenderer.sortingOrder;
        _origScale = transform.localScale;
        _origPosition = transform.position;
    }

    public int Index { get => _index; set => _index = value; }
    public Vector3 RandomPos { get => _randomPos; set => _randomPos = value; }

    public void MoveToPoint(Vector3 poitPosition, float mult)
    {
        transform.DOMove(poitPosition, moveToPointDuration * mult).OnComplete(() =>
         {
             _spriteRenderer.sortingOrder = _origOrder;
             _clickHandler.IsReady = true;
         });
    }

    public void ZoomDownHandler(float num)
    {
        transform.DOScale(_origScale * num, scaleDuration);

        if (_spriteRenderer && num==1) _spriteRenderer.sortingOrder = _origOrder + 2;
        else _spriteRenderer.sortingOrder = _origOrder + 1;
    }

    public Vector3 GetOrigPos()
    {
        return _origPosition;
    }

    public void CheckOrigPos()
    {
        float distance = Vector2.Distance(transform.position, _origPosition);

        if (distance < distanceToOrigPos)
        {
            transform.DOKill();
            ItemPlacedEvent?.Invoke(distance);
            _clickHandler.IsHome = true;
            MoveToPoint(_origPosition, 0.2f);
            _clickHandler.IsReady = false;
            return;
        }
        else
        {
            MoveToPoint(_randomPos, 0.3f);
        }
    }
}
