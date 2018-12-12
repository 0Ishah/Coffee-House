using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Animation2D;
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
        private const int SCREEN_HEIGHT = 680;

        // Graphics design elements
        private Rectangle shopRec;
        private Rectangle[] casheerRec = new Rectangle[3];

        private const int CUSTOMER_REC_WIDTH = 40;
        private const int CUSTOMER_REC_HEIGHT = 40;

        private List<Rectangle> customersRec = new List<Rectangle>();

        private Texture2D shopTexture;
        private Texture2D customerTexture;

        private SpriteFont font;

        // Simulation related elements
        private double currentSimulationTime;

        private const int SIMULATION_UPDATE_TIME = 1;
        private double simulationUpdateCounter;

        private int numCurrentCoffee;
        private int numCurrentFood;
        private int numCurrentCombo;

        private Casheer[] casheers = new Casheer[3] { new Casheer(), new Casheer(), new Casheer() };

        private CustomerQueue outsideQueue = new CustomerQueue();
        private CustomerQueue insideQueue = new CustomerQueue();

        private List<Customer> processedCustomers = new List<Customer>();

        private double customerSpawnCounter;
        private double[] customerServeCounter = new double[3];

        private Customer outsideCurrent;
        private Customer insideCurrent; //TODO: Find better place for vars

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
            shopRec = new Rectangle(0, 0, 800, 480);//TODO: Change magic numbers

            // Load design textures
            shopTexture = Content.Load<Texture2D>("Sprites/shop");
            customerTexture = Content.Load<Texture2D>("Sprites/customer");

            font = Content.Load<SpriteFont>("Fonts/myFont");//TODO: Change name
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

            // Move customers into the inside queue if there are less then 20 peopel inside
            if (insideQueue.Count < Information.MAX_PEOPLE_INSIDE && !outsideQueue.IsEmpty() && outsideQueue.Peek().IsAtDestination())
            {
                insideQueue.Enqueue(outsideQueue.Dequeue());
            }

            // Move customers in the outside line
            if (!outsideQueue.IsEmpty())
            {
                outsideCurrent = outsideQueue.Peek();
                // If the firest element is not ate the destiantion position
                if (!outsideCurrent.IsAtDestination())
                {
                    // Set the destination position to the start of the line
                    outsideCurrent.DestinationPosition = Information.OUTSIDE_LINE_START;
                    outsideCurrent.Move();
                }
                for (int i = 0; i < outsideQueue.Count - 1; i++)
                {
                    // Move all the customers except the first to their destanation behind the first
                    if (!outsideCurrent.GetNext().IsAtDestination())
                    {
                        outsideCurrent.GetNext().DestinationPosition = Information.OUTSIDE_LINE_START + new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + 40) * (i + 1), 0); //TODO: Magic numbers
                        outsideCurrent.GetNext().Move();
                    }
                    outsideCurrent = outsideCurrent.GetNext();
                }
                outsideCurrent = null;
            }

            // Move customer in the inside queue
            if (!insideQueue.IsEmpty())
            {   
                insideCurrent = insideQueue.Peek();
                for (int i = 0; i < insideQueue.Count; i++)
                {
                    switch (insideCurrent.TravelPoint)
                    {
                        case (0):
                            insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[0];
                            insideCurrent.Move();
                            if (insideCurrent.IsAtDestination())
                            {
                                insideCurrent.TravelPoint = 1;
                            }
                            break;
                        case (1):
                            insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[1];
                            insideCurrent.Move();
                            if (insideCurrent.IsAtDestination())
                            {
                                insideCurrent.TravelPoint = 2;
                            }
                            break;
                        case (2):
                            insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[2];
                            if (i > 10)
                            {
                                insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[2] - new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + 40) * (i - 10), 0);//TODO: Magic numbers
                            }
                            insideCurrent.Move();
                            if (insideCurrent.IsAtDestination() && i < 10)
                            {
                                insideCurrent.TravelPoint = 3;
                            }
                            break;
                        case (3):
                            insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[3];
                            insideCurrent.Move();
                            if (insideCurrent.IsAtDestination())
                            {
                                insideCurrent.TravelPoint = 4;
                            }
                            break;
                        case (4):
                            insideCurrent.DestinationPosition = Information.INSIDE_LINE_TRAVEL_POINTS[4] + new Vector2((Information.SPACE_BETWEEN_CUSTOMERS + 40) * i, 0);//TODO: Magic numbers
                            insideCurrent.Move();
                            if (insideCurrent.IsAtDestination())
                            {
                                insideCurrent.TravelPoint = 5;
                            }
                            break;
                    }
                    insideCurrent = insideCurrent.GetNext();
                }
            }
            
            // Move the customers to the nest avalible casheer
            if (!insideQueue.IsEmpty())//TOOD: Magic numbers
            {
                for (int i = 0; i < casheers.Length; i++)
                {
                    if (!casheers[i].IsOccupied && insideQueue.Peek().TravelPoint == 5)
                    {
                        casheers[i].StartProcessing(insideQueue.Dequeue());
                    }
                }
            }

            // Update all fo the casheers
            if (simulationUpdateCounter >= SIMULATION_UPDATE_TIME)
            {
                for (int i = 0; i < casheers.Length; i++)
                {
                    if (casheers[i].IsOccupied)
                    {
                        if (casheers[i].CurrentCustomer.TravelPoint == 6)//TODO: Magic numbers
                        {
                            casheers[i].Process();
                        }
                        if (!casheers[i].IsOccupied && casheers[i].ContainsCustomer())
                        {
                            processedCustomers.Add(casheers[i].CurrentCustomer);
                        }
                    }
                }
                simulationUpdateCounter = 0;
            }

            //Move customers to the casheer serving area graphically
            for (int i = 0; i < casheers.Length; i++)
            {
                if (casheers[i].IsOccupied && casheers[i].CurrentCustomer.TravelPoint == 5)//TODO: Magic numbers
                {
                    casheers[i].CurrentCustomer.DestinationPosition = Information.CUSTOMER_SERVING_POSITIONS[i];
                    casheers[i].CurrentCustomer.Move();
                    if (casheers[i].CurrentCustomer.IsAtDestination())
                    {
                        casheers[i].CurrentCustomer.TravelPoint = 6;
                    }
                }
            }

            //Move all of the customers outside of the coffee shop
            foreach (Customer customer in processedCustomers)
            {
                customer.DestinationPosition = new Vector2(0, 500); //TODO: Move to Information
                customer.Move();
            }

            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                     DRAW                            ////////////////
        //////////////////////////////////////////////////////////////////////////////////
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //CODE HERE
            spriteBatch.Begin();

            spriteBatch.Draw(shopTexture, shopRec, Color.White);

            // Draw outside queue
            if (!outsideQueue.IsEmpty())
            {
                outsideCurrent = outsideQueue.Peek();
                for (int i = 0; i < outsideQueue.Count; i++)
                {
                    spriteBatch.Draw(customerTexture, new Rectangle((int)outsideCurrent.Position.X, (int)outsideCurrent.Position.Y, 40, 40), Color.White); //TODO: Magic mumbers and ugly
                    spriteBatch.DrawString(font, outsideCurrent.Name, outsideCurrent.Position + new Vector2(0,40), Color.Purple); //TODO: Move new vector 2 to info as displacement var
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
                    spriteBatch.Draw(customerTexture, new Rectangle((int)insideCurrent.Position.X, (int)insideCurrent.Position.Y, 40, 40), Color.White); //TODO: Magic mumbers and ugly
                    spriteBatch.DrawString(font, insideCurrent.Name, insideCurrent.Position + new Vector2(0, 40), Color.Purple);
                    insideCurrent = insideCurrent.GetNext();
                }
                insideCurrent = null;
            }

            // Draw customers that are being served
            for (int i = 0; i < casheers.Length; i++)
            {
                if (casheers[i].IsOccupied && casheers[i].ContainsCustomer())
                {
                    spriteBatch.Draw(customerTexture, new Rectangle((int)casheers[i].CurrentCustomer.Position.X, (int)casheers[i].CurrentCustomer.Position.Y, 40, 40), Color.White);
                    spriteBatch.DrawString(font, casheers[i].CurrentCustomer.Name, casheers[i].CurrentCustomer.Position + new Vector2(0, 40), Color.Purple);
                    spriteBatch.DrawString(font, Math.Round(casheers[i].CurrentProcessTime,2).ToString(), casheers[i].CurrentCustomer.Position + new Vector2(0,48), Color.Red);
                }
            }

            //Draw exiting customers
            foreach (Customer customer in processedCustomers)
            {
                spriteBatch.Draw(customerTexture, new Rectangle((int)customer.Position.X, (int)customer.Position.Y, 40, 40), Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                     OTHER                           ////////////////
        //////////////////////////////////////////////////////////////////////////////////
    }
}
