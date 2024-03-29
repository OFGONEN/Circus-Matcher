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
	[Header( "Event Listeners" )]
	public EventListenerDelegateResponse levelRevealedListener;
	public EventListenerDelegateResponse levelFailedListener;

	[Header( "Shared Variables" )]
	public SharedVector2 inputDirection;
	public SharedReferenceProperty mainCamera;
	public SharedReferenceProperty levelProgressIndicator;
	public ActorSet actorSet;

	[Header( "Fired Events" )]
	public ParticleSpawnEvent actorCollision_ParticleEvent;
	public ActorCollisionEvent actorCollisionEvent;
	public GameEvent actorSpawned;
	public GameEvent ascentComplete;

	[ BoxGroup( "Configure" ), Tooltip( "Actor that has same couple ID will match correctly" ) ] public int coupleID;
	[ BoxGroup( "Configure" ), Tooltip( "Multiply the input coming for rotating" ) ] public float rotateMultiplier;
	[ BoxGroup( "Configure" ), Tooltip( "Swing duration for one way" ) ] public float swingDuration = 1f;
	[ BoxGroup( "Configure" ), Tooltip( "Wait time every time a swing is complete" ) ] public float swingWaitDuration = 0.05f;

	[HorizontalLine( 2, EColor.Blue )]
	[Header( "Actor Related" )]
	public Animator actorAnimator;
	public Transform ragdollBody;
	public Rigidbody attachPoint;
	public Transform handle;
	public ColliderListener_EventRaiser collision_actor_Listener;

	// Property
	public Rigidbody GetAttachPoint 
	{
		get 
		{
			return attachPoint;
		}
	}

	public bool SwingingFoward
	{
		get
		{
			return swingingFoward;
		}
	}

	// Private Fields \\

	// Ragdoll
	private Rigidbody[] ragdollRigidbodies;
	private Collider[] ragdollColliders;

	// Actor colliders
	private Collider collider_actor;
	[ SerializeField ] private Collider collider_obstacle;

	// Swinging path points
	[ SerializeField, ReadOnly ] private Vector3[] swingWayPoints;

	// Sequences
	private Sequence ascentSequence;
	private Sequence ascent_IgnitionSequence;
	private Sequence swingSequence;

	// swing control variables
	private GetNormalizedTime swingNormalizedTime;
	private GetNormalizedTime next_swingNormalizedTime;
	private bool swingingFoward = true;
	private float swing_NormalizedTime = 0;

#endregion

