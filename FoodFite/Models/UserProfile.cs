// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Linq;

namespace FoodFite.Models
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {
        public string Name { get; set; }
        public List<Item> Inventory { get; set;}
        public double Health { get; set;}
        public Protection Clothes { get; set; }


        public List<Item> ListFood(){
            return Inventory.Where(x => x.Throwable == true).ToList<Item>();
        }

        public List<Item> ListProtection(){
            List<Item> list = Inventory.Where(x => x.Throwable == false).ToList<Item>();
            list.Add(Clothes);
            return list;
        }

        public void ChangeClothes(Protection newClothes){
            Protection oldClothes = Clothes;
            Inventory.Remove(newClothes);
            Clothes = newClothes;
            if(oldClothes != null){
                Inventory.Add(oldClothes);
            }
        }

        public double ThrowFood(Food food, UserProfile target){
            double damage = food.Attack(0);

            if(!food.hasAmmo()){
                Inventory.Remove(food);
            }
            
            return target.GetHit(damage);
        }

        public double GetHit(double damage){
            
            double damageDone = (Clothes == null) ? damage : Clothes.TakeDamage(damage, 0);
            
            if(Clothes.isBroken()){
                Clothes = null;
            }

            Health -= damageDone;

            return damageDone;
        }

        public Dictionary<string, Food> FoodMap { get; set; }

        public List<Food> FoodInventory{get; set;}

        public Protection Protection{get; set;}

        public void addFood(Food food) {
            if(FoodMap.ContainsKey(food.Name)) {
                Food mergedFood = FoodMap[food.Name];
                mergedFood.Ammo = mergedFood.Ammo + food.Ammo;
            } else {
                FoodInventory.Add(food);
                FoodMap.Add(food.Name, food);
            }
        }


    }
}
