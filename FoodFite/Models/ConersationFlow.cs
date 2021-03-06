namespace FoodFite.Models
{    
    public class ConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            Opponent,
            Weapon,
            Action,
            ActionRouting,
            ChangeClothes,
            Back,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}