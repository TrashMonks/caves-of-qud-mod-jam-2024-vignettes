// note from gnarf: I wrote this in like 1 hour at midnight, I'm not proud of it
// open to PR's to clean up syntax/code organization of this little tool

using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;
using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [HasModSensitiveStaticCache]
    public class ModJam_Wish_Handler : IPlayerPart
    {
        public static string COMMAND_NAME = "ModJam_Wish_Menu";

        public class WishCommandXML
        {
            public string DisplayName = "default";
            public List<string> Commands;
            public string Author;
            public string ModName;
            public IRenderable Renderable;
            public string DisplayText
            {
                get
                {
                    string result = $"{DisplayName}\n";
                    if (!(string.IsNullOrEmpty(Author) || string.IsNullOrEmpty(ModName)))
                    {
                        result =
                            $"{result}{Markup.Color("c", $"- From \"{ModName}\" by {Author}")}\n";
                    }

                    foreach (var command in Commands)
                        result = $"{result}{Markup.Color("K", $"- wish: {command}")}\n";
                    return result;
                }
            }
        }

        [ModSensitiveStaticCache]
        public static List<WishCommandXML> MenuItems = new List<WishCommandXML>();

        [ModSensitiveCacheInit]
        public static void Init()
        {
            if (!AbilityManager.WorldMapAllowed.Contains(COMMAND_NAME))
            {
                AbilityManager.WorldMapAllowed.Add(COMMAND_NAME);
            }
        }

        public static void CacheInit()
        {
            MenuItems ??= new List<WishCommandXML>();
            if (MenuItems.Count == 0)
            {
                foreach (var stream in DataManager.YieldXMLStreamsWithRoot("wishcommands"))
                {
                    stream.HandleNodes(nodes);
                }
            }
        }

        public static void HandleNodes(XmlDataHelper xml) => xml.HandleNodes(nodes);

        public static Dictionary<string, Action<XmlDataHelper>> nodes = new Dictionary<
            string,
            Action<XmlDataHelper>
        >
        {
            { "wishcommands", HandleNodes },
            { "wish", HandleWishNode },
        };

        // also for 204.x compat - remove in 206.x there is default
        private static List<string> parseCSV(string s) => s.Split(',').ToList();

        /// <summary>
        ///  A really quiet attempt at creating a blueprint hopefully.  The normal createSample throws loud errors
        /// </summary>
        private static bool TryCreateSample(string blueprint, out GameObject result)
        {
            result = null;
            try {
                result = GameObjectFactory.Factory.CreateObject(
                    blueprint,
                    BonusModChance: -9999,
                    Context: "Sample"
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void HandleWishNode(XmlDataHelper xml)
        {
            WishCommandXML data = new WishCommandXML();
            data.Renderable = Renderable.UITile("empty", 'k', 'k');
            data.Commands = xml.ParseAttribute(
                "Commands",
                data.Commands,
                required: true,
                parse: parseCSV
            );
            var firstCommand = data.Commands[0];
            try
            {
                if (firstCommand.StartsWith("goto:"))
                {
                    var location = firstCommand.Substring(5);
                    var z = The.ZoneManager.GetZone(location);
                    var world = The.ZoneManager.GetZone(z.GetZoneWorld());
                    var cell = world.GetCell(z.wX, z.wY);
                    var tile = cell.GetFirstObjectWithPart("TerrainTravel");
                    data.DisplayName = $"Teleport to {tile.the}{tile.DisplayName}";
                    data.Renderable = new Renderable(tile.RenderForUI());
                }
                else
                {
                    if (firstCommand.StartsWith("item:"))
                    {
                        firstCommand = firstCommand.Substring(5);
                    }
                    TryCreateSample(firstCommand, out var sample);
                    data.DisplayName = $"Spawn {sample.an(AsIfKnown: true)}";
                    data.Renderable = new Renderable(sample.RenderForUI());
                }
            }
            catch (Exception)
            {
                // xml.HandleException(e);
            }
            data.DisplayName = xml.ParseAttribute("DisplayName", data.DisplayName);
            data.Author = xml.ParseAttribute("Author", data.Author, required: false);
            data.ModName = xml.ParseAttribute("ModName", data.ModName, required: false);
            var icon = xml.ParseAttribute<string>("Icon", null);
            if (!string.IsNullOrEmpty(icon))
            {
                var colors = xml.ParseAttribute<string>("ColorPair", "yw");
                data.Renderable = Renderable.UITile(icon, colors[0], colors[1]);
            }
            // if (string.IsNullOrEmpty(data.DisplayName))
            // {

            // }
            MenuItems.Add(data);
            xml.DoneWithElement();
        }


        [WishCommand("menu")]
        public static void WishMenu()
        {
            CacheInit();
            var choice = Popup.ShowOptionList(
                "Wish Menu",
                MenuItems.Select(a => a.DisplayText).ToArray(),
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

        [WishCommand("modjam_revealandgoto")]
        public static void RevealAndGoto(string rest)
        {
            var note = Qud.API.JournalAPI.GetMapNote(rest);
            if (note != null)
            {
                if (!note.Revealed)
                {
                    Qud.API.JournalAPI.RevealMapNote(note);
                }
                XRL.World.Capabilities.Wishing.HandleWish(The.Player, $"goto:{note.ZoneID}");
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == CommandEvent.ID;
        }

        public override bool HandleEvent(CommandEvent E)
        {
            if (E.Command == COMMAND_NAME) {
                WishMenu();
            }
            return base.HandleEvent(E);
        }
    }
}

[PlayerMutator]
public class ModJam_PlayerMutator : IPlayerMutator
{
    public void mutate(GameObject player)
    {
        // modify the player object when a New Game begins
        // for example, add a custom part to the player:
        player.RequirePart<XRL.World.Parts.ModJam_Wish_Handler>();
    }
}
