using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.braineeeedevs.mouseflight.game
{

public interface ITranslate 
{
    void PhysicsMove(Vector3 moveDelta);
    void Steer(Vector3 steering);
}

}