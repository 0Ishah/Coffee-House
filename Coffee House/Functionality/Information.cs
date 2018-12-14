using Microsoft.Xna.Framework;

namespace Coffee_House.Functionality
{
    static class Information
    {
        public const int SIMULATION_TOTAL_TIME = 300;

        public const int MAX_PEOPLE_INSIDE = 20;

        public static readonly Vector2 CUSTOMER_SPAWN_POS = new Vector2(500, 500);

        public static readonly Vector2 OUTSIDE_LINE_START = new Vector2(180, 435);

        public static readonly Vector2[] INSIDE_LINE_TRAVEL_POINTS = new Vector2[]
        {
            new Vector2(140,435),// First point outside of the shop
            new Vector2(190,295),// Inside queue point
            new Vector2(730,295),// Inside queue point
            new Vector2(730,235),// Inside queue point
            new Vector2(330, 235)// Front of the queue
        };

        public static readonly Vector2[] CUSTOMER_SERVING_POSITIONS = new Vector2[]
        {
            new Vector2(160, 105),
            new Vector2(370, 105),
            new Vector2(580, 105)
        };

        public static readonly Vector2 EXIT_POSITION_INSIDE = new Vector2(100, 250);
        public static readonly Vector2 EXIT_POSITION_OUTSIDE = new Vector2(100, 500);

        public static readonly Vector2 CUSTOMER_NAME_OFFSET = new Vector2(0, 30);
        public static readonly Vector2 CUSTOMER_ORDER_TIME_OFFSET = new Vector2(0, 38);

        public static readonly Vector2 SIMULATION_TIME_POSITION = new Vector2(10,150);
        public static readonly Vector2 TOTAL_CUSTOMERS_POSITION = new Vector2(10, 160);
        public static readonly Vector2 AVERAGE_WAIT_TIME_POSITION = new Vector2(10,170);
        public static readonly Vector2 MAX_WAIT_TIME_POSITION = new Vector2(10,180);
        public static readonly Vector2 MIN_WAIT_TIME_POSITION = new Vector2(10,190);
        public static readonly Vector2[] TOP_WAIT_TIME_POSITION = new Vector2[]
        {
            new Vector2(10,200),
            new Vector2(10,210),
            new Vector2(10,220),
            new Vector2(10,230),
            new Vector2(10,240)
        };
        public static readonly Vector2 END_TEXT_POSITION = new Vector2(10, 260);

        public const int SPACE_BETWEEN_CUSTOMERS = 20;

        public const int CUSTOMER_MOVE_SPEED = 1;

        public const int POSSIBLE_FOODS = 3;
        public const int POSSIBLE_DISHES = 3;

        public const int COFFEE = 0;
        public const int FOOD = 1;
        public const int COMBO = 2;

        public const double CUSTOMER_SPAWN_INTERVAL = 3;
        public const double COFFEE_SERVE_TIME = 12; //12
        public const double FOOD_SERVE_TIME = 18; //18
        public const double COMBO_SERVE_TIME = 30;

        public static readonly string[] COFFEE_TYPES = new string[] { "Latte", "Espresso", "Cappuccino" };
        public static readonly string[] FOOD_TYPES = new string[] { "Sandwich", "Cake", "Croissant" };
        public static readonly string[] COMBO_TYPES = new string[] { "Breakfast", "Lunch", "Dinner" };
    }
}
