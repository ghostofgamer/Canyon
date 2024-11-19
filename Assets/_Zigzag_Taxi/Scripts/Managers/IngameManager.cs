using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ClawbearGames
{
    public class IngameManager : MonoBehaviour
    {
        public static IngameManager Instance { private set; get; }
        public static event System.Action<IngameState> IngameStateChanged = delegate { };


        [Header("Enter a number of level to test. Set back to 0 to disable this feature.")]
        [SerializeField] private int testingLevel = 0;



        [Header("Ingame Configuration")]
        [SerializeField] private float reviveWaitTime = 5f;


        [Header("Level Configuration")]
        [SerializeField] private List<LevelConfiguration> listLevelConfiguration = new List<LevelConfiguration>();


        [Header("Ingame References")]
        [SerializeField] private Material backgroundMaterial = null;
        [SerializeField] private ParticleSystem[] confettiEffects = null;

        public IngameState IngameState
        {
            get { return ingameState; }
            private set
            {
                if (value != ingameState)
                {
                    ingameState = value;
                    IngameStateChanged(ingameState);
                }
            }
        }

        public PlatformController NextPlatform { private set; get; }
        public PlatformController CurrentPlatform { private set; get; }
        public float ReviveWaitTime { get { return reviveWaitTime; } }
        public int BridgeBuildingSpeed { private set; get; }
        public int CurrentLevel { private set; get; }
        public bool IsRevived { private set; get; }


        private IngameState ingameState = IngameState.Ingame_GameOver;
        private List<PlatformParams> listPlatformParams = new List<PlatformParams>();
        private LevelConfiguration currentLevelConfigs = null;
        private Vector3 nextPlatformPos = Vector3.zero;
        private AudioClip backgroundMusic = null;
        private int platformParamsIndex = 0;
        private int timeToCompleteLevel = 0;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            ServicesManager.Instance.CoinManager.SetCollectedCoins(0);
            StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));

            //Setup variables
            IsRevived = false;
            confettiEffects[0].transform.root.gameObject.SetActive(false);
            nextPlatformPos = PlayerController.Instance.transform.position;

            //Load level parameters
            CurrentLevel = (testingLevel != 0) ? testingLevel : PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, 1);
            foreach(LevelConfiguration levelConfigs in listLevelConfiguration)
            {
                if (CurrentLevel >= levelConfigs.MinLevel && CurrentLevel < levelConfigs.MaxLevel)
                {
                    //Setup background and others parameters
                    currentLevelConfigs = levelConfigs;
                    backgroundMusic = levelConfigs.BackgroundMusicClip;
                    backgroundMaterial.SetColor("_TopColor", levelConfigs.BackgroundTopColor);
                    backgroundMaterial.SetColor("_BottomColor", levelConfigs.BackgroundBottomColor);
                    BridgeBuildingSpeed = Random.Range(levelConfigs.MinBridgeBuildingSpeed, levelConfigs.MaxBridgeBuildingSpeed);
                    PlayerController.Instance.SetupParams(Random.Range(levelConfigs.MinPlayerMovementSpeed, levelConfigs.MaxPlayerMovementSpeed));

                    int platformAmount = Random.Range(levelConfigs.MinPlatformAmount, levelConfigs.MaxPlatformAmount);
                    for (int i = 0; i < platformAmount; i++)
                    {
                        PlatformParams platformParams = new PlatformParams();
                        platformParams.SetFirstPlatfor(i == 0);
                        platformParams.SetPlatformSize(GetPlatformSize());
                        platformParams.SetPlatformDistance(Random.Range(levelConfigs.MinPlatformDistance, levelConfigs.MaxPlatformDistance));
                        platformParams.SetCoinItemAmount(Random.Range(levelConfigs.MinCoinItemAmount, levelConfigs.MaxCoinItemAmount));
                        listPlatformParams.Add(platformParams);
                    }
                    break;
                }
            }


            //Create a platform at player's position
            CurrentPlatform = PoolManager.Instance.GetPlatformController(currentLevelConfigs.PlatformType, PlatformSize.MEDIUM);
            CurrentPlatform.transform.position = PlayerController.Instance.transform.position;
            CurrentPlatform.transform.forward = Vector3.forward;
            CurrentPlatform.gameObject.SetActive(true);
            nextPlatformPos = CurrentPlatform.transform.position + CurrentPlatform.transform.forward * 5;
            CurrentPlatform.OnSetup(listPlatformParams[0].CoinItemAmount);

            if (Utilities.IsShowTutorial())
            {
                Invoke(nameof(PlayingGame), 0.15f);
            }
        }


        /// <summary>
        /// Call IngameState.Ingame_Playing event and handle other actions.
        /// Actual start the game.
        /// </summary>
        public void PlayingGame()
        {
            //Fire event
            IngameState = IngameState.Ingame_Playing;
            ingameState = IngameState.Ingame_Playing;

            //Other actions
            if (IsRevived)
            {
                StartCoroutine(CRShowViewWithDelay(ViewType.INGAME_VIEW, 0f));
                ServicesManager.Instance.SoundManager.ResumeMusic(0.5f);
                CurrentPlatform.OnReset();
                StartCoroutine(CRCountDownTime());
            }
            else
            {
                ServicesManager.Instance.SoundManager.PlayMusic(backgroundMusic, 0.5f);

                //Create next platform
                PlatformParams platformParams = listPlatformParams[platformParamsIndex];
                NextPlatform = PoolManager.Instance.GetPlatformController(currentLevelConfigs.PlatformType, platformParams.PlatformSize);
                NextPlatform.transform.position = nextPlatformPos;
                NextPlatform.transform.forward = (platformParamsIndex == 0) ? (Vector3.forward) : ((Random.value <= 0.5f) ? Vector3.forward : Vector3.left);
                NextPlatform.gameObject.SetActive(true);
                nextPlatformPos = NextPlatform.transform.position + NextPlatform.transform.forward * platformParams.PlatformDistance;
                NextPlatform.OnSetup(platformParams.CoinItemAmount);

                //Update paramaters
                platformParamsIndex++;

                //Start counting down time
                timeToCompleteLevel = Random.Range(currentLevelConfigs.MinTimeToCompleteLevel, currentLevelConfigs.MaxTimeToCompleteLevel);
                StartCoroutine(CRCountDownTime());
            }
        }


        /// <summary>
        /// Call IngameState.Ingame_Revive event and handle other actions.
        /// </summary>
        public void Revive()
        {
            //Fire event
            IngameState = IngameState.Ingame_Revive;
            ingameState = IngameState.Ingame_Revive;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.REVIVE_VIEW, 1f));
            ServicesManager.Instance.SoundManager.PauseMusic(0.5f);
        }


        /// <summary>
        /// Call IngameState.Ingame_GameOver event and handle other actions.
        /// </summary>
        public void GameOver()
        {
            //Fire event
            IngameState = IngameState.Ingame_GameOver;
            ingameState = IngameState.Ingame_GameOver;

            //Add another actions here
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 0.25f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelFailed);
        }


        /// <summary>
        /// Call IngameState.Ingame_CompleteLevel event and handle other actions.
        /// </summary>
        public void CompletedLevel()
        {
            //Fire event
            IngameState = IngameState.Ingame_CompleteLevel;
            ingameState = IngameState.Ingame_CompleteLevel;

            //Other actions
            StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
            ServicesManager.Instance.SoundManager.StopMusic(0.5f);
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelCompleted);
            foreach(ParticleSystem par in confettiEffects)
            {
                par.Play();
            }

            //Save level
            if (testingLevel == 0)
            {
                PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) + 1);

                //Report level to leaderboard
                string username = PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);
                if (!string.IsNullOrEmpty(username))
                {
                    ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();
                }
            }
        }



        /// <summary>
        /// Get an PlatformSize based on level configs.
        /// </summary>
        /// <returns></returns>
        public PlatformSize GetPlatformSize()
        {
            //Calculate the total frequency
            float totalFreq = 0;
            foreach (PlatformSizeConfiguration configuration in currentLevelConfigs.ListPlatformSize)
            {
                totalFreq += configuration.Frequency;
            }

            float randomFreq = Random.Range(0, totalFreq);
            for (int i = 0; i < currentLevelConfigs.ListPlatformSize.Count; i++)
            {
                if (randomFreq < currentLevelConfigs.ListPlatformSize[i].Frequency)
                {
                    return currentLevelConfigs.ListPlatformSize[i].PlatformSize;
                }
                else
                {
                    randomFreq -= currentLevelConfigs.ListPlatformSize[i].Frequency;
                }
            }

            return currentLevelConfigs.ListPlatformSize[0].PlatformSize;
        }


        /// <summary>
        /// Coroutine show the view with given viewType and delay time.
        /// </summary>
        /// <param name="viewType"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator CRShowViewWithDelay(ViewType viewType, float delay)
        {
            yield return new WaitForSeconds(delay);
            ViewManager.Instance.OnShowView(viewType);
        }



        /// <summary>
        /// Coroutine count down the time to complete level.
        /// </summary>
        /// <param name="maxTime"></param>
        /// <returns></returns>
        private IEnumerator CRCountDownTime()
        {
            float currentTime = timeToCompleteLevel;
            ViewManager.Instance.IngameViewController.UpdateTimeSlider(timeToCompleteLevel, currentTime);
            ViewManager.Instance.IngameViewController.UpdateTimeText(Mathf.RoundToInt(currentTime));
            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                yield return null;
                ViewManager.Instance.IngameViewController.UpdateTimeSlider(timeToCompleteLevel, currentTime);
                ViewManager.Instance.IngameViewController.UpdateTimeText(Mathf.RoundToInt(currentTime));

                if (ingameState != IngameState.Ingame_Playing)
                    yield break;
            }

            PlayerController.Instance.HandleTimeOut();
            HandlePlayerDied();
        }


        //////////////////////////////////////Publish functions



        /// <summary>
        /// Continue the game
        /// </summary>
        public void SetContinueGame()
        {
            IsRevived = true;
            Invoke(nameof(PlayingGame), 0.05f);
        }



        /// <summary>
        /// Handle actions when player died.
        /// </summary>
        public void HandlePlayerDied()
        {
            if (IsRevived || !ServicesManager.Instance.AdManager.IsRewardedAdReady())
            {
                //Fire event
                IngameState = IngameState.Ingame_GameOver;
                ingameState = IngameState.Ingame_GameOver;

                //Add another actions here
                StartCoroutine(CRShowViewWithDelay(ViewType.ENDGAME_VIEW, 1f));
                ServicesManager.Instance.SoundManager.StopMusic(0.5f);
                ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.LevelFailed);
            }
            else
            {
                Revive();
            }
        }


        /// <summary>
        /// Update the next platform.
        /// </summary>
        public void UpdateNextPlatform()
        {
            CurrentPlatform = NextPlatform;

            //Create the next platform
            if (platformParamsIndex == listPlatformParams.Count && !confettiEffects[0].transform.root.gameObject.activeSelf)
            {
                //Enable finish platform
                confettiEffects[0].transform.root.position = nextPlatformPos;
                confettiEffects[0].transform.root.gameObject.SetActive(true);

                //Create the finish platform
                NextPlatform = PoolManager.Instance.GetFinishPlatform(currentLevelConfigs.PlatformType);
                NextPlatform.transform.position = nextPlatformPos;
                NextPlatform.transform.forward = CurrentPlatform.transform.forward;
                NextPlatform.gameObject.SetActive(true);
                NextPlatform.OnSetup(0, true);
            }
            else if (platformParamsIndex < listPlatformParams.Count)
            {
                //Create the platform
                PlatformParams platformParams = listPlatformParams[platformParamsIndex];
                NextPlatform = PoolManager.Instance.GetPlatformController(currentLevelConfigs.PlatformType, platformParams.PlatformSize);
                NextPlatform.transform.position = nextPlatformPos;
                NextPlatform.transform.forward = (platformParamsIndex == 0) ? (Vector3.forward) : ((Random.value <= 0.5f) ? Vector3.forward : Vector3.left);
                NextPlatform.gameObject.SetActive(true);
                nextPlatformPos = NextPlatform.transform.position + NextPlatform.transform.forward * platformParams.PlatformDistance;
                NextPlatform.OnSetup(platformParams.CoinItemAmount);

                //Update paramaters
                platformParamsIndex++;
            }
        }
    }
}
