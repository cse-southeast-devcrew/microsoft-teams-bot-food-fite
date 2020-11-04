namespace FoodFite.Factories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using FoodFite.Models;
    public class ItemFactory
    {
        public interface IItemFactory {
            Item create();
        }
        
        public class JelloFactory : IItemFactory {
            public Item create() {
                return new Food("Jello",60,0,100,2);
            }
        }

        public class BananaFactory : IItemFactory {
            public Item create() {
                return new Food("Banana",40,30,70,5);
            }
        }

        public class PizzaFactory : IItemFactory {
            public Item create() {
                return new Food("Pizza",30,50,60,2);
            }
        }

        public class GrapeFactory : IItemFactory {
            public Item create() {
                return new Food("Grape",20,60,60,10);
            }
        }

        public class TrashCanLidFactory : IItemFactory {
            public Item create() {
                return new Protection("Trash Can Lid", 20,0,50);
            }
        }

        public class RaincoatFactory : IItemFactory {
            public Item create() {
                return new Protection("Raincoat", 35,30,10);
            }
        }

        public class WhiteTeeShirtFactory : IItemFactory {
            public Item create() {
                return new Protection("White Tee Shirt", 10,5,20);
            }
        }

        public class TrayFactory : IItemFactory {
            public Item create() {
                return new Protection("Tray", 20,0,30);
            }
        }
        
        private static List<IItemFactory> foodList = 
            new List<IItemFactory>{new JelloFactory(), new BananaFactory(), new GrapeFactory(), new PizzaFactory()};

        private static List<IItemFactory> defenseGearList = 
            new List<IItemFactory>{new TrashCanLidFactory(), new WhiteTeeShirtFactory(), new TrayFactory(), new RaincoatFactory()};


        public static Item RandomFoodFactory() {
            var random = new Random();
            int index = random.Next(foodList.Count);
            return foodList[index].create();
        }

        public static Item RandomItem(){
            var r = new Random();
            return (r.Next(5) == 1) ? RandomDefenseGearFactory() : RandomFoodFactory();
        }

        public static Item RandomDefenseGearFactory() {
            var random = new Random();
            int index = random.Next(defenseGearList.Count);
            return defenseGearList[index].create();
        }
    }
}