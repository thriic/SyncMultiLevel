using StardewModdingAPI;
using StardewValley;

namespace SyncMultiLevel
{
    internal class SyncMultiLevel : Mod
    {
        private const int FARMING_SKILL = 0;
        private const int FISHING_SKILL = 1;
        private const int FORAGING_SKILL = 2;
        private const int MINING_SKILL = 3;
        private const int COMBAT_SKILL = 4;
        private const int LUCK_SKILL = 5;
        private const string HelpText = "SkillID:\n  0 Farming\n  1 Fishing\n  2 Foraging\n  3 Mining\n  4 Combat\n  5 Luck";

        private int[] experiencePoints = { 0, 0, 0, 0, 0 };

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            helper.ConsoleCommands.Add("add_exp", $"add_exp [SkillID] [Exp] [PlayerName]\ne.g. add_exp 1 1000\n  give 1000exp to LocalPlayer on Fishing\n{HelpText}", ExpCommand);
            helper.ConsoleCommands.Add("set_level", $"set_level [SkillID] [Level] [PlayerName]\ne.g. set_level 0 4\n  set LocalPlayer's Farming Level to 4\n{HelpText}", LevelCommand);
        }

        private void GameLoop_OneSecondUpdateTicked(object? sender, StardewModdingAPI.Events.OneSecondUpdateTickedEventArgs e)
        {
            var farmers = Game1.getOnlineFarmers();
            foreach (var farmer in farmers)
            {
                if(farmer.Name == "") continue;
                var farmerPoints = farmer.experiencePoints;
                for (int i = 0; i < experiencePoints.Length; i++)
                {
                    var difference = experiencePoints[i] - farmerPoints[i];
                    if (difference < 0)
                    {
                        experiencePoints[i] = farmer.experiencePoints[i];
                        //this.Monitor.Log($"update Skill {SkillName(i)} exp to {experiencePoints[i]}", LogLevel.Debug);
                    }
                    else if (difference > 0)
                    {
                        farmer.gainExperience(i, difference);
                        //this.Monitor.Log($"gain Player {farmer.Name} Skill {SkillName(i)} exp to {experiencePoints[i]}", LogLevel.Debug);
                    }
                }
            }
        }

        private void ExpCommand(string _, string[] args)
        {
            var skillId = 0;
            var exp = 1000;
            var player = Game1.player;
            if (args.Length >= 3)
            {
                var farmers = Game1.getOnlineFarmers();
                bool flag = false;
                foreach (Farmer farmer in farmers)
                {
                    if (farmer.Name == args[2])
                    {
                        flag = true;
                        player = farmer;
                    }
                }
                if (!flag)
                {
                    this.Monitor.Log($"cannot find player {args[2]}", LogLevel.Error);
                    return;
                }
            }
            if (args.Length >= 2)
            {
                exp = Convert.ToInt32(args[1]);
            }
            if (args.Length >= 1)
            {
                skillId = Convert.ToInt32(args[0]);
            }
            player.gainExperience(skillId, exp);
            this.Monitor.Log($"add {exp}exp on {SkillName(skillId)} to {player.Name}", LogLevel.Info);
        }

        private void LevelCommand(string _, string[] args)
        {
            var player = Game1.player;
            var level = 1;
            var skillLevel = player.foragingLevel;
            if (args.Length >= 3)
            {
                var farmers = Game1.getOnlineFarmers();
                bool flag = false;
                foreach (Farmer farmer in farmers)
                {
                    if (farmer.Name == args[2])
                    {
                        flag = true;
                        player = farmer;
                    }
                }
                if (!flag)
                {
                    this.Monitor.Log($"cannot find player {args[2]}", LogLevel.Error);
                    return;
                }
            }
            if (args.Length >= 2)
            {
                level = Convert.ToInt32(args[1]);
            }
            if (args.Length >= 1)
            {
                skillLevel = Convert.ToInt32(args[0]) switch
                {
                    FARMING_SKILL => player.farmingLevel,
                    MINING_SKILL => player.miningLevel,
                    FISHING_SKILL => player.fishingLevel,
                    FORAGING_SKILL => player.foragingLevel,
                    LUCK_SKILL => player.luckLevel,
                    COMBAT_SKILL => player.combatLevel,
                    _ => player.farmingLevel
                };
            }
            skillLevel.Value = level;
            this.Monitor.Log($"add {level}exp on {SkillName(Convert.ToInt32(args[0]))} to {player.Name}", LogLevel.Info);
        }

        private static string SkillName(int skillId) => skillId switch
        {
            FARMING_SKILL => "Farming",
            MINING_SKILL => "Mining",
            FISHING_SKILL => "Fishing",
            FORAGING_SKILL => "Foraging",
            LUCK_SKILL => "Luck",
            COMBAT_SKILL => "Combat",
            _ => "Unknown"
        };
    }
}