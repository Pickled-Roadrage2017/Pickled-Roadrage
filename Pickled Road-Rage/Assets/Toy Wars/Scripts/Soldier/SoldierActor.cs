﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//--------------------------------------------------------------------------------------
// SoldierActor: The controller that PlayerActor will access when it is its turn to act
//
// PlayerActor has a Uni-Directional Association with SoldierActor
// (SoldierActor does not know about the relationship)
//
// SoldierActor has a Bi-Directional Association with the WeaponClass and all of its children
//  SoldierActor will pass down it's m_fCharge value to its currentWeapons m_fPower
//
//--------------------------------------------------------------------------------------
public class SoldierActor : MonoBehaviour
{
    // Slider for the aiming arrow
    public Slider m_sAimSlider;

    // Is the Charge bar meant to be ascending?
    bool m_bIsAscending;

    // Float for Max Charge
    public float m_fMaxCharge = 30;

    bool m_bFiring;

    // Speed at which the Soldier moves
    [Tooltip("The Speed at which the Soldier moves")]
    public float m_fSpeed;

    //Speed at which the soldier rotates towards the mouse
    [Tooltip("The Speed at which the Soldier rotates towards the mouse")]
    public float m_fRotSpeed;

    // The Soldiers health when initilizied
    [Tooltip("The Soldiers health when initilizied")]
    public int m_nMaxHealth;

    // Intiger value for the Soldiers current weapon
    // 0 = RocketLauncher
    // 1 = Minigun
    // 2 = Grenade
    [HideInInspector]
    public int m_nCurrentWeapon;

    // Values for the inventory count of this Soldier
    public int m_nGrenadeCount;
    public int m_nGotMinigun;
    public int m_nGotGrenade;

    // Current Charge to pass on to the weapons firing Power (m_fPower) 
    [Tooltip("Current Charge to pass on to the weapons firing Power")]
    public float m_fCharge = 1;

    // This Soldiers rigidBody for movement purposes
    private Rigidbody m_rbRigidBody;

    // A boolean for living, will be false when the soldier dies
    private bool m_bAlive;

    // A Number so the PlayerActor can know which soldier to move
    private int m_nSoldierNumber;

    // The Soldiers current health,
    // (Will be equal to m_nMaxHealth until it takes damage
    private int m_nCurrentHealth;

    Vector3 RotateRocketLauncher;

    public RocketLauncher m_goRPG;

    
    void Start()
    {
        m_rbRigidBody = GetComponent<Rigidbody>();
        // Soldiers Current health should always start at MaxHealth
        m_nCurrentHealth = m_nMaxHealth;
        // Soldier should start off as alive
        m_bAlive = true;
        m_bFiring = false;
    }

    void FixedUpdate()
    {
        Move();
        m_rbRigidBody.freezeRotation = true;
        FaceMouse();

        Fire(m_fCharge);
        // reset the AimSlider to min value
        m_sAimSlider.value = m_fCharge;
    }


    //--------------------------------------------------------------------------------------
    //  SwitchWeapon: Call when the Player wants to change the Soldiers currentWeapon
    //
    // Returns: Void
    //
    // 
    //
    //--------------------------------------------------------------------------------------
    private void SwitchWeapon()
    {

    }


    //--------------------------------------------------------------------------------------
    // Fire: Call when the Player commands the Soldier to fire
    //
    // Returns: Void
    //
    //
    // 
    //--------------------------------------------------------------------------------------
    private void Fire(float fCharge)
    {
        if (Input.GetButton("Fire1"))
        {
            m_bFiring = true;
            // If current weapon is rocketLauncher
            if (m_nCurrentWeapon == 0)
            {

                if (m_bIsAscending && m_fCharge <= m_fMaxCharge)
                {
                    m_fCharge += 1;
                    m_sAimSlider.value = m_fCharge;
                    if (m_fCharge == m_fMaxCharge)
                    {
                        m_bIsAscending = false;
                    }
                }
                else
                {
                    m_bIsAscending = false;
                    m_fCharge -= 1;

                    if (m_fCharge == 0)
                    {
                        m_bIsAscending = true;
                    }
                }

            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            // If current weapon is rocketLauncher
            if (m_nCurrentWeapon == 0)
            {
                m_goRPG.Fire(m_fCharge);
                m_fCharge = 1;
            }

            m_bFiring = false;
        }
    }


    //--------------------------------------------------------------------------------------
    // Move: Uses the Soldiers RigibBody to move with a drag value, 
    //       making it so the Soldier doesn't stop instantly
    //
    // Returns: Void
    //
    //  
    // 
    //--------------------------------------------------------------------------------------
    private void Move()
    {
        if (!m_bFiring)
        {
            float fMoveHorizontal = Input.GetAxis("Horizontal");
            float fMoveVertical = Input.GetAxis("Vertical");
            Vector3 v3Movement = new Vector3(fMoveHorizontal, 0, fMoveVertical);
            m_rbRigidBody.velocity = v3Movement * m_fSpeed;
        }
    }

    //--------------------------------------------------------------------------------------
    // FaceMouse: will make the Soldier rotate towards the mouse only along it's y axis
    //
    //
    // Returns: The Quaternion for the Soldier to rotate to the mouse
    //
    // NOTE: m_fRotSpeed is the speed modifier for it, the speed will affect how quickly the Soldier rotates 
    // 
    //--------------------------------------------------------------------------------------
    private Quaternion FaceMouse()
    {
        // Generate a plane that intersects the transform's position.
        Plane pSoldierPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray rMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);


        float fHitDistance = 0.0f;
        // If the ray is parallel to the plane, Raycast will return false.
        if (pSoldierPlane.Raycast(rMouseRay, out fHitDistance))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 v3TargetPos = rMouseRay.GetPoint(fHitDistance);

            // Determine the target rotation.  This is the rotation if the transform looks at the target point.
            Quaternion v3TargetRotation = Quaternion.LookRotation(v3TargetPos - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, v3TargetRotation, m_fRotSpeed * Time.deltaTime);
        }



        return transform.rotation;
    }


}