using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    [SerializeField] private PazzleController pazzleController;

    public delegate void ClickAction(bool isPressed);
    public static event ClickAction OnDrag;
    private bool _isDragging = false;
    private bool _isHome = false;
    private bool _isReady = false;
    private bool _isCanZoom = true;

    private Vector3 _offset;

    public bool IsHome { get => _isHome; set => _isHome = value; }
    public bool IsReady { get => _isReady; set => _isReady = value; }

    private void Start()
    {
        ClickHandler.OnDrag += CanZoom;
    }

    private void OnDisable()
    {
        ClickHandler.OnDrag -= CanZoom;
    }

    private void CanZoom(bool isPressed) => _isCanZoom = !isPressed;

    private void OnMouseDown()
    {
        if (_isHome || !IsReady) return;

        _isDragging = true;
        pazzleController.ZoomDownHandler(1f);
        _offset = transform.position - GetMouseWorldPosition();
        OnDrag?.Invoke(_isDragging);
    }

    private void OnMouseEnter()
    {
        if (_isHome || !_isReady || !_isCanZoom) return;
        pazzleController.ZoomDownHandler(1f);
    }

    private void OnMouseExit()
    {
        if (_isHome || !_isReady || !_isCanZoom) return;
        pazzleController.ZoomDownHandler(0.5f);
    }

    private void OnMouseUp()
    {
        if (_isHome || !IsReady) return;

        _isDragging = false;
        pazzleController.CheckOrigPos();
        OnDrag?.Invoke(_isDragging);
    }

    private void OnMouseDrag()
    {
        if (!_isReady) return;
       
        if (_isDragging && !_isHome)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            transform.position = mousePosition + _offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
