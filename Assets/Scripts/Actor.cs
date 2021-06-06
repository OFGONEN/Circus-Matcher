/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Actor : MonoBehaviour
{
#region Fields
    public ActorSet actorSet;
#endregion

#region Unity API
    private void OnEnable()
    {
		actorSet.AddDictionary( gameObject.GetInstanceID(), this );
	}

    private void OnDisable()
    {
		actorSet.RemoveDictionary( gameObject.GetInstanceID() );
	}
#endregion

#region API
#endregion

#region Implementation
#endregion
}
