using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.Client.UI
{
    public class BossBarUI : MonoBehaviour
    {
        [SerializeField] private Text bossNameText;
        [SerializeField] private Image healthFill;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float showHideSpeed = 5f;

        private float _targetAlpha = 0f;

        public void Show(string bossName)
        {
            if (bossNameText) bossNameText.text = bossName;
            _targetAlpha = 1f;
        }
        public void Hide() { _targetAlpha = 0f; }
        public void SetHealth01(float v) { if (healthFill) healthFill.fillAmount = Mathf.Clamp01(v); }

        void Update()
        {
            if (canvasGroup)
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _targetAlpha, Time.deltaTime * showHideSpeed);
        }
    }
}

