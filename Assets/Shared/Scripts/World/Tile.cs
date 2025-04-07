using System;
using Fighting;
using Player;
using Unity.Cinemachine;
using UnityEngine;

namespace World
{
    public class Tile : MonoBehaviour
    {
        public Transform enemyPosition;
        public Transform duelPosition;
        public Transform restingPosition;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(restingPosition.position, .1f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(duelPosition.position, .1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(enemyPosition.position, .1f);
        }
    }
}