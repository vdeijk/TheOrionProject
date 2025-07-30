using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class ScrapManager : Singleton<ScrapManager>
    {
        public float curScrap { get; private set; }
        public float latestReward { get; private set; } = 0;

        [SerializeField] float MaxScrap;

        public static event EventHandler OnScrapChanged;

        protected override void Awake()
        {
            Instance = SetSingleton();
            curScrap = 1000; // Initial starting scrap
        }

        // Initializes economy for a new campaign
        public Economy InitEconomy()
        {
            Economy economy = new Economy();
            economy.scrap = curScrap = 1000;
            return economy;
        }

        // Updates current scrap from loaded economy data
        public void SetEconomy(Economy economy)
        {
            curScrap = economy.scrap;
        }

        // Returns current economy state
        public Economy GetEconomy()
        {
            Economy economy = new Economy();
            economy.scrap = curScrap;
            return economy;
        }

        // Calculates and adds scrap reward based on current level
        public void SetScrapReward()
        {
            int level = LevelManager.Instance.level;
            latestReward = 200 + level * 100;
            AddScrap(latestReward);
        }

        // Adds scrap, clamping to maximum allowed
        public void AddScrap(float amount)
        {
            curScrap += amount;
            if (curScrap > MaxScrap) curScrap = MaxScrap;
            OnScrapChanged.Invoke(this, EventArgs.Empty);
        }

        // Removes scrap, clamping to zero
        public void RemoveScrap(float amount)
        {
            curScrap -= amount;
            if (curScrap < 0) curScrap = 0;
            OnScrapChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