#region Unity API
	private void OnEnable()
	{
		actorSet.AddDictionary( collider_obstacle.gameObject.GetInstanceID(), this );
		actorSet.AddDictionary( collider_actor.gameObject.GetInstanceID(), this );

		collision_actor_Listener.triggerEnter += OnActorCollision;

		levelRevealedListener.OnEnable();
		levelFailedListener  .OnEnable();
	}

	private void OnDisable()
	{
		actorSet.RemoveDictionary( collider_obstacle.gameObject.GetInstanceID() );
		actorSet.RemoveDictionary( collider_actor.gameObject.GetInstanceID() );

		collision_actor_Listener.triggerEnter -= OnActorCollision;

		levelRevealedListener.OnDisable();
		levelFailedListener  .OnDisable();

		if( ascentSequence != null )
		{
			ascentSequence.Kill();
			ascentSequence = null;
		}

		if( swingSequence != null )
		{
			swingSequence.Kill();
			swingSequence = null;
		}

		if( ascent_IgnitionSequence != null )
		{
			ascent_IgnitionSequence.Kill();
			ascent_IgnitionSequence = null;
		}
	}

	private void Awake()
	{
		ragdollRigidbodies = ragdollBody.GetComponentsInChildren< Rigidbody >();
		ragdollColliders   = ragdollBody.GetComponentsInChildren< Collider  >();
		collider_actor     = collision_actor_Listener.GetComponent< Collider >();

		foreach( var ragdoll in ragdollRigidbodies )
		{
			ragdoll.isKinematic = true;
			ragdoll.useGravity  = false;
		}

		foreach( var collider in ragdollColliders )
			collider.isTrigger = true;


		levelRevealedListener.response = StartSwinging;
		levelFailedListener.response   = ActivateRagdoll;
	}

	private void Start()
	{
		actorSpawned.Raise();
		actorAnimator.SetFloat( "normalized", 0 );

		swingingFoward = true;
	}

	private void Update()
	{
		transform.Rotate( Vector3.up * inputDirection.sharedValue.x * rotateMultiplier * Time.deltaTime, Space.World ); // Rotate around Y axis
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


		actorCollision_ParticleEvent.changePosition = true;
		actorCollision_ParticleEvent.spawnPoint = ( attachPoint.position + targetAttachPoint.position ) / 2; 
		actorCollision_ParticleEvent.particleAlias = "Actor";
		actorCollision_ParticleEvent.Raise();

		transform.DOMove( transform.position + Vector3.up * 20, 1 ).OnComplete( DisableChilds );
		target.transform.DOMove( target.transform.position + Vector3.up * 20, 1 ).OnComplete( target.DisableChilds );

		attachPoint.isKinematic = true;
		attachPoint.useGravity  = false;

		ascent_IgnitionSequence = DOTween.Sequence();

		ascent_IgnitionSequence.AppendInterval( GameSettings.Instance.actor_ascent_IgnitionTime );

		ascent_IgnitionSequence.AppendCallback( () =>
		{
			attachPoint.isKinematic = false;
			attachPoint.useGravity = true;
		} );

		ascent_IgnitionSequence.AppendInterval( GameSettings.Instance.actor_ascent_FallDuration );

		ascent_IgnitionSequence.AppendCallback( () =>
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

			var screenPos      = indicator.position;
			    // screenPos.z    = GameSettings.Instance.actor_ascentDistance_Z;
			    screenPos.z    = Vector3.Distance( coupleParent.position, camera.transform.position );
			var targetPosition = camera.ScreenToWorldPoint( screenPos );

			ascentSequence = DOTween.Sequence();

			ascentSequence.Join( coupleParent.DOMove( targetPosition, GameSettings.Instance.actor_ascent_Duration ) );
			ascentSequence.Join( coupleParent.DOLookAt( targetPosition, GameSettings.Instance.actor_ascent_Duration ) );
			ascentSequence.Join( coupleParent.DOScale( 0, GameSettings.Instance.actor_ascent_Scale_Duration ).SetDelay( GameSettings.Instance.actor_ascent_Scale_Delay ) );

			ascentSequence.OnComplete( () => OnAscentDone( target ) );

		} );

		ascent_IgnitionSequence.OnComplete( () => ascent_IgnitionSequence = null );
	}

	public void ActivateRagdoll()
	{
		if( swingSequence != null )
		{
			swingSequence.Kill();
			swingSequence = null;
		}

		collider_actor.enabled    = false;
		collider_obstacle.enabled = false;

		actorAnimator.enabled = false;

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
	private void StartSwinging()
	{
		swingNormalizedTime      = GetNormalizedTime_Foward;
		next_swingNormalizedTime = GetNormalizedTime_Backward;

		swingSequence = DOTween.Sequence();

		swingSequence.SetDelay( swingWaitDuration );
		swingSequence.Append( handle.DOLocalPath( swingWayPoints, swingDuration ).SetEase( Ease.Linear ) );
		swingSequence.AppendInterval( swingWaitDuration );
		swingSequence.SetLoops( -1, LoopType.Yoyo );
		swingSequence.OnStepComplete( OnSwingStopComplete );
		swingSequence.OnUpdate( OnSwingUpdate );
	}

	private void OnSwingStopComplete()
	{
		var current                  = swingNormalizedTime;
		    swingNormalizedTime      = next_swingNormalizedTime;
		    next_swingNormalizedTime = current;

		swingingFoward = !swingingFoward;
	}

	[Button]
	private void OnSwingUpdate()
	{
		swing_NormalizedTime = swingNormalizedTime();
		actorAnimator.SetFloat( "normalized", swing_NormalizedTime );
	}

	private float GetNormalizedTime_Foward()
	{
		var normalizedTime = swingSequence.ElapsedPercentage( false );

		if( normalizedTime < swing_NormalizedTime )
			normalizedTime = swing_NormalizedTime;

		return normalizedTime;
	}

	private float GetNormalizedTime_Backward()
	{
		var normalizedTime = 1 - swingSequence.ElapsedPercentage( false );

		if( normalizedTime > swing_NormalizedTime )
			normalizedTime = swing_NormalizedTime;

		return normalizedTime;
	}
	private void OnActorCollision( Collider other )
	{
		collider_actor.enabled    = false;
		collider_obstacle.enabled = false;

		// FFLogger.Log( "Actor Collision: " + collider_actor.gameObject.GetInstanceID() + " - " + other.gameObject.GetInstanceID() );

		actorCollisionEvent.actorCollision.baseActorID   = collider_actor.gameObject.GetInstanceID();
		actorCollisionEvent.actorCollision.targetActorID = other.gameObject.GetInstanceID();
		actorCollisionEvent.Raise();
	}

	private void OnAscentDone( Actor target )
	{
		ascentSequence = null;
		ascentComplete.Raise();

		ragdollBody.gameObject	     .SetActive( false );
		target.ragdollBody.gameObject.SetActive( false );
	}

	private void DisableChilds()
	{
		for( var i = 0; i < transform.childCount; i++ )
		{
			transform.GetChild( i ).gameObject.SetActive( false );
		}
	}
#endregion

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{

	}

	[ Button ]
	private void SetSwingWayPoints()
	{
		var waypointsParent = transform.GetChild( 4 );

		swingWayPoints = new Vector3[ waypointsParent.childCount ];

		for( var i = 0; i < waypointsParent.childCount; i++ )
			swingWayPoints[ i ] = waypointsParent.GetChild( i ).position - transform.position;
	}
#endif
}