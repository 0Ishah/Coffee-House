using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffee_House.Functionality
{
    class Customer //TODO: Add documentation !!
    {
        private Customer next;

        public int Order { get; }
        public string Name { get; }
        public double CreationTime { get; }
        public int TimeInQueue { get; set; } = 0;
        public Vector2 DestinationPosition { get; set; }
        public Vector2 Position { get; private set; } = new Vector2(Information.CUSTOMER_SPAWN_POS.X, Information.CUSTOMER_SPAWN_POS.Y);

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="order">Customer's order as int</param>
        /// <param name="name">Customer's unique name</param>
        /// <param name="creationTime">Time in seconds since the simulation start</param>
        public Customer(int order, string name, double creationTime)
        {
            Order = order;
            Name = name;
            CreationTime = creationTime;
        }

        /// <summary>
        /// Gets the next customer int the queue
        /// </summary>
        /// <returns></returns>
        public Customer GetNext()
        {
            return next;
        }

        /// <summary>
        /// Sets the next customer in the queue
        /// </summary>
        /// <param name="newCustomer"></param>
        public void SetNext(Customer newCustomer)
        {
            next = newCustomer;
        }

        /// <summary>
        /// Checks if the customer is currently at its destination
        /// </summary>
        /// <returns></returns>
        public bool IsAtDestination()
        {
            if (DestinationPosition == Position)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves the customer to their destination coordinates
        /// </summary>
        public void Move()
        {
            if (!IsAtDestination())
            {
                Vector2 dir = DestinationPosition - Position;
                dir.Normalize();

                Position += dir * Information.CUSTOMER_MOVE_SPEED;
            }
        }
    }
}
