using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
   public string retriesFormat = "00";
   public string coinsFormat = "000";
   public string healthFormat = "0";

   [Header("UI Elements")]
   public Text retries;
   public Text coins;
   public Text health;
   public Text timer;

   protected void Awake()
   {
      
   }
}
