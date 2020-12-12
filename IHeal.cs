using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public interface IHeal 
{
    IEnumerator RecoverHP();
    IEnumerator AwaitNoDamage();
}
}