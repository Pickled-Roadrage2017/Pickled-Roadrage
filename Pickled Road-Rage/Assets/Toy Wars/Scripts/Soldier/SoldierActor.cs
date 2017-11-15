﻿//--------------------------------------------------------------------------------------
// Purpose: Used to control the Soldier
//
// Description:  Inheriting from MonoBehaviour, allows Player.cs to control all Soldier 
//               related functions.
//
// Author: Callan Davies
//--------------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EWeaponType
{
    EWEP_RPG,
    EWEP_MINIGUN,
    EWEP_GRENADE
}

public class SoldierActor : MonoBehaviour
{
    [Header("Sounds")]
    [LabelOverride("Footstep 1")]
    public AudioClip m_acFootstep1;

    [LabelOverride("Footstep 2")]
    public AudioClip m_acFootstep2;

    [LabelOverride("Grunt Sound")]
    public AudioClip m_acGruntSound;

    [LabelOverride("Damage Sound")]
    [Tooltip("Will play when soldier takes damage")]
    public AudioClip m_acDamageSound;

    [LabelOverride("Death Sound")]
    [Tooltip("Will play as the Soldier dies")]
    public AudioClip m_acDeathSound;

    [Header("Moving Variables")]
    // Speed at which the Soldier moves
    [Tooltip("The Speed at which the Soldier moves")]
    public float m_fSpeed;

    //Speed at which the soldier rotates towards the mouse
    [Tooltip("The Speed at which the Soldier rotates towards the mouse")]
    public float m_fRotSpeed;

    // The Soldiers health when initilizied
    [Tooltip("The Soldiers health when initilizied")]
    public float m_fMaxHealth = 100;

    // Intiger value for the Soldiers current weapon
    // 0 = RocketLauncher
    // 1 = Minigun
    // 2 = Grenade
    [Header("Soldier Weapons")]
    //[Range(0,2)]
    [Tooltip("0 = RocketLauncher, 1 = Minigun, 2 = Grenade")]
    public EWeaponType m_eCurrentWeapon;

    // Values for the inventory count of this Soldier
    [Tooltip("How many Grenades this Soldier has")]
    public int m_nGrenadeCount = 1;
    public int m_nGotMinigun = 0;
    public int m_nGotGrenade = 1;

    // The GameObject RocketLauncher that the Soldier will be using
    [Space(10)]
    [Tooltip("This Soldiers RocketLauncher")]
    public GameObject m_gRocketLauncher;

    // The GameObject GrenadeLauncher that the Soldier will be using
    [Space(10)]
    [Tooltip("This Soldiers Grenadelauncher")]
    public GameObject m_gGrenadeLauncher;

    // The Soldiers current health,
    // (Will be equal to m_nMaxHealth until it takes damage)
    [HideInInspector]
    public float m_fCurrentHealth;

    // Movement vector for the Soldier
    [HideInInspector]
    public Vector3 m_v3Movement;

    // The RocketLauncher script of GameObject RocketLauncher
    private RocketLauncher m_gLauncherScript;

    // The GrenadeLauncher script of GameObject GrenadeLauncher
    private GrenadeLauncher m_gGrenadeScript;

    // Will be set to the Soldiers rigidbody property
    [HideInInspector]
    public Rigidbody m_rbRigidBody;

    // boolean for an animation of the soldier taking damage
    [HideInInspector]
    public bool m_bDamageAnimation;

    // a boolean for an animation to play whilst the soldier is moving
    [HideInInspector]
    public bool m_bMovingAnimation;

    // a boolean for an animation at the start of the firing function
    [HideInInspector]
    public bool m_bStartFireAnimation;

    // a boolean for an animation at the end of the firing function
    [HideInInspector]
    public bool m_bFinalFireAnimation;

    // public float for the radius of the movement circle.
    [LabelOverride("Movement Radius")]
    [Tooltip("The Radius of the movement circle, this is how far the soldier can move.")]
    public float m_fMovementRadius;

    // public gameobject for the movement circle prefab.
    [LabelOverride("Radius Object")]
    [Tooltip("The Prefab for the Radius object.")]
    public GameObject m_gMovementCirlceBluePrint;

    // The gameobject for the soldier moevement circle.
    private GameObject m_gMovementCircle;

    // The current position of the soldier.
    private Vector3 m_v3CurrentPostion;

    // this Soldiers audioSource
    private AudioSource m_asAudioSource;

    // the soldiers animator
    private Animator m_aAnimator;
    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        m_aAnimator = GetComponent<Animator>();

        // initilising animation booleans to false
        m_bStartFireAnimation = false;
        m_bFinalFireAnimation = false;
        m_bDamageAnimation = false;

        // set boolean for moving to false
        m_bMovingAnimation = false;

        // animator boolean setup
        m_aAnimator.SetBool("m_bDamageAnimation", m_bDamageAnimation);
        m_aAnimator.SetBool("m_bStartFireAnimation", m_bStartFireAnimation);
        m_aAnimator.SetBool("m_bFinalFireAnimation", m_bFinalFireAnimation);
        m_aAnimator.SetBool("m_bMovingAnimation", m_bMovingAnimation);

        // get the soldiers rigidbody
        m_rbRigidBody = GetComponent<Rigidbody>();
        // get the rocketlaunchers script
        m_gLauncherScript = m_gRocketLauncher.GetComponent<RocketLauncher>();
        // get the grenadeLaunchers script
        m_gGrenadeScript = m_gGrenadeLauncher.GetComponent<GrenadeLauncher>();
        // Soldiers Current health should always start at MaxHealth
        m_fCurrentHealth = m_fMaxHealth;
        // so the soldiers cannot move upwards
        m_rbRigidBody.constraints = RigidbodyConstraints.FreezePositionY;
        // so the soldier doesn't rotate unless it is to FaceMouse()
        m_rbRigidBody.freezeRotation = true;

