namespace FoodFite.Factories
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema;
    using FoodFite.Models;
    public class ItemFactory
    {

        public static Item JelloFactory() {
            return new Food("Jello",60,0,100,2);
        }

        public static Item BananaFactory() {
            return new Food("Banana",40,30,70,5);
        }

        public static Item GrapeFactory() {
            return new Food("Grape",20,60,40,10);
        }

        public static Item TrashCanLidFactory() {
            return new Protection("Trash Can Lid", 10,10,10);
        }
    }
}