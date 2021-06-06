/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class Actor : MonoBehaviour
{
#region Fields
	[Header( "Shared Variables" )]
	public ActorSet actorSet;

	[HorizontalLine( 2, EColor.Blue )]
	[Header( "Actor Objects" )]
	public Transform ragdollBody;
	public GameObject handle;

	// Private Fields
	private Rigidbody[] ragdollRigidbodies;
	private Collider[] ragdollColliders;
#endregion

#region Unity API
	private void OnEnable()
	{
		actorSet.AddDictionary( gameObject.GetInstanceID(), this );
	}

	private void OnDisable()
	{
		actorSet.RemoveDictionary( gameObject.GetInstanceID() );
	}

	private void Awake()
	{
		ragdollRigidbodies = ragdollBody.GetComponentsInChildren< Rigidbody >();
		ragdollColliders   = ragdollBody.GetComponentsInChildren< Collider  >();
	}
#endregion

#region API

	[Button]
	public void ActivateRagdoll()
	{
		ragdollBody.SetParent( null );

		foreach( var ragdoll in ragdollRigidbodies )
		{
			ragdoll.isKinematic = false;
			ragdoll.useGravity  = true;
		}

		foreach( var collider in ragdollColliders )
			collider.isTrigger = false;

		var handleRigidbody = handle.GetComponent< Rigidbody >();
		var handleCollider  = handle.GetComponent< Collider  >();

		handleRigidbody.isKinematic = false;
		handleRigidbody.useGravity  = true;
		handleCollider.isTrigger    = false;
	}
#endregion

#region Implementation
#endregion

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{

	}
#endif
}