        // Instantiate and setActive of MovementCircle to false.
        m_gMovementCircle = Instantiate(m_gMovementCirlceBluePrint);
        m_gMovementCircle.SetActive(false);

        // Set the Current position to the soldiers postion.
        m_v3CurrentPostion = transform.position;

        // Make the size of the MovementCircle the same as the MovementRadius.
        m_gMovementCircle.transform.localScale *= m_fMovementRadius;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void Update()
    {
        // animation boolean resets
        if (m_bStartFireAnimation == true)
        {
            m_bStartFireAnimation = false;
        }

        if (m_bFinalFireAnimation == true)
        {
            m_bFinalFireAnimation = false;
        }

        if (m_bDamageAnimation == true)
        {
            m_bDamageAnimation = false;
        }
        // As health is a float, anything below one will be displayed as 0 to the player
        if (m_fCurrentHealth < 1)
        {
            Die();
        }

        // if the soldier is not moving
        if (m_rbRigidBody.velocity == Vector3.zero)
        {
            m_bMovingAnimation = false;
        }
        // soldier is moving
        else
        {
            m_bMovingAnimation = true;
        }

        if (m_bDamageAnimation == true)
        {
            m_bDamageAnimation = false;
        }
    }

    //--------------------------------------------------------------------------------------
    // CurrentTurn: A function for reseting soldier values for turn starts or turn ends.
    //
    // Param:
    //      bActive: a bool for if this soldiers is currently having a turn or not.
    //--------------------------------------------------------------------------------------
    public void CurrentTurn(bool bActive)
    {
        // if true is passed in.
        if (bActive)
        {

            // Set the current soldier postion.
            m_v3CurrentPostion = transform.position;

            // activate the solider aim line.
            m_gRocketLauncher.GetComponent<LineRenderer>().enabled = true;

            // Set the movementcircle postion.
            m_gMovementCircle.transform.position = m_v3CurrentPostion;

            // activate the movememntcircle.
            m_gMovementCircle.SetActive(true);
        }

        // if false is passed in.
        else if (!bActive)
        {
            // deactivate the solider aimming line and set the position count to 0.
            m_gRocketLauncher.GetComponent<LineRenderer>().enabled = false;
            m_gRocketLauncher.GetComponent<LineRenderer>().positionCount = 0;

            // deactivate the movementcircle.
            m_gMovementCircle.SetActive(false);
        }
    }

    //--------------------------------------------------------------------------------------
    // Fire: Call when the Player commands the Soldier to fire
    //
    //--------------------------------------------------------------------------------------
    public void MouseDown()
    {
        m_bStartFireAnimation = true;
        if (m_eCurrentWeapon == EWeaponType.EWEP_RPG)
        {
            m_gLauncherScript.MouseDown();
        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_MINIGUN)
        {

        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_GRENADE)
        {
            m_gGrenadeScript.MouseDown();
        }

        else
        {

        }
    }

    public void MouseHeld()
    {
        if (m_eCurrentWeapon == EWeaponType.EWEP_RPG)
        {
            m_gLauncherScript.MouseHeld();
        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_MINIGUN)
        {

        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_GRENADE)
        {
            m_gGrenadeScript.MouseHeld();
        }
        else
        {
        }
    }

    public void MouseUp()
    {
        m_bFinalFireAnimation = true;
        if (m_eCurrentWeapon == EWeaponType.EWEP_RPG)
        {
            m_gLauncherScript.MouseUp();
        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_MINIGUN)
        {

        }
        else if (m_eCurrentWeapon == EWeaponType.EWEP_GRENADE)
        {
            m_gGrenadeScript.MouseUp();
        }
        else
        {
        }
    }

    //--------------------------------------------------------------------------------------
    // Move: Uses the Soldiers RigibBody to move with a drag value, 
    //       making it so the Soldier doesn't stop instantly
    // 
    //--------------------------------------------------------------------------------------
    public void Move(float fMoveHorizontal, float fMoveVertical)
    {
        // Apply fMoveHor and fMoveVer to the movement vector.
        m_v3Movement = new Vector3(fMoveHorizontal, 0, fMoveVertical);

        // Update the rigidbody velocity.
        m_rbRigidBody.velocity = m_v3Movement * m_fSpeed;
       
    }

    //--------------------------------------------------------------------------------------
    // FaceMouse: will make the Soldier rotate towards the mouse only along it's y axis
    //
    // Returns: The Quaternion for the Soldier to rotate to the mouse
    //
    // NOTE: m_fRotSpeed is the speed modifier for it, the speed will affect how quickly the Soldier rotates 
    // 
    //--------------------------------------------------------------------------------------
    public Quaternion FaceMouse()
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

    //--------------------------------------------------------------------------------------
    // TakeDamage: Function for taking damage, for weapons to access
    //
    // Param: The amount of damage that the soldier will take to m_fCurrentHealth
    //
    //--------------------------------------------------------------------------------------
    public void TakeDamage(float fDamage)
    {
        // TODO: 
        m_bDamageAnimation = true;

        // Minus the soldiers currentHealth by the fDamage argument
        m_fCurrentHealth -= fDamage;
    }

    //--------------------------------------------------------------------------------------
    // Die: Sets the soldier to inactive and resets its variables
    //--------------------------------------------------------------------------------------
    public void Die()
    {
        // TODO: Animation and such
        // Set the Soldier to inactive
        gameObject.SetActive(false);
        // Reset the Soldiers values to initial
        m_fCurrentHealth = m_fMaxHealth;
        m_rbRigidBody.isKinematic = false;
    }
}
