using UnityEngine;
using System.Collections;

// 城镇居民
public class CityCitizen : MonoBehaviour
{
    public enum State
    {
        NONE,   // 空状态，不做任何逻辑处理
        IDLE,
        WALK,
        RUNNING,
        BUILDING,
    }

    private NavMeshAgent agent;
    private Animation _animation;

    public float _minIdleWaitTime = 1;
    public float _maxIdleWaitTime = 3;
    public float _minWanderRange = 10;
    public float _maxWanderRange = 50;
    public float _maxBlockingTime = 3;

    public State _state;
    public Vector3 _dest;
    public CityBuilding _workingBuilding; // 正在哪个建筑修建

    private bool _isBlocking = false;
    private float _startBlockTime = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
    }
    
	void Start ()
	{
	    ChangeToIdleState();
	}
	
	void Update () {
	    switch (_state) {
	        case State.IDLE:
                IdleState();
	            break;
            case State.WALK:
	            WalkState();
                break;
            case State.RUNNING:
                RunningState();
	            break;
            case State.BUILDING:
                BuildingState();
	            break;
	    }
	}

    private void IdleState()
    {
    }

    private void WalkState()
    {
        // 到达目的地了，切换到待机状态
        if (Vector3.Distance(transform.position, _dest) <= 0.5f) {
            ChangeToIdleState();
            return;
        }

        if (Physics.Raycast(transform.position, transform.forward, 1)) {
            if (_isBlocking == false) {
                _startBlockTime = Time.realtimeSinceStartup;
            }

            _isBlocking = true;
            if (Time.realtimeSinceStartup - _startBlockTime >= _maxBlockingTime) {
                _isBlocking = false;
                ChangeToIdleState();
            }
        } else {
            _isBlocking = false;
        }
    }

    private void RunningState()
    {
        // 跑到目的地了，切换到建筑状态
        if (_workingBuilding != null) {
            if (Vector3.Distance(transform.position, _workingBuilding.transform.position) <= 1) {
                ChangeToBuildingState();
            }
        }
    }

    private void BuildingState()
    {   
    }

    private void WaitFor(float delay, System.Action callback)
    {
        StartCoroutine(DoWaitFor(delay, callback));
    }

    private IEnumerator DoWaitFor(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }

    // 切换到待机状态
    private void ChangeToIdleState()
    {
        _state = State.IDLE;
        _animation.Play("pichai");

        // 等待3秒，切换到巡逻状态
        WaitFor(Random.Range(_minIdleWaitTime, _maxIdleWaitTime), ChangeToWalkState);
    }

    // 切换到巡逻状态
    private void ChangeToWalkState()
    {
        _state = State.WALK;
        _animation.Play("kongshou zou");
        StartWander();
    }

    // 切换到跑步状态（跑步到建筑物）
    private void ChangeToRunningState()
    {
        _state = State.RUNNING;
        _animation.Play("kongshou zou");
    }

    // 切换到修建状态
    private void ChangeToBuildingState()
    {
        _state = State.BUILDING;
        _animation.Play("xiufangzi");
    }

    private void StartWander()
    {
        Vector3 offset = transform.position;
        Vector2 random = Random.insideUnitCircle;
        float range = Random.Range(_minWanderRange, _maxWanderRange);
        offset.x = offset.x + random.x*range;
        offset.z = offset.z + random.y*range;
        _dest = offset;
        
        agent.destination = _dest;
    }

    public bool CouldWork()
    {
        return _workingBuilding == null;
    }

    // 跑到建筑物周围
    public void RunToBuilding(CityBuilding building)
    {
        _workingBuilding = building;
        agent.destination = building.transform.position;
        ChangeToRunningState();
    }

    public bool IsWorking()
    {
        return _workingBuilding != null;
    }

    public void WorkFinish()
    {
        _workingBuilding = null;

        // 修建完了，切换到待机状态
        ChangeToIdleState();
    }

    // 发生碰撞
    void OnCollisionEnter(Collision col)
    {
        Debug.Log(col);
    }

    // 发生碰撞
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log(hit);
    }
}
