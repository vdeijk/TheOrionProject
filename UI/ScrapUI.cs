using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class ScrapUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scrapText;

        private void OnEnable()
        {
            ScrapManager.OnScrapChanged += ScrapManager_OnScrapChanged;
        }

        private void OnDisable()
        {
            ScrapManager.OnScrapChanged -= ScrapManager_OnScrapChanged;
        }

        private void Start()
        {
            SetScrapText();
        }

        private void ScrapManager_OnScrapChanged(object sender, EventArgs e)
        {
            SetScrapText();
        }

        private void SetScrapText()
        {
            scrapText.text = ScrapManager.Instance.curScrap.ToString() + " Scr";
        }
    }
}