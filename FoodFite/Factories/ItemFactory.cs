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
            return new Food("Jello",60,0,100,2);
        }

        public static Item BananaFactory() {
            return new Food("Banana",40,30,70,5);
        }

        public static Item GrapeFactory() {
            return new Food("Grape",20,60,60,10);
        }

        public static Item PizzaFactory() {
            return new Food("Pizza",30,50,60,2);
        }

        public static Item RandomFoodFactory() {
            var random = new Random();
            int index = random.Next(foodList.Count);
            return foodList[index];
        }

        public static Item TrashCanLidFactory() {
            return new Protection("Trash Can Lid", 20,0,50);
        }
        public static Item WhiteTeeShirtFactory() {
            return new Protection("White Tee Shirt", 10,5,20);
        }
        public static Item TrayFactory() {
            return new Protection("Tray", 20,0,30);
        }
        public static Item RaincoatFactory() {
            return new Protection("Raincoat", 35,30,10);
        }

        public static Item RandomItem(){
            var r = new Random();
            return (r.Next(5) == 1) ? RandomDefenseGearFactory() : RandomFoodFactory();
        }

        public static Item RandomDefenseGearFactory() {
            var random = new Random();
            int index = random.Next(defenseGearList.Count);
            return defenseGearList[index];
        }
    }
}