using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace FFStudio
{
	public class GameSettings : ScriptableObject
    {
#region Fields
        [ BoxGroup ( "Actor" ), Tooltip( "After first collision actors hanging in air duration " ) ] 
        public float actor_ascent_IgnitionTime = 0.25f; // Anticipation graph straight line duration

        [ BoxGroup ( "Actor" ), Tooltip( "After hanging in the air falling duration" ) ] 
        public float actor_ascent_FallDuration = 0.25f; // Anticipation graph declining duration 

        [ BoxGroup ( "Actor" ) ] public float actor_ascent_Duration = 0.75f;
        [ BoxGroup ( "Actor" ) ] public float actor_ascent_Scale_Duration = 0.25f;
        [ BoxGroup ( "Actor" ) ] public float actor_ascent_Scale_Delay = 0.5f;
        [ HideInInspector ] public int maxLevelCount;
        [Foldout("UI Settings"), Tooltip("Duration of the movement for ui element")] public float ui_Entity_Move_TweenDuration;
        [Foldout("UI Settings"), Tooltip("Duration of the fading for ui element")] public float ui_Entity_Fade_TweenDuration;
		[Foldout("UI Settings"), Tooltip("Duration of the scaling for ui element")] public float ui_Entity_Scale_TweenDuration;
		[Foldout("UI Settings"), Tooltip("Duration of the movement for floating ui element")] public float ui_Entity_FloatingMove_TweenDuration;
        [Foldout("UI Settings"), Tooltip("Percentage of the screen to register a swipe")] public float swipeThreshold;


        private static GameSettings instance;

        private delegate GameSettings ReturnGameSettings();
        private static ReturnGameSettings returnInstance = LoadInstance;

        public static GameSettings Instance
        {
            get
            {
                return returnInstance();
            }
        }
#endregion

#region Implementation
        static GameSettings LoadInstance()
        {
            if (instance == null)
                instance = Resources.Load<GameSettings>("game_settings");

            returnInstance = ReturnInstance;

            return instance;
        }

        static GameSettings ReturnInstance()
        {
            return instance;
        }
#endregion
    }
}
