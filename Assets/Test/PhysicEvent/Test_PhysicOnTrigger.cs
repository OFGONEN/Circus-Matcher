/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PhysicOnTrigger : MonoBehaviour
{
#region Fields
#endregion

#region Unity API
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
