using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DataAssets;
using System;

public class Health : MonoBehaviour, IDamageable {

    [Header("General Settings")]
    [SerializeField] int maxHealth = 100;   // max health value
    public int MaxHealth { get { return maxHealth; } }
    [SerializeField] int currentHealth = 100;      // current health value; viewable from inspector
    public int CurrentHealth { get { return currentHealth; } }
    [SerializeField] bool isInvulnerable = false;
    public bool IsInvulnerable { get { return isInvulnerable; } }
    [SerializeField] float hitInvulnerableTime = .2f;       // how long is this entity invulnerable when hit
    public int HitInvulnerableTime { get { return HitInvulnerableTime; } }

    public UnityEvent OnTakeDamage;     // event that triggers when takes damage
    public UnityEvent OnHeal;           // event that triggers when gains health
    public UnityEvent OnDeath;          // event that triggers when dies

    [Header("SFX")]
    [SerializeField] AudioClip sfx_heal;
    [SerializeField] AudioClip sfx_takeDamage;
    [SerializeField] AudioClip sfx_death;

    [Header("GFX")]
    [SerializeField] GameObject gfx_damaged;
    [SerializeField] GameObject gfx_healed;
    [SerializeField] GameObject gfxDeath;        //Effect that happens when we die

    // states
    private bool isDead = false;       //Keep track of whether or not this entity is alive
    public bool IsDead { get { return isDead;  } }

    #region Setup
    private void Start()
    {
        SetDefaults();
    }

    public void SetDefaults()
    {
        // player is alive once again
        isDead = false;
    }
    #endregion

    //Subtract amount from the current health
    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable || isDead)
            return;

        // apply damage
        currentHealth -= damageAmount;
        MakeInvulnerable(hitInvulnerableTime);
        // our health has been changed, call the event
        OnTakeDamage.Invoke();
        //We've just run out of health. This entity is dead!
        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    /// <summary>
    /// Heal the player a specified amount, adding it to the total (but not going over the max). Update the UI
    /// </summary>
    /// <param name="healAmount"></param>
    public void AddHealth(int healAmount)
    {
        // if we're dead, we can't heal
        if (isDead)
            return;
        // apply heal amount
        currentHealth += healAmount;
        // clamp health to be under max
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        // health amount has changed, call the event
        OnHeal.Invoke();
    }

    //Kill this Object
    public void Kill()
    {
        // we are now dead
        isDead = true;
        // activate OnDeath() effects specified in the Inspector
        OnDeath.Invoke();
        // spawn Death effect
        if (gfxDeath != null)
        {
            GameObject gfxDeath = (GameObject)Instantiate(this.gfxDeath, transform.position, Quaternion.identity);
            Destroy(gfxDeath, 3f);
        }
        //TODO add Death Audio
    }

    /// <summary>
    /// Flag entity for invulnerability, if they aren't already. Will not take damage while invulnerable
    /// </summary>
    /// <param name="invulDuration"></param>
    public void MakeInvulnerable(float invulDuration)
    {
        // if we're not already invulnerable, make it so
        if (!isInvulnerable)
        {
            StartCoroutine(IEMakeInvulnerable(invulDuration));
        }
    }
    //Coroutine for MakeInvulnerable function
    IEnumerator IEMakeInvulnerable(float secondsInvul)
    {
        // we are now invulnerable
        isInvulnerable = true;
        // wait for our invulnerability time
        yield return new WaitForSeconds(secondsInvul);
        // we are no longer invulnerable
        isInvulnerable = false;
    }
}
