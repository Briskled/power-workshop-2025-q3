using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fighting
{
    public class Fighter : MonoBehaviour
    {
        [SerializeField] private float damage;

        public async UniTask Attack(Health target)
        {
            // todo animation
            target.Damage(damage);
            Debug.Log($"{name} just attacked {target.name} dealing {damage} damage");
            await UniTask.WaitForSeconds(0.7f);
        }
    }
}