using System;
using UnityEngine;

namespace IdleSpace.Core
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private float maxHp = 100f;

        public float CurrentHp { get; private set; }
        public float MaxHp => maxHp;

        public event Action<float, float> OnHpChanged;
        public event Action OnShipDefeated;

        public void ConfigureMaxHp(float hp)
        {
            maxHp = Mathf.Max(1f, hp);
            CurrentHp = Mathf.Min(CurrentHp <= 0f ? maxHp : CurrentHp, maxHp);
            OnHpChanged?.Invoke(CurrentHp, maxHp);
        }

        public void ResetHp(float? overrideHp = null)
        {
            CurrentHp = Mathf.Clamp(overrideHp ?? maxHp, 0f, maxHp);
            OnHpChanged?.Invoke(CurrentHp, maxHp);
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || CurrentHp <= 0f)
            {
                return;
            }

            CurrentHp = Mathf.Max(0f, CurrentHp - amount);
            OnHpChanged?.Invoke(CurrentHp, maxHp);
            if (CurrentHp <= 0f)
            {
                OnShipDefeated?.Invoke();
            }
        }
    }
}
