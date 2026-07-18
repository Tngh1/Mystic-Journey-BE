using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DAL.Data;

class Program {
    static void Main() {
        var options = new DbContextOptionsBuilder<MysticJourneyDbContext>()
            .UseNpgsql("Host=localhost;Database=MysticJourney;Username=postgres;Password=admin")
            .Options;
        using var ctx = new MysticJourneyDbContext(options);
        var quests = ctx.Quests.OrderBy(q => q.QuestId).ToList();
        foreach (var q in quests) {
            Console.WriteLine($"[{q.QuestId}] {q.Name} | Type={q.Type} | Map={q.MapName} | Next={q.NextQuestId} | Active={q.IsActive}");
        }
    }
}
