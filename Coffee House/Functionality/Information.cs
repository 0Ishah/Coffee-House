using Microsoft.Xna.Framework;

namespace Coffee_House.Functionality
{
    static class Information
    {
        public const int MAX_PEOPLE_INSIDE = 20;

        public static readonly Vector2 CUSTOMER_SPAWN_POS = new Vector2(500, 500);//TODO: Move outside of viewport

        public static readonly Vector2 OUTSIDE_LINE_START = new Vector2(180, 420);

        public static readonly Vector2[] INSIDE_LINE_TRAVEL_POINTS = new Vector2[]
        {
            new Vector2(130,420),// First point outside of the shop
            new Vector2(130,320),// Inside queue point
            new Vector2(730,320),// Inside queue point
            new Vector2(730,250),// Inside queue point
            new Vector2(330, 250)// Front of the queue
        };

        public static readonly Vector2[] CUSTOMER_SERVING_POSITIONS = new Vector2[]
        {
            new Vector2(200, 100),
            new Vector2(400, 100),
            new Vector2(600, 100)
        };

        public static readonly Vector2 EXIT_POSITION_INSIDE = new Vector2(100, 100);
        public static readonly Vector2 EXIT_POSITION_OUTSIDE = new Vector2(100, 500);

        public const int SPACE_BETWEEN_CUSTOMERS = 5;

        public const int CUSTOMER_MOVE_SPEED = 1;

        public const int POSSIBLE_FOODS = 3;
        public const int POSSIBLE_DISHES = 3;

        public const int COFFEE = 0;
        public const int FOOD = 1;
        public const int COMBO = 2;

        public const double CUSTOMER_SPAWN_INTERVAL = 3;
        public const double COFFEE_SERVE_TIME = 12;
        public const double FOOD_SERVE_TIME = 18;
        public const double COMBO_SERVE_TIME = 30;

        public static readonly string[] COFFEE_TYPES = new string[] { "Coffee", "Coffee", "Coffee" }; //TODO: Add variations
        public static readonly string[] FOOD_TYPES = new string[] { "Food", "Food", "Food" };
        public static readonly string[] COMBO_TYPES = new string[] { "Combo", "Combo", "Combo" };
    }
}
