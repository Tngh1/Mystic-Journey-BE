using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class QuestRepository : IQuestRepository
    {
        private readonly MysticJourneyDbContext _context;

        public QuestRepository(MysticJourneyDbContext context)
        {
            _context = context;
        }

        public async Task<Quest?> GetQuestById(int id)
        {
            return await _context.Quests
                .FirstOrDefaultAsync(q => q.QuestId == id);
        }

        public async Task<Quest?> GetByIdWithReward(int id)
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Include(q => q.RewardItems)
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)
                .FirstOrDefaultAsync(q => q.QuestId == id);
        }

        public async Task<List<Quest>> GetActiveQuests()
        {
            return await _context.Quests
                .Include(q => q.RewardItem)
                .Include(q => q.RewardItems)
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)
                .Where(q => q.IsActive)
                .ToListAsync();
        }

        public async Task<Quest> AddQuest(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
            return quest;
        }
        public async Task<Quest> UpdateQuest(Quest quest)
        {
            _context.Quests.Update(quest);
            await _context.SaveChangesAsync();
            return quest;
        }

        public async Task<NPCDialogue?> GetQuestDialogueByQuestId(int questId)
        {
            return await _context.NPCDialogues
                .Include(d => d.NPC)
                .Include(d => d.LinkedQuest)
                .Where(d => d.LinkedQuestId == questId && d.ResponseType == "Quest")
                .OrderByDescending(d => d.IsActive)
                .ThenBy(d => d.DisplayOrder)
                .ThenBy(d => d.NPCDialogueId)
                .FirstOrDefaultAsync();
        }

        public async Task<NPC?> GetNpcByNameAndMap(string? npcName, string mapName)
        {
            if (string.IsNullOrWhiteSpace(npcName))
                return null;

            var normalizedName = npcName.Trim();
            return await _context.NPCs
                .Where(n => n.Name == normalizedName && n.MapName == mapName)
                .OrderByDescending(n => n.IsActive)
                .FirstOrDefaultAsync()
                ?? await _context.NPCs
                    .Where(n => n.Name == normalizedName)
                    .OrderByDescending(n => n.IsActive)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<NPC>> GetQuestNpcOptions(string? mapName)
        {
            var query = _context.NPCs
                .AsNoTracking()
                .Where(n => n.IsActive);

            if (!string.IsNullOrWhiteSpace(mapName))
            {
                var normalizedMapName = mapName.Trim();
                query = query.Where(n => n.MapName == normalizedMapName);
            }

            return await query
                .OrderBy(n => n.MapName)
                .ThenBy(n => n.Name)
                .ThenBy(n => n.NPCId)
                .Take(200)
                .ToListAsync();
        }
        public void AddQuestDialogue(NPCDialogue dialogue)
        {
            _context.NPCDialogues.Add(dialogue);
        }

        public async Task<(int TotalCount, List<Quest> Items)> GetQuestsPaged(int page, int pageSize, string? search, string? type, bool? isActive, string? mapName, string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Quests
                .Include(q => q.RewardItem)
                .Include(q => q.RewardItems)
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }
            if (!string.IsNullOrEmpty(mapName))
            {
                query = query.Where(x => x.MapName == mapName);
            }

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "title" => desc ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "type" => desc ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "requiredlevel" => desc ? query.OrderByDescending(x => x.RequiredLevel) : query.OrderBy(x => x.RequiredLevel),
                "rewardgold" => desc ? query.OrderByDescending(x => x.RewardGold) : query.OrderBy(x => x.RewardGold),
                "rewardexp" => desc ? query.OrderByDescending(x => x.RewardExperience) : query.OrderBy(x => x.RewardExperience),
                "mapname" => desc ? query.OrderByDescending(x => x.MapName) : query.OrderBy(x => x.MapName),
                "isactive" => desc ? query.OrderByDescending(x => x.IsActive) : query.OrderBy(x => x.IsActive),
                _ => desc ? query.OrderByDescending(x => x.QuestId) : query.OrderBy(x => x.QuestId),
            };

            int totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
