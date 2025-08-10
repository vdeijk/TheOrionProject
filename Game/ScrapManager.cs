using System;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class ScrapManager
    {
        private static ScrapManager _instance;
        private GameController _gameController;

        public float curScrap { get; private set; }
        public float latestReward { get; private set; } = 0;

        public static ScrapManager Instance => _instance ??= new ScrapManager();

        public static event EventHandler OnScrapChanged;

        public void Init(GameController gameController)
        {
            _gameController = gameController;
            curScrap = 1000; // Initial starting scrap
        }

        // Initializes economy for a new campaign
        public EconomyState InitEconomy()
        {
            EconomyState economy = new EconomyState();
            economy.scrap = curScrap = 1000;
            return economy;
        }

        // Updates current scrap from loaded economy data
        public void SetEconomy(EconomyState economy)
        {
            curScrap = economy.scrap;
        }

        // Returns current economy state
        public EconomyState GetEconomy()
        {
            EconomyState economy = new EconomyState();
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
            if (curScrap > _gameController.MaxScrap) curScrap = _gameController.MaxScrap;
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
