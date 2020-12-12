using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{

public interface IFire
{
    void Fire();
    void Reload();
    void Aim(Vector3 target);
    IEnumerator Discharge();
}

}

