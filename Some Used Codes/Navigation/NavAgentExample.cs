using UnityEngine;
using System.Collections;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentExample : MonoBehaviour 
{

	public AIWaypointNetwork WaypointNetwork = null;
	public int CurrentIndex = 0;
	public bool HasPath = false;
	public bool PathPending = false;
	public bool PathStale = false;
	public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;
	public AnimationCurve JumpCurve = new AnimationCurve();

	private NavMeshAgent _navAgent = null;

	void Start () {


		_navAgent = GetComponent<NavMeshAgent> ();
	/*	_navAgent.updatePosition = false;
		_navAgent.updateRotation = false;
*/
		if (WaypointNetwork == null)
			return;
		SetNextDestination(false);

	}


	void SetNextDestination( bool increment){

		if (!WaypointNetwork)
			return ;

		int IncStep = increment ? 1 : 0;
		Transform nextWaypointTransform = null;


		
			int nextWaypoint = (CurrentIndex + IncStep >= WaypointNetwork.Waypoints.Count) ? 0 : CurrentIndex + IncStep;
			nextWaypointTransform = WaypointNetwork.Waypoints [nextWaypoint];
		
			if (nextWaypointTransform != null) {
			
				CurrentIndex = nextWaypoint;
				_navAgent.destination = nextWaypointTransform.position;
				return;
			}
		
		CurrentIndex++;

	}

	void Update () 
	{

		HasPath = _navAgent.hasPath;
		PathPending = _navAgent.pathPending;
		PathStale = _navAgent.isPathStale;
		PathStatus = _navAgent.pathStatus;


		if (_navAgent.isOnOffMeshLink) {
			StartCoroutine (Jump (1.0f));
			return;
		}


		if ((_navAgent.remainingDistance<= _navAgent.stoppingDistance && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid /*|| PathStatus == NavMeshPathStatus.PathPartial*/)
		{
			SetNextDestination (true);
		} else if (_navAgent.isPathStale)
			SetNextDestination (false);  
	}

	IEnumerator Jump ( float duration ){
	
		OffMeshLinkData data = _navAgent.currentOffMeshLinkData;
		Vector3 startPos = _navAgent.transform.position;
		Vector3 endPos = data.endPos + (_navAgent.baseOffset * Vector3.up);
		float time = 0.0f;
		 
		while (time <= duration) {
		
			float t = time / duration;
			_navAgent.transform.position = Vector3.Lerp (startPos, endPos, t) + (JumpCurve.Evaluate(t) * Vector3.up);
			time += Time.deltaTime;
			yield return null;

		}
		_navAgent.CompleteOffMeshLink();
			
	
	}

}
