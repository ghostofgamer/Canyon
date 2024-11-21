using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ClawbearGames
{
    public class HomeViewController : BaseViewController
    {
        [SerializeField] private RectTransform topPanelTrans = null;
        [SerializeField] private RectTransform bottomPanelTrans = null;
        [SerializeField] private RectTransform leftPanelTrans = null;
        [SerializeField] private RectTransform gameNameTrans = null;
        [SerializeField] private RectTransform soundOnButtonTrans = null;
        [SerializeField] private RectTransform soundOffButtonTrans = null;
        [SerializeField] private RectTransform musicOnButtonTrans = null;
        [SerializeField] private RectTransform musicOffButtonTrans = null;
        [SerializeField] private RectTransform warningSignTrans = null;
        [SerializeField] private Text currentLevelText = null;
        [SerializeField] private Text totalCoinsText = null;
        private int settingButtonTurn = 1;

        private void Update()
        {
            totalCoinsText.text = ServicesManager.Instance.CoinManager.TotalCoins.ToString();
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            MoveRectTransform(topPanelTrans, topPanelTrans.anchoredPosition, new Vector2(topPanelTrans.anchoredPosition.x, 0f), 0.5f);
            MoveRectTransform(bottomPanelTrans, bottomPanelTrans.anchoredPosition, new Vector2(bottomPanelTrans.anchoredPosition.x, 0f), 0.5f);
            ScaleRectTransform(gameNameTrans, Vector2.zero, Vector2.one, 1f);

            settingButtonTurn = 1;
            currentLevelText.text = PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1).ToString();

            //Update sound buttons
            if (ServicesManager.Instance.SoundManager.IsSoundOff())
            {
                soundOnButtonTrans.gameObject.SetActive(false);
                soundOffButtonTrans.gameObject.SetActive(true);
            }
            else
            {
                soundOnButtonTrans.gameObject.SetActive(true);
                soundOffButtonTrans.gameObject.SetActive(false);
            }

            //Update music buttons
            if (ServicesManager.Instance.SoundManager.IsMusicOff())
            {
                musicOffButtonTrans.gameObject.SetActive(true);
                musicOnButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                musicOffButtonTrans.gameObject.SetActive(false);
                musicOnButtonTrans.gameObject.SetActive(true);
            }


            //Handle warning sign
            if (ServicesManager.Instance.AdManager.IsRewardedAdReady())
            {
                warningSignTrans.gameObject.SetActive(true);
            }
            else
            {
                int timeRemains = ServicesManager.Instance.DailyRewardManager.TimeRemainsTillNextReward();
                warningSignTrans.gameObject.SetActive((timeRemains == 0) ? true : false);
            }
        }


        public override void OnClose()
        {
            topPanelTrans.anchoredPosition = new Vector2(topPanelTrans.anchoredPosition.x, 200f);
            bottomPanelTrans.anchoredPosition = new Vector2(bottomPanelTrans.anchoredPosition.x, -300f);
            leftPanelTrans.anchoredPosition = new Vector2(-150f, leftPanelTrans.anchoredPosition.y);
            gameNameTrans.localScale = Vector2.zero;
            gameObject.SetActive(false);
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>


        public void OnClickPlayButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Ingame", 0.25f);
        }


        public void OnClickRewardButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ViewManager.Instance.OnShowView(ViewType.DAILY_REWARD_VIEW);
        }


        public void OnClickCharacterButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            LoadScene("Character", 0.25f);
        }

        public void OnClickSettingButton()
        {
            settingButtonTurn *= -1;
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            Vector3 hidePos = new Vector2(-150f, leftPanelTrans.anchoredPosition.y);
            Vector3 showPos = new Vector2(10f, leftPanelTrans.anchoredPosition.y);
            MoveRectTransform(leftPanelTrans, leftPanelTrans.anchoredPosition, (settingButtonTurn < 0) ? showPos : hidePos, 0.5f);
        }

        public void OnClickShareButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.ShareManager.NativeShare();
        }

public void OnOpenWebSite()
{
Application.OpenURL("https://www.maxfabrique.com/");
}

        public void OnClickSoundButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.SoundManager.ToggleSound();
            if (ServicesManager.Instance.SoundManager.IsSoundOff())
            {
                soundOnButtonTrans.gameObject.SetActive(false);
                soundOffButtonTrans.gameObject.SetActive(true);
            }
            else
            {
                soundOnButtonTrans.gameObject.SetActive(true);
                soundOffButtonTrans.gameObject.SetActive(false);
            }
        }

        public void OnClickMusicButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ServicesManager.Instance.SoundManager.ToggleMusic();
            if (ServicesManager.Instance.SoundManager.IsMusicOff())
            {
                musicOffButtonTrans.gameObject.SetActive(true);
                musicOnButtonTrans.gameObject.SetActive(false);
            }
            else
            {
                musicOffButtonTrans.gameObject.SetActive(false);
                musicOnButtonTrans.gameObject.SetActive(true);
            }
        }



        public void OnClickLeaderboardButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ViewManager.Instance.OnShowView(ViewType.LEADERBOARD_VIEW);
        }

        public void OnClickRateAppButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            Application.OpenURL(ServicesManager.Instance.ShareManager.AppUrl);
        }
        public void OnClickRemoveAdsButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
        }
    }
}
