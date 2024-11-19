using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{

    #region --------------------Ingame Enums
    public enum IngameState
    {
        Ingame_Playing = 0,
        Ingame_Revive = 1,
        Ingame_GameOver = 2,
        Ingame_CompleteLevel = 3,
    }

    public enum PlayerState
    {
        Player_Prepare = 0,
        Player_Living = 1,
        Player_Died = 2,
        Player_CompletedLevel = 3,
    }


    public enum ItemType
    {
        COIN = 0,
    }


    public enum PlatformType
    {
        GREEN = 0,
        BLUE = 1,
        YELLOW = 2,
        RED = 3,
        PINK = 4,
        CYAN = 5,
        ORANGE = 6,
        GRAY = 7,
        BROWN = 8,
        PURPLE = 9,
    }

    public enum PlatformSize
    {
        HUGE = 0,
        BIG = 1,
        MEDIUM = 2,
        NORMAL = 3,
        SMALL = 4,
    }


    public enum DayType
    {
        DAY_1 = 0,
        DAY_2 = 1,
        DAY_3 = 2,
        DAY_4 = 3,
        DAY_5 = 4,
        DAY_6 = 5,
        DAY_7 = 6,
        DAY_8 = 7,
        DAY_9 = 8,
    }

    #endregion



    #region --------------------Ads Enums
    public enum BannerAdType
    {
        NONE = 0,
        ADMOB = 1,
        UNITY = 2,
    }

    public enum InterstitialAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }


    public enum RewardedAdType
    {
        UNITY = 0,
        ADMOB = 1,
    }

    public enum RewardedAdTarget
    {
        GET_FREE_COINS = 0,
        GET_DOUBLE_COIN = 1,
        REVIVE_PLAYER = 2,
    }

    #endregion



    #region --------------------View Enums
    public enum ViewType
    {
        HOME_VIEW = 0,
        LEADERBOARD_VIEW = 1,
        DAILY_REWARD_VIEW = 2,
        LOADING_VIEW = 3,
        INGAME_VIEW = 4,
        REVIVE_VIEW = 5,
        ENDGAME_VIEW = 6,
        CHARACTER_VIEW = 7,
    }

    #endregion



    #region --------------------Classes

    [System.Serializable]
    public class LevelConfiguration
    {
        [Header("Level Number Configuration")]
        [SerializeField] private int minLevel = 1;
        public int MinLevel => minLevel;
        [SerializeField] private int maxLevel = 1;
        public int MaxLevel => maxLevel;



        [Header("Background Configuration")]
        [SerializeField] private Color backgroundTopColor = Color.white;
        public Color BackgroundTopColor => backgroundTopColor;
        [SerializeField] private Color backgroundBottomColor = Color.white;
        public Color BackgroundBottomColor => backgroundBottomColor;
        [SerializeField] private AudioClip backgroundMusicClip = null;
        public AudioClip BackgroundMusicClip => backgroundMusicClip;


        [Header("Player Parameters Configuration")]
        [SerializeField][Range(1, 200)] private int minPlayerMovementSpeed = 3;
        public int MinPlayerMovementSpeed => minPlayerMovementSpeed;
        [SerializeField][Range(1, 200)] private int maxPlayerMovementSpeed = 5;
        public int MaxPlayerMovementSpeed => maxPlayerMovementSpeed;


        [Header("Time To Complete Level Configuration")]
        [SerializeField][Range(1, 600)] private int minTimeToCompleteLevel = 60;
        public int MinTimeToCompleteLevel => minTimeToCompleteLevel;
        [SerializeField][Range(1, 600)] private int maxTimeToCompleteLevel = 120;
        public int MaxTimeToCompleteLevel => maxTimeToCompleteLevel;

        [Header("Bridge Building Speed Configuration")]
        [SerializeField][Range(1, 50)] private int minBridgeBuildingSpeed = 3;
        public int MinBridgeBuildingSpeed => minBridgeBuildingSpeed;
        [SerializeField][Range(1, 50)] private int maxBridgeBuildingSpeed = 10;
        public int MaxBridgeBuildingSpeed => maxBridgeBuildingSpeed;



        [Header("Items Configuration")]
        [SerializeField] [Range(1, 20)] private int minCoinItemAmount = 1;
        public int MinCoinItemAmount => minCoinItemAmount;
        [SerializeField] [Range(1, 20)] private int maxCoinItemAmount = 5;
        public int MaxCoinItemAmount => maxCoinItemAmount;


        [Header("Platforms Configuration")]
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        public PlatformType PlatformType => platformType;
        [SerializeField] private int minPlatformAmount = 10;
        public int MinPlatformAmount => minPlatformAmount;
        [SerializeField] private int maxPlatformAmount = 20;
        public int MaxPlatformAmount => maxPlatformAmount;
        [SerializeField][Min(6)] private int minPlatformDistance = 8;
        public int MinPlatformDistance => minPlatformDistance;
        [SerializeField][Min(6)] private int maxPlatformDistance = 15;
        public int MaxPlatformDistance => maxPlatformDistance;
        [SerializeField] private List<PlatformSizeConfiguration> listPlatformSize = new List<PlatformSizeConfiguration>();
        public List<PlatformSizeConfiguration> ListPlatformSize => listPlatformSize;
    }

    [System.Serializable]
    public class PlatformSizeConfiguration
    {
        [SerializeField] private PlatformSize platformSize = PlatformSize.HUGE;
        public PlatformSize PlatformSize => platformSize;
        [SerializeField][Range(0f, 1f)] private float frequency = 0.1f;
        public float Frequency => frequency;
    }


    [System.Serializable]
    public class PlatformPrefabConfiguration
    {
        [SerializeField] private PlatformType platformType = PlatformType.GREEN;
        public PlatformType PlatformType => platformType;
        [SerializeField] private PlatformController finishPlatformControllerPrefab = null;
        public PlatformController FinishPlatformControllerPrefab => finishPlatformControllerPrefab;
        [SerializeField] private PlatformController[] platformControllerPrefabs = null;
        public PlatformController[] PlatformControllerPrefabs => platformControllerPrefabs;
    }





    [System.Serializable]
    public class DailyRewardConfiguration
    {
        [SerializeField] private DayType dayType = DayType.DAY_1;

        /// <summary>
        /// the day type of this DailyRewardItem.
        /// </summary>
        public DayType DayType { get { return dayType; } }


        [SerializeField] private int coinAmount = 0;


        /// <summary>
        /// The amount of coins reward to player.
        /// </summary>
        public int CoinAmount { get { return coinAmount; } }
    }


    [System.Serializable]
    public class InterstitialAdConfiguration
    {
        [SerializeField] private IngameState ingameStateWhenShowingAd = IngameState.Ingame_CompleteLevel;
        public IngameState IngameStateWhenShowingAd { get { return ingameStateWhenShowingAd; } }

        [SerializeField] private int ingameStateAmountWhenShowingAd = 3;
        public int IngameStateAmountWhenShowingAd { get { return ingameStateAmountWhenShowingAd; } }


        [SerializeField] private float delayTimeWhenShowingAd = 2f;
        public float DelayTimeWhenShowingAd { get { return delayTimeWhenShowingAd; } }

        [SerializeField] private List<InterstitialAdType> listInterstitialAdType = new List<InterstitialAdType>();
        public List<InterstitialAdType> ListInterstitialAdType { get { return listInterstitialAdType; } }
    }



    public class PlatformParams
    {
        public bool IsFirstPlatform { private set; get; }
        public void SetFirstPlatfor(bool isFirstPlatform)
        {
            IsFirstPlatform = isFirstPlatform;
        }

        public PlatformSize PlatformSize { private set; get; }
        public void SetPlatformSize(PlatformSize platformSize)
        {
            PlatformSize = platformSize;
        }


        public int PlatformDistance { private set; get; }
        public void SetPlatformDistance(int distance)
        {
            PlatformDistance = distance;
        }

        public int CoinItemAmount { private set; get; }
        public void SetCoinItemAmount(int coinItemAmount)
        {
            CoinItemAmount = coinItemAmount;
        }
    }


    public class PlayerParams
    {
        public float PlayerMovementSpeed { private set; get; }
        public void SetPlayerMovementSpeed(float playerMovementSpeed)
        {
            PlayerMovementSpeed = playerMovementSpeed;
        }

        public float PlayerJumpingPoints { private set; get; }
        public void SetPlayerJumpingPoints(float playerJumpingPoints)
        {
            PlayerJumpingPoints = playerJumpingPoints;
        }
    }




    public class LeaderboardParams
    {
        public string Username { private set; get; }
        public void SetUsername(string username)
        {
            Username = username;
        }

        public int Level { private set; get; }
        public void SetLevel(int level)
        {
            Level = level;
        }
    }

    public class LeaderboardComparer : IComparer<LeaderboardParams>
    {
        public int Compare(LeaderboardParams dataX, LeaderboardParams dataY)
        {
            if (dataX.Level < dataY.Level)
                return 1;
            if (dataX.Level > dataY.Level)
                return -1;
            else
                return 0;
        }
    }

    #endregion
}
