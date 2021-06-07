/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class Actor : MonoBehaviour
{
	#region Fields
	[Header( "Shared Variables" )]
	public SharedVector2 inputDirection;
	public ActorSet actorSet;

	[HorizontalLine( 2, EColor.Blue )]
	[Header( "Actor Related" )]
	public float rotateMultiplier;
	public Transform ragdollBody;
	public GameObject handle;
	public ColliderListener_EventRaiser collision_actor_Listener;

	// Private Fields
	private Rigidbody[] ragdollRigidbodies;
	private Collider[] ragdollColliders;

	private Collider collider_actor;
	[ SerializeField ] private Collider collider_obstacle;

	#endregion

	#region Unity API
	private void OnEnable()
	{
		actorSet.AddDictionary( collider_obstacle.gameObject.GetInstanceID(), this );
		actorSet.AddDictionary( collider_actor.gameObject.GetInstanceID(), this );

		collision_actor_Listener.triggerEnter += OnActorCollision;
	}

	private void OnDisable()
	{
		actorSet.RemoveDictionary( collider_obstacle.gameObject.GetInstanceID() );
		actorSet.RemoveDictionary( collider_actor.gameObject.GetInstanceID() );

		collision_actor_Listener.triggerEnter -= OnActorCollision;
	}

	private void Awake()
	{
		ragdollRigidbodies = ragdollBody.GetComponentsInChildren< Rigidbody >();
		ragdollColliders   = ragdollBody.GetComponentsInChildren< Collider  >();
		collider_actor     = collision_actor_Listener.GetComponent< Collider >();
	}

	private void Update()
	{
		transform.Rotate( Vector3.up * inputDirection.sharedValue.x * rotateMultiplier, Space.World ); // Rotate around Y axis
	}
#endregion

#region API
	[Button]
	public void ActivateRagdoll()
	{
		collider_actor.enabled    = false;
		collider_obstacle.enabled = false;

		rotateMultiplier = 0;

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
	private void OnActorCollision( Collider other )
	{
		collider_actor.enabled    = false;
		collider_obstacle.enabled = false;


	}
#endregion

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{

	}
#endif
}