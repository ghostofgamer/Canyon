using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ClawbearGames
{
    public class IngameViewController : BaseViewController
    {
        private int index =0 ;
        
        [SerializeField] private RectTransform lelfPanelTrans = null;
        [SerializeField] private RectTransform tutorialPanelTrans = null;
        [SerializeField] private Image timeCountSlider = null;
        [SerializeField] private Text timeCountText = null;
        [SerializeField] private Text levelText = null;

        [SerializeField] private CanvasGroup _gamePlayGroupe;
            
        private bool isTouchingBuildButton = false;
        private float lastBuildTime = 0f;
        private float buildInterval = 0.03f;
        private int maxBuilds = 160;
        private void Update()
        {
            /*if (isTouchingBuildButton)
            {
                if (Time.time - lastBuildTime >= buildInterval && index < maxBuilds)
                {
                    index++;
                    Debug.Log("Create True " + index);
                    IngameManager.Instance.CurrentPlatform.BuildBridge();
                    lastBuildTime = Time.time;
                }
            }*/
            
            
            if (isTouchingBuildButton)
            {
                /*index++;
                Debug.Log("Create True " + index);
                if (index >= 160)
                    return;*/
                
                IngameManager.Instance.CurrentPlatform.BuildBridge();
            }
        }
        

        public void ClearIndex()
        {
            index = 0;
        }
        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>


        public override void OnShow()
        {
            MoveRectTransform(lelfPanelTrans, lelfPanelTrans.anchoredPosition, new Vector2(lelfPanelTrans.anchoredPosition.x, -10f), 0.5f);

            //Update texts and other fields, parameters
            levelText.text = IngameManager.Instance.CurrentLevel.ToString();
            tutorialPanelTrans.gameObject.SetActive(!Utilities.IsShowTutorial());

            if (!Utilities.IsShowTutorial())
            {
                ChangeValueVanvasGroupGamePlayObjects(0, false);
            }
        }

        private void ChangeValueVanvasGroupGamePlayObjects(float alpha,bool value)
        {
            _gamePlayGroupe.alpha = alpha;
            _gamePlayGroupe.interactable = value;
            _gamePlayGroupe.blocksRaycasts = value;
        }

        public override void OnClose()
        {
            isTouchingBuildButton = false;
            timeCountSlider.fillAmount = 1f;
            lelfPanelTrans.anchoredPosition = new Vector2(lelfPanelTrans.anchoredPosition.x, 150f);
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Update the time slider.
        /// </summary>
        /// <param name="maxTime"></param>
        /// <param name="currentTime"></param>
        public void UpdateTimeSlider(int maxTime, float currentTime)
        {
            timeCountSlider.fillAmount = currentTime / (float)maxTime;
        }


        /// <summary>
        /// Update the time text.
        /// </summary>
        /// <param name="currentTime"></param>
        public void UpdateTimeText(int currentTime)
        {
            timeCountText.text = currentTime.ToString();
        }

        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>



        public void OnClickCloseTutorialButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_TUTORIAL, 1);
            tutorialPanelTrans.gameObject.SetActive(false);
            ChangeValueVanvasGroupGamePlayObjects(1, true);
            IngameManager.Instance.PlayingGame();
        }



        public void OnPointerDownBuildBridgeButton()
        {
            if (IngameManager.Instance.CurrentPlatform.IsReady && IngameManager.Instance.NextPlatform.IsReady)
            {
                if (!IngameManager.Instance.CurrentPlatform.IsCreatedBridge)
                {
                    IngameManager.Instance.CurrentPlatform.CreateBridge();
                    
                   
                }

                isTouchingBuildButton = true;
            }
        }


        public void OnPointerUpBuildBridgeButton()
        {
            isTouchingBuildButton = false;
            Debug.Log("False On Pointer Up");
        }


        public void OnClickPlaceBridgeButton()
        {
            if (IngameManager.Instance.CurrentPlatform.IsReady && IngameManager.Instance.CurrentPlatform.IsCreatedBridge)
            {
                IngameManager.Instance.CurrentPlatform.PlaceBridge();
                isTouchingBuildButton = false;
                index = 0;
            }
        }
    }
}
