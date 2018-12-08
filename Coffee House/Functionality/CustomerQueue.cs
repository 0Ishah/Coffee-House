using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coffee_House.Functionality;
using Microsoft.Xna.Framework;

namespace Coffee_House.Functionality
{
    class CustomerQueue
    {
        /// <summary>
        /// Counts the length of the queue
        /// </summary>
        public int Count { get; private set; }

        private Customer front;

        /// <summary>
        /// Peeks into the queue looking at the first element
        /// </summary>
        /// <returns></returns>
        public Customer Peek()
        {
            return front;
        }

        /// <summary>
        /// Returns the first eklement of the queue and remeves it from the queue
        /// </summary>
        /// <returns></returns>
        public Customer Dequeue()
        {
            if (!IsEmpty())
            {
                Customer temp = front;
                if (front.GetNext() != null)
                {
                    front = front.GetNext();
                }
                else
                {
                    front = null;
                }
                Count--;
            }
            return null;
        }

        /// <summary>
        /// Add a customer to the end of the line
        /// </summary>
        /// <param name="newCustomer">new Customer</param>
        public void Enqueue(Customer newCustomer)
        {
            if (!IsEmpty())
            {
                Customer current = front;
                while (current.GetNext() != null)
                {
                    current = current.GetNext(); 
                }
                current.SetNext(newCustomer);
            }
            else
            {
                front = newCustomer;
            }
            Count++;
        }

        /// <summary>
        /// Checks if the queue is empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
