using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piston : MonoBehaviour {

	public enum FollowType
	{
		MoveTowards,
		Lerp
	}
	public FollowType Type = FollowType.MoveTowards;

	public enum PistonState
	{
		Splash,
		Return, 
		Kill
	}
	
	private PistonState state = PistonState.Splash;

	public MovementPath path;

	public float returningSpeed;
	public float splashSpeed;

	public float maxDistance;

	public float delayBetweenSplash = 2;
	public float returningDelay = 1;
	public float startDelay = 2;
	
	private IEnumerator<Transform> _currentPoint;
	private bool _isReadyToMove = false;

    public KillingZone killingZone;

    private float startTime = 0;
    private bool _isStarting = true;

    private bool _forceTurnOff = false;
    private bool _forceTurnOn = false;

	public void OnEnable ()
	{
        if (!_forceTurnOff)
        {
            StartWorking();
        }
        else
        {
            _forceTurnOn = true;
        }
	}
	
    private void StartWorking ()
    {
        if (path != null)
        {
            _currentPoint = path.GetPathEnumerator();
            _currentPoint.MoveNext();

            if (_currentPoint.Current == null)
            {
                return;
            }

            transform.position = _currentPoint.Current.position;
        }

        _isReadyToMove = false;
        startTime = 0;
        _isStarting = true;
        state = PistonState.Splash;
        //StartCoroutine("StartWorking");
    }

    public void StopWorking ()
	{
        if (_isStarting)
        {
            _isReadyToMove = false;
            _isStarting = false;
            this.enabled = false;
        } 
        else
        {
            _forceTurnOff = true;
        }

        
		//StopCoroutine("StartWorking");
	}

    //public IEnumerator StartWorking () {
    //    Debug.Log("START");
    //    yield return new WaitForSeconds (startDelay);
    //    Debug.Log("STOP");
    //    _isReadyToMove = true;

		
    //}

	public IEnumerator Move (float delay) {
		_isReadyToMove = false;
		yield return new WaitForSeconds (delay);
		_currentPoint.MoveNext();
		_isReadyToMove = true;

	}
	
	public void Update () 
	{
		if (_isReadyToMove == true) 
		{
			if (_currentPoint == null || _currentPoint.Current == null) 
			{ 
				return;
			}

			var speed = state == PistonState.Splash ? returningSpeed : splashSpeed;

			if (Type == FollowType.MoveTowards)
			{
				transform.position = Vector3.MoveTowards(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
			}
			else if (Type == FollowType.Lerp)
			{
				transform.position = Vector3.Lerp(transform.position, _currentPoint.Current.position, Time.deltaTime * speed);
			}
			
			var distance = (transform.position - _currentPoint.Current.position).sqrMagnitude;
			if (distance < maxDistance * maxDistance)
			{
				float delay;
				if (state == PistonState.Splash)
				{
					delay = delayBetweenSplash;
					state = PistonState.Return;
                    if (killingZone != null)
                    {
                        killingZone.SetEnabled();
                    }

                    if (_forceTurnOff)
                    {
                        _isReadyToMove = false;
                        _forceTurnOff = false;
                        if (_forceTurnOn)
                        {
                            _forceTurnOn = false;
                            StartWorking();
                        }
                        else
                        {
                            this.enabled = false;
                        }
                    }         
                   
				}
				else 
				{
					delay = returningDelay;
					state = PistonState.Splash;
                    if (killingZone != null)
                    {
                        killingZone.SetDisabled();
                        
                    }
                            
				}

				StartCoroutine("Move", delay);

			}
		}

        else if (_isStarting)
        {
            if (Mathf.Abs(startTime -= Time.deltaTime) > startDelay)
            {
                _isStarting = false;
                _isReadyToMove = true;
            }
        }
	
	}

}
