using GFSUtilities;
using System;
using System.Collections;
using TMPro;
using UnityEngine;


namespace GFSManagers
{
    public class NoneBGManager : MonoBehaviour
    {
        int _currentMoney;

        public int _CurrentGold
        {
            get => _currentMoney;
            set => ChangeMoney(value, value > _currentMoney);
        }
        [SerializeField] TextMeshProUGUI _moneyText;

        public void InitManager(int defaultMoney)
        {
            SetMoney(defaultMoney);
        }
        public void SetMoney(int money)
        {
            _currentMoney = money;
            _moneyText.text = _CurrentGold.ToString("N0");
        }
        void ChangeMoney(int money, bool isAdd)
        {
            //¿Ã∆Â∆Æ
            if (isAdd)
            {

            }
            else
            {

            }

            SetMoney(money);
        }
        public void ErrorMoney()
        {
            //¿Ã∆Â∆Æ
        }

        public bool TryChangeGold(int add)
        {
            if (add < -_CurrentGold)
            {
                ErrorMoney();
                return false;
            }

            _CurrentGold += add;
            return true;

        }
    }
}