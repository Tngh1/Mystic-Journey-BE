using DAL.Data;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
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
            var quests = await _context.Quests
                .Include(q => q.RewardItem)
                .Include(q => q.RewardItems)
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)
                .Where(q => q.IsActive)
                // RewardItems × RewardSkills trong cùng một query nhân số hàng trả về.
                // Vẫn giữ tracking vì đoạn tự-sửa dữ liệu bên dưới có thể SaveChanges.
                .AsSplitQuery()
                .ToListAsync();

            bool modified = false;
            foreach (var q in quests)
            {
                if (q.Title != null && q.Title.Contains("Where Are We", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.ObjectiveTarget != "Elder Rowan" || q.QuestGiverName != "Elder Rowan")
                    {
                        q.ObjectiveTarget = "Elder Rowan";
                        q.QuestGiverName = "Elder Rowan";
                        modified = true;
                    }
                }
                else if (q.Title != null && q.Title.Contains("Work for Food", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.QuestGiverName != "Fa") { q.QuestGiverName = "Fa"; modified = true; }
                }
                else if (q.Title != null && q.Title.Contains("Delivery to the City", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.QuestGiverName != "Fa") { q.QuestGiverName = "Fa"; modified = true; }
                }
                else if (q.Title != null && q.Title.Contains("The Ruined City", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.QuestGiverName != "Tristan") { q.QuestGiverName = "Tristan"; modified = true; }
                }
                else if (q.Title != null && q.Title.Contains("Silver Knight", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.QuestGiverName != "Arthur") { q.QuestGiverName = "Arthur"; modified = true; }
                }
                else if (q.Title != null && q.Title.Contains("Defeat the Evil Monsters", StringComparison.OrdinalIgnoreCase))
                {
                    if (q.QuestGiverName != "Arthur") { q.QuestGiverName = "Arthur"; modified = true; }
                }
            }

            if (modified)
            {
                await _context.SaveChangesAsync();
            }

            return quests;
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
                .Where(n => n.Name == normalizedName && (n.MapName == mapName || (mapName.StartsWith("Autumn") && n.MapName.StartsWith("Autumn"))))
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
                if (normalizedMapName.Equals("AutumnTown", StringComparison.OrdinalIgnoreCase) ||
                    normalizedMapName.Equals("AutumnPumpkin", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n => n.MapName == "AutumnTown" || n.MapName == "AutumnPumpkin");
                }
                else
                {
                    query = query.Where(n => n.MapName == normalizedMapName);
                }
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
            // Đếm trên query chưa Include: COUNT không cần join sang 4 bảng reward.
            var filtered = _context.Quests.AsNoTracking();

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(x => x.Title.Contains(search));
            }
            if (!string.IsNullOrEmpty(type))
            {
                filtered = filtered.Where(x => x.Type == type);
            }
            if (isActive.HasValue)
            {
                filtered = filtered.Where(x => x.IsActive == isActive.Value);
            }
            if (!string.IsNullOrEmpty(mapName))
            {
                if (mapName.Equals("AutumnTown", StringComparison.OrdinalIgnoreCase) ||
                    mapName.Equals("AutumnPumpkin", StringComparison.OrdinalIgnoreCase))
                {
                    filtered = filtered.Where(x => x.MapName == "AutumnTown" || x.MapName == "AutumnPumpkin");
                }
                else
                {
                    filtered = filtered.Where(x => x.MapName == mapName);
                }
            }

            int totalCount = await filtered.CountAsync();

            var query = filtered
                .Include(q => q.RewardItem)
                .Include(q => q.RewardItems)
                    .ThenInclude(r => r.Item)
                .Include(q => q.RewardSkills)
                    .ThenInclude(r => r.Skill)
                .Include(q => q.RewardSkill)
                // RewardItems × RewardSkills nhân số hàng trả về nếu gộp một query.
                .AsSplitQuery();

            bool desc = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            IQueryable<Quest> ordered = (sortBy?.ToLowerInvariant()) switch
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

            var items = await ordered.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (totalCount, items);
        }
    }
}
