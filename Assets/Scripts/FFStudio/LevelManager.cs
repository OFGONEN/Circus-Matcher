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
		public EventListenerDelegateResponse collision_ObstacleListener;
		public EventListenerDelegateResponse collision_ActorListener;

		[Header("Fired Events")]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;


        [Header("Level Releated")]
        public SharedFloatProperty levelProgress;
		public ActorSet actorSet;

		// Private Fields
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
		}

        private void OnDisable()
        {
            levelLoadedListener       .OnDisable();
            levelRevealedListener     .OnDisable();
            levelStartedListener      .OnDisable();
			collision_ObstacleListener.OnDisable();
			collision_ActorListener   .OnDisable();
        }

        private void Awake()
        {
            levelLoadedListener.response        = LevelLoadedResponse;
            levelRevealedListener.response      = LevelRevealedResponse;
            levelStartedListener.response       = LevelStartedResponse;
            collision_ObstacleListener.response = CollisionObstacleResponse;
			collision_ActorListener.response    = CollisionActorResponse;
		}

        #endregion

        #region Implementation
        void LevelLoadedResponse()
        {
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

			// levelFailedEvent.Raise();
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

					baseActor.ActivateRagdoll();
					targetActor.ActivateRagdoll();

                    FFLogger.Log( "Collision between " + baseActor.gameObject.name + " - " + targetActor.gameObject.name );
					return;
				}
            }

            // Collision is not found so add this collision to list
            FFLogger.Log( "Actor Collision is not found!" );
		    FFLogger.Log( "Actor Collision: " + changeEvent.actorCollision.baseActorID + " - " + changeEvent.actorCollision.targetActorID );
			actorCollisions.Add( changeEvent.actorCollision );
		}
		#endregion
	}
}