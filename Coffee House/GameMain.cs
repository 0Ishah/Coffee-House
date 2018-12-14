using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Coffee_House.Functionality;

namespace Coffee_House
{
    public class GameMain : Game
    {
        // System elements
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Random rng = new Random();


        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 480;

        // Graphics design elements
        private Rectangle shopRec;

        public const int CUSTOMER_REC_WIDTH = 25;
        public const int CUSTOMER_REC_HEIGHT = 30;

        private Texture2D shopTexture;
        private Texture2D customerTexture;
        private Texture2D cashierTexture;

        private SpriteFont font;

        // Simulation related elements
        private double currentSimulationTime;

        private const int SIMULATION_UPDATE_TIME = 1;
        private double simulationUpdateCounter;

        private int numCurrentCoffee;
        private int numCurrentFood;
        private int numCurrentCombo;

        private Cashier[] cashiers = new Cashier[] { new Cashier(), new Cashier(), new Cashier() };
        private Rectangle[] cashierRec = new Rectangle[]
        {
            new Rectangle((int)Information.CUSTOMER_SERVING_POSITIONS[0].X, (int)Information.CUSTOMER_SERVING_POSITIONS[0].Y - 90,CUSTOMER_REC_WIDTH,CUSTOMER_REC_HEIGHT),
            new Rectangle((int)Information.CUSTOMER_SERVING_POSITIONS[1].X, (int)Information.CUSTOMER_SERVING_POSITIONS[1].Y - 90,CUSTOMER_REC_WIDTH,CUSTOMER_REC_HEIGHT),
            new Rectangle((int)Information.CUSTOMER_SERVING_POSITIONS[2].X, (int)Information.CUSTOMER_SERVING_POSITIONS[2].Y - 90,CUSTOMER_REC_WIDTH,CUSTOMER_REC_HEIGHT)
        };

        private CustomerQueue outsideQueue = new CustomerQueue();
        private CustomerQueue insideQueue = new CustomerQueue();

        private List<Customer> processedCustomers = new List<Customer>();

        private double customerSpawnCounter;
        private double[] customerServeCounter = new double[3];

        private Customer outsideCurrent;
        private Customer insideCurrent;

