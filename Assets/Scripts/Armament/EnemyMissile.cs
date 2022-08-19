using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : Missile
{
    protected override void AdjustValuesByDifficulty()
    {
        smartTrackingRate *= MissionData.GetFloatFromDifficultyXML("enemyMissileTrackingRateFactor", smartTrackingRate);
        boresightAngle *= MissionData.GetFloatFromDifficultyXML("enemyMissileBoresightAngleFactor", boresightAngle);
        turningForce *= MissionData.GetFloatFromDifficultyXML("enemyMissileTurningForceFactor", turningForce);

        smartTrackingRate = Mathf.Clamp(smartTrackingRate, 0, 0.9f);
        boresightAngle = Mathf.Clamp(boresightAngle, 0, 120);
        turningForce = Mathf.Clamp(turningForce, 0, 2.5f);
    }
}
