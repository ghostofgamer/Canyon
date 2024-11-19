using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class PlayerController : MonoBehaviour
    {

        public static PlayerController Instance { private set; get; }
        public static event System.Action<PlayerState> PlayerStateChanged = delegate { };

        [Header("Player References")]
        [SerializeField] private Rigidbody rigidbody3D = null;
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private MeshRenderer meshRenderer = null;



        public PlayerState PlayerState
        {
            get { return playerState; }
            private set
            {
                if (value != playerState)
                {
                    value = playerState;
                    PlayerStateChanged(playerState);
                }
            }
        }

        public int MovementSpeed { private set; get; }
        private PlayerState playerState = PlayerState.Player_Prepare;

        private void OnEnable()
        {
            IngameManager.IngameStateChanged += IngameManager_IngameStateChanged;
        }
        private void OnDisable()
        {
            IngameManager.IngameStateChanged -= IngameManager_IngameStateChanged;
        }
        private void IngameManager_IngameStateChanged(IngameState obj)
        {
            if (obj == IngameState.Ingame_Playing)
            {
                PlayerLiving();
            }
            else if (obj == IngameState.Ingame_CompleteLevel)
            {
                PlayerCompletedLevel();
            }
        }




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
            //Fire event
            PlayerState = PlayerState.Player_Prepare;
            playerState = PlayerState.Player_Prepare;

            //Add other actions here

            //Setup character
            CharacterInforController charControl = ServicesManager.Instance.CharacterContainer.CharacterInforControllers[ServicesManager.Instance.CharacterContainer.SelectedCharacterIndex];
            meshFilter.mesh = charControl.Mesh;
            meshRenderer.material = charControl.Material;
        }


        /// <summary>
        /// Call PlayerState.Player_Living event and handle other actions.
        /// </summary>
        private void PlayerLiving()
        {
            //Fire event
            PlayerState = PlayerState.Player_Living;
            playerState = PlayerState.Player_Living;

            //Add other actions here
            if (IngameManager.Instance.IsRevived)
            {
                StartCoroutine(CRHandleActionsAfterRevived());
            }
        }


        /// <summary>
        /// Call PlayerState.Player_Died event and handle other actions.
        /// </summary>
        public void PlayerDied()
        {
            //Fire event
            PlayerState = PlayerState.Player_Died;
            playerState = PlayerState.Player_Died;

            //Add other actions here
            ServicesManager.Instance.ShareManager.CreateScreenshot();
            CameraRootController.Instance.Shake();
        }



        /// <summary>
        /// Fire Player_CompletedLevel event and handle other actions.
        /// </summary>
        private void PlayerCompletedLevel()
        {
            //Fire event
            PlayerState = PlayerState.Player_CompletedLevel;
            playerState = PlayerState.Player_CompletedLevel;

            //Add others action here
            ServicesManager.Instance.ShareManager.CreateScreenshot();
        }


        /// <summary>
        /// Coroutine handle actions after player revived.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRHandleActionsAfterRevived()
        {
            rigidbody3D.isKinematic = true;
            meshRenderer.enabled = true;
            meshRenderer.transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;
            transform.position = IngameManager.Instance.CurrentPlatform.transform.position;
            transform.forward = IngameManager.Instance.CurrentPlatform.transform.forward;
            yield return new WaitForSeconds(0.5f);
            CameraRootController.Instance.FollowPlayer(true);
        }



        /// <summary>
        /// Coroutine move the player to the position of next platform.
        /// </summary>
        /// <returns></returns>
        private IEnumerator CRMoveToNextPlatform()
        {
            //Move to next platform's position
            Vector3 startPos = transform.position;
            Vector3 endPos = IngameManager.Instance.NextPlatform.transform.position;
            float moveTime = Vector3.Distance(startPos, endPos) / MovementSpeed;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / moveTime);
                transform.position = Vector3.Lerp(startPos, endPos, factor);
                yield return null;

                if (playerState != PlayerState.Player_Living)
                    yield break;
            }


            //Rotate player according to next platform's forward direction
            if (!transform.forward.Equals(IngameManager.Instance.NextPlatform.transform.forward))
            {               
                Vector3 startDir = transform.forward;
                Vector3 endDir = IngameManager.Instance.NextPlatform.transform.forward;
                t = 0;
                moveTime = 1f;
                while (t < moveTime)
                {
                    t += Time.deltaTime;
                    float factor = EasyType.MatchedLerpType(LerpType.EaseOutCubic, t / moveTime);
                    transform.forward = Vector3.Lerp(startDir, endDir, factor);
                    yield return null;

                    if (playerState != PlayerState.Player_Living)
                        yield break;
                }
            }

            if (IngameManager.Instance.NextPlatform.IsFinishPlatform)
            {
                IngameManager.Instance.CompletedLevel();
            }
            else
            {
                IngameManager.Instance.UpdateNextPlatform();
            }
        }



        /// <summary>
        /// Coroutine move to the end of the bridge and fall down.
        /// </summary>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private IEnumerator CRMoveToEndOfBridge(Vector3 endPos)
        {
            //Move to target position
            Vector3 startPos = transform.position;
            float moveTime = Vector3.Distance(startPos, endPos) / MovementSpeed;
            float t = 0;
            while (t < moveTime)
            {
                t += Time.deltaTime;
                float factor = t / moveTime;
                transform.position = Vector3.Lerp(startPos, endPos, factor);
                yield return null;

                if (playerState != PlayerState.Player_Living)
                    yield break;
            }

            //Player died
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerFalldown);
            PlayerDied();
            rigidbody3D.isKinematic = false;
            yield return new WaitForFixedUpdate();
            rigidbody3D.AddForce((transform.forward * 2 + Vector3.down).normalized * MovementSpeed, ForceMode.Impulse);
            rigidbody3D.AddTorque(transform.right * 15f, ForceMode.Impulse);
            yield return new WaitForSeconds(1f);
            IngameManager.Instance.HandlePlayerDied();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////Public functions


        /// <summary>
        /// Setup the parameters for the player.
        /// </summary>
        /// <param name="movementSpeed"></param>
        public void SetupParams(int movementSpeed)
        {
            MovementSpeed = movementSpeed;
        }



        /// <summary>
        /// Move the player to next platform's position.
        /// </summary>
        /// <param name="platform"></param>
        public void MoveToNextPlatform()
        {
            if (IngameManager.Instance.NextPlatform.IsFinishPlatform)
                CameraRootController.Instance.FollowPlayer(false);
            StartCoroutine(CRMoveToNextPlatform());
        }



        /// <summary>
        /// Move from current position to the end of the bridge and fall down.
        /// </summary>
        /// <param name="endPos"></param>
        public void MoveToEndOfBridge(Vector3 endPos)
        {
            StartCoroutine(CRMoveToEndOfBridge(endPos));
        }


        /// <summary>
        /// Handle actions when time to complete level is out.
        /// </summary>
        public void HandleTimeOut()
        {
            ServicesManager.Instance.SoundManager.PlaySound(ServicesManager.Instance.SoundManager.PlayerExploded);
            EffectManager.Instance.CreatePlayerExplodeEffect(meshFilter.sharedMesh, transform.position, transform.forward);
            meshRenderer.enabled = false;
            PlayerDied();
        }
    }
}
