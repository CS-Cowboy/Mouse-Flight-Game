using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    [CreateAssetMenu(fileName = "Stats", menuName = "Generic Stats")]
	public class Stats : ScriptableObject
    {
		public float initialHealth = 10f, moveForce = 100f, mass = 0.5f, turnForce = 1f, 
        lifespan = Mathf.Infinity, drag = 1f;
    }
}