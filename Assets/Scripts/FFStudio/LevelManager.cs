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
		public EventListenerDelegateResponse collisionObstacleListener;

		[Header("Fired Events")]
        public GameEvent levelFailedEvent;
        public GameEvent levelCompleted;


        [Header("Level Releated")]
        public SharedFloatProperty levelProgress;
		public ActorSet actorSet;

		#endregion

		#region UnityAPI

		private void OnEnable()
        {
            levelLoadedListener      .OnEnable();
            levelRevealedListener    .OnEnable();
            levelStartedListener     .OnEnable();
			collisionObstacleListener.OnEnable();
		}

        private void OnDisable()
        {
            levelLoadedListener      .OnDisable();
            levelRevealedListener    .OnDisable();
            levelStartedListener     .OnDisable();
			collisionObstacleListener.OnDisable();
        }

        private void Awake()
        {
            levelLoadedListener.response       = LevelLoadedResponse;
            levelRevealedListener.response     = LevelRevealedResponse;
            levelStartedListener.response      = LevelStartedResponse;
            collisionObstacleListener.response = CollisionObstacleResponse;
		}

        #endregion

        #region Implementation
        void LevelLoadedResponse()
        {
            levelProgress.SetValue(0);
        }

        void LevelRevealedResponse()
        {

        }

        void LevelStartedResponse()
        {

        }

		void CollisionObstacleResponse()
		{
			var changeEvent = collisionObstacleListener.gameEvent as ReferenceGameEvent;
			var instanceId = ( changeEvent.eventValue as Collider ).gameObject.GetInstanceID();

			Actor actor;
			actorSet.itemDictionary.TryGetValue( instanceId, out actor );

			if( actor == null )
				return;

			actor.ActivateRagdoll();
            FFLogger.Log( "Activate Ragdoll: " + actor.gameObject.name );

			// levelFailedEvent.Raise();
		}
		#endregion
	}
}