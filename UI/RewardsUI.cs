using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class RewardsUI : MonoBehaviour
    {
        [SerializeField] Transform rewardWindow;

        private void Start()
        {
            rewardWindow.gameObject.SetActive(false);   
        }

        public void CloseRewardPopup()
        {
            rewardWindow.gameObject.SetActive(false);
        }

        public void OpenRewardPopup()
        {
            rewardWindow.gameObject.SetActive(true);
        }
    }
}
