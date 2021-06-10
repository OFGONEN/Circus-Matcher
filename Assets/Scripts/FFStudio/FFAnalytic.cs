using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using Facebook.Unity;
using ElephantSDK;

namespace FFStudio
{
	public class FFAnalytic : MonoBehaviour
	{
#region Fields
		[Header( "Event Listeners" )]
		public EventListenerDelegateResponse elephantEventListener;
#endregion

#region UnityAPI
		private void OnEnable()
		{
			elephantEventListener.OnEnable();
		}

		private void OnDisable()
		{
			elephantEventListener.OnDisable();
		}

		private void Awake()
		{
			elephantEventListener.response = ElephantEventResponse;

			if( !FB.IsInitialized )
				FB.Init( OnFacebookInitialized, OnHideUnity );
			else
				FB.ActivateApp();
		}

#endregion

#region Implementation
		void ElephantEventResponse()
		{
			var gameEvent = elephantEventListener.gameEvent as ElephantLevelEvent;

			switch( gameEvent.elephantEventType )
			{
				case ElephantEvent.LevelStarted:
					Elephant.LevelStarted( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelStarted: " + gameEvent.level );
					break;
				case ElephantEvent.LevelCompleted:
					Elephant.LevelCompleted( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelFinished: " + gameEvent.level );
					break;
				case ElephantEvent.LevelFailed:
					Elephant.LevelFailed( gameEvent.level );
					FFLogger.Log( "FFAnalytic Elephant LevelFailed: " + gameEvent.level );
					break;
			}
		}
		void OnFacebookInitialized()
		{
			if( FB.IsInitialized )
			{
				FB.ActivateApp();
				Debug.Log( "[FFAnalitic] Facebook initiliazed" );
			}
			else
				Debug.Log( "[FFAnalitic] Failed to initialize Facebook SDK" );


			DontDestroyOnLoad( gameObject );

		}

		void OnHideUnity( bool hide )
		{

		}
#endregion
	}
}