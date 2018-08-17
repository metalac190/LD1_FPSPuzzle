using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

    void TakeDamage(int damageAmount);
    void AddHealth(int healAmount);
    void Kill();
    void MakeInvulnerable(float duration);
}
