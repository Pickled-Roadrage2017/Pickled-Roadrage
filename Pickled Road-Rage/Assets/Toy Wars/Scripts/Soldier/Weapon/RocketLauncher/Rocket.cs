﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------------------------
// Rocket: Inheriting from Weapon.cs. Used for propelling rockets and their collison/damage numbers
//
// Author: Callan Davies
//-------------------------------------------------------------------------------------------------
public class Rocket : Weapon
{
    //A timer to stop the rocket from colliding with its Launcher
    [LabelOverride("Maximum time for Activation")]
    [Tooltip("Seconds it takes for the missle to not be within collision range of its Launcher")]
    public float m_fMaxActivateTimer = 2;

    [LabelOverride("Mask for Knockback and/or damage")]
    [Tooltip("Set this to the Unit layer, so the Rocket doesn't knockback objects that should be stationary")]
    public LayerMask m_UnitMask;

    // The force that hits back all Unit layered objects
    [Header("Explosion Variables")]
    [LabelOverride("Force of Explosion")]
    [Tooltip("The force that hits back all Unit layered objects")]
    public float m_ExplosionForce = 1000f;

    [LabelOverride("Direct Hit Modifier")]
    [Tooltip("The velocity is multiplied by this to reach an acceptable knockback")]
    public float m_fHitMultiplier = 0.5f;

    // Radius for the Area of Effect Explosion that should follow any Collision
    [LabelOverride("Radius of Explosion")]
    [Tooltip("Radius for the Area of Effect Explosion that should follow any Collision")]
    public float m_fExplosionRadius = 5f;

    // how fast the rocket will reach its target
    [LabelOverride("Rocket Speed")]
    [Tooltip("How quickly the rocket will reach its objective")]
    public float m_fSpeed;

    // pointer to the RocketLauncher so it knows where to spawn
    [HideInInspector]
    public GameObject m_gSpawnPoint;

    // time it will take for the rocket to reach its target position.
    [HideInInspector]
    public float m_fLerpTime = 0.0f;

    // The direction the Rocket should move
    [HideInInspector]
    public Vector3 m_v3Target;

    // Starts at m_fMaxActivateTimer and ticks down to zero, then resetting upon being set Inactive
    [HideInInspector]
    public float m_fCurrentActivateTimer;
    
    // The height that the rocket will arc to, this is passed down from its Launcher
    [HideInInspector]
    public float m_fArcHeight;

    // its own rigidbody
    private Rigidbody m_rbRocket;

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        m_rbRocket = GetComponent<Rigidbody>();
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void Update()
    {
        // ActivateTimer decreases by 1 each frame (Rocket can only collide when this is lower than 1)
        m_fCurrentActivateTimer -= 1;

        m_fLerpTime += Time.deltaTime * m_fSpeed;
        if (m_fLerpTime > 1.0f)
        {
            m_fLerpTime = 1.0f;
        }

        transform.position = BezierCurve.CalculateBezier(m_gSpawnPoint.transform.position, m_v3Target, m_fLerpTime, m_fArcHeight);

    }

    //--------------------------------------------------------------------------------------
    // OnTriggerEnter: When a rocket Collides (Is Trigger)
    //
    // Param: other is the other object it is colliding with at call
    //
    //--------------------------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        // if the rocket can now activate
        if (m_fCurrentActivateTimer <= 0)
        {
            // if the rocket directly hits a Soldier 
            if (other.tag == "Soldier")
            {
                Rigidbody rbTarget = other.GetComponent<Rigidbody>();
                rbTarget = other.GetComponent<Rigidbody>();
                // Directly knockback the Soldier, 
                // NOTE: This soldier will be taking damage in the next if Soldier segment
                rbTarget.AddForce(m_rbRocket.velocity * m_fHitMultiplier, ForceMode.Impulse);
            }

            // Collect all possible colliders 
            Collider[] aColliders = Physics.OverlapSphere(transform.position, m_fExplosionRadius, m_UnitMask);

            for (int i = 0; i < aColliders.Length; i++)
            {
                Rigidbody rbTarget = aColliders[i].GetComponent<Rigidbody>();

                //if it does not have a rigidbody
                if (!rbTarget)
                {
                    continue;
                }

                // if an object in collision zone is a Soldier
                if (aColliders[i].gameObject.tag == "Soldier")
                {
                    // TODO: Explosion particle effect here

                    SoldierActor gtarget = rbTarget.GetComponent<SoldierActor>();

                    // Soldier will take damage based on position (See CalculateDamge function below)
                    gtarget.TakeDamage(CalculateDamage(aColliders[i].transform.position));

                    // add explosive force for knockback 
                    // NOTE: May be replaced with a non-rigidbody knockback
                    rbTarget.AddExplosionForce(m_ExplosionForce, transform.position, m_fExplosionRadius, 0.0f, ForceMode.Impulse);
                }
                // if the rocket splash finds a Teddy 
                // NOTE: This may be removed if we decide to no longer have the Teddy as an objective
                else if (aColliders[i].gameObject.tag == "Teddy")
                {
                    Teddy gtarget = rbTarget.GetComponent<Teddy>();
                    gtarget.TakeDamage(CalculateDamage(aColliders[i].transform.position));
                }

            }
            // Disable Rocket after it has completed all damage dealing and knockbacks
            RocketDisable();
        }
    }

    //--------------------------------------------------------------------------------------
    //  CalculateDamage: Calculates the damage so being further from the explosion results in less damage
    //
    //  Returns: the damage for the Soldiers within range to take
    //
    //--------------------------------------------------------------------------------------
    private float CalculateDamage(Vector3 v3TargetPosition)
    {
        // create a vector from the shell to the target
        Vector3 v3ExplosionToTarget = v3TargetPosition - transform.position;

        // Calculated the distance from the shell to the target
        float fExplosionDistance = v3ExplosionToTarget.magnitude;

        // calculate the proportion of the Maximum distance the target is away
        float fRelativeDistance = (m_fExplosionRadius - fExplosionDistance) / m_fExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage
        float fDamage = fRelativeDistance * m_fDamage;

        fDamage = Mathf.Max(0f, fDamage);

        return fDamage;
    }


    //--------------------------------------------------------------------------------------
    //  RocketDisable: Disables rocket and allows for resetting of variables if needed
    //
    //--------------------------------------------------------------------------------------
    private void RocketDisable()
    {
        gameObject.SetActive(false);
    }
}
