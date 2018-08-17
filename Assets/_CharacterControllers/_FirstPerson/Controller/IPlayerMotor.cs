using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerMotor {

    void AddPlayerForce(Vector3 force);
    // adjust player speed value
    void AdjustMoveSpeed(float speedChangeAmount);
    // adjust player speed value, but return it after a short amount of time
    void AdjustMoveSpeed(float speedChangeAmount, float duration);
}
