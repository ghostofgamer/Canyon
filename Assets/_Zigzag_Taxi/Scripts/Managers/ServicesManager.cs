﻿using UnityEngine;

namespace ClawbearGames
{
    public class ServicesManager : MonoBehaviour
    {
        public static ServicesManager Instance { private set; get; }

        [SerializeField] private SoundManager soundManager = null;
        [SerializeField] private CoinManager coinManager = null;
        [SerializeField] private ShareManager shareManager = null;
        [SerializeField] private CharacterContainer characterContainer = null;
        [SerializeField] private DailyRewardManager dailyRewardManager = null;
        [SerializeField] private LeaderboardManager leaderboardManager = null;
        [SerializeField] private NotificationManager notificationManager = null;
        [SerializeField] private AdManager adManager = null;

        public AdManager AdManager => adManager;
        public SoundManager SoundManager => soundManager;
        public CoinManager CoinManager => coinManager;
        public ShareManager ShareManager => shareManager;
        public DailyRewardManager DailyRewardManager => dailyRewardManager;
        public LeaderboardManager LeaderboardManager => leaderboardManager;
        public NotificationManager NotificationManager => notificationManager;
        public CharacterContainer CharacterContainer => characterContainer;


        private void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

