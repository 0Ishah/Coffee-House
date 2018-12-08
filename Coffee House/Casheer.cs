using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee_House.Functionality;

namespace Coffee_House
{
    class Casheer
    {
        public bool IsOccupied { get; private set; } = false;
        private double currentProcessTime = 0;
        private double currentProcessTotalTime = 0;
        public Customer CurrentCustomer { get; private set; }

        public bool ContainsCustomer()
        {
            if (CurrentCustomer != null)
            {
                return true;
            }
            return false;
        }

        public void StartProcessing(Customer newCustomer)
        {
            IsOccupied = true;
            CurrentCustomer = newCustomer;
            switch(newCustomer.Order)
            {
                //Coffee
                case 0:
                    currentProcessTotalTime = Information.COFFEE_SERVE_TIME;
                    break;
                //Food
                case 1:
                    currentProcessTotalTime = Information.FOOD_SERVE_TIME;
                    break;
                //Combo
                case 2:
                    currentProcessTotalTime = Information.COMBO_SERVE_TIME;
                    break;
            }
        }

        public void RemoveCurrentCustomer()
        {
            CurrentCustomer = null;
        }

        public void Process()
        {
            if (IsOccupied)
            {
                currentProcessTime++;
                if (currentProcessTime >= currentProcessTotalTime)
                {
                    IsOccupied = false;
                    currentProcessTime = 0;
                    currentProcessTotalTime = 0;
                }
            }
        }
    }
}
