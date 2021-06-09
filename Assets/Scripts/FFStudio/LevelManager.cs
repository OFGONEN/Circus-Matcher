using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace FFStudio
{
    public class LevelManager : MonoBehaviour
    {
#region Fields
        [Header("Event Listeners")]
        public EventListenerDelegateResponse levelLoadedListener;
        public EventListenerDelegateResponse levelRevealedListener;
        public EventListenerDelegateResponse levelStartedListener;

        // Level Related
		public EventListenerDelegateResponse actor_SpawnedListener;
		public EventListenerDelegateResponse actor_MathcedListener;
		public EventListenerDelegateResponse collision_ObstacleListener;
		public EventListenerDelegateResponse collision_ActorListener;


		[Header("Fired Events")]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;


        [Header("Level Releated")]
        public SharedFloatProperty levelProgress;
		public ActorSet actorSet;

		// Private Fields
		int actor_Count = 0;
		int actor_CoupleMatched_Count = 0;
		List< ActorCollision > actorCollisions = new List< ActorCollision >( 8 );
#endregion

#region UnityAPI

		private void OnEnable()
        {
            levelLoadedListener       .OnEnable();
            levelRevealedListener     .OnEnable();
            levelStartedListener      .OnEnable();
			collision_ObstacleListener.OnEnable();
			collision_ActorListener   .OnEnable();
			actor_SpawnedListener     .OnEnable();
			actor_MathcedListener     .OnEnable();
		}

        private void OnDisable()
        {
            levelLoadedListener       .OnDisable();
            levelRevealedListener     .OnDisable();
            levelStartedListener      .OnDisable();
			collision_ObstacleListener.OnDisable();
			collision_ActorListener   .OnDisable();
			actor_SpawnedListener     .OnDisable();
			actor_MathcedListener     .OnDisable();
        }

        private void Awake()
        {
            levelLoadedListener.response        = LevelLoadedResponse;
            levelRevealedListener.response      = LevelRevealedResponse;
            levelStartedListener.response       = LevelStartedResponse;
            collision_ObstacleListener.response = CollisionObstacleResponse;
			collision_ActorListener.response    = CollisionActorResponse;
			actor_SpawnedListener.response      = ActorSpawned;
			actor_MathcedListener.response      = ActorCoupleMatchedResponse;
		}

#endregion

#region Implementation
        void LevelLoadedResponse()
        {
			actor_Count               = 0;
			actor_CoupleMatched_Count = 0;
			levelProgress.SetValue(0);
			actorCollisions.Clear();
		}

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {

        }

		void CollisionObstacleResponse()
		{
			var changeEvent = collision_ObstacleListener.gameEvent as ReferenceGameEvent;
			var instanceId = ( changeEvent.eventValue as Collider ).gameObject.GetInstanceID();

			Actor actor;
			actorSet.itemDictionary.TryGetValue( instanceId, out actor );

			if( actor == null )
				return;

			actor.ActivateRagdoll();
            FFLogger.Log( "Activate Ragdoll: " + actor.gameObject.name );

			levelFailedEvent.Raise();
		}

        void CollisionActorResponse()
        {
			var changeEvent = collision_ActorListener.gameEvent as ActorCollisionEvent;

            for( var i = 0; i < actorCollisions.Count; i++ )
            {
                if(actorCollisions[i].targetActorID == changeEvent.actorCollision.baseActorID )
                {
					// handle collision
					Actor baseActor;
					Actor targetActor;

					actorSet.itemDictionary.TryGetValue( changeEvent.actorCollision.baseActorID, out baseActor );
					actorSet.itemDictionary.TryGetValue( changeEvent.actorCollision.targetActorID, out targetActor );

					// if ( /* Correct Couple */ baseActor.coupleID == targetActor.coupleID && 
					// 	 /* Both actors swinging foward */ baseActor.SwingingFoward && targetActor.SwingingFoward ) 
					// * Swinging foward check is removed no matter what direction is the swinging its always valid
					if( /* Correct Couple */ baseActor.coupleID == targetActor.coupleID )
					{
						baseActor.Ascent( targetActor );
					}
                    else 
                    {
						levelFailedEvent.Raise();

						baseActor.ActivateRagdoll();
					    targetActor.ActivateRagdoll();
                    }

                    // FFLogger.Log( "Collision between " + baseActor.gameObject.name + " - " + targetActor.gameObject.name );
					return;
				}
            }

            // Collision is not found so add this collision to list
            // FFLogger.Log( "Actor Collision is not found!" );
		    // FFLogger.Log( "Actor Collision: " + changeEvent.actorCollision.baseActorID + " - " + changeEvent.actorCollision.targetActorID );
			actorCollisions.Add( changeEvent.actorCollision );
		}

        void ActorCoupleMatchedResponse()
        {
			actor_CoupleMatched_Count++;

			int coupleCount = actor_Count / 2;

			float progress = actor_CoupleMatched_Count / ( float )coupleCount;

			DOTween.To(
				() => levelProgress.sharedValue, // Getter
				x => levelProgress.SetValue( x ), // Setter
				progress, // End value
				GameSettings.Instance.ui_Entity_Move_TweenDuration /* duration */ )
				.OnComplete( RaiseLevelComplete );
		}

        void ActorSpawned()
        {
			actor_Count++;
		}

		void RaiseLevelComplete()
		{
			int coupleCount = actor_Count / 2;

			if ( coupleCount == actor_CoupleMatched_Count )
				levelCompleted.Raise();
		}
#endregion
	}
}