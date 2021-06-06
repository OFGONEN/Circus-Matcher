/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NaughtyAttributes;

[ ExecuteInEditMode ]
public class ArcPathGenerator : MonoBehaviour
{
#region Fields
	[ InfoBox( "These points are in relative coordinates; they are relative to the position of this GameObject's transform." ) ]
	public Vector3 startPointOffset;
    public Vector3 endPointOffset;
	[ InfoBox( "Height = [middle of start & end] - [most bottom point]" ), Label( "Height (in Y)") ]
	public float height;
	public int numberOfWaypoints;
    
    public AnimationCurve curve;
    
    private List< Transform > waypoints;
	private Vector3[] waypointVectors;
#endregion

#region Properties
#endregion

#region Unity API
    private void Reset()
    {
		waypoints = null;
		waypointVectors = null;
	}
#endregion

#region API
    [ Button(), EnableIf( "CanGenerate" ) ]
    public void GenerateWaypoints()
    {
		Transform waypointsParentTransform;
		if( ( waypointsParentTransform = transform.Find( "waypoints" ) ) != null ) // There already is a GameObject named "waypoints".
            // Remove all children as they will be created again now.
            for( var i = waypointsParentTransform.childCount - 1; i >= 0; i-- )
                DestroyImmediate( waypointsParentTransform.GetChild( i ).gameObject );
		else // Create a GameObject named "waypoints".
        {
			waypointsParentTransform = new GameObject( "waypoints" ).transform;
			waypointsParentTransform.SetParent( transform );
        }

        if( waypoints == null )
			waypoints = new List<Transform>();
		else 
            waypoints.Clear();

		/* Handles.DrawDottedLines() expects PAIRS of positions (start & end) for each LINE SEGMENT.
		 * In an array of N points, there are N-1 line segments and the number of pairs are the double of line segment count. */
		waypointVectors = new Vector3[ 2 * ( numberOfWaypoints - 1 ) ];
        
        var horizontalDistance  = Vector3.Distance( endPointOffset, startPointOffset );
		var horizontalDelta     = horizontalDistance / ( numberOfWaypoints - 1 );
		var horizontalDirection = ( endPointOffset - startPointOffset ).normalized;
        
		var verticalDistance    = height;
		var verticalDirection   = -Vector3.up;

        /* Generate waypoints and pairs for Handles.DrawDottedLines(). */
		for( var i = 0; i < numberOfWaypoints; i++ )
        {
            /* This waypoint. */
			var x = i * horizontalDelta;
			var y = curve.Evaluate( ( float )x / horizontalDistance ) * verticalDistance;
			Vector3 position = transform.position + startPointOffset +
							   x * horizontalDirection +
							   y * verticalDirection;
			Transform waypointTransform = new GameObject( "waypoint_" + ( i + 1 ) ).transform;
			waypointTransform.position = position;
			waypointTransform.SetParent( waypointsParentTransform );
			waypoints.Add( waypointTransform );

			/* Don't include the pair [N-1, N]. */
			if( i < numberOfWaypoints - 1 )
            {
				/* Next waypoint. */
				var nextX = ( i + 1 ) * horizontalDelta;
				var nextY = curve.Evaluate( ( float )nextX / horizontalDistance ) * verticalDistance;
				Vector3 nextPosition = transform.position + startPointOffset +
									   nextX * horizontalDirection +
									   nextY * verticalDirection;

				waypointVectors[ i * 2 + 0 ] = position;
				waypointVectors[ i * 2 + 1 ] = nextPosition;
			}
		}
	}
#endregion

#region Implementation
    private bool CanGenerate()
    {
		return Vector3.Distance( endPointOffset, startPointOffset ) > float.Epsilon && !Mathf.Approximately( height, 0 ) && 
               numberOfWaypoints > 2 && curve != null;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /* Waypoints. */
		if( waypoints == null || waypoints.Count < 2 )
		{
		    Gizmos.color = Color.red;
			Gizmos.DrawWireSphere( transform.position + startPointOffset,  0.1f );
		    Gizmos.color = Color.green;
			Gizmos.DrawWireSphere( transform.position + endPointOffset, 0.1f );
		}
        else
			for( var i = 0; i < waypoints.Count; i++ )
			{
				Gizmos.color = Color.Lerp( Color.red, Color.green, ( float )i / ( waypoints.Count - 1 ) );
				Gizmos.DrawWireSphere( waypoints[ i ].position, 0.1f );
			}

		/* Line segments as dotted lines. */
		Handles.color = Color.black;
		if( waypointVectors != null && waypointVectors.Length > 0 )
			Handles.DrawDottedLines( waypointVectors, 1.0f );
	}
#endif
#endregion
}