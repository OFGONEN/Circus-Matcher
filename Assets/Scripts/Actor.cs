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
	public SharedReferenceProperty mainCamera;
	public SharedReferenceProperty levelProgressIndicator;
	public ActorSet actorSet;

	[Header( "Fired Events" )]
	public ActorCollisionEvent actorCollisionEvent;

	[HorizontalLine( 2, EColor.Blue )]
	[Header( "Actor Related" )]
	public int coupleID;
	public float rotateMultiplier;
	public Transform ragdollBody;
	public Rigidbody attachPoint;
	public GameObject handle;
	public ColliderListener_EventRaiser collision_actor_Listener;

	// Property
	public Rigidbody GetAttachPoint 
	{
		get 
		{
			return attachPoint;
		}
	}

	// Private Fields
	private Rigidbody[] ragdollRigidbodies;
	private Collider[] ragdollColliders;

	private Collider collider_actor;
	[ SerializeField ] private Collider collider_obstacle;

	private Sequence ascentTween;

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

		if(ascentTween != null)
		{
			ascentTween.Kill();
			ascentTween = null;
		}
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
	public void Ascent(Actor target)
	{
		// FFLogger.Log( "Ascent: " + gameObject.name + " - " + target.gameObject.name );
		ActivateRagdoll();
		target.ActivateRagdoll();

		var targetAttachPoint = target.GetAttachPoint;
		var targetJoint       = targetAttachPoint.gameObject.AddComponent< FixedJoint >();

		targetJoint.connectedBody       = attachPoint;
		targetJoint.enablePreprocessing = false;
		targetJoint.connectedMassScale  = 1;
		targetJoint.massScale           = 1;


		DOVirtual.DelayedCall( 0.25f, () => 
		{
			// Make base rigidbody kinematic for tweening
			attachPoint.isKinematic = true;
			attachPoint.useGravity  = false;

			// Create a empty object to child both of the ragdoll
			var coupleParent             = new GameObject( "CoupleParent" ).transform;
			    coupleParent.position    = attachPoint.position;
			    coupleParent.eulerAngles = attachPoint.rotation.eulerAngles;

			ragdollBody       .SetParent( coupleParent );
			target.ragdollBody.SetParent( coupleParent );

			var camera    = mainCamera.sharedValue as Camera;
			var indicator = levelProgressIndicator.sharedValue as RectTransform;

			var targetPosition   = camera.ScreenToWorldPoint( indicator.position );
			    targetPosition.z = coupleParent.position.z;

			ascentTween = DOTween.Sequence();

			ascentTween.Join( coupleParent.DOMove( targetPosition, 0.75f ) );
			ascentTween.Join( coupleParent.DOLookAt( targetPosition, 0.75f ) );
			ascentTween.Join( coupleParent.DOScale( 0, 0.25f ).SetDelay( 0.5f ) );

			ascentTween.OnComplete( () => ascentTween = null );
		} );
	}

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

		// FFLogger.Log( "Actor Collision: " + collider_actor.gameObject.GetInstanceID() + " - " + other.gameObject.GetInstanceID() );

		actorCollisionEvent.actorCollision.baseActorID   = collider_actor.gameObject.GetInstanceID();
		actorCollisionEvent.actorCollision.targetActorID = other.gameObject.GetInstanceID();
		actorCollisionEvent.Raise();
	}
#endregion

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{

	}
#endif
}
