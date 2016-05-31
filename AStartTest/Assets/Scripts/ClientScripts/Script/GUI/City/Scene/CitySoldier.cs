using UnityEngine;
using System.Collections;

// 兵营中的士兵
public class CitySoldier : MonoBehaviour {
    public enum State
    {
        NONE,   // 空状态，不做任何逻辑处理
        IDLE,
        IN_TROOP,
        IN_TRAINING,
        RUNNING_TO_TRAINING,
        RUNNING_BACK,
    }

    private NavMeshAgent agent;
    private Animation _animation;

    public State _state;
    public Vector3 _dest;

    private bool _isBlocking = false;
    private float _startBlockTime = 0;
    private CityBuilding _troopBuilding;
    private CityBuilding _trainBuilding;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _animation = GetComponent<Animation>();
    }

    void Start()
    {
        ChangeToIdleState();
    }

    void Update()
    {
        switch (_state) {
            case State.IDLE:
                IdleState();
                break;
            case State.IN_TROOP:
                InTroopState();
                break;
            case State.RUNNING_TO_TRAINING:
                RunningToTrainingState();
                break;
            case State.IN_TRAINING:
                InTrainingState();
                break;
            case State.RUNNING_BACK:
                RunningBackState();
                break;
        }
    }

    private void IdleState()
    {
    }

    private void InTroopState()
    {
        
    }

    private void RunningToTrainingState()
    {
        // 跑到目的地了，切换到建筑状态
        if (_trainBuilding != null) {
            if (Vector3.Distance(transform.position, _trainBuilding.transform.position) <= 1) {
                ChangeToInTrainingState();
            }
        }
    }

    private void InTrainingState()
    {
        
    }

    private void RunningBackState()
    {
        if (_troopBuilding != null) {
            if (Vector3.Distance(transform.position, _troopBuilding.transform.position) <= 1) {
                ChangeToInTroopState();
            }
        }
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
        _animation.Play("idle_re");
    }

    // 切换兵营状态
    private void ChangeToInTroopState()
    {
        _state = State.IN_TROOP;
        _animation.Play("attack_01");
    }

    // 切换到跑步状态（跑步到建筑物）
    private void ChangeToRunningToTainingState()
    {
        _state = State.RUNNING_TO_TRAINING;
        _animation.Play("run");
    }

    // 切换到训练状态
    private void ChangeToInTrainingState()
    {
        _state = State.IN_TRAINING;
        _animation.Play("attack_01");
    }

    // 跑回兵营
    private void ChangeToRunningBackState()
    {
        _state = State.RUNNING_BACK;
        _animation.Play("run");
    }

    // 跑到建筑物周围
    public void RunToTroop(CityBuilding building)
    {
        _troopBuilding = building;
        agent.destination = building.transform.position;
        ChangeToRunningBackState();
    }

    public void RunToTrain(CityBuilding building)
    {
        _trainBuilding = building;
        agent.destination = building.transform.position;
        ChangeToRunningBackState();
    }
}
