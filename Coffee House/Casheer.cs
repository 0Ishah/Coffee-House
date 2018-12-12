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
        public double CurrentProcessTime { get; private set; } = 0;
        public Customer CurrentCustomer { get; private set; }

        public bool ContainsCustomer()
        {
            if (CurrentCustomer != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a new customer to casheer and updates service time requirements
        /// </summary>
        /// <param name="newCustomer">New Customer</param>
        public void StartProcessing(Customer newCustomer)
        {
            IsOccupied = true;
            CurrentCustomer = newCustomer;
            CurrentProcessTime = 0;
            switch (CurrentCustomer.Order)
            {
                case 0://Coffee
                    CurrentProcessTime = Information.COFFEE_SERVE_TIME;
                    break;
                case 1://Food
                    CurrentProcessTime = Information.FOOD_SERVE_TIME;
                    break;
                case 2://Combo
                    CurrentProcessTime = Information.COMBO_SERVE_TIME;
                    break;
            }
        }    

        /// <summary>
        /// Adds a second to the processing timer
        /// If the timer runs out, sets the casheer to not occupied
        /// </summary>
        public void Process()
        {
            CurrentProcessTime -= 1;
            if (CurrentProcessTime <= 0)
            {
                IsOccupied = false;
                CurrentProcessTime = 0;
            }
        }

        /// <summary>
        /// Sets the current customer to null
        /// </summary>
        public void RemoveCurrentCustomer()
        {
            CurrentCustomer = null;
        }
    }
}
