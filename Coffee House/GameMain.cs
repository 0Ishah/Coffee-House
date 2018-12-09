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
        //System elements
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Random rng = new Random();


        private const int SCREEN_WIDTH = 800;
        private const int SCREEN_HEIGHT = 480;

        //Graphics design elements
        private Rectangle shopRec;
        private Rectangle[] casheerRec = new Rectangle[3];

        private const int CUSTOMER_REC_WIDTH = 40;
        private const int CUSTOMER_REC_HEIGHT = 40;

        private List<Rectangle> customersRec = new List<Rectangle>();

        private Texture2D shopTexture;
        private Texture2D customerTexture;

        //Simulation related elements
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

            //Initialize design rectangles
            shopRec = new Rectangle(0, 0, 800, 480);//TODO: Change magic numbers

            //Load design textures
            shopTexture = Content.Load<Texture2D>("Sprites/shop");
            customerTexture = Content.Load<Texture2D>("Sprites/customer");
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

            //Count all of the timers
            customerSpawnCounter += gameTime.ElapsedGameTime.TotalSeconds;
            simulationUpdateCounter += gameTime.ElapsedGameTime.TotalSeconds;
            currentSimulationTime += gameTime.ElapsedGameTime.TotalSeconds;

            //Spawn a new customer and place into outside queue every 3 seconds
            if (customerSpawnCounter >= Information.CUSTOMER_SPAWN_INTERVAL)
            {
                int randomType = rng.Next(0, Information.POSSIBLE_FOODS);
                int randomDish = rng.Next(0, Information.POSSIBLE_DISHES);
                switch (randomType)
                {
                    //Coffee
                    case 0:
                        numCurrentCoffee++;
                        outsideQueue.Enqueue(new Customer(randomType, Information.COFFEE_TYPES[randomDish] + " " + numCurrentCoffee, currentSimulationTime));
                        break;
                    //Foods
                    case 1:
                        numCurrentFood++;
                        outsideQueue.Enqueue(new Customer(randomType, Information.FOOD_TYPES[randomDish] + " " + numCurrentFood, currentSimulationTime));
                        break;
                    //Combos
                    case 2:
                        numCurrentCombo++;
                        outsideQueue.Enqueue(new Customer(randomType, Information.COMBO_TYPES[randomDish] + " " + numCurrentCombo, currentSimulationTime));
                        break;
                }

                Console.WriteLine("Added"); //TODO: Remove
                Console.WriteLine(outsideQueue.Count);
                customerSpawnCounter = 0;
            }

            //Move customers in the outside line
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

            //Move customers into the inside queue if there are less then 20 peopel inside
            if (insideQueue.Count < Information.MAX_PEOPLE_INSIDE && !outsideQueue.IsEmpty() && outsideQueue.Peek().IsAtDestination() && 2 == 1) //TODO: 2=1
            {
                insideQueue.Enqueue(outsideQueue.Dequeue());
            }

            //Move the customers to the nest avalible casheer
            if (!insideQueue.IsEmpty() && insideQueue.Peek().IsAtDestination())
            {
                for (int i = 0; i < casheers.Length; i++)
                {
                    if (!casheers[i].IsOccupied && casheers[i].ContainsCustomer())
                    {
                        //If the serving process was finished and new customer came
                        if (casheers[i].ContainsCustomer())
                        {
                            //Add the processed custoemr to the list and calculate its time in queue
                            casheers[i].CurrentCustomer.TimeInQueue = Convert.ToInt32(currentSimulationTime - casheers[i].CurrentCustomer.CreationTime);
                            //casheers[i].CurrentCustomer.IsWaiting = false; 
                            //TODO: change this
                            processedCustomers.Add(casheers[i].CurrentCustomer);
                            casheers[i].StartProcessing(insideQueue.Dequeue());
                        }
                        //Else if this is the first customer of the day
                        else if (!casheers[i].ContainsCustomer())
                        {
                            casheers[i].StartProcessing(insideQueue.Dequeue());
                        }
                        Console.WriteLine("Processing started");
                    }
                }
            }

            //Update all fo the casheers
            if (simulationUpdateCounter >= SIMULATION_UPDATE_TIME)
            {
                foreach (Casheer casheer in casheers)
                {
                    casheer.Process();
                    if (!casheer.IsOccupied && casheer.ContainsCustomer())
                    {
                        processedCustomers.Add(casheer.CurrentCustomer);
                        casheer.RemoveCurrentCustomer();
                    }
                }
                simulationUpdateCounter = 0;
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

            if (!outsideQueue.IsEmpty())
            {
                outsideCurrent = outsideQueue.Peek();
                for (int i = 0; i < outsideQueue.Count; i++)
                {
                    spriteBatch.Draw(customerTexture, new Rectangle((int)outsideCurrent.Position.X, (int)outsideCurrent.Position.Y, 40, 40), Color.White); //TODO: Magic mumbers and ugly
                    outsideCurrent = outsideCurrent.GetNext();
                }
                outsideCurrent = null;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////
        /////////////                     OTHER                           ////////////////
        //////////////////////////////////////////////////////////////////////////////////
    }
}
