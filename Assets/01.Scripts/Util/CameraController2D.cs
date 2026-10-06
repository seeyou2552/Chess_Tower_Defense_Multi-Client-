using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CameraController2D : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _cam;

    [Header("Bounds")]
    [SerializeField] private TilemapCollider2D _mapBounds;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 0.05f;
    [SerializeField] private float _minZoom = 3f;
    [SerializeField] private float _maxZoom = 10f;

    [Header("Pan")]
    [SerializeField] private float _panSpeed = 1f;
    [SerializeField] private LayerMask _minionLayerMask = 1 << 8;

    [Header("Drag Edge Camera Pan")]
    [SerializeField] private float _dragEdgeThreshold = 100f; // 화면 끝에서 카메라 움직임을 시작하는 거리 (픽셀)
    [SerializeField] private float _dragCameraPanSpeed = 5f; // 드래그 중 카메라 이동 속도

    private Vector2 _lastPanPosition;
    private int _panFingerId = -1;
    private bool _isPointerOverMinion;
    private bool _isDraggingMinion = false;

    void Start()
    {
        if (_cam == null)
            _cam = Camera.main;

        ClampZoom();
        ClampPosition();
        Input.multiTouchEnabled = true;
    }

    void Update()
    {
        if (_cam == null || _mapBounds == null)
            return;

        // 두 손가락(핀치 줌)을 먼저 확인 - 우선순위 높음
        if (Input.touchCount >= 2)
        {
            HandlePinchZoom(Input.GetTouch(0), Input.GetTouch(1));
            _panFingerId = -1; // Pan 초기화
            _isDraggingMinion = false;
        }
        // 한 손가락(팬) 처리
        else if (Input.touchCount == 1)
        {
            
            HandlePan(Input.GetTouch(0));
            
            // Minion 드래그 중이면 매 프레임 화면 끝 체크
            if (_isDraggingMinion)
            {
                CheckDragEdgeAndPanCamera(Input.GetTouch(0).position);
            }
        }
        else
        {
            _panFingerId = -1;
            _isDraggingMinion = false;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            
            // UI 위에 있지 않으면 (Tilemap 포함) AllHideUI 호출
            if (!IsPointerOverUI(touch.fingerId))
            {
                CommonUIManager.Instance.AllHideUI();
            }

        }

        // 터치 없을 때만 마우스 줌 처리
        if (Input.touchCount == 0)
        {
            HandleMouseZoom();
        }

        ClampZoom();
        ClampPosition();
    }

    void HandlePan(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            _lastPanPosition = touch.position;
            _panFingerId = touch.fingerId;
            
            
            bool isOverUI = IsPointerOverUI(touch.fingerId);
            bool isOverMinion = IsPointerOverMinion(touch.position);

            _isPointerOverMinion = isOverUI;
            _isDraggingMinion = isOverMinion;
            return;
        }

        if (touch.fingerId != _panFingerId)
            return;

        if (touch.phase == TouchPhase.Moved && !_isPointerOverMinion)
        {
            Vector2 delta = touch.position - _lastPanPosition;
            PanCamera(delta);
            _lastPanPosition = touch.position;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isPointerOverMinion = false;
            _isDraggingMinion = false;
        }
    }

    void PanCamera(Vector2 screenDelta)
    {
        Vector3 worldDelta = _cam.ScreenToWorldPoint(new Vector3(screenDelta.x, screenDelta.y, 0f)) -
                             _cam.ScreenToWorldPoint(Vector3.zero);
        transform.position -= worldDelta * _panSpeed;
    }

    void CheckDragEdgeAndPanCamera(Vector2 screenPosition)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector3 panDirection = Vector3.zero;

        // 화면 가로 끝쪽 체크
        if (screenPosition.x < _dragEdgeThreshold)
        {
            panDirection.x = -1f; // 왼쪽 이동
        }
        else if (screenPosition.x > screenWidth - _dragEdgeThreshold)
        {
            panDirection.x = 1f; // 오른쪽 이동
        }

        // 화면 세로 끝쪽 체크
        if (screenPosition.y < _dragEdgeThreshold)
        {
            panDirection.y = -1f; // 아래쪽 이동
        }
        else if (screenPosition.y > screenHeight - _dragEdgeThreshold)
        {
            panDirection.y = 1f; // 위쪽 이동
        }

        // 끝쪽에 있으면 카메라 이동
        if (panDirection.magnitude > 0)
        {
            transform.position += panDirection.normalized * _dragCameraPanSpeed * Time.deltaTime;
        }
    }

    void HandlePinchZoom(Touch touch0, Touch touch1)
    {
        Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
        Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

        float prevDistance = Vector2.Distance(prevTouch0, prevTouch1);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        float delta = currentDistance - prevDistance;

        float newZoom = _cam.orthographicSize - delta * _zoomSpeed * Time.deltaTime;
        _cam.orthographicSize = Mathf.Max(newZoom, _minZoom);
    }

    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f))
            return;

        float newZoom = _cam.orthographicSize - scroll * _zoomSpeed * 10f;
        _cam.orthographicSize = Mathf.Max(newZoom, _minZoom);
    }


    void ClampZoom()
    {
        Bounds bounds = _mapBounds.bounds;
        float boundsMaxZoom = Mathf.Max(bounds.size.y * 0.5f, bounds.size.x * 0.5f / _cam.aspect);
        
        // mapBounds를 벗어나지 않도록 maxZoom을 제한
        float effectiveMaxZoom = Mathf.Min(_maxZoom, boundsMaxZoom);

        _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize, _minZoom, effectiveMaxZoom);
    }

    bool IsPointerOverMinion(Vector2 screenPosition)
    {
        Vector2 worldPos = _cam.ScreenToWorldPoint(screenPosition);

        Collider2D hit = Physics2D.OverlapPoint(worldPos, _minionLayerMask);
        return hit != null;
    }


    private bool IsPointerOverUI(int fingerId)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        
        if (fingerId >= 0 && Input.touchCount > 0)
        {
            pointerEventData.position = Input.GetTouch(fingerId).position;
        }
        else
        {
            pointerEventData.position = Input.mousePosition;
        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        // Raycast 결과에서 UI 요소를 감지하되, Tilemap은 제외
        foreach (RaycastResult result in results)
        {
            Graphic graphic = result.gameObject.GetComponent<Graphic>();
            Canvas canvas = result.gameObject.GetComponent<Canvas>();
            
            if ((graphic != null || canvas != null) && 
                result.gameObject.GetComponent<TilemapCollider2D>() == null)
            {
                return true;
            }
        }

        // Minion 클릭 감지 - AllHideUI 호출 및 카메라 이동 방지
        if (fingerId >= 0 && Input.touchCount > 0)
        {
            return IsPointerOverMinion(Input.GetTouch(fingerId).position);   
        }
        
        return false;
    }

    void ClampPosition()
    {
        Bounds bounds = _mapBounds.bounds;
        Vector3 pos = transform.position;

        float camHeight = _cam.orthographicSize;
        float camWidth = camHeight * _cam.aspect;

        float minX = bounds.min.x + camWidth;
        float maxX = bounds.max.x - camWidth;
        float minY = bounds.min.y + camHeight;
        float maxY = bounds.max.y - camHeight;

        if (minX > maxX)
        {
            pos.x = bounds.center.x;
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
        }

        if (minY > maxY)
        {
            pos.y = bounds.center.y;
        }
        else
        {
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }

        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}





