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
            double damageAbsorbed = Math.Min(((double)(r.Next(accuracy, accuracy+Variance))/100.0) * damage, Health);
            Health = Health - damageAbsorbed;
            return damageAbsorbed;
        }

        public bool isBroken(){
            return Health <= 0;
        }

    }
}