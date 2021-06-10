/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class Test_FollowArcPathTween : MonoBehaviour
{
#region Fields
    public Transform[] waypoints;
#endregion

#region Properties
#endregion

#region Unity API
    private void Start()
    {
		var pathPositions = waypoints.Select( waypoint => waypoint.position ).ToArray();
		transform.DOPath( pathPositions, 1.0f ).SetLoops( -1, LoopType.Yoyo );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
