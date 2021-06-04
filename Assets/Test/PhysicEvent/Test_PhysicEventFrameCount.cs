/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PhysicEventFrameCount : MonoBehaviour
{
#region Fields
    public float speed;
    
    Rigidbody rb;
#endregion

#region Unity API
    private void Awake()
    {
		rb = GetComponent< Rigidbody >();

		rb.isKinematic = true;
		rb.useGravity  = false;
	}

    private void FixedUpdate() 
    {
		rb.MovePosition( rb.position + Vector3.right * speed * Time.fixedDeltaTime );
	}

    private void OnTriggerEnter( Collider other )
    {
		Debug.Log( other.name + " : " + Time.frameCount );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
