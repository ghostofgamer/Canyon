using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClawbearGames
{
    public class BridgeBodyController : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null;
        [SerializeField] private MeshRenderer meshRenderer = null;
        [SerializeField] private MeshCollider meshCollider = null;
        [SerializeField] private Mesh bridgeBodyMesh = null;
        [SerializeField] private Mesh bridgeTailMesh = null;

        public Vector3 CenterPosition => meshRenderer.bounds.center + Vector3.up * 0.01f;

        /// <summary>
        /// Deactive this bridge body.
        /// </summary>
        public void OnDeactive()
        {
            meshFilter.mesh = bridgeBodyMesh;
            meshCollider.sharedMesh = bridgeBodyMesh;
            transform.SetParent(null);
            transform.eulerAngles = Vector3.zero;
            gameObject.SetActive(false);
        }



        /// <summary>
        /// change this bridge body to bridge tail.
        /// </summary>
        public void ChangeToTail()
        {
            meshFilter.mesh = bridgeTailMesh;
            meshCollider.sharedMesh = bridgeTailMesh;
        }
    }
}
