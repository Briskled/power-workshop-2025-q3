using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Fighting
{
    public delegate void DamageChangedDelegate(int damageBefore, int damageNow);

    public class Fighter : MonoBehaviour
    {
        [SerializeField] private int damage;

        public int Damage
        {
            get => damage;
            set
            {
                var oldValue = damage;
                var newValue = Mathf.Max(value, 0);
                damage = newValue;
                onDamageChanged?.Invoke(oldValue, newValue);
            }
        }

        public event DamageChangedDelegate onDamageChanged;

        public async UniTask Attack(Health target)
        {
            // todo animation
            target.Damage(damage);
            Debug.Log($"{name} just attacked {target.name} dealing {damage} damage");
            await UniTask.WaitForSeconds(0.7f);
        }
    }
}