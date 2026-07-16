namespace DAL.Models
{
    public class QuestRewardSkill
    {
        public int QuestRewardSkillId { get; set; }

        public int QuestId { get; set; }
        public Quest? Quest { get; set; }

        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
    }
}