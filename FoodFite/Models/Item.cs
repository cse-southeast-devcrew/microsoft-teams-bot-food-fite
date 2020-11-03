namespace FoodFite.Models
{
    public class Item
    {
        public Item(string name, bool throwable){
            this.Name = name;
            this.Throwable = throwable;
        }

        public string Name {get;set;}
        public bool Throwable {get;set;}
    }
}