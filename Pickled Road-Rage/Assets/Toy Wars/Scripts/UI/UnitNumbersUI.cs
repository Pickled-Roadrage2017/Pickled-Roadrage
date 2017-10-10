﻿// Using, etc
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--------------------------------------------------------------------------------------
// UnitNumbersUI object. Inheriting from MonoBehaviour. Script for the Players 
// unit numbers text object. 
//--------------------------------------------------------------------------------------
public class UnitNumbersUI : MonoBehaviour
{
    // public text object for displaying player1s unit numbers.
    [Tooltip("The Player unit number text object in the Canvas.")]
    public Text m_tUnitNumber1Text;

    // public object for the player object to tie this object to.
    [Tooltip("Player object to tie this UI element to.")]
    public GameObject m_gPlayerObject;

    //--------------------------------------------------------------------------------------
    // initialization.
    //--------------------------------------------------------------------------------------
    void Awake()
    {
        // Set text object to support richtext.
        m_tUnitNumber1Text.supportRichText = true;
    }

    //--------------------------------------------------------------------------------------
    // Update: Function that calls each frame to update game objects.
    //--------------------------------------------------------------------------------------
    void Update()
    {
        // Get current player.
        Player pPlayer = m_gPlayerObject.GetComponent<Player>();

        // new string for the active soldiers for the player.
        string sActiveSoldiers = string.Format("{0}", pPlayer.GetActiveSoldiers());

        // Set the text to the active soldier string.
        m_tUnitNumber1Text.text = sActiveSoldiers;
    }
}
