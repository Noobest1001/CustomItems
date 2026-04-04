using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using ExtendedItems.Items;
using UnityEngine;
using UserSettings.ServerSpecific;

namespace ExtendedItems
{
    // ReSharper disable once InconsistentNaming
    public class SSSS
    {
        // ReSharper disable once InconsistentNaming
        private static IEnumerable<SettingBase>? _settings;

        public static void Register()
        {
            Log.Info("Keybind Registered");
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += Keybind;

            _settings =
            [
                new HeaderSetting(10, "Custom Items", "Custom keybinds for Custom Items plugin made my Noobest1001", true),
                new KeybindSetting(24, "C-4 Detonation", KeyCode.Delete, hintDescription: "Explodes all active instances of C-4 thrown by you"),
            ];
            SettingBase.Register(_settings);
        }

        public static void Unregister()
        {
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= Keybind;
        }

        private static void Keybind(ReferenceHub referenceHub, ServerSpecificSettingBase settingBase)
        {
            if (settingBase is not SSKeybindSetting { SettingId: 24, SyncIsPressed: true } keybindSetting)
                return;
            if (!Player.TryGet(referenceHub, out var player))
                return;

            Log.Info("Keybind used by " + player.Nickname);
            if (keybindSetting.SettingId == 24)
            {
                var i = 0;
                foreach (var charge in Plastic.PlacedCharges.ToList())
                {
                    var posy = charge.Key.Position.y;
                    if (charge.Value != player) continue;

                    if (player.Position.y >= posy - 100 && player.Position.y <= posy + 100)
                    {
                        Plastic.Instance.Handler(charge.Key, Plastic.C4RemoveMethod.Detonate, player);
                        i++;
                    }
                    else
                    {
                        player.SendConsoleMessage(
                            "One of your charges is out of range. You need to get within the zone that it was placed",
                            "yellow");
                    }

                    player.ShowHint(i == 1
                        ? $"\n<color=green>{i} C4 charge has been detonated!</color>"
                        : $"\n<color=green>{i} C4 charges have been detonated!</color>");
                }
            }
            
        }
    }
}