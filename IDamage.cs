using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{public interface IDamage
{
    void DoDamage(float dmg);
    IEnumerator Die(bool wait);
}

}