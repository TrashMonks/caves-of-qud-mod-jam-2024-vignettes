using Genkit;
using System;
using System.Linq;
using System.Collections.Generic;
using static Qud.API.EncountersAPI;
using XRL.Rules;
using NPRand = NPRandom.Sarc_Random;
using XRL.World.AI;

namespace XRL.World.Parts
{
    [Serializable]

    public class Sarcose_Yurlfriend : IPart
    {

    }

    public class Sarcose_Kill_Neuroparents : IPart
    {

    }

    public class Sarcose_Has_Animate_Ally : IPart
    {

    }

    public class FreedBySafra : IPart
    {
        public int num;
        public bool isSetup = false;

        public FreedBySafra(){}
        public FreedBySafra(int num){
            this.num = num;
        }

        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID || ID == EarlyBeforeDeathRemovalEvent.ID;
        public override bool HandleEvent(EndTurnEvent E){
            if(!isSetup){
                string convo = "Sarcose_SafraAnimate";
                if (num < 7){
                    convo = $"{convo}{num}";
                }else{
                    convo = $"{convo}{NPRand.Next(1,6)}";
                }

                ParentObject.RemovePart("ConversationScript");
                ParentObject.AddPart(new ConversationScript(convo, ClearLost: true));
                int preachChance = 30;
                ParentObject.RemovePart("Preacher");
                if(preachChance.in100()){
                    Preacher chatter = ParentObject.AddPart<Preacher>();
                    if(NPRand.Next(0,1) == 0){
                        chatter.Prefix = ParentObject.Does("say") + ", &W'";
                        chatter.Postfix = "'";
                        chatter.ChatWait = (NPRand.Next(400,600));
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_SafraAnimateBook";
                        chatter.inOrder = true;
                    }else{
                        chatter.Prefix = "";
                        chatter.Postfix = "";
                        chatter.ChatWait = (NPRand.Next(400,600));
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_NeuroParentAnimateBookNoise";
                        chatter.inOrder = true;
                }
                isSetup = true;
                }
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(EarlyBeforeDeathRemovalEvent E){
            if(ParentObject.HasPart("Sarcose_Has_Animate_Ally")){
                ParentObject.RemovePart("Sarcose_Has_Animate_Ally");
                The.Player.RemovePart("Sarcose_Has_Animate_Ally");
            }
            return base.HandleEvent(E);
        }
    }

    public class HasAnimatedServants : IPart
    {
        public string ServantTier = "1";
        public string NumberOfServants = "1";
        public string ServantType = "Any";
        public bool ServantsPlaced;
        public bool ServantModified;
        public bool isPeaceful =  false;
        public int maxRad = 4;
        public int minRad = 1;
        public bool Safra = false;
        public Dictionary<string, int> baseAnimateWeights = new Dictionary<string, int>()
        {
            {"Walltrap", 10},
            {"Wall", 10},
            {"Chair", 80},
            {"Bed", 80},
            {"Door", 30},
            {"Container", 80}
        };
        public Dictionary<string, int> onlyOneType = new Dictionary<string, int>()
        {
            {"Wall", 5},
            {"Chair", 1},
            {"Bed", 3},
            {"Door", 6},
            {"Container", 3}
        };
        public Dictionary<string, int> toughAnimateWeights = new Dictionary<string, int>()
        {
            {"Walltrap", 10},
            {"Wall", 40},
            {"Chair", 30},
            {"Bed", 30},
            {"Door", 40},
            {"Container", 70}
        };
        [NonSerialized]
        public List<GameObject> gatheredObjects = new List<GameObject>();
        public Dictionary<string, int> weight;


        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public HasAnimatedServants (){}
        public HasAnimatedServants (string ServantTier = "1-3", string NumberOfServants = "2-4", string ServantType = "Any", bool Safra = false) : this()
        {
            this.ServantTier = ServantTier;
            this.NumberOfServants = NumberOfServants;
            this.ServantType = ServantType;
            this.Safra = Safra;
        }
        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == ObjectEnteredCellEvent.ID;

        public override bool HandleEvent(ObjectEnteredCellEvent E)
        {
            if (!ServantsPlaced)
            {

                ServantsPlaced = true;
                Cell cell = ParentObject.CurrentCell;
                Zone zone = ParentObject.CurrentZone;
                int num = NumberOfServants.RollCached();
                bool isOneType = false;
                int tier = ServantTier.RollCached();
                if (tier > 7 && !Safra){
                    if (NPRand.Next(0,100) <= 40){
                        weight = onlyOneType;
                        isOneType = true;
                    }else{
                        weight = toughAnimateWeights;
                    }
                }else if(!Safra){
                    if (NPRand.Next(0,100) <= 20){
                        weight = onlyOneType;
                        isOneType = true;
                    }else{
                        weight = baseAnimateWeights;
                    }
                }else {weight = baseAnimateWeights;}
                float previousAngle1 = 0f;
                float previousAngle2 = 0f;
                for (int i = 0; i < num; i++)
                {
                    tier = ServantTier.RollCached();
                    GameObject animatedServant = GetAnAnimatedServant(ParentObject, tier, num, gatheredObjects, weight, isOneType, Safra);
                    if (animatedServant == null)
                    {
                        UnityEngine.Debug.LogError($"GetAnAnimatedServant({ParentObject}, {tier}, {num}, {gatheredObjects}, {weight}, {isOneType}, {Safra}) returned null");
                        continue;
                    }

                    if(Safra){animatedServant.AddPart(new FreedBySafra(i));}
                    Cell randCell = GetRandCell(minRad, maxRad, zone, ref previousAngle1, ref previousAngle2);
                    if (randCell != null)
                    {
                        randCell.AddObject(animatedServant).MakeActive();
                    }
                }
                if (isPeaceful){
                    int campAmt = NPRand.Next(5,8);
                    for (int i = 0; i < campAmt; i++){
                        var campItem = PopulationManager.CreateOneFrom("Sarcose_NeuroparentCampEnvironment");
                        Cell randCell = GetRandCell(minRad/2, maxRad/2, zone, ref previousAngle1, ref previousAngle2);
                        if (randCell != null && campItem != null)
                        {
                            randCell.AddObject(campItem).MakeActive();
                        }
                    }
                }
                if (Safra){     //Safracamp?

                }
            }
            return base.HandleEvent(E);
        }
        public static Cell GetRandCell(int minRad, int maxRad, Zone zone, ref float prevAngle1, ref float prevAngle2)
        {
            var centerX = zone.Width / 2;
            var centerY = zone.Height / 2;

            float angle;
            do
            {
                angle = Stat.Random(0f, (float)(2.0 * Math.PI));
            }
            while (IsCloseToPreviousAngles(angle, prevAngle1, prevAngle2));

            prevAngle2 = prevAngle1;
            prevAngle1 = angle;

            var distance = Stat.Random(minRad, maxRad);
            int deltaX = (int)Math.Floor(distance * Math.Cos(angle));
            int deltaY = (int)Math.Floor(distance * Math.Sin(angle));
            var widgetLocation = Location2D.Get(centerX + deltaX, centerY + deltaY);

            return zone.GetCell(widgetLocation);
        }

        private static bool IsCloseToPreviousAngles(float angle, float prevAngle1, float prevAngle2, float minAngleDifference = (float)(Math.PI / 6))
        {
            float angleDifference1 = Math.Abs(prevAngle1 - angle);
            float angleDifference2 = Math.Abs(prevAngle2 - angle);

            // Ensure angles are within the range [0, 2π)
            angleDifference1 %= (float)(2.0 * Math.PI);
            angleDifference2 %= (float)(2.0 * Math.PI);

            // Find the minimum difference (considering angle wraps around)
            angleDifference1 = Math.Min(angleDifference1, (float)(2.0 * Math.PI) - angleDifference1);
            angleDifference2 = Math.Min(angleDifference2, (float)(2.0 * Math.PI) - angleDifference2);

            return angleDifference1 < minAngleDifference || angleDifference2 < minAngleDifference;
        }

        public static GameObject GetAnAnimatedServant(GameObject ParentObject, int tier, int num, List<GameObject> allObjects, Dictionary<string, int> weight, bool isOneType = false, bool Safra = false)
        {
            List<GameObjectBlueprint> list = new List<GameObjectBlueprint>(64);
            List<string> list2 = new List<string>();
            string tagToType = "";
            if (isOneType){
                    List<string> tags = new List<string>();
                    foreach (KeyValuePair<string, int> pair in weight)
                    {
                        if (pair.Value >= tier){
                            tags.Add(pair.Key);
                        }
                    }
                tagToType = tags[NPRand.Next(0,tags.Count)];
            }
            foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
            {
                if (!IsEligibleForDynamicEncounters(blueprint) || !blueprint.HasTag("Animatable") || !(blueprint.Tier == tier) || (isOneType && !blueprint.HasTag(tagToType))
                || IsOverTagWeight(blueprint, allObjects, weight) || blueprint.HasTag("BaseObject") || blueprint.HasPart("Walltrap"))   //just remove walltraps altogether for now, they're totally unbalanced
                {
                    continue;
                }
                if (blueprint.HasTag("AggregateWith"))
                {
                    string tag = blueprint.GetTag("AggregateWith");
                    if (list2.Contains(tag))
                    {
                        continue;
                    }
                    list2.Add(tag);
                }
                list.Add(blueprint);
            }
            GameObject gameObject = GameObject.Create(list.GetRandomElement().Name);
            AnimateObject.Animate(gameObject);
            if (!Safra){
                gameObject.SetAlliedLeader<AllyConstructed>(ParentObject);
                gameObject.AddPart(new MadeByAnimator(ParentObject, tier, num));
            }
            return gameObject;
        }
        public static bool IsOverTagWeight(GameObjectBlueprint blueprint, List<GameObject> allObjects, Dictionary<string,int> weight){
            string tag;
            if (blueprint.HasPart("Walltrap")){tag = "Walltrap";}
            else if (blueprint.HasTag("Wall")){tag = "Wall";}
            else if(blueprint.HasTag("Door")){tag = "Door";}
            else if(blueprint.HasTag("Chair")){tag = "Chair";}
            else if(blueprint.HasTag("Bed")){tag = "Bed";}
            else if(blueprint.HasTag("Container")){tag = "Container";}
            else{ return false;}
            int numbTags = 0;
            if (tag == "Walltrap"){
                    for (int i = 0; i < allObjects.Count; i++){
                    if (allObjects[i].HasPart(tag)){
                        numbTags++;
                    }
                }
            }else{
                for (int i = 0; i < allObjects.Count; i++){
                    if (allObjects[i].HasTag(tag)){
                        numbTags++;
                    }
                }
            }
            double percentage = (numbTags / (double)allObjects.Count) * 100;
            int currentWeight = (int)Math.Round(percentage);
            if(currentWeight > weight[tag]){ return true;}else{ return false;}
        }

    }

    public class MadeByAnimator : IPart
    {
        public int tier; public int num; public bool isSetup; GameObject Maker;
        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public MadeByAnimator(){}
        public MadeByAnimator(GameObject Maker, int tier, int num){
            this.tier = tier;
            this.num = num;
            this.Maker = Maker;
        }
        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID;
        public override bool HandleEvent(EndTurnEvent E){
            if(!isSetup){
                ParentObject.RemovePart("ConversationScript");
                ParentObject.AddPart(new ConversationScript("Sarcose_NeuroParentAnimate", ClearLost: true));
                ParentObject.RemovePart("Preacher");
                int preachChance = 30;
                ParentObject.RemovePart("Preacher");
                if(preachChance.in100()){
                    Preacher chatter = ParentObject.AddPart<Preacher>();
                    if(NPRand.Next(0,1) == 0){
                        chatter.Prefix = ParentObject.Does("say") + ", &W'";
                        chatter.Postfix = "'";
                        chatter.ChatWait = (NPRand.Next(600,800));
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_NeuroParentAnimateBook";
                        chatter.inOrder = true;
                    }else{
                        chatter.Prefix = "";
                        chatter.Postfix = "";
                        chatter.ChatWait = (NPRand.Next(600,800)) * num;
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_NeuroParentAnimateBookNoise";
                        chatter.inOrder = true;
                    }
                }

                if (Maker.TryGetPart<NeuroParentManager>(out var part)){
                    part.OwnedAnimates.Add(ParentObject);
                }
                isSetup = true;
            }
            return base.HandleEvent(E);
        }
        public void MakerDied(){
                if(ParentObject.HasPart("Preacher")){
                    ParentObject.RemovePart("Preacher");
                    Preacher chatter = ParentObject.AddPart<Preacher>();
                    if(NPRand.Next(0,1) == 0){
                        chatter.Prefix = ParentObject.Does("say") + ", &W'";
                        chatter.Postfix = "'";
                        chatter.ChatWait = (NPRand.Next(400,600)) * num;
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_NeuroParentAnimateBook_Freed";
                        chatter.inOrder = true;
                    }else{
                        chatter.Prefix = "";
                        chatter.Postfix = "";
                        chatter.ChatWait = (NPRand.Next(400,600)) * num;
                        chatter.Volume = 20;
                        chatter.Book = "Sarcose_NeuroParentAnimateBookNoise";
                        chatter.inOrder = true;
                    }
                }
            ParentObject.RemovePart("ConversationScript");
            ParentObject.AddPart(new ConversationScript("Sarcose_NeuroParentAnimate_Freed", ClearLost: true));

            ParentObject.Brain.PartyMembers.Clear();
            ParentObject.Brain.PartyLeader = null;
            ParentObject.Brain.Goals.Clear();
            ParentObject.Brain.Opinions.Clear();
            if (ParentObject.Brain.FriendlyFire != null)
            {
                ParentObject.Brain.FriendlyFire.Clear();
            }
            ParentObject.Brain.FactionFeelings.Clear();
            ParentObject.Brain.Allegiance.Clear();
            ParentObject.Brain.Allegiance.Hostile = false;
            ParentObject.Brain.Allegiance.Calm = true;
            ParentObject.StopFight();
            ParentObject.StopMoving();
            ParentObject.Brain.SetFactionFeeling("Player", 350);
            ParentObject.Brain.SetFactionMembership("Newly Sentient Beings", 100);

        }
    }

    public class NeuroParentManager : IPart
    {
        public int animateWait = 350; public int Chance = 15;
        public int lastAnimated; public bool doAnimate;
        [NonSerialized]
        public List<GameObject> OwnedAnimates = new List<GameObject>();

        public override bool WantEvent(int ID, int cascade) => base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID || ID == EarlyBeforeDeathRemovalEvent.ID;

        public override bool HandleEvent(EndTurnEvent E){
            lastAnimated--;
            if ((lastAnimated < 0 & Chance.in100()) || doAnimate)
            {
                doAnimate = true;
                if (tryToAnimate()){lastAnimated = animateWait; doAnimate = false;}
            }
            return base.HandleEvent(E);
        }

        private bool tryToAnimate(){ //[ ]TODO: tryToAnimate
            return false;
        }
        public override bool HandleEvent(EarlyBeforeDeathRemovalEvent E)
        {

            for (int i = 0; i <= OwnedAnimates.Count; i++)
            {
                GameObject obj = OwnedAnimates[i];
                if (obj.TryGetPart<MadeByAnimator>(out var part)){
                    part.MakerDied();
                }
            }
            return base.HandleEvent(E);
        }
    }

    public class NeuroparentSpawner : IPart
    {
		public override bool WantEvent(int ID, int cascade)
		{
			return base.WantEvent(ID, cascade)
			       || ID == BeforeObjectCreatedEvent.ID
				;
		}
        public override bool HandleEvent(BeforeObjectCreatedEvent E)
        {
            GameObject gameObject = CreateNPBase(ParentObject);
            gameObject.SetStringProperty("SpawnedFrom", ParentObject.Blueprint);
            E.ReplacementObject = gameObject;
            return base.HandleEvent(E);

        }
        public static HashSet<string> formerFactions = new HashSet<string> {"Mechanimist", "Templar", "Daughters", "Farmers", "Merchants", "Wardens", "Seekers", "Barathrumite", "Dromad"};
        public static List<string> epithetList = new List<string> {"the Mad","the Unhinged","the Lost","the Confused","the Inane","the Rambling","the Genius"};
        public static string pickEpithet()
        {
            return epithetList[NPRand.Next(0,epithetList.Count-1)];
        }
        public static GameObject CreateNPBase(GameObject ParentObject)
        {
            _ = ParentObject.CurrentCell;
            bool isPeaceful = (ParentObject.Blueprint == "Neuroparent_Camper");
            int tier = ParentObject.GetTag("ServantTier", "1-3").RollCached();
            GameObject baseObject = GameObject.Create(GetValidBlueprint(tier));
            AllegianceSet baseAllegiance = baseObject.Brain.GetBaseAllegiance();
            bool addFormer = baseAllegiance != null && formerFactions.Any((faction) => baseAllegiance.TryGetValue(faction, out int membership) && membership > 0);
            HasAnimatedServants animservpart = baseObject.AddPart(new HasAnimatedServants(ParentObject.GetTag("ServantTier"), ParentObject.GetTag("NumberOfServants")));
            baseObject.GiveProperName();
            if (addFormer){
                baseObject.RequirePart<Titles>().AddTitle($"former {baseObject.GetCreatureType()}");
            }else{
                baseObject.RequirePart<Titles>().AddTitle(baseObject.GetCreatureType());
            }
            baseObject.Brain.Allegiance.Clear();
		    baseObject.Brain.SetFactionMembership("Humanoids", 100);
            baseObject.RequirePart<Titles>().AddTitle(NPRand.ChooseString(new List<string>{"Neuroparent","Animator"}));
            baseObject.RequirePart<Epithets>().AddEpithet(pickEpithet());
            baseObject.RequirePart<DisplayNameColor>().SetColor("Y"); //TODO: check this
            baseObject.Render.ColorString = "&Y"; //TODO: color
            baseObject.Render.TileColor = "&C";
            baseObject.Render.DetailColor = "r";
            if (baseObject.BaseStat("Intelligence") < 20)
            {
                baseObject.GetStat("Intelligence").BaseValue = 20;
            }
            if (baseObject.BaseStat("Ego") < 20)
            {
                baseObject.GetStat("Ego").BaseValue = 20;
            }
            baseObject.RemovePart("AISItting");   //should I check
            baseObject.RemovePart("Chat");
            baseObject.RemovePart("ConversationScript");
            baseObject.RemovePart("Miner");
            baseObject.RemovePart("TurretTinker");
            if(tier < 6){
			    baseObject.ReceiveObject("Neuroparent_Spray-a-Brain");
            }else{
			    baseObject.ReceiveObject("Neuroparent_NanoNeuro");
            }
            NeuroParentManager manager = baseObject.AddPart<NeuroParentManager>();
            if (!isPeaceful){
                baseObject.AddPart(new ConversationScript("Sarcose_NeuroParent", ClearLost: true));
                (baseObject.GetPart("Description") as Description).Short = "An armchair lurches to life. A door ponders aloud what it means to open itself. A thick wall trundles forth with blind abandon. And at the nucleus, =pronouns.subjective= regards =pronouns.possessive= 'children' with a loving, mad gaze. Wearing a labcoat covering up the tattered cultural remnants of =pronouns.possessive= past, this rambling Animator long ago abandoned =pronouns.possessive= own culture, seeking instead the echoes of a naive crowd of crackling, creaking, metallic, squeaking voices built entirely of =pronouns.possessive= inane whim.";
            }
            else{
                (baseObject.GetPart("Description") as Description).Short = "Situated in amongst a low rumble of murmuring walls and lurching furniture, there hunches a weary, old figure. =pronouns.subjective= regards =pronouns.possessive= creations with a loving comfort, and eyes you with suspicion and curiosity. Whatever light was present in those eyes has long ago dimmed, and now what you see is entirely inscrutable. You approach, aware that at any moment, the walls may literally close in around you.";
                animservpart.maxRad = 10;
                animservpart.minRad = 3;
                animservpart.isPeaceful = true;
                baseObject.ReceiveObject("Neuroparent_Notepage");
                baseObject.StopFight();
                baseObject.Brain.Allegiance.Hostile = false;
                baseObject.Brain.Allegiance.Calm = true;
                baseObject.Brain.SetFactionFeeling("Player",200);
                baseObject.Brain.Allegiance.Clear();
                baseObject.Brain.SetFactionMembership("Pariahs",100);
                baseObject.AddPart(new ConversationScript("Sarcose_NeuroParentCamper", ClearLost: true));
            }


            return baseObject;
        }

        public static string GetValidBlueprint(int tier = 2)
        {
            List<GameObjectBlueprint> list = new List<GameObjectBlueprint>(32);
            foreach (GameObjectBlueprint blueprint in GameObjectFactory.Factory.BlueprintList)
            {
                if (!IsEligibleForDynamicEncounters(blueprint) || !IsLegendaryEligible(blueprint) || !blueprint.HasPart("Body") || !blueprint.HasPart("Combat") || !blueprint.HasTag("Humanoid") || blueprint.HasTag("Merchant") || !(blueprint.Tier == tier))
                {
                    continue;
                }
                list.Add(blueprint);
            }
            if (list.Count <= 0)
            {
                return null;
            }
            return list.GetRandomElement().Name;
        }
    }

}
