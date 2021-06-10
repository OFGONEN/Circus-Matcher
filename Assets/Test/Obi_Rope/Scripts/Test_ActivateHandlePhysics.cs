/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;

public class Test_ActivateHandlePhysics : MonoBehaviour
{
#region Fields
    public EventListenerDelegateResponse swingFinishedEventListener;

	private Rigidbody theRigidbody;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
		theRigidbody = GetComponent< Rigidbody >();
    }

    private void OnEnable()
    {
		swingFinishedEventListener.OnEnable();
	}
    
    private void OnDisable()
    {
		swingFinishedEventListener.OnDisable();
	}
    
    private void Awake()
    {
		swingFinishedEventListener.response = SwingFinishedResponse;
	}
#endregion

#region API
#endregion

#region Implementation
	private void SwingFinishedResponse()
	{
		theRigidbody.isKinematic = false;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
