namespace FoodFite.Models
{
    using System;
    public class Food : Item
    {
        public Food(string name, double damage, int minAcc, int variance, int ammo) : base(name, true){
            this.Damage = damage;
            this.MinAccuracy = minAcc;
            this.Variance = variance;
            this.Ammo = ammo;
        }
        public double Damage {get; set;}
        public int MinAccuracy {get; set;}
        public int Variance{get;set;}
        public int Ammo{get;set;}

        public double Attack(int accuracyMod){
            Random r = new Random();
            int accuracy = MinAccuracy + accuracyMod;
            Ammo--;
            return  ((double)(r.Next(accuracy, accuracy+Variance))/100.0) * Damage;            
        }

        public bool hasAmmo(){
            return Ammo > 0;
        }
    }
}