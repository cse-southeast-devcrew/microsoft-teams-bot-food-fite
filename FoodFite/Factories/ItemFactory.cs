namespace FoodFite.Factories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using FoodFite.Models;
    public class ItemFactory
    {

        private static List<Item> foodList = 
            new List<Item>{JelloFactory(), BananaFactory(), GrapeFactory(), PizzaFactory()};

        private static List<Item> defenseGearList = 
            new List<Item>{TrashCanLidFactory(), WhiteTeeShirtFactory(), TrayFactory(), RaincoatFactory()};

        public static Item JelloFactory() {
            return new Food("Jello",70,0,100,10);
        }

        public static Item BananaFactory() {
            return new Food("Banana",40,30,70,10);
        }

        public static Item GrapeFactory() {
            return new Food("Grape",20,60,40,10);
        }

        public static Item PizzaFactory() {
            return new Food("Pizza",30,50,10,2);
        }

        public static Item RandomFoodFactory() {
            var random = new Random();
            int index = random.Next(foodList.Count);
            return foodList[index];
        }

        public static Item TrashCanLidFactory() {
            return new Protection("Trash Can Lid", 10,10,10);
        }
        public static Item WhiteTeeShirtFactory() {
            return new Protection("White Tee Shirt", 10,10,10);
        }
        public static Item TrayFactory() {
            return new Protection("Tray", 20,20,10);
        }
        public static Item RaincoatFactory() {
            return new Protection("Raincoat", 35,35,10);
        }
        public static Item RandomDefenseGearFactory() {
            var random = new Random();
            int index = random.Next(defenseGearList.Count);
            return defenseGearList[index];
        }
    }
}