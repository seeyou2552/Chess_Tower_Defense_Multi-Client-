using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MinionDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private const float SearchRadius = 0.5f;
    private Minion _minion;
    private Vector3 _startPos;

    [Header("Layer Setting")]
    [SerializeField] private LayerMask _areaLayer;
    [SerializeField] private LayerMask _minionLayer;
    [SerializeField] private LayerMask _exceptionLayer;

    private Vector3 _lastValidTilePos;

    // Action 이벤트
    private bool _isDragging = false;
    public event Action OnPlacementSuccess; // 신규 설치 혹은 위치 이동 성공
    public event Action OnPlacementFailedAndReturn;  // 설치 실패로 인한 자동 판매 반환
    public event Action OnDragStart;
    public event Action OnDragEnd;

    private bool _owner;


    void Awake()
    {
        _minion = GetComponent<Minion>();
    }

    void OnDisable()
    {
        _startPos = Vector3.zero;
        _isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 현재 마우스 바로 아래에 있는 실제 오브젝트 확인
        GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObj == null) return;

        // 대상의 레이어가 허용 레이어에 포함되지 않는다면 드래그 차단
        if ((_exceptionLayer.value & (1 << clickedObj.layer)) != 0)
        {
            eventData.pointerDrag = null; 
            return;
        }

        if (_startPos == Vector3.zero)
        {
            StartCoroutine(FirstDragingCheckCoroutine());
        }

        if (_minion == null)
            _minion = GetComponent<Minion>();

        // 소유권 체크
        if (_minion.State.OwnerId == PlayerManager.Instance.PlayerId || _minion.State.OwnerId == 0)
            _owner = true;

        else
        {
            _owner = false;
            return;
        }
            

        OnDragStart?.Invoke();

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_owner)
        {
            _isDragging = true;
            
            if (_minion.IsSiege && !MinionTypeAbility.CanBypassSiege(_minion.State.Data.MinionType) ||
                GameManager.Instance.GameState != GameState.Intermission)
                return;

            if(_minion != null)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                _minion.gameObject.transform.position = pos;

                bool canPlace = CanPlace(pos);

                // 설치 가능하면 가장 가까운 유효한 타일 위치 저장
                if (canPlace)
                {
                    // searchRadius 범위 내의 모든 cell을 확인
                    Vector3Int centerCell = StageManager.Instance.MinionTilemap.WorldToCell(pos);
                    int radius = Mathf.CeilToInt(SearchRadius / StageManager.Instance.MinionTilemap.cellSize.x);
                    
                    float minDistance = float.MaxValue;
                    Vector3Int closestCell = centerCell;
                    bool foundValidCell = false;
                    
                    for (int x = centerCell.x - radius; x <= centerCell.x + radius; x++)
                    {
                        for (int y = centerCell.y - radius; y <= centerCell.y + radius; y++)
                        {
                            Vector3Int checkCell = new Vector3Int(x, y, 0);
                            Vector3 cellCenterWorld = StageManager.Instance.MinionTilemap.GetCellCenterWorld(checkCell);
                            
                            // 현재 위치에서 searchRadius 내인지 확인
                            if (Vector3.Distance(pos, cellCenterWorld) > SearchRadius)
                                continue;
                            
                            // 이 cell에 areaLayer가 있는지 확인
                            Collider2D areaCollider = Physics2D.OverlapPoint(cellCenterWorld, _areaLayer);
                            if (areaCollider != null)
                            {
                                float distance = Vector3.Distance(pos, cellCenterWorld);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    closestCell = checkCell;
                                    foundValidCell = true;
                                }
                            }
                        }
                    }
                    
                    if (foundValidCell)
                    {
                        _lastValidTilePos = StageManager.Instance.MinionTilemap.GetCellCenterWorld(closestCell);
                        _lastValidTilePos.z = 0;
                    }
                }
                
                if (_minion.AttackRange != null)
                {
                    if (canPlace)
                        _minion.AttackRange.SetOriginalColor();
                    else
                        _minion.AttackRange.SetImposibleColor();
                }

            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_owner)
        {
            if (_minion == null)
                _minion = GetComponent<Minion>();

            if (_minion.State.InstanceId == 0)
            {
                MinionManager.Instance.SendSpawnMinion(
                    (uint)_minion.State.Data.MinionIndex,
                    (int)(_lastValidTilePos.x * 2.0f),
                    (int)(_lastValidTilePos.y * 2.0f)
                );

                _minion.ReturnToPool();   
            }
            else if (CanPlace(_minion.transform.position))
            {
                // 설치 가능한 위치 - OnDrag에서 저장한 유효한 타일 위치에 배치
                MinionManager.Instance.SendRelocateMinion(
                    _minion.State.InstanceId,
                    (int)(_lastValidTilePos.x * 2.0f),
                    (int)(_lastValidTilePos.y * 2.0f) 
                );

                _startPos = _lastValidTilePos;

                OnPlacementSuccess?.Invoke();
            }
            else
            {
                // 설치 불가능한 위치 & 시작 위치 없음 - 미니언 판매 처리
                if(_startPos == Vector3.zero && !_minion.IsSiege)
                {
                    Debug.Log("Player Id = " + PlayerManager.Instance.PlayerId);
                    Debug.Log("Owner Id = " + _minion.State.OwnerId);
                    OnPlacementFailedAndReturn?.Invoke();
                }
                    
                
                // 설치 불가능한 위치 - 원래 위치로 되돌리기
                else
                {
                    _minion.gameObject.transform.position = _startPos;
                    _minion.SelectMinion(false);
                    _minion.AttackRange.SetOriginalColor();
                }
            }

            OnDragEnd?.Invoke();
        }
        
    }

    IEnumerator FirstDragingCheckCoroutine()
    {
        while (!_isDragging)
        {
            yield return null;
            
            if (Input.touchCount == 0)
            {
                OnPlacementFailedAndReturn?.Invoke();
                yield break;
            }
        }
    }

    /// <summary>
    /// 미니언이 설치 가능한 위치인지 확인하는 함수
    /// </summary>

    private bool CanPlace(Vector3 pos)
    {
        // 원 범위 탐색
        Collider2D[] results = new Collider2D[10];
        
        // areaLayer 확인 - 기본적으로 설치 가능
        int areaCount = Physics2D.OverlapCircleNonAlloc(pos, SearchRadius, results, _areaLayer);
        bool canPlace = areaCount > 0;
        
        // minionLayer 확인 - minion과 닿으면 불가능 (자신 제외)
        if (canPlace)
        {
            int minionCount = Physics2D.OverlapCircleNonAlloc(pos, SearchRadius, results, _minionLayer);
            
            // 자신을 제외하고 다른 minion이 있는지 확인
            bool hasOtherMinion = false;
            for (int i = 0; i < minionCount; i++)
            {
                if (results[i].gameObject != _minion.gameObject)
                {
                    hasOtherMinion = true;
                    break;
                }
            }
            
            canPlace = !hasOtherMinion;
        }
        
        return canPlace;
    }

    public void SetStartPos(Vector3 startPos)
    {
        _startPos = startPos;
    }

}

#region  EventBus

public readonly struct SpwanMinionEvent {};

#endregion
