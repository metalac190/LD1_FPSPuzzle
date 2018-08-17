using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer {
    // whether or not the player can be controlled
    void SetControllable(bool canControl);
    // what happens when the player is hit
    void PlayerHit(int damageTaken, float pushAmount, float stunDuration, Vector3 impactLocation);
    // pushes back the player, from the impact location
    void Pushback(float pushStrength, Vector3 impactLocation);
    // stuns the player, removing control for a short amount of time
    void Stun(float stunDuration);
    // apply a force to the player with a direction and strength
    void ApplyForce(Vector3 forceDirection, float forceAmount);
    // apply a force to the player, using x,y,z with a direction and strength
    void ApplyForce(float xDirection, float yDirection, float zDirection, float forceAmount);
}
