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
        public string Opponent { get; set; }
        public Protection FoundItem { get; set; }
        public Food Weapon { get; set; }
        public List<Item> Inventory { get; set;}
        public double Health { get; set;}
        public Protection Clothes { get; set; }
        public Dictionary<string, Food> FoodMap { get; set; }


        public UserProfile() {
            this.Inventory = new List<Item>();
            this.FoodMap = new Dictionary<string, Food>();
        }

        public List<Item> ListFood(){
            return Inventory.Where(x => x.Throwable == true).ToList<Item>();
        }

        public List<Item> ListProtection(){
            return Inventory.Where(x => x.Throwable == false).ToList<Item>();
        }

        public void ChangeClothes(Protection newClothes){
            if(newClothes != null){
                if (Clothes != null){
                    int index = Inventory.FindIndex(item => item.Name == Clothes.Name);
                    Inventory.RemoveAt(index);
                }
                Clothes = newClothes;
                Inventory.Add(newClothes);
            }
        }

        public double ThrowFood(Food food){
            double damage = food.Attack(0);

            if(!food.hasAmmo()){
                int index = Inventory.FindIndex(item => item.Name == food.Name);
                Inventory.RemoveAt(index);
                FoodMap.Remove(food.Name);
            }
            
            return damage;
        }

        public double GetHit(double damage){
            
            double damageDone = (Clothes == null) ? damage : damage - Clothes.TakeDamage(damage, 0);
            
            if (Clothes != null){
                if(Clothes.isBroken()){
                    Clothes = null;
                }
            }

            Health -= damageDone;

            return damageDone;
        }

        public void addFood(Food food) {
            if(FoodMap.ContainsKey(food.Name)) {
                FoodMap[food.Name].Ammo += food.Ammo;
            } else {
                Inventory.Add(food);
                FoodMap.Add(food.Name, food);
            }
        }
    }
}
