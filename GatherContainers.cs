using System;
using Oxide.Core.Configuration;

namespace Oxide.Plugins
{
    [Info("Gather Containers", "NoSoyLito", "0.0.1")]
    [Description("Server PVP Gamemode that randomly divides the server population in two factions")]
    public class GatherContainers : RustPlugin
    {
        #region "Fields"

        private PluginConfig config;
        private DynamicConfigFile dataFile;

        #endregion

        #region "Oxide Hooks"

        void Init()
        {
            config = Config.ReadObject<PluginConfig>();
        }

        void OnLootEntityEnd(BasePlayer player, BaseCombatEntity entity)
        {
            LootContainer container = entity as LootContainer;
            if (container.inventory.itemList.Count == 0)
            {
                GiveMaterialsForCrate(player, container);
            }
        }

        object OnEntityTakeDamage(BaseCombatEntity entity, HitInfo info)
        {
            if (IsPlayer(info))
            {
                if (IsLootContainer(entity))
                {
                    if (IsBreakableContainer(entity))
                    {
                        GiveMaterialsForBarrel(entity, info);
                    }
                }
            }
            return null;
        }

        #endregion

        #region "Plugin guts"

        private bool IsPlayer(HitInfo info)
        {
            if (info != null && info.Initiator != null && info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
            {
                return true;
            }
            return false;
        }

        private bool IsLootContainer(BaseCombatEntity entity)
        {
            if (entity.GetType().Name.Equals("LootContainer"))
            {
                return true;
            }
            return false;
        }

        private bool IsBreakableContainer(BaseCombatEntity entity)
        {
            if (entity.ShortPrefabName.Contains("barrel"))
            {
                return true;
            }
            return false;
        }

        private void GiveMaterialsForBarrel(BaseCombatEntity entity, HitInfo info)
        {
            //Puts(info.damageProperties.name);
            //Puts(info.damageTypes.Total().ToString());
            float amount = config.ContainerGatheringMultiplier * info.damageTypes.Total();
            int maxAmount = (int)Math.Round(amount / 4.0);
            int minAmount = (int)Math.Round(maxAmount / 2.0);
            if (maxAmount < 1) { maxAmount = 1; minAmount = 1; }
            if (minAmount < 1) { minAmount = 1; }
            info.InitiatorPlayer.inventory.GiveItem(ItemManager.CreateByItemID(69511070, Core.Random.Range(minAmount, maxAmount)));
        }

        private void GiveMaterialsForCrate(BasePlayer player, LootContainer container)
        {
            switch (container.ShortPrefabName)
            {
                case "crate_basic":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 10 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_tools":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(69511070, 15 * config.ContainerGatheringMultiplier));
                    player.inventory.GiveItem(ItemManager.CreateByItemID(317398316, 1 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_mine":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 15 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_normal_2":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 30 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_normal_2_food":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 20 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_normal_2_medical":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 20 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_normal":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(-151838493, 50 * config.ContainerGatheringMultiplier));
                    player.inventory.GiveItem(ItemManager.CreateByItemID(69511070, 5 * config.ContainerGatheringMultiplier));
                    break;
                case "crate_elite":
                    player.inventory.GiveItem(ItemManager.CreateByItemID(69511070, 100 * config.ContainerGatheringMultiplier));
                    player.inventory.GiveItem(ItemManager.CreateByItemID(317398316, 15 * config.ContainerGatheringMultiplier));
                    break;
            }
        }

        private string GetCrateContainerName(BaseCombatEntity entity)
        {
            return entity.GetType().Name;
        }

        #endregion

        #region "Config logic"

        private class PluginConfig
        {
            public int ContainerGatheringMultiplier;
        }

        protected override void LoadDefaultConfig()
        {
            Config.WriteObject(GetDefaultConfig(), true);
        }

        private PluginConfig GetDefaultConfig()
        {
            return new PluginConfig
            {
                ContainerGatheringMultiplier = 1
            };
        }

        #endregion

        #region "Helpers"

        string ColorString(string text, string color)
        {
            return "<color=" + color + ">" + text + "</color>";
        }

        #endregion
    }
}

