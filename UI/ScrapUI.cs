using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
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