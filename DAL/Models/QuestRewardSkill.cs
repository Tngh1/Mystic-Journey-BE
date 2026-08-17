namespace DAL.Models
{
    // Initializes a new default instance of the QuestRewardSkill class.
    public class QuestRewardSkill
    {
        // Executes quest reward skill id operation.
        public int QuestRewardSkillId { get; set; }

        // Executes quest id operation.
        public int QuestId { get; set; }
        // Executes quest operation.
        public Quest? Quest { get; set; }

        // Executes skill id operation.
        public int SkillId { get; set; }
        // Executes skill operation.
        public Skill? Skill { get; set; }
    }
}
