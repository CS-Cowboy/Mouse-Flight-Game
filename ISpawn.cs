using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public interface ISpawn
{
    void Init(Transform start, ObjectPooler src, StarShip backRef);
}
}