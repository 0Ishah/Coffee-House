using Microsoft.Xna.Framework;
using System;

namespace Coffee_House.Functionality
{
    class Customer
    {
        private Customer next;

        public int Order { get; }
        public string Name { get; }
        public double CreationTime { get; }
        public double TimeInQueue { get; set; } = 0;
        public int TravelPoint { get; set; } = 0;
        public Vector2 DestinationPosition { get; set; }

        //Customer position is created as a rectangle in order to avoid creating new rectangles when drawing
        //A separate variable position is created on top of the esisting property to allow editing of the position inside the class
        private Rectangle position = new Rectangle((int) Information.CUSTOMER_SPAWN_POS.X, (int) Information.CUSTOMER_SPAWN_POS.Y, GameMain.CUSTOMER_REC_WIDTH, GameMain.CUSTOMER_REC_HEIGHT);
        public Rectangle Position
        {
            get { return position; }
        }

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
            if (DestinationPosition.X == Position.X && DestinationPosition.Y == Position.Y)
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
                Vector2 dir = new Vector2(DestinationPosition.X - position.X, DestinationPosition.Y - position.Y);
                dir.Normalize();
                dir.X = Convert.ToInt32(dir.X);
                dir.Y = Convert.ToInt32(dir.Y);

                position.X += (int)(dir.X * Information.CUSTOMER_MOVE_SPEED);
                position.Y += (int)(dir.Y * Information.CUSTOMER_MOVE_SPEED);
            }
        }
    }
}
