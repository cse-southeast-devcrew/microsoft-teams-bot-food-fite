// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;
using System.Collections.Generic;
namespace FoodFite.Models
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {

        public UserProfile() {
            this.FoodInventory = new List<Food>();
            this.FoodMap = new Dictionary<string, Food>();
        }

        public string Name { get; set; }
        
        public string Opponent { get; set; }

        public string Weapon { get; set; }

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
