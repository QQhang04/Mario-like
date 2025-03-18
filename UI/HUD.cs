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
   public Image[] starsImages;
   
   protected Game m_game;
   protected LevelScore m_score;
   protected Player m_player;

   protected float timerStep;
   protected static float timerRefreshRate = .3f;
   
   protected virtual void UpdateCoins(int value)
   {
      coins.text = value.ToString(coinsFormat);
   }

   protected virtual void UpdateRetries(int value)
   {
      retries.text = value.ToString(retriesFormat);
   }

   protected virtual void UpdateHealth()
   {
      health.text = m_player.health?.current.ToString(healthFormat);
   }

   protected virtual void UpdateStars(bool[] value)
   {
      for (int i = 0; i < starsImages.Length; i++)
      {
         starsImages[i].enabled = value[i];
      }
   }

   public virtual void Refresh()
   {
      UpdateCoins(m_score.coins);
      UpdateRetries(m_game.retries);
      UpdateHealth();
      UpdateStars(m_score.stars);
   }

   protected virtual void UpdateTimer()
   {
      timerStep += Time.deltaTime;
      if (timerStep >= timerRefreshRate)
      {
         timer.text = GameLevel.FormattedTime(m_score.time);
         timerStep = 0;
      }
   }

   protected void Awake()
   {
      m_game = Game.Instance;
      m_score = LevelScore.Instance;
      m_player = FindObjectOfType<Player>();
      
      m_score.OnScoreLoaded.AddListener(() =>
      {
         m_score.OnCoinsSet.AddListener(UpdateCoins);
         m_score.OnStarsSet.AddListener(UpdateStars);
         m_score.OnRetriesSet.AddListener(UpdateRetries);
         m_player.health.onChange.AddListener(UpdateHealth);
      });
      Refresh();
   }

   protected void Update() => UpdateTimer();
}
