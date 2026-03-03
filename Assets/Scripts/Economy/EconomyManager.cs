using System;
using UnityEngine;

namespace IdleSpace.Economy
{
    public class EconomyManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem goldDropFxPrefab;

        public double Gold { get; private set; }

        public event Action<double> OnGoldChanged;

        public void SetGold(double amount)
        {
            Gold = Math.Max(0d, amount);
            OnGoldChanged?.Invoke(Gold);
        }

        public void AddGold(double amount, Vector3 worldPosition)
        {
            if (amount <= 0d)
            {
                return;
            }

            Gold += amount;
            SpawnGoldFx(worldPosition);
            OnGoldChanged?.Invoke(Gold);
        }

        public bool TrySpend(double amount)
        {
            if (amount <= 0d || Gold < amount)
            {
                return false;
            }

            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }

        private void SpawnGoldFx(Vector3 worldPosition)
        {
            if (goldDropFxPrefab == null)
            {
                return;
            }

            ParticleSystem fx = Instantiate(goldDropFxPrefab, worldPosition, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, 2f);
        }
    }
}
