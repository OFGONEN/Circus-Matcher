/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ApplyForce : MonoBehaviour
{
#region Fields
    public Vector3 force;
    private Rigidbody[] childRigidbodies;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
        childRigidbodies = GetComponentsInChildren< Rigidbody >();

        foreach( var rigidbody in childRigidbodies )
    		rigidbody.AddForce( force );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
