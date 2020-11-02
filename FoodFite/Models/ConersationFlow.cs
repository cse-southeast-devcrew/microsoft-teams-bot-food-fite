namespace FoodFite.Models
{    
    public class ConversationFlow
    {
        // Identifies the last question asked.
        public enum Question
        {
            Name,
            Opponent,
            Weapon,
            None, // Our last action did not involve a question.
        }

        // The last question asked.
        public Question LastQuestionAsked { get; set; } = Question.None;
    }
}