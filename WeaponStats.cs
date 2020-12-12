using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
    [CreateAssetMenu(fileName = "WeaponStats", menuName = "Weapon Stats")]
    public class WeaponStats : Stats
    {
        public int magazineCount, ammoCapacity;
        public float fireDelay, reloadDelay; 
}

}