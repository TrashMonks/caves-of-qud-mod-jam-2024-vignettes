using XRL;
using XRL.UI;
using XRL.Wish;

namespace Kernelmethod.Vignettes2024 {
    [HasWishCommand]
    public class WishHandler {
        [WishCommand(Command = "kernelmethod_vignettes2024")]
        public static bool WishCommand(string rest) {
            switch (rest) {
                case "goto":
                    The.Player.ZoneTeleport(The.Game.GetStringGameState("Kernelmethod_Vignettes2024_MechLairZoneID"));
                    break;
                default:
                    Popup.ShowFail($"Unknown wish command for Kernelmethod.Vignettes2024: {rest}");
                    break;
            }

            return true;
        }
    }
}