        private double averageWaitTime;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                  INITIALIZE                         ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void Initialize()
        {
            //CODE HERE
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
            base.Initialize();
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                 LOAD CONTENT                        ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //CODE HERE

            // Initialize design rectangles
            shopRec = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

            // Load design textures
            shopTexture = Content.Load<Texture2D>("Sprites/shop");
            customerTexture = Content.Load<Texture2D>("Sprites/customer");
            cashierTexture = Content.Load<Texture2D>("Sprites/cashier");

            font = Content.Load<SpriteFont>("Fonts/myFont");
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                UNLOAD CONTENT                       ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void UnloadContent()
        {
            //CODE HERE
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                    UPDATE                           ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void Update(GameTime gameTime)
        {
            //CODE HERE
            if (currentSimulationTime <= Information.SIMULATION_TOTAL_TIME)
            {
                // Count all of the timers
                customerSpawnCounter += gameTime.ElapsedGameTime.TotalSeconds;
                simulationUpdateCounter += gameTime.ElapsedGameTime.TotalSeconds;
                currentSimulationTime += gameTime.ElapsedGameTime.TotalSeconds;

                // Spawn a new customer and place into outside queue every 3 seconds
                if (customerSpawnCounter >= Information.CUSTOMER_SPAWN_INTERVAL)
                {
                    int randomType = rng.Next(0, Information.POSSIBLE_FOODS);
                    int randomDish = rng.Next(0, Information.POSSIBLE_DISHES);
                    switch (randomType)
                    {
                        // Coffee
                        case 0:
                            numCurrentCoffee++;
                            outsideQueue.Enqueue(new Customer(randomType, Information.COFFEE_TYPES[randomDish] + " " + numCurrentCoffee, currentSimulationTime));
                            break;
                        // Foods
                        case 1:
                            numCurrentFood++;
                            outsideQueue.Enqueue(new Customer(randomType, Information.FOOD_TYPES[randomDish] + " " + numCurrentFood, currentSimulationTime));
                            break;
                        // Combos
                        case 2:
                            numCurrentCombo++;
                            outsideQueue.Enqueue(new Customer(randomType, Information.COMBO_TYPES[randomDish] + " " + numCurrentCombo, currentSimulationTime));
                            break;
                    }
                    customerSpawnCounter = 0;
                }

                // Transfer customers into the inside queue if there are less then 20 peopel inside
                if (insideQueue.Count < Information.MAX_PEOPLE_INSIDE && !outsideQueue.IsEmpty() && outsideQueue.Peek().IsAtDestination())
                {
                    insideQueue.Enqueue(outsideQueue.Dequeue());
                }

                // Move customers in the outside line
                if (!outsideQueue.IsEmpty())
                {
                    outsideCurrent = outsideQueue.Peek();
                    // If the firest element is not ate the destiantion position
                    if (outsideCurrent.Position.X != Information.OUTSIDE_LINE_START.X || outsideCurrent.Position.Y != Information.OUTSIDE_LINE_START.Y)
                    {
                        // Set the destination position to the start of the line
                        outsideCurrent.DestinationPosition = Information.OUTSIDE_LINE_START;
                    }
                    for (int i = 0; i < outsideQueue.Count - 1; i++)
                    {
                        // Move all the customers except the first to their destanation behind the first
                        outsideCurrent.GetNext().DestinationPosition = Information.OUTSIDE_LINE_START + new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + CUSTOMER_REC_WIDTH) * (i + 1), 0);

                        outsideCurrent = outsideCurrent.GetNext();
                    }
                    outsideCurrent = null;
                }

                // Move customers in the inside queue
                if (!insideQueue.IsEmpty())
                {
                    insideCurrent = insideQueue.Peek();
                    for (int i = 0; i < insideQueue.Count; i++)
                    {
                        //Calculate customer position based oont heir position in the line
                        switch (insideCurrent.TravelPoint)
                        {
                            case (0):
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[0];
                                if (insideCurrent.IsAtDestination())
                                {
                                    insideCurrent.TravelPoint = 1;
                                }
                                break;
                            case (1):
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[1];
                                if (insideCurrent.IsAtDestination())
                                {
                                    insideCurrent.TravelPoint = 2;
                                }
                                break;
                            case (2):
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[2];
                                if (i > 10)
                                {
                                    insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[2] - new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + CUSTOMER_REC_WIDTH) * (i - 10), 0);
                                }
                                if (insideCurrent.Position.X == Information.INSIDE_LINE_TRAVEL_POINTS[2].X && insideCurrent.Position.Y == Information.INSIDE_LINE_TRAVEL_POINTS[2].Y && i < 10)
                                {
                                    insideCurrent.TravelPoint = 3;
                                }
                                break;
                            case (3):
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[3];
                                if (insideCurrent.IsAtDestination())
                                {
                                    insideCurrent.TravelPoint = 4;
                                }
                                break;
                            case (4):
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[4] + new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + CUSTOMER_REC_WIDTH) * i, 0);
                                if (insideCurrent.Position.X == Information.INSIDE_LINE_TRAVEL_POINTS[4].X && insideCurrent.Position.Y == Information.INSIDE_LINE_TRAVEL_POINTS[4].Y)
                                {
                                    insideCurrent.TravelPoint = 5;
                                }
                                break;
                        }
                        insideCurrent = insideCurrent.GetNext();
                    }
                }

                // Move the customers to the next avalible cashier
                if (!insideQueue.IsEmpty())
                {
                    for (int i = 0; i < cashiers.Length; i++)
                    {
                        if (!cashiers[i].IsOccupied && insideQueue.Peek().TravelPoint == 5)
                        {
                            cashiers[i].StartProcessing(insideQueue.Dequeue());
                        }
                    }
                }

                // Update all fo the cashiers
                if (simulationUpdateCounter >= SIMULATION_UPDATE_TIME)
                {
                    for (int i = 0; i < cashiers.Length; i++)
                    {
                        if (cashiers[i].IsOccupied)
                        {
                            //Process customer if in seving location
                            if (cashiers[i].CurrentCustomer.TravelPoint == 6)
                            {
                                cashiers[i].Process();
                            }
                            //Remove customer from the cashier
                            if (!cashiers[i].IsOccupied && cashiers[i].ContainsCustomer())
                            {
                                cashiers[i].CurrentCustomer.TimeInQueue = currentSimulationTime - cashiers[i].CurrentCustomer.CreationTime;
                                processedCustomers.Add(cashiers[i].CurrentCustomer);
                                processedCustomers = MergeSort(processedCustomers);
                                averageWaitTime = 0;
                                foreach (Customer customer in processedCustomers)
                                {
                                    averageWaitTime += customer.TimeInQueue;
                                }
                                averageWaitTime /= processedCustomers.Count;
                            }
                        }
                    }
                    simulationUpdateCounter = 0;
                }

                //Move customers to the cashier serving area graphically
                for (int i = 0; i < cashiers.Length; i++)
                {
                    if (cashiers[i].IsOccupied && cashiers[i].CurrentCustomer.TravelPoint == 5)
                    {
                        cashiers[i].CurrentCustomer.DestinationPosition = Information.CUSTOMER_SERVING_POSITIONS[i];
                        if (cashiers[i].CurrentCustomer.IsAtDestination())
                        {
                            cashiers[i].CurrentCustomer.TravelPoint = 6;
                        }
                    }
                }

                //Move processed customer outside of the coffe shop
                foreach (Customer customer in processedCustomers)
                {
                    switch (customer.TravelPoint)
                    {
                        case 6:
                            customer.DestinationPosition = Information.EXIT_POSITION_INSIDE;
                            if (customer.IsAtDestination())
                            {
                                customer.TravelPoint = 7;
                            }
                            break;
                        case 7:
                            customer.DestinationPosition = Information.EXIT_POSITION_OUTSIDE;
                            if (customer.IsAtDestination())
                            {
                                customer.TravelPoint = 8;
                            }
                            break;
                    }
                }

                //Move all of the customers in a simulation
                outsideCurrent = outsideQueue.Peek();
                for (int i = 0; i < outsideQueue.Count; i++)
                {
                    outsideCurrent.Move();
                    outsideCurrent = outsideCurrent.GetNext();
                }
                insideCurrent = insideQueue.Peek();
                for (int i = 0; i < insideQueue.Count; i++)
                {
                    insideCurrent.Move();
                    insideCurrent = insideCurrent.GetNext();
                }
                foreach (Cashier cashier in cashiers)
                {
                    if (cashier.ContainsCustomer())
                    {
                        cashier.CurrentCustomer.Move();
                    }
                }
                foreach (Customer customer in processedCustomers)
                {
                    customer.Move();
                }

                base.Update(gameTime);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                     DRAW                            ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //CODE HERE
            spriteBatch.Begin();

            //Draw the background
            spriteBatch.Draw(shopTexture, shopRec, Color.White);

            // Draw outside queue
            if (!outsideQueue.IsEmpty())
            {
                outsideCurrent = outsideQueue.Peek();
                for (int i = 0; i < outsideQueue.Count; i++)
                {
                    spriteBatch.Draw(customerTexture, outsideCurrent.Position, Color.White);
                    spriteBatch.DrawString(font, outsideCurrent.Name, ToVector2(outsideCurrent.Position) + Information.CUSTOMER_NAME_OFFSET , Color.Purple);
                    outsideCurrent = outsideCurrent.GetNext();
                }
                outsideCurrent = null;
            }

            // Draw inside queue
            if (!insideQueue.IsEmpty())
            {
                insideCurrent = insideQueue.Peek();
                for (int i = 0; i < insideQueue.Count; i++)
                {
                    spriteBatch.Draw(customerTexture, insideCurrent.Position, Color.White);
                    spriteBatch.DrawString(font, insideCurrent.Name, ToVector2(insideCurrent.Position) + Information.CUSTOMER_NAME_OFFSET, Color.Purple);
                    insideCurrent = insideCurrent.GetNext();
                }
                insideCurrent = null;
            }

            // Draw customers that are being served
            for (int i = 0; i < cashiers.Length; i++)
            {
                if (cashiers[i].IsOccupied && cashiers[i].ContainsCustomer())
                {
                    spriteBatch.Draw(customerTexture, cashiers[i].CurrentCustomer.Position, Color.White);
                    spriteBatch.DrawString(font, cashiers[i].CurrentCustomer.Name, ToVector2(cashiers[i].CurrentCustomer.Position) + Information.CUSTOMER_NAME_OFFSET, Color.Purple);
                    spriteBatch.DrawString(font, Math.Round(cashiers[i].CurrentProcessTime,2).ToString(), ToVector2(cashiers[i].CurrentCustomer.Position) + Information.CUSTOMER_ORDER_TIME_OFFSET, Color.Green);
                }
            }

            //Draw exiting customers while they are on the screen
            foreach (Customer customer in processedCustomers)
            {
                if (customer.Position.Y < SCREEN_HEIGHT)
                {
                    spriteBatch.Draw(customerTexture, customer.Position, Color.White);
                }
            }

            //Draw cashiers
            for (int i = 0; i < cashiers.Length; i++)
            {
                spriteBatch.Draw(cashierTexture, cashierRec[i], Color.White);
            }

            //Draw UI
            spriteBatch.DrawString(font, "Time left: " + Convert.ToString(Math.Round(Information.SIMULATION_TOTAL_TIME - currentSimulationTime, 1)), Information.SIMULATION_TIME_POSITION, Color.Black);
            if (processedCustomers.Count > 0)
            {
                spriteBatch.DrawString(font, "Total  :   " + processedCustomers.Count, Information.TOTAL_CUSTOMERS_POSITION, Color.Black);
                spriteBatch.DrawString(font, "Average:   " + Math.Round(averageWaitTime, 1), Information.AVERAGE_WAIT_TIME_POSITION, Color.Black);
                spriteBatch.DrawString(font, "Max:       " + Math.Round(processedCustomers[0].TimeInQueue, 1), Information.MAX_WAIT_TIME_POSITION, Color.Black);
                spriteBatch.DrawString(font, "Min:       " + Math.Round(processedCustomers[processedCustomers.Count - 1].TimeInQueue, 1), Information.MIN_WAIT_TIME_POSITION, Color.Black);
            }
            for (int i = 0; i < processedCustomers.Count; i++)
            {
                if (i == 5)
                {
                    break;
                }
                spriteBatch.DrawString(font, i + 1 + "st:   " + Math.Round(processedCustomers[i].TimeInQueue, 1), Information.TOP_WAIT_TIME_POSITION[i], Color.Black);
            }

            //Draw end text
            if (currentSimulationTime >= Information.SIMULATION_TOTAL_TIME)
            {
                spriteBatch.DrawString(font, "SIMULATION FINISHED", Information.END_TEXT_POSITION, Color.Red);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                     OTHER                           ////////////////
        //////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts a recatngle to a vector2
        /// </summary>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public Vector2 ToVector2(Rectangle rectangle)
        {
            return new Vector2(rectangle.X, rectangle.Y);
        }

        /// <summary>
        /// Sorts a list of customers in a descening order
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Customer> MergeSort(List<Customer> list)
        {
            if (list.Count <= 1)
            {
                return list;
            }

            int mid = list.Count / 2;

            List<Customer> left = new List<Customer>();
            List<Customer> right = new List<Customer>();

            for (int i = 0; i < mid; i++)
            {
                left.Add(list[i]);
            }

            for (int i = mid; i < list.Count; i++)
            {
                right.Add(list[i]);
            }

            return Merge(MergeSort(left), MergeSort(right));
        }

        /// <summary>
        /// Sorts and merges 2 lists of customers togeter
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private List<Customer> Merge(List<Customer> left, List<Customer> right)
        {
            List<Customer> result = new List<Customer>();

            while (left.Count > 0 && right.Count > 0)
            {
                if (left[0].TimeInQueue <= right[0].TimeInQueue)
                {
                    result.Add(right[0]);
                    right.RemoveAt(0);
                }
                else
                {
                    result.Add(left[0]);
                    left.RemoveAt(0);
                }
            }

            for (int i = 0; i < left.Count; i++)
            {
                result.Add(left[i]);
            }

            for (int i = 0; i < right.Count; i++)
            {
                result.Add(right[i]);
            }

            return result;
        }
    }
}
