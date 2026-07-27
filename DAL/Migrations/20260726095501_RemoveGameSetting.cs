using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGameSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameSettings");

            migrationBuilder.DropIndex(
                name: "IX_PlayerQuests_PlayerProfileId",
                table: "PlayerQuests");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 1,
                column: "Content",
                value: "Ah... a new face, and not one born of these woods. Welcome to the Elf Forest, traveler.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 2,
                column: "Content",
                value: "For a thousand years this forest kept itself in peace. Now something gathers in the dark beneath the roots.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 3,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "I am Elder Rowan, and I need your hands and your courage. Speak with me when you are ready to begin.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 4,
                column: "Content",
                value: "Before we stand against the darkness, we must be able to mend what it breaks.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 5,
                column: "Content",
                value: "By the old willow clearing grows a white flower that only opens where the air is still clean.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 6,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Go to the clearing and gather 3 White Flowers for me. Take care, even slimes wander there now.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 7,
                column: "Content",
                value: "Back already? Let me see your hands... ah, you found them.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 8,
                column: "Content",
                value: "Not a petal bruised. Crushed with spring water, these will close a wound in minutes.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 9,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "You have earned this. Take it, with an old elf's thanks.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 10,
                column: "Content",
                value: "A remedy keeps you alive. It does not keep you standing. For that you need a skill.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 11,
                column: "Content",
                value: "Every warrior in this world channels power through learned technique. Bare fists will not answer a demon.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 12,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Open your Skill Panel and equip your first combat skill. Do not step past the treeline without it.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 13,
                column: "Content",
                value: "Good. I can feel the power settled in you now. It must be tested before it is trusted.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 14,
                column: "Content",
                value: "The outskirts crawl with little slimes. They were harmless once, now they hunt in packs.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 15,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Go out and defeat 3 little slimes, then return and tell me what you felt out there.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 16,
                column: "Content",
                value: "You handled them cleanly. But the slimes are only spillage from something far worse.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 17,
                column: "Content",
                value: "Deep in the swamp lies a Demon. The water rots around it, and the corruption creeps closer each night.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 18,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Destroy the Swamp Demon and bring back the Swamp Seal Book. Everything rests on this.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 19,
                column: "Content",
                value: "Come closer, brave one. I am Lyra, not elf and not ghost. I am the spirit of the Origin Tree itself.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 20,
                column: "Content",
                value: "Look at my bark. The curse has reached my heartwood, and I am dying slowly, from the inside outward.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 21,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Only the 4 Seal Books can cleanse me. You hold one already, find the remaining three, and hurry!", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 22,
                column: "Content",
                value: "Heh... so you are the little errand-runner gathering up the Seal Books.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 23,
                column: "Content",
                value: "You carry them and do not even know what they are, or whose hand cursed that tree.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 24,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "The truth waits through this portal. Follow me, or stay and keep watering a dying tree.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 25,
                column: "Content",
                value: "Steady, traveler. That portal spat us both out here on the beach, and the cloaked one is long gone.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 26,
                column: "Content",
                value: "We are far from the forest now, with no coin between us and no way back that I can see.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 27,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Go and speak with Fa, the farmer just up the path. He always needs hands.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 28,
                column: "Content",
                value: "Elder Rowan sent you? Good timing, stranger. My back is not what it was.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 29,
                column: "Content",
                value: "The whole field came ripe at once, and the harvest cart leaves for the city at dusk.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 30,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Collect 8 Enchanted Pumpkins for me and I will see you fed tonight.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 31,
                column: "Content",
                value: "Eight, and not one bruised. You work like a farmhand born, not a wanderer.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 32,
                column: "Content",
                value: "Now the hard half of the job. These are owed at the city gate before nightfall.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 33,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Take them to the guard Tristan at the ruined city, and tell him Fa sent you.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 34,
                column: "Content",
                value: "Halt! Who goes... ah, pumpkins from Fa. Set them down, you may be the last delivery this gate sees.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 35,
                column: "Content",
                value: "Something is wrong inside. No bells, no market noise, no smoke from a single chimney since dawn.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 36,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Go in and look at the fallen with your own eyes. Then come back and tell me the truth of it.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 37,
                column: "Content",
                value: "All of them? Every soul in the city, cut down where they stood? Gods, I stood here and heard nothing.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 38,
                column: "Content",
                value: "No bandit crew does this in one night. Whatever walked in there was not a man with a sword.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 39,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Find Arthur and report what you saw. Go, before whatever did this moves on to the next town.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 40,
                column: "Content",
                value: "Lower your guard, I am no enemy. I am Arthur, once called the silver knight of this city.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 41,
                column: "Content",
                value: "I met the thing that emptied these streets. It broke something inside me and sealed my power away.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 42,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Clear my old training dungeon. Survive it, and I will give you everything I have left. Go!", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 43,
                column: "Content",
                value: "You walked out of that dungeon on your own two feet. Not many did, back when it was mine.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 44,
                column: "Content",
                value: "Then take what I can still give. My dark explosion technique, and this Silver Necklace off my own neck.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 45,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Cut down 10 of them in the Ruined City. Let the streets be quiet for once.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 46,
                column: "Content",
                value: "You came back quieter than you left. That is how I know the fighting took hold in you.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 47,
                column: "Content",
                value: "Then hear the rest of it. The monsters were never the cause. Something older nests above the ruins.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 48,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Finish what I could not. Climb to its nest and slay the dragon!", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 49,
                column: "Content",
                value: "The dragon is dead. I felt it go — the whole city breathed out at once. Thank you.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 50,
                column: "Content",
                value: "You want to know about the cloaked one. Yes. He passed through here before the dragon ever came.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 51,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "He went north, into the frozen lands. Follow him to the Frozen Mountains. I will hold this city.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 52,
                column: "Content",
                value: "A living stranger, walking in out of the snow. I am Roselyn Aurora, and what is left of this kingdom is mine to hold.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 53,
                column: "Content",
                value: "The codex passed over these fields and the cold turned wrong. My soldiers are gone. Only volunteers stand the walls now.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 54,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Clear 8 ice slimes from the Snow Fields. I would ask a knight, but I have none left to ask.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 55,
                column: "Content",
                value: "The fields are quiet tonight. My people walked to the grain stores without an escort for the first time in a month.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 56,
                column: "Content",
                value: "Now the harder trouble. Priest Zephyr keeps a rite burning at the peak — it is the only thing holding the cold back.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 57,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Take this Magic Flour to him at the mountain peak. If the rite goes out, we all freeze with it.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 58,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "The Queen's flour, and a courier still breathing. The rite can go on. You have bought this mountain another season.", "Reward" });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 59,
                column: "Content",
                value: "Stay a moment. The codex passed over this peak too, and what it touched did not simply die — it changed.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 60,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Climb the peak and slay all 5. Do it, and the Queen's borders hold one more winter.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 61,
                column: "Content",
                value: "Halt. Beyond this line is under ban, and I am Roland, the warden who keeps it.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 62,
                column: "Content",
                value: "Wait. That cold on you — dragon frost. You came down off the peak, not up from the road.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 63,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Walk it with me and map what waits inside. I will not send you where I do not go myself.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 64,
                column: "Content",
                value: "Now I know why my order was told to guard this place and never enter it. The codex did not begin in the world. It began here.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 65,
                column: "Content",
                value: "And it is not finished. One of the old Seal Books lies at the heart of the ban, still holding.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 66,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Break the stone guardian and take the Golem Seal Book. It is worth more in your hands than under my ban.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 67,
                column: "Content",
                value: "Back, stranger, keep your back to the rock! They come up out of the valley floor faster than I can cut them down.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 68,
                column: "Content",
                value: "This is no ordinary haunting. An ancient power is leaking somewhere near, and the dead rise faster than they fall.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 69,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Cut down 12 of them in the valley with me. Two blades may be enough where one was not.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 70,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "The animals are fleeing from the abandoned village of Tide-Knell. Look into it, and find the girl Natalie.", 5 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 71,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "You can see me. Nobody has seen me in a very long time. My name is Natalie, and this village is Tide-Knell.", 1 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 72,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please. Dig beside the old well and lift out the skull you find there. I am ready to be found.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 73,
                column: "Content",
                value: "(A weathered letter lies where Natalie once stood. It is her own hand, and it is a farewell.)");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 74,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Thank you for bringing my remains home. Please bury me under the ivy tree in my courtyard, where I used to sit.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "The ancient power leak was my doing, and I have paid for it here. Take this Mystic Key — it opens the castle gates on the deserted island.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 76,
                column: "Content",
                value: "An outsider, with a Mystic Key, standing on my island. The sea should have kept you. Yet here you are.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 77,
                column: "Content",
                value: "I am the last guard of this place. I know what you carry, and I know the forest you are trying to reach.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 78,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Collect 5 Ancient Leaves from the Northern Plateau. Bring them, and I will begin the rite of return.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 79,
                column: "Content",
                value: "The leaves are enough. The rite is ready. And yet I cannot light it — something below the castle is smothering it.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 80,
                column: "Content",
                value: "The UnderKing has woken. He held the last Seal Book in his hands long before you were born.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 81,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "End his reign. Defeat the UnderKing and take the fourth book from him.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 82,
                column: "Content",
                value: "It is done. The UnderKing has fallen, and all four Seal Books are in one pair of hands for the first time in an age.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 83,
                column: "Content",
                value: "You want the way home. I will give it, but understand what waits: the Origin Tree is nearly gone.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 84,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Then go. The portal to the Elf Forest is open. Save the tree, outsider.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 85,
                column: "Content",
                value: "You came back. Through the ruins, the snow, the ban, the sea — and you are carrying all four seals.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 86,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Bring the four books to me here, at the roots. Hurry.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 88,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Thank you, truly. The Origin Tree is saved. But this is not the end... To be continued.", 4 });

            migrationBuilder.InsertData(
                table: "NPCDialogues",
                columns: new[] { "NPCDialogueId", "Content", "DisplayOrder", "IsActive", "LinkedQuestId", "LinkedShopItemId", "NPCId", "ResponseType" },
                values: new object[,]
                {
                    { 89, "I cannot hold the rites while they circle the shrine — their shadows alone put out the candles.", 2, true, 20, null, 9, "None" },
                    { 90, "The tree has almost no strength left. Every leaf it drops, the curse takes a little more of the forest.", 2, true, 29, null, 2, "None" },
                    { 91, "The four seals are whole. I have opened the rite... but I cannot finish it.", 1, true, 30, null, 2, "None" },
                    { 92, "The seals answer only to the one who won them. It must be your hand, not mine.", 2, true, 30, null, 2, "None" },
                    { 93, "Step to the Origin Tree and set the four Seal Books upon it. Break the curse.", 4, true, 30, null, 2, "Quest" },
                    { 94, "The Origin Tree at our heart is sickening. Its leaves fall in high summer, and the animals no longer sleep here.", 3, true, 1, null, 1, "None" },
                    { 95, "Where those flowers still bloom, the curse has not yet reached. They are medicine and warning both.", 3, true, 2, null, 1, "None" },
                    { 96, "Three flowers, three doses. Keep one for yourself, out there you may be your only healer.", 3, true, 3, null, 1, "None" },
                    { 97, "Your body already holds the spark. What you lack is a shape to pour it into.", 3, true, 4, null, 1, "None" },
                    { 98, "They are the curse's smallest children. Where they spread, the soil dies behind them.", 3, true, 5, null, 1, "None" },
                    { 99, "It guards a book bound in black, the Swamp Seal Book. One of four, and the tree cannot be saved without them.", 3, true, 6, null, 1, "None" },
                    { 100, "Long ago the elders bound an ancient power into four books. That binding has broken, and the leak is poisoning me.", 3, true, 7, null, 2, "None" },
                    { 101, "The elves told you a story with the ugly parts cut out. I can show you what they buried.", 3, true, 8, null, 3, "None" },
                    { 102, "This is farming country. Folk here trade a day of work for supper, and honest work is easy to find.", 3, true, 9, null, 4, "None" },
                    { 103, "Mind the ones that glow faintly. An enchanted pumpkin keeps a lantern lit all winter, that is why the city pays.", 3, true, 10, null, 7, "None" },
                    { 104, "I would carry them myself, but no one from this farm has come back from that road in a week.", 3, true, 11, null, 7, "None" },
                    { 105, "I am Tristan, and my orders bind me to this gate. I cannot take one step past it, even now.", 3, true, 12, null, 5, "None" },
                    { 106, "There is one person left who might stand against it. Arthur, the silver knight, camped in the old ruins.", 3, true, 13, null, 5, "None" },
                    { 107, "I cannot lift my blade again. But a blade is only steel, what matters is the hand that learns to swing it.", 3, true, 14, null, 6, "None" },
                    { 108, "The city outside is still crawling. Every hour they spread further, and the dead cannot be buried while they roam.", 3, true, 15, null, 6, "None" },
                    { 109, "A dragon. It is the thing that broke this city, and the thing that broke me. I have carried that shame for years.", 3, true, 16, null, 6, "None" },
                    { 110, "He carries something that should have stayed sealed. Wherever he walks, the land sickens behind him.", 3, true, 17, null, 6, "None" },
                    { 111, "The slimes come closer each night. They freeze whatever they touch, and my people cannot reach the grain stores.", 3, true, 18, null, 8, "None" },
                    { 112, "His supplies ran out days ago and no courier of mine has come back down that road alive.", 3, true, 19, null, 8, "None" },
                    { 113, "Ice dragons, five of them. Young, but the codex made them hungry in a way no beast should be.", 3, true, 20, null, 9, "None" },
                    { 114, "I have watched this ban for eleven years and never once set foot inside. Now something in there has begun to stir.", 3, true, 21, null, 10, "None" },
                    { 115, "A stone golem stands over it. The elders left it there to keep hands off the book — mine included.", 3, true, 22, null, 10, "None" },
                    { 116, "There is a Seal Book buried under all this bone. I have felt it since the day the leak began.", 3, true, 23, null, 11, "None" },
                    { 117, "I cannot leave the well. I have tried. Something of me is still down in that ground, and it holds me here.", 2, true, 24, null, 12, "None" },
                    { 118, "The animals knew before you did. That is why they ran. They will not drink from a well with a girl in it.", 3, true, 24, null, 12, "None" },
                    { 119, "(She writes of a book she opened as a child, of a seal she did not understand, and of the day the valley began to fill with bone.)", 2, true, 25, null, 12, "None" },
                    { 120, "A portal home cannot be forced. It must be grown, and for that the rite needs leaves older than the curse itself.", 3, true, 26, null, 13, "None" },
                    { 121, "Three seals you have already. Without his, the Origin Tree cannot be cleansed and the forest ends with the tree.", 3, true, 27, null, 13, "None" },
                    { 122, "The rite will open once and close behind you. Whatever you leave undone on this side stays undone.", 3, true, 28, null, 13, "None" },
                    { 123, "Four books, four elders, four bindings broken. Set them together and the curse has nowhere left to hide.", 3, true, 29, null, 2, "None" },
                    { 124, "I am the tree's spirit. If the curse takes the roots, it takes me with them — so do not hesitate at the last step.", 3, true, 30, null, 2, "None" },
                    { 125, "Look at the roots. Green, after all this time. The forest will remember the one who stood here today.", 2, true, 31, null, 2, "None" },
                    { 126, "And yet the cloaked one was never found, and no one has said who broke the four bindings in the first place.", 3, true, 31, null, 2, "None" }
                });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "You wake at the edge of the Elf Forest with no memory of how you arrived. Elder Rowan is waiting by the great roots — go to him and hear why the forest called you here.", "[Chapter 1] A Word with Elder Rowan" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 2,
                column: "Description",
                value: "The elders brew their healing draught from white flowers that only bloom in the shade of the old woods. Search the clearings and gather 3 White Flowers for Elder Rowan.");

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Bring the gathered flowers back to Elder Rowan. In return he will teach you the first strike an elf ever learns.", "[Chapter 1] Deliver the White Flowers" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "A skill is useless until it sits in your hand. Open the Skill panel and equip the technique Elder Rowan just taught you.", "[Chapter 1] Equip Your First Skill" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 5,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Little slimes have crept out of the marsh and are eating the flower beds. Put your new skill to work and defeat 3 of them.", "[Chapter 1] Cull the Little Slimes" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 6,
                columns: new[] { "Description", "Title" },
                values: new object[] { "The slimes were only fleeing something worse. Deep in the woods a Swamp Demon guards the first of four Seal Books — kill it and take the seal.", "[Chapter 1] Slay the Swamp Demon" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 7,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Take the Seal Book to the guardian Lyra at the Origin Tree. She alone can explain the curse rotting its roots and why four seals are needed to lift it.", "[Chapter 1] Lyra and the Origin Tree" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 8,
                columns: new[] { "Description", "Title" },
                values: new object[] { "A cloaked figure has been watching you since you woke, and now walks into a portal at the forest edge. Step through it before the way closes.", "[Chapter 1] Follow the Cloaked Figure" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 9,
                columns: new[] { "Description", "Title" },
                values: new object[] { "The portal spits you onto a cold beach under an autumn sky. Climb to the castle and find Elder Rowan — or someone wearing his face — and ask what land this is.", "[Chapter 2] Ask Where You Are" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 10,
                columns: new[] { "Description", "Title" },
                values: new object[] { "You have no coin in this land and no one gives bread away. Farmer Fa will trade a meal for labour: pick 8 Enchanted Pumpkins from his field.", "[Chapter 2] Harvest for Your Supper" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 11,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Fa is too old to make the road alone. Carry the harvest to the city gate and hand it to the guard Tristan.", "[Chapter 2] Deliver the Harvest" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 12,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Beyond the gate the city is silent and the streets are full of the dead. Examine 5 of the bodies and learn what killed them.", "[Chapter 2] Examine the Fallen" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 13,
                column: "Description",
                value: "Tristan pales at your report: only one man ever held these ruins. Search the city for the silver knight Arthur and ask for his help.");

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 14,
                columns: new[] { "Description", "RequiredLevel", "RewardExperience", "Title" },
                values: new object[] { "Arthur's wounds run deeper than his armour and his power is sealed away; he cannot fight for the city. He can, however, make you strong enough to. Clear his training dungeon.", 4, 250, "[Chapter 2] Train in the Old Dungeon" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 15,
                columns: new[] { "Description", "RequiredLevel", "RewardExperience", "Title" },
                values: new object[] { "With Arthur's dark technique and his Silver Necklace, you stand in the knight's place. Hunt down 10 of the creatures still prowling the ruins.", 4, 300, "[Chapter 2] Purge the Ruined City" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 16,
                columns: new[] { "Description", "RewardExperience" },
                values: new object[] { "Arthur admits you now fight as well as he once did — and tells you what truly broke the city. A dragon nests in the ruins. End it.", 350 });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 17,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Return to Arthur for the knight's thanks and ask where the cursed codex came from. He points north, to a kingdom the codex froze solid.", 150, "[Chapter 2] Arthur's Parting Words" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 18,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Queen Roselyn Aurora receives you in a hall of ice. Her fields are overrun before the winter stores are in — defeat 8 ice slimes for her.", 200, "[Chapter 3] Slimes of the Snow Fields" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 19,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "The Queen entrusts you with Magic Flour for the mountain shrine. Carry it up to the priest Zephyr before the pass closes.", 150, "[Chapter 3] Deliver the Magic Flour" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 20,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Zephyr cannot hold his rites while ice dragons circle the shrine. Climb the peak and bring down 5 of them.", 250, "[Chapter 3] Dragons of the Frozen Peak" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 21,
                columns: new[] { "Description", "ObjectiveType", "RewardExperience", "Title" },
                values: new object[] { "Zephyr says the codex's mark lies inside the forbidden zone, and only its warden may open the way. Find Roland at the boundary stones and ask for passage.", "Talk", 150, "[Chapter 3] The Warden of the Ban" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 22,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Roland tells you what the kingdom buried here: the codex itself, and the golem forged to guard it. Destroy the golem and take the second Seal Book.", 400, "[Chapter 3] Break the Stone Guardian" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 23,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "The trail of the seals ends at a ruined castle where the dead still keep watch. The Valiant Warrior holds the valley alone — help him put down 12 skeletons.", 300, "[Chapter 4] Break the Skeleton Army" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 24,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "In the drowned village of Tide-Knell a girl named Natalie asks a strange favour: dig beside the old well and lift out the skull buried there.", 200, "[Chapter 4] The Skull by the Well" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 25,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "The skull is hers. Read the letter she left behind, bury her remains beneath the ivy tree, and she will give you the key she died holding.", 200, "[Chapter 4] Lay Natalie to Rest" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 26,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Natalie's key opens the way to a deserted island where one elf guard still stands his post. He needs 5 Ancient Leaves from the plateau to break the seal below.", 250, "[Chapter 4] Ancient Leaves of the Isle" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 27,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "The leaves burn away the ward and the crypt opens. The UnderKing holds the last two Seal Books — take them from him.", 500, "[Chapter 4] Defeat the UnderKing" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 28,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "All four seals are in your pack. Speak to the Elf Guard — he can open a portal back to the Elf Forest.", 150, "[Chapter 4] Ask for the Way Home" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 29,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "You are home, and the Origin Tree is worse than you left it. Bring all four Seal Books to Lyra.", 250, "[Chapter 5] Return with the Seals" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 30,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Lyra opens the rite and steps back — the seals must be set by the one who won them. Place the four Seal Books on the Origin Tree and break the curse.", 400, "[Chapter 5] Heal the Origin Tree" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 31,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "The Origin Tree is green again and the forest wakes around it. Speak with Lyra one last time — the codex had a master, and that story is not finished.", 300, "[Chapter 5] A New Dawn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 114);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 115);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 116);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 117);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 118);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 119);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 120);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 125);

            migrationBuilder.DeleteData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 126);

            migrationBuilder.CreateTable(
                name: "GameSettings",
                columns: table => new
                {
                    GameSettingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedByAccountAccountId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedByAccountAccountId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedByAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedByAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSettings", x => x.GameSettingId);
                    table.ForeignKey(
                        name: "FK_GameSettings_Accounts_CreatedByAccountAccountId",
                        column: x => x.CreatedByAccountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_GameSettings_Accounts_UpdatedByAccountAccountId",
                        column: x => x.UpdatedByAccountAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId");
                });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 1,
                column: "Content",
                value: "Ah, a new traveler. Welcome to the Elf Forest.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 2,
                column: "Content",
                value: "This forest has been peaceful for centuries, but recently, dark forces have begun to gather.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 3,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "I need your help to protect this place. Come speak to me when you are ready to begin.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 4,
                column: "Content",
                value: "Before we can confront the darkness, we need to prepare some basic remedies.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 5,
                column: "Content",
                value: "The old willow clearing has some magical herbs we can use.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 6,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please head over there and gather 3 White Flowers for me.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 7,
                column: "Content",
                value: "You have returned quickly. Did you find the flowers?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 8,
                column: "Content",
                value: "Excellent, these are in perfect condition. They will make fine healing poultices.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 9,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Thank you! Take this as a token of my appreciation.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 10,
                column: "Content",
                value: "Now that you have your reward, it is time to learn how to defend yourself.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 11,
                column: "Content",
                value: "In this world, skills are essential for survival. You cannot fight with bare hands alone.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 12,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Open your Skill Panel and equip your first combat skill before you face real danger.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 13,
                column: "Content",
                value: "Good, you are armed and ready. It is time to test your newfound abilities.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 14,
                column: "Content",
                value: "The outskirts of our forest have been overrun by strange, aggressive slimes.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 15,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Head out and defeat 3 SlimeLittle monsters to prove your worth to the village.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 16,
                column: "Content",
                value: "You handled those slimes well. But a much greater threat lurks in the deep woods.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 17,
                column: "Content",
                value: "A terrible Swamp Demon has made its lair there, corrupting the land with its presence.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 18,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "You must destroy the Swamp Demon and claim the Swamp Seal Book it guards. We are counting on you!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 19,
                column: "Content",
                value: "Greetings, brave warrior. I am Lyra, the spirit of the Origin Tree.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 20,
                column: "Content",
                value: "As you can see, the tree has been cursed and is slowly dying.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 21,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Only the 4 Seal Books can cleanse it. You have one, but you must find the remaining three!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 22,
                column: "Content",
                value: "Heh... So you are the one collecting the Seal Books?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 23,
                column: "Content",
                value: "You know nothing of the true history of this world, or why the tree was cursed.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 24,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "If you want the truth, follow me through this portal. Don't keep me waiting.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 25,
                column: "Content",
                value: "Welcome to the beach. We were teleported here by the portal.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 26,
                column: "Content",
                value: "You seem to have no money for food. Why don't you look for some work?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 27,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Go talk to Fa, he is nearby and might need some help.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 28,
                column: "Content",
                value: "Ah, Elder Rowan sent you? Good timing.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 29,
                column: "Content",
                value: "I need someone to help me harvest the fields.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 30,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please collect 8 Enchanted Pumpkins for me.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 31,
                column: "Content",
                value: "Great job with the pumpkins! You are a hard worker.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 32,
                column: "Content",
                value: "Now, I need these delivered to the city gate.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 33,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please take them to guard Tristan at the ruined city.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 34,
                column: "Content",
                value: "Halt! Who goes there? Ah, you brought pumpkins from Fa?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 35,
                column: "Content",
                value: "Something is wrong in the city... It is too quiet.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 36,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please go inside and investigate. Let me know if you find anything suspicious.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 37,
                column: "Content",
                value: "What?! The people inside have all been massacred? Corpses everywhere?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 38,
                column: "Content",
                value: "This is a disaster. We need someone strong to handle this.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 39,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please, go find the silver knight Arthur and report this!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 40,
                column: "Content",
                value: "Greetings, warrior. I am Arthur, the silver knight.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 41,
                column: "Content",
                value: "I suffered severe internal injuries and my power has been sealed away.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 42,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "You must train in Dungeon 2 to level up and unlock your true potential. Go!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 43,
                column: "Content",
                value: "Splendid! You have trained well and cleared the dungeon.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 44,
                column: "Content",
                value: "As promised, take this DarkExplosion skill and Silver Necklace.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 45,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Now, use your power to defeat 10 evil monsters in the Ruined City!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 46,
                column: "Content",
                value: "You have returned, and you are stronger than before.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 47,
                column: "Content",
                value: "I recognize your true strength now. You are ready for the ultimate challenge.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 48,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "A terrible dragon threatens our existence. Go and slay the DragonBossIdle!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 49,
                column: "Content",
                value: "You did it! The dragon is slain. I cannot thank you enough.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 50,
                column: "Content",
                value: "You ask about the mysterious figure? The one who did this?");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 51,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "He went towards the frozen lands devastated by the codex. Head to the Frozen Mountains next.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 52,
                column: "Content",
                value: "Ah, a survivor from the ruins. I am Queen Roselyn Aurora.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 53,
                column: "Content",
                value: "This land is devastated by the codex. Only volunteers remain to defend it.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 54,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please, clear out 8 ice slimes from the Snow Fields to help us.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 55,
                column: "Content",
                value: "Your efforts have not gone unnoticed. The slimes are thinning out.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 56,
                column: "Content",
                value: "However, our Priest Zephyr requires supplies for a ritual.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 57,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Take this Magic Flour and deliver it to him at the mountain peak.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 58,
                columns: new[] { "Content", "ResponseType" },
                values: new object[] { "Ah, the flour from the Queen! Thank you, traveler.", "None" });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 59,
                column: "Content",
                value: "The codex has warped the creatures here. The beasts have become feral and dangerous.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 60,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "To secure our borders, go slay 5 Ice Dragons on the mountain.", 2 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 61,
                column: "Content",
                value: "Halt! This is the forbidden zone. None may enter.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 62,
                column: "Content",
                value: "Wait... you have the aura of one who has fought the Ice Dragons.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 63,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Since you made it this far, help me explore this dangerous area.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 64,
                column: "Content",
                value: "We have uncovered the origin of the codex... The truth is terrifying.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 65,
                column: "Content",
                value: "A massive ancient golem guards the final piece of the puzzle.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 66,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Defeat the giant GolemBoss to claim the Golem Seal Book! Do not fail us.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 67,
                column: "Content",
                value: "Stay back! The undead are relentless today.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 68,
                column: "Content",
                value: "An ancient power is leaking, causing skeletons to multiply out of control.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 69,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "I can't hold them off alone. Defeat 12 of them in the valley!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 70,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "The animals are fleeing from the abandoned village Tide-Knell. Investigate it and find Natalie.", 4 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 71,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Are you here to help me? I cannot leave this place...", 2 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 72,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please... dig up what is buried under the small tree near the old well.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 73,
                column: "Content",
                value: "(A weathered suicide letter lies where Natalie once stood...)");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 74,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Thank you for bringing my remains back to my homeland. Please bury me under the ivy tree in my courtyard.", 2 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 75,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "The ancient power leak was my doing. I am deeply sorry. Take this Mystic Key. It will unlock the gates to the castle on the deserted island.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 76,
                column: "Content",
                value: "You actually survived the waves and made it to this deserted island.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 77,
                column: "Content",
                value: "I need your assistance to prepare a ritual of return.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 78,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Help me collect 5 Ancient Leaves from the Northern Plateau.", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 79,
                column: "Content",
                value: "We have everything we need. But a dark presence blocks our path.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 80,
                column: "Content",
                value: "The UnderKing himself has awakened, and he guards the final Seal Book.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 81,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "You must end his reign! Defeat the UnderKing and claim the book!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 82,
                column: "Content",
                value: "It is done. The UnderKing is defeated, and you have all 4 Seal Books.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 83,
                column: "Content",
                value: "The fate of the Origin Tree now rests entirely in your hands.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 84,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Farewell, hero. I will use my power to open a portal back to the Elf Forest. Save the tree!", 3 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 85,
                column: "Content",
                value: "You have returned! And I can sense the power of the 4 Seal Books.");

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 86,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Please, hurry! Use the books on the Origin Tree to cleanse the corruption.", 2 });

            migrationBuilder.UpdateData(
                table: "NPCDialogues",
                keyColumn: "NPCDialogueId",
                keyValue: 88,
                columns: new[] { "Content", "DisplayOrder" },
                values: new object[] { "Thank you! The Origin Tree is saved. But this is not the end... To be continued.", 2 });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 1,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Talk to Elder Rowan in the Elf Forest.", "[Chapter 1] Speak with Elder Rowan" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 2,
                column: "Description",
                value: "Collect 3 White Flowers from the forest.");

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 3,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Deliver the gathered flowers to Elder Rowan.", "[Chapter 1] Deliver White Flowers" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 4,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Equip your first combat skill.", "[Chapter 1] Equip Your Skill" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 5,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Kill 3 SlimeLittle monsters in the forest.", "[Chapter 1] Defeat Slimes" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 6,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Slay the Swamp Demon and obtain its Seal Book.", "[Chapter 1] The Swamp Demon" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 7,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Talk to Lyra about the cursed Origin Tree and the 4 Seal Books.", "[Chapter 1] The Origin Tree" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 8,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Follow the cloaked figure through the portal to Autumn Pumpkin.", "[Chapter 1] The Mysterious Figure" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 9,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Teleported onto the beach, proceed to the castle and ask Elder Rowan where this is. After introductions, realize you have no money and ask if there is work to earn food.", "[Chapter 2] Where Are We?" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 10,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Collect 8 Enchanted Pumpkins from the field and hand them over to farmer Fa.", "[Chapter 2] Work for Food" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 11,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Help Fa deliver the harvested pumpkins to guard Tristan at the ruined city gate.", "[Chapter 2] Delivery to the City" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 12,
                columns: new[] { "Description", "Title" },
                values: new object[] { "Enter the city and investigate the dead bodies, then report back to guard Tristan.", "[Chapter 2] The Ruined City" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 13,
                column: "Description",
                value: "Report the massacre to Tristan. He asks you to find the silver knight Arthur for help.");

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 14,
                columns: new[] { "Description", "RequiredLevel", "RewardExperience", "Title" },
                values: new object[] { "Speak with Arthur and learn about his internal injuries and sealed power. Enter Dungeon ID 2 to train and level up your strength.", 12, 15, "[Chapter 2] The Silver Knight's Training" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 15,
                columns: new[] { "Description", "RequiredLevel", "RewardExperience", "Title" },
                values: new object[] { "Receive the DarkExplosion skill and Silver Necklace from Arthur. Take his place to defeat 10 evil monsters in the Ruined City.", 12, 20, "[Chapter 2] Defeat the Evil Monsters" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 16,
                columns: new[] { "Description", "RewardExperience" },
                values: new object[] { "Turn in the quest and get Arthur's recognition of your strength, receive quest to kill DragonBossIdle. Go kill dragon DragonBossIdle.", 50 });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 17,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Talk to Arthur and receive the knight's thanks, ask about the whereabouts of the ??? and he directs you to the frozen land devastated by the codex, go to Frozen Mountains.", 10, "[Chapter 2] The Frozen Threat" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 18,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Meet Queen Roselyn Aurora and defeat 8 Ice Slimes.", 30, "[Chapter 3] The Ice Slimes" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 19,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Deliver Magic Flour (obtained from the Queen) to the Priest (Zephyr).", 15, "[Chapter 3] Magic Flour for the Priest" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 20,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Meet Zephyr and slay 5 Ice Dragons on the mountain.", 40, "[Chapter 3] Dragons of Snow" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 21,
                columns: new[] { "Description", "ObjectiveType", "RewardExperience", "Title" },
                values: new object[] { "Head to the forbidden zone and speak with Roland to explore it.", "Explore", 15, "[Chapter 3] The Forbidden Zone" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 22,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Discover the truth of the codex and defeat GolemBoss to get the Golem Seal Book.", 80, "[Chapter 3] Truth of the Codex" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 23,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Defeat 12 skeletons in the valley for Valiant Warrior.", 50, "[Chapter 4] Skeleton Army" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 24,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Dig up the skull near the old well.", 30, "[Chapter 4] The Abandoned Village" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 25,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Read Natalie's suicide letter and bury her remains under the ivy tree. Receive Mystic Key.", 40, "[Chapter 4] Rest in Peace" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 26,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Talk to Elf Guard on the deserted island and collect 5 Ancient Leaves.", 45, "[Chapter 4] Deserted Island" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 27,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Defeat the UnderKing to claim the final UnderKing Seal Book.", 200, "[Chapter 4] The UnderKing" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 28,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Talk to Elf Guard. He will open a portal back to the Elf Forest.", 10, "[Chapter 4] Return to Elf Forest" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 29,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Talk to Lyra about the 4 Seal Books.", 50, "[Chapter 1] Return with the Seals" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 30,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Use the 4 Seal Books on the Origin Tree.", 250, "[Chapter 1] Heal the Origin Tree" });

            migrationBuilder.UpdateData(
                table: "Quests",
                keyColumn: "QuestId",
                keyValue: 31,
                columns: new[] { "Description", "RewardExperience", "Title" },
                values: new object[] { "Talk to Lyra. The Origin Tree is saved. To be continued...", 200, "[Chapter 1] A New Dawn" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerQuests_PlayerProfileId",
                table: "PlayerQuests",
                column: "PlayerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_CreatedByAccountAccountId",
                table: "GameSettings",
                column: "CreatedByAccountAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSettings_UpdatedByAccountAccountId",
                table: "GameSettings",
                column: "UpdatedByAccountAccountId");
        }
    }
}
