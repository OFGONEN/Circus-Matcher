/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "ActorCollisionEvent", menuName = "FF/Event/ActorCollision" ) ]
public class ActorCollisionEvent : GameEvent
{
	public ActorCollision actorCollision;
}
