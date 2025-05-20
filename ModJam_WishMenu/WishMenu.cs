using System;
using System.Collections.Generic;
using System.Linq;
using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [HasModSensitiveStaticCache]
    public class Vignettes_ModJam_Wish_Handler : IPlayerPart
    {
        public const string XML_ROOT = "vignetteswishcommands";

        public static string COMMAND_NAME = "CmdVignettesWishMenu";

        [ModSensitiveStaticCache]
        public static List<WishMenu.WishCommandXML> MenuItems = new();

        public static void CacheInit()
        {
            MenuItems ??= new();
            if (MenuItems.Count == 0)
            {
                var Existing = WishMenu.MenuItems;

                try
                {
                    WishMenu.MenuItems = MenuItems;

                    foreach (var stream in DataManager.YieldXMLStreamsWithRoot(XML_ROOT))
                    {
                        stream.HandleNodes(nodes);
                    }
                }
                finally
                {
                    WishMenu.MenuItems = Existing;
                }
            }
        }

        public static void HandleNodes(XmlDataHelper xml) => xml.HandleNodes(nodes);

        public static Dictionary<string, Action<XmlDataHelper>> nodes = new Dictionary<
            string,
            Action<XmlDataHelper>
        >
        {
            { XML_ROOT, HandleNodes },
            { "wish", Parts.WishMenu.HandleWishNode },
        };

        [WishCommand("vignetteswishmenu")]
        public static void RunWishMenu()
        {
            CacheInit();
            var choice = Popup.PickOption(
                "Vignettes Mash-Up Wish Menu",
                Options: MenuItems.Select(a => a.DisplayText).ToArray(),
                AllowEscape: true,
                Icons: MenuItems.Select(a => a.Renderable).ToArray()
            );
            if (choice >= 0)
            {
                foreach (var command in MenuItems[choice].Commands)
                {
                    XRL.World.Capabilities.Wishing.HandleWish(The.Player, command);
                }
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == CommandEvent.ID;
        }

        public override bool HandleEvent(CommandEvent E)
        {
            if (E.Command == COMMAND_NAME) {
                RunWishMenu();
            }
            return base.HandleEvent(E);
        }
    }
}

[PlayerMutator]
public class Vignettes_ModJam_PlayerMutator : IPlayerMutator
{
    public void mutate(GameObject player)
    {
        player.RequirePart<XRL.World.Parts.Vignettes_ModJam_Wish_Handler>();
    }
}
