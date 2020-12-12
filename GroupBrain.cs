using System.Collections.Generic;
using UnityEngine;

namespace com.braineeeedevs.mouseflight.game
{
public class GroupBrain : MonoBehaviour
{
    protected List<TargetData> knownAllies = new List<TargetData>(), knownEnemies = new List<TargetData>();
    public enum GOAL
    {
        AttackEnemy, GoToWaypoint, DefendSite
    }
    public enum POSTURE
    {
        WaitUntilAttacked, AttackOnSight, FleeWhenAttacked
    }

    public enum STATUS
    {
        UnderAttack, SituationNormal, Retreating
    }
    protected GOAL objective;
    protected POSTURE ROE;
    protected STATUS latest;
    protected int updateCounter;
    public int updateFreq = 60;
    public float targetSpotProbability = 0.55f;
    void FixedUpdate()
    {
        TrySpotFoe();
        updateCounter++;
    }
    protected void TrySpotFoe()
    {
        if (knownEnemies.Count > 0)
        {
            if (updateCounter % updateFreq == 0)
            {
                bool newTargetFound = UnityEngine.Random.Range(0f, 1f) >= targetSpotProbability;
                if (newTargetFound)
                {
                    if (this.ROE == GroupBrain.POSTURE.AttackOnSight || this.ROE == GroupBrain.POSTURE.WaitUntilAttacked && latest == GroupBrain.STATUS.UnderAttack)
                    {
                        objective = GroupBrain.GOAL.AttackEnemy;
                    }
                }
                else
                {
                    objective = GroupBrain.GOAL.GoToWaypoint;
                }
            }
        }
    }

    protected void MessageGroup(List<Brain> grp, Message msg)
    {
        foreach(Brain b in grp)
        {
            b.DoUpdate(msg);
        }
    }
}
}