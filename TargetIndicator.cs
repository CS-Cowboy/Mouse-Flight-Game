using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.braineeeedevs.mouseflight.game
{
	public class TargetIndicator : MonoBehaviour {
		
        [SerializeField] protected StarShip target;
		[SerializeField] protected Image indicator, leadpoint;
		public static Vector3 FindIntercept(StarShip target, StarShip shooter)
        {
            Vector3 futurePosition = shooter.physics.velocity + shooter.physics.position; //Save the next position of the ship
                if (target.physics != null)
            {
                Vector3 futureTarget = target.physics.velocity + target.physics.position;
                Vector3 toTarget = futureTarget - futurePosition; //Use the next position of the ship as input to my prediction algorithm

                float a = target.physics.velocity.sqrMagnitude - Mathf.Pow(shooter.stats.moveForce, 2f);
                float c = toTarget.sqrMagnitude;

                float b = -Vector3.Dot(target.physics.velocity, toTarget);

                float R = Mathf.Pow(b, 2f) - (a * c);
                float D = a;

                if (Mathf.Sign(R) == -1 || a == 0f || target.physics.velocity.magnitude == 0f)
                {
                    return target.physics.transform.position;
                }

                float radicand = Mathf.Sqrt(R);
                float t0 = (b + radicand) / D, t1 = (b - radicand) / D, t = 0;

                t = t1 < 0f ? t0 : t1;
                return (target.physics.velocity * t) + target.physics.position;
            }
            return target.transform.position;
        }
		
}
}