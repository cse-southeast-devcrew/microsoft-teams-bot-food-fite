namespace FoodFite.Models
{
    using System;
    public class Protection : Item
    {
        public Protection(string name, int health, int minProc, int variance): base(name, false) {
            this.Health = health;
            this.MinProtection = minProc;
            this.Variance = variance;
        }
        public double Health {get;set;}
        public int MinProtection {get; set;}
        public int Variance {get; set;}

        public double TakeDamage(double damage, int protectionMod){

            Random r = new Random();
            int accuracy = MinProtection + protectionMod;
            double damageAbsorbed = ((double)(r.Next(accuracy, accuracy+Variance))/100.0) * damage;
            double damageTaken = damage - damageAbsorbed;
            Health = Health - damageTaken;
            // if health is above 0 then return the full damage
            // if health is less than 0. then return damage + Health
            return (Health < 0) ? damage + Health : damage;    
        }

        public bool isBroken(){
            return Health <= 0;
        }

    }
}