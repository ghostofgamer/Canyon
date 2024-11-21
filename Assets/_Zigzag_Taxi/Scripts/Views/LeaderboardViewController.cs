﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;

namespace ClawbearGames
{
    public class LeaderboardViewController : BaseViewController
    {
        public NewLeaderboardManager leaderboardManager;


        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RectTransform servicesUnavailablePanelTrans = null;
        [SerializeField] private RectTransform setUsernamePanelTrans = null;
        [SerializeField] private RectTransform leaderboardPanelTrans = null;
        [SerializeField] private RectTransform contentTrans = null;
        [SerializeField] private InputField usernameInputField = null;
        [SerializeField] private Text localUsernameText = null;
        [SerializeField] private Text usernameErrorText = null;
        [SerializeField] private LeaderboardItemController leaderboardItemControllerPrefab = null;


        private List<LeaderboardItemController> listLeaderboardItemController = new List<LeaderboardItemController>();




        /// <summary>
        /// Get an inactive LeaderboardItemController object.
        /// </summary>
        /// <returns></returns>
        private LeaderboardItemController GetLeaderboardItemController()
        {
            //Find in the list
            LeaderboardItemController item = listLeaderboardItemController.Where(a => !a.gameObject.activeInHierarchy).FirstOrDefault();

            if (item == null)
            {
                //Didn't find one -> create new one
                item = Instantiate(leaderboardItemControllerPrefab, Vector3.zero, Quaternion.identity);
                item.gameObject.SetActive(false);
                listLeaderboardItemController.Add(item);
            }

            return item;
        }



        /// <summary>
        /// Coroutine create the leaderboard items.
        /// </summary>
        /// <param name="listPlayerData"></param>
        /// <param name="maxItem"></param>
        /// <returns></returns>
        private IEnumerator CRCreateLeaderboardItems(List<LeaderboardParams> listPlayerData)
        {
            for (int i = 0; i < listPlayerData.Count; i++)
            {
                //Create items
                LeaderboardItemController itemController = GetLeaderboardItemController();
                itemController.transform.SetParent(contentTrans);
                itemController.gameObject.SetActive(true);
                itemController.OnSetup(i + 1, listPlayerData[i]);

                //Set local user
                if (listPlayerData[i].Username.Equals(PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME)))
                {
                    localUsernameText.text = "#" + (i + 1).ToString() + "." + " " + listPlayerData[i].Username;
                }

                yield return new WaitForSeconds(0.05f);
            }
        }


        /// <summary>
        ////////////////////////////////////////////////// Public Functions
        /// </summary>

        public void ShowLeaderBoard()
        {
            Debug.Log("SHOW");

            if (PlayerPrefs.HasKey("PlayerName"))
            {
                Debug.Log("HASKEY");
                setUsernamePanelTrans.gameObject.SetActive(false);
                leaderboardPanelTrans.gameObject.SetActive(true);
                leaderboardManager.DisplayLeaderboard();
            }
            else
            {
                Debug.Log("DONT HASKEY");
                setUsernamePanelTrans.gameObject.SetActive(true);
                leaderboardPanelTrans.gameObject.SetActive(false);
            }

        }


        public override void OnShow()
        {
            /*     ShowLeaderBoard();
                 return;*/

            FadeInCanvasGroup(canvasGroup, 0.75f);
            servicesUnavailablePanelTrans.gameObject.SetActive(false);
            setUsernamePanelTrans.gameObject.SetActive(false);
            leaderboardPanelTrans.gameObject.SetActive(false);


            if (PlayerPrefs.HasKey("PlayerName"))
            {
                setUsernamePanelTrans.gameObject.SetActive(false);
                leaderboardPanelTrans.gameObject.SetActive(true);
                leaderboardManager.DisplayLeaderboard();

                /*//User name already been set up.
                localUsernameText.text = "#. " + PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);

                //Check connect to Dreamlo services
                ServicesManager.Instance.LeaderboardManager.CheckConnectedToDreamloServices((isConnected) =>
                {
                    if (isConnected)
                    {
                        //Connected to Dreamlo services -> show leaderboard
                        leaderboardPanelTrans.gameObject.SetActive(true);

                        //Create items and set data for local player.
                        ServicesManager.Instance.LeaderboardManager.GetPlayerLeaderboardData((playerDatas) =>
                        {
                            StartCoroutine(CRCreateLeaderboardItems(playerDatas));
                        });
                    }
                    else
                    {
                        //Not connect to Dreamlo services -> show servicesUnavailableView
                        servicesUnavailablePanelTrans.gameObject.SetActive(true);
                    }
                });*/
            }
            else
            {
                //User name is not set up yet -> set user name.
                setUsernamePanelTrans.gameObject.SetActive(true);
                usernameErrorText.gameObject.SetActive(false);
                localUsernameText.text = string.Empty;
            }

            return;




            FadeInCanvasGroup(canvasGroup, 0.75f);
            servicesUnavailablePanelTrans.gameObject.SetActive(false);
            setUsernamePanelTrans.gameObject.SetActive(false);
            leaderboardPanelTrans.gameObject.SetActive(false);
            if (!ServicesManager.Instance.LeaderboardManager.IsSetUsername())
            {
                //User name is not set up yet -> set user name.
                setUsernamePanelTrans.gameObject.SetActive(true);
                usernameErrorText.gameObject.SetActive(false);
                localUsernameText.text = string.Empty;
            }
            else
            {
                //User name already been set up.
                localUsernameText.text = "#. " + PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);

                //Check connect to Dreamlo services
                ServicesManager.Instance.LeaderboardManager.CheckConnectedToDreamloServices((isConnected) =>
                {
                    if (isConnected)
                    {
                        //Connected to Dreamlo services -> show leaderboard
                        leaderboardPanelTrans.gameObject.SetActive(true);

                        //Create items and set data for local player.
                        ServicesManager.Instance.LeaderboardManager.GetPlayerLeaderboardData((playerDatas) =>
                        {
                            StartCoroutine(CRCreateLeaderboardItems(playerDatas));
                        });
                    }
                    else
                    {
                        //Not connect to Dreamlo services -> show servicesUnavailableView
                        servicesUnavailablePanelTrans.gameObject.SetActive(true);
                    }
                });
            }
        }

        public override void OnClose()
        {

            Debug.Log("OnClose");
            foreach (LeaderboardItemController o in listLeaderboardItemController)
            {
                o.gameObject.SetActive(false);
            }
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }


        /// <summary>
        ////////////////////////////////////////////////// UI Buttons
        /// </summary>



        public void OnClickConfirmButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            usernameErrorText.gameObject.SetActive(false);

            Regex regex = new Regex(@"^[A-z][A-z|\.|\s]+$");
            if (!regex.IsMatch(usernameInputField.text))
            {
                usernameErrorText.gameObject.SetActive(true);
                usernameErrorText.text = "Please Choose A Different Username !";
            }
            else //Username passed the regex check
            {
                //Check connect to Dreamlo services
                ServicesManager.Instance.LeaderboardManager.CheckConnectedToDreamloServices((isConnected) =>
                {
                    if (isConnected)
                    {
                        //Connected to Dreamlo services -> check username exists
                        string username = usernameInputField.text.Trim();
                        usernameInputField.text = username;
                        ServicesManager.Instance.LeaderboardManager.CheckUsernameExists(username, (isExists) =>
                        {
                            if (isExists)
                            {
                                //Username already exists
                                usernameErrorText.gameObject.SetActive(true);
                                usernameErrorText.text = "The Username Already Exists !";
                            }
                            else
                            {
                                //Username not exists -> set username and show leaderboard with the username
                                usernameErrorText.gameObject.SetActive(false);
                                setUsernamePanelTrans.gameObject.SetActive(false);
                                leaderboardPanelTrans.gameObject.SetActive(true);
                                PlayerPrefs.SetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME, usernameInputField.text);
                                localUsernameText.text = "N/A. " + PlayerPrefs.GetString(PlayerPrefsKeys.PPK_SAVED_USER_NAME);
                                ServicesManager.Instance.LeaderboardManager.SetPlayerLeaderboardData();

                                //Create items and set data for local player.
                                ServicesManager.Instance.LeaderboardManager.GetPlayerLeaderboardData((playerDatas) =>
                                {
                                    StartCoroutine(CRCreateLeaderboardItems(playerDatas));
                                });
                            }
                        });

                    }
                    else
                    {
                        //Not connect to Dreamlo services -> show servicesUnavailablePanel
                        servicesUnavailablePanelTrans.gameObject.SetActive(true);
                        setUsernamePanelTrans.gameObject.SetActive(false);
                        leaderboardPanelTrans.gameObject.SetActive(false);
                    }
                });
            }
        }


        public void OnClickCloseButton()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.Button);
            ViewManager.Instance.OnShowView(ViewType.HOME_VIEW);
        }
    }
}
