using UnityEngine;

namespace Fighting
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Fighter))]
    public class Enemy : MonoBehaviour
    {
        public Health Health { get; private set; }
        public Fighter Fighter { get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
            Fighter = GetComponent<Fighter>();
        }
    }
}