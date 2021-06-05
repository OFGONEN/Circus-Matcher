/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FFStudio
{
	public class UIHorizontalProgressIndicator : UIProgressIndicator
	{
#region Fields
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
        protected override void OnProgressChange()
        {
			var position             = indicator_BasePosition;
			    position.x           = Mathf.Lerp( indicator_BasePosition.x, indicator_EndPosition.x, indicatorProgress.sharedValue );
			    uiTransform.position = position;
		}
		protected override void GetIndicatorPositions()
        {
            indicator_BasePosition = (indicatingParentWorldPos[ 0 ] + indicatingParentWorldPos[ 1 ]) / 2;
            indicator_EndPosition  = (indicatingParentWorldPos[ 2 ] + indicatingParentWorldPos[ 3 ]) / 2;
        }
#endregion
	}
}