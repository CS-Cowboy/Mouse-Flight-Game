using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    [CreateAssetMenu(fileName = "ShipStats", menuName = "Ship Stats")]
    public class ShipStats : Stats
    {
	  	public float strafeForce = 50f, chargeDT = 0.001f, rechargeDelay = 2f;
    }
}