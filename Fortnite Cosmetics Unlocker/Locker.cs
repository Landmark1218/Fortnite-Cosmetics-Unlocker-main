using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Fortnite_Cosmetics_Unlocker
{
    internal class Locker
    {
        public static string LoadoutTemplate = @"{
    ""deploymentId"": """",
    ""accountId"": """",
    ""athenaItemId"": ""9092ba65-01e8-4598-9c09-6de6be10ea49"",
    ""creationTime"": ""0001-01-01T00:00:00.000Z"",
    ""updatedTime"": ""0001-01-01T00:00:00.000Z"",
    ""loadouts"": {
        ""CosmeticLoadout:LoadoutSchema_Character"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Character"",
                    ""equippedItemId"": ""AthenaCharacter:cid_defaultoutfit"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Backpack"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Pickaxe"",
                    ""equippedItemId"": ""AthenaPickaxe:defaultpickaxe"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Glider"",
                    ""equippedItemId"": ""AthenaGlider:defaultglider"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Shoes"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Contrails"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Aura"",
                    ""equippedItemId"": ""SparksAura:sparksaura_default"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Emotes"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_0"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_1"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_2"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_3"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_4"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Emote_5"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Jam"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong0"",
                    ""equippedItemId"": ""SparksSong:sid_placeholder_35"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong1"",
                    ""equippedItemId"": ""SparksSong:sid_placeholder_58"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong2"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong3"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong4"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong5"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong6"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_JamSong7"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_LegoSets"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Juno_BuildSet"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Juno_PropBundle"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Moments"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_MomentsBus"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_MomentsVictory"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Platform"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Banner_Icon"",
                    ""equippedItemId"": ""HomebaseBannerIcon:standardbanner20"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Banner_Color"",
                    ""equippedItemId"": ""HomebaseBannerColor:defaultcolor2"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_LobbyMusic"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_LoadingScreen"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Sparks"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Bass"",
                    ""equippedItemId"": ""SparksBass:sparks_bass_generic"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Guitar"",
                    ""equippedItemId"": ""SparksGuitar:sparks_guitar_generic"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Drum"",
                    ""equippedItemId"": ""SparksDrums:sparks_drum_generic"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Keyboard"",
                    ""equippedItemId"": ""SparksKeyboard:sparks_keytar_generic"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Microphone"",
                    ""equippedItemId"": ""SparksMicrophone:sparks_mic_generic"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Vehicle"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Body"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Skin"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Wheel"",
                    ""equippedItemId"": ""VehicleCosmetics_Wheel:id_wheel_oem"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_DriftSmoke"",
                    ""equippedItemId"": ""VehicleCosmetics_DriftTrail:id_drifttrail_standard"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Booster"",
                    ""equippedItemId"": ""VehicleCosmetics_Booster:id_booster_standard"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Vehicle_SUV"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Body_SUV"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Skin_SUV"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Wheel_SUV"",
                    ""equippedItemId"": ""VehicleCosmetics_Wheel:id_wheel_oem"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_DriftSmoke_SUV"",
                    ""equippedItemId"": ""VehicleCosmetics_DriftTrail:id_drifttrail_standard"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Vehicle_Booster_SUV"",
                    ""equippedItemId"": ""VehicleCosmetics_Booster:id_booster_standard"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        },
        ""CosmeticLoadout:LoadoutSchema_Wraps"": {
            ""loadoutSlots"": [
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_0"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_1"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_2"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_3"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_4"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_5"",
                    ""itemCustomizations"": []
                },
                {
                    ""slotTemplate"": ""CosmeticLoadoutSlotTemplate:LoadoutSlot_Wrap_6"",
                    ""itemCustomizations"": []
                }
            ],
            ""shuffleType"": ""DISABLED""
        }
    },
    ""shuffleType"": ""DISABLED""
}";

        public static bool Initialize(string deploymentId, string accountId)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "loadouts")))
            {
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "loadouts"));
            }

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json")))
            {
                JObject loadouts = JObject.Parse(LoadoutTemplate);

                loadouts["deploymentId"] = deploymentId;
                loadouts["accountId"] = accountId;

                File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json"), loadouts.ToString());
            }

            return true;
        }

        public static JObject Get(string deploymentId, string accountId)
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json")))
            {
                return null;
            }

            JObject loadouts = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json")));

            if (loadouts == null)
            {
                return null;
            }

            return new JObject
            {
                ["activeLoadoutGroup"] = loadouts,
                ["loadoutGroupPresets"] = new JArray(),
                ["loadoutPresets"] = new JArray(),
            };
        }

        public static JObject Put(string deploymentId, string accountId, string data)
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json")))
            {
                return null;
            }

            JObject loadouts = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json")));

            if (loadouts == null)
            {
                return null;
            }

            JObject json = JObject.Parse(data);

            // 2025-11-05 とりあえず修正
            {
                foreach (var loadout in (JObject)json["loadouts"])
                {
                    JArray loadoutSlots = (JArray)json["loadouts"][loadout.Key]["loadoutSlots"];

                    int index = 0;
                    foreach (JObject slot in loadoutSlots)
                    {
                        JToken equippedItemId = json["loadouts"][loadout.Key]["loadoutSlots"][index]["equippedItemId"];
                        if (equippedItemId != null)
                        {
                            json["loadouts"][loadout.Key]["loadoutSlots"][index]["equippedItemId"] = equippedItemId.ToString().Split(':')[0] + ":" + equippedItemId.ToString().Split(':')[1];
                        }
                        index++;
                    }
                }
            }

            if (loadouts["athenaItemId"].ToString() == json["athenaItemId"].ToString())
            {
                loadouts["loadouts"] = json["loadouts"];
            }

            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "loadouts", $"{deploymentId}-{accountId}.json"), loadouts.ToString());

            return loadouts;
        }
    }
}
