using System.Collections;
using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

namespace FFStudio
{
    public class MobileInput : MonoBehaviour
    {
		[Header( "Fired Events" )]
		public SwipeInputEvent swipeInputEvent;
		public IntGameEvent tapInputEvent;
		public SharedVector2 inputDirection;

		// Private fields
		private LeanFingerDelegate fingerUpdate;
		private Vector2 fingerStartPosition;

		private float swipeThreshold;

		private void Awake()
		{
			swipeThreshold = Screen.width * GameSettings.Instance.swipeThreshold / 100;
			fingerUpdate   = FingerDown;
		}
		public void Swiped( Vector2 delta )
		{
			swipeInputEvent.ReceiveInput( delta );
		}
		public void Tapped( int count )
		{
			tapInputEvent.eventValue = count;

			tapInputEvent.Raise();
		}

		public void LeanFingerUpdate( LeanFinger finger )
		{
			fingerUpdate( finger );
		}
		public void LeanFingerUp( LeanFinger finger )
		{
			inputDirection.sharedValue = Vector2.zero;
			fingerUpdate               = FingerDown;
		}

		private void FingerDown( LeanFinger finger )
		{
			fingerStartPosition = finger.ScreenPosition;
			fingerUpdate        = FingerUpdate;
		}
		private void FingerUpdate( LeanFinger finger )
		{
			var diff = finger.ScreenPosition.x - fingerStartPosition.x;

			if( Mathf.Abs( diff ) <= swipeThreshold )
				inputDirection.sharedValue = Vector2.zero;
			else 
			{
				inputDirection.sharedValue.x = Mathf.Sign( diff );
				inputDirection.sharedValue.y = 0;
			}

		}
    }
}