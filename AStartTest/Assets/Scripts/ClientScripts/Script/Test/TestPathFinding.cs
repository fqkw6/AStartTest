using UnityEngine;
using System.Collections;

public class TestPathFinding : MonoBehaviour
{
    public Transform _target;

    private NavMeshAgent _agent;

	// Use this for initialization
	void Start ()
	{
	    _agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButton(0)) {
	        _agent.SetDestination(_target.transform.position);
	    }
	}
}
