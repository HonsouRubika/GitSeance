using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seance.Player
{
    public class TmpPlayer : MonoBehaviour
    {
        public int _maxHp = 10;
        private int _currentHp;
        public int _maxArmor = 3;
        private int _currentArmor;

        private void Start()
        {
            _currentHp = _maxHp;
            _currentArmor = _maxArmor;
        }

        public void Heal(int hp)
        {
            _currentHp += hp;
            if (_currentHp > _maxHp) _currentHp = _maxHp;
        }

        public void GainArmor(int armorGain)
        {
            _currentArmor += armorGain;
            if (_currentArmor > _maxArmor) _currentArmor = _maxArmor;
        }

        public void ReceiveDamage(int damage)
        {
            int damageAfterArmor = damage - _currentArmor;
            if (damageAfterArmor < 0) damage = 0;

            _currentHp -= damage;
        }

        public void ReceiveTrueDamage(int damage)
        {
            _currentHp -= damage;
        }

        public void ReceiveArmorDamage(int damage)
        {
            _currentArmor -= damage;
            if (_currentArmor < 0) _currentArmor = 0;
        }
    }
}
