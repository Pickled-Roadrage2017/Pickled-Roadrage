﻿// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// Purpose: Scripting for the Bears
//
// Description: Inheriting from MonoBehaviour, 
// All of the funtionality of the Teddy is in here
//
// Author: Callan Davies
//--------------------------------------------------------------------------------------
public class Teddy : MonoBehaviour
{
    [Header("Sounds")]
    [LabelOverride("Place Sound")]
    [Tooltip("Will play when the teddy places a soldier")]
    public AudioClip m_acPlaceSound;

    [LabelOverride("Damage Sound")]
    [Tooltip("Will play when Teddy takes damage")]
    public AudioClip m_acDamageSound;

    [Header("Health Variables")]
    [LabelOverride("Teddy Max Health")][Tooltip("Teddy bear Maximum health.")]
    public float m_fMaxHealth;

    // health that will be set to MaxHealth in Awake
    [LabelOverride("Current Teddy Health")]
    [Tooltip("For displaying the current health, will be made private when we have a diplay for it somewhere on-screen")]
    public float m_fCurrentHealth;

    [Header("Throwing Variables")]
    // Speed for the rotation of the aiming arrow
    [LabelOverride("Facing Speed")]
    public float m_fRotSpeed;

    // public float for the teddy throwing charge.
    [LabelOverride("Current Charge")][Tooltip("The charge of the Teddy throwing mechanic.")]
    public float m_fCharge;

    // Minimum power for a shot
    [LabelOverride("Charge Minimum")][Tooltip("Minimum charge for the Charge, be sure that this matches the 'min value' variable in the Sliders inspector")]
    public float m_fMinCharge = 1f;

    // Float for Max Charge
    [LabelOverride("Charge Maximum")][Tooltip("Maximum charge for the Charge, be sure that this matches the 'max value' variable in the Sliders inspector")]
    public float m_fMaxCharge = 2f;

    // Speed for the slider
    [LabelOverride("Charge Speed")]
    [Tooltip("Speed that the charge increased by per update")]
    public float m_fChargeSpeed = 0.0f;

    // public color to apply to teddy objects.
    [LabelOverride("Teddy Color")] [Tooltip("The material color of this the Teddy bear object.")]
    public Color m_cTeddyColor;

    // Pool of Projectile objects
    private GameObject[] m_agProjectileList;

    // pool for projectiles
    private int m_nPoolSize = 1;

    // Health bar slider on teddy canvas.
    [LabelOverride("Health Bar Slider")] [Tooltip("Drag in a UI slider to be used as the Teddy health bar.")]
    public Slider m_sHealthBar;

    // boolean for an animation of the Teddy taking damage
    [HideInInspector]
    public bool m_bDamageAnimation;

    // boolean for an animation of the Teddy taking damage
    [HideInInspector]
    public bool m_bSpawnSoldier;

    // boolean for an animation of the Teddy taking damage
    [HideInInspector]
    public bool m_bPlaceSoldier;

    // this Teddys audioSource
    private AudioSource m_asAudioSource;

    // the bears animator
    private Animator m_aAnimator;
    //--------------------------------------------------------------------------------------
    // Initialization.
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        m_bDamageAnimation = false;
        m_bSpawnSoldier = false;
        m_bPlaceSoldier = false;

        m_aAnimator = GetComponent<Animator>();
        m_aAnimator.SetBool("m_bDamageAnimation", m_bDamageAnimation);
        m_aAnimator.SetBool("m_bSpawnSoldier", m_bSpawnSoldier);
        m_aAnimator.SetBool("m_bPlaceSoldier", m_bPlaceSoldier);
        // Set the health slider value to the current health.
        m_sHealthBar.value = CalcHealth();

        // loop through each material on the teddy.
        for (int o = 0; o < GetComponent<Renderer>().materials.Length; ++o)
        {
            // Change the color of each material to the m_cTeddyColor.
            GetComponent<Renderer>().materials[o].color = m_cTeddyColor;
        }
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void Update()
    {
        if(!IsAlive())
        {
            gameObject.SetActive(false);
        }
       
        // Apply damage to the health bar.
        m_sHealthBar.value = CalcHealth();

        if (m_bDamageAnimation == true)
        {
            m_bDamageAnimation = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // TakeDamage: Function for taking damage, for weapons to access
    //
    // Param: The amount of damage that the Teddy will take to m_fCurrentHealth
    //
    //--------------------------------------------------------------------------------------
    public void TakeDamage(float fDamage)
    {
        m_bDamageAnimation = true;
        // Minus the Teddys currentHealth by the fDamage argument
        m_fCurrentHealth -= fDamage;
    }

    public bool IsAlive()
    {
        if(m_fCurrentHealth <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //--------------------------------------------------------------------------------------
    // CalcHealth: Calculate the health percentage to apply to the health bar.
    //
    // Return:
    //      float: The teddy health in percentage.
    //--------------------------------------------------------------------------------------
    float CalcHealth()
    {
        // Get the percentage of health.
        return m_fCurrentHealth / m_fMaxHealth;
    }
}

