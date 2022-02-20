using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System.Drawing;
using System.Windows.Forms;

namespace TutorialCallouts.Callouts
{
    [CalloutInfo("IntoxicatedPerson", CalloutProbability.High)]
    public class IntoxicatedPerson : Callout
    {
        private Ped suspect;
        private Vector3 spawnPoint;
        private Blip blip;
        private bool convoStarted;
        private int counter;

        public override bool OnBeforeCalloutDisplayed()
        {
            spawnPoint = new Vector3(2760.251f, 3472.108f, 55.26629f);
            ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 30f);
            AddMinimumDistanceCheck(30f, spawnPoint);

            CalloutMessage = "Intoxicated person in public";
            CalloutPosition = spawnPoint;
            CalloutAdvisory = "Investigate the scene";
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_DISTURBING_THE_PEACE_01 IN_OR_ON_POSITION", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspect = new Ped(spawnPoint, 66.64632f);
            suspect.IsPersistent = true;
            suspect.BlockPermanentEvents = true;

            suspect.Inventory.GiveNewWeapon("WEAPON_KNIFE", -1, true);

            blip = suspect.AttachBlip();
            blip.Color = Color.Red;
            blip.IsRouteEnabled = true;

            convoStarted = false;
            counter = 0;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (Game.LocalPlayer.Character.DistanceTo(suspect) <= 10f)
            {
                if (!convoStarted)
                {
                    Game.DisplayHelp("Press '" + Main.interactionKey + "' to advance conversation", false);
                    if (blip.Exists())
                        blip.Delete();
                    Game.DisplaySubtitle("Suspect: What do you want?");
                    convoStarted = true;
                }

                if (Game.IsKeyDown(Main.interactionKey))
                    counter++;

                switch (counter)
                {
                    case 1:
                        Game.DisplaySubtitle(Main.playerName + ": I received a report of an intoxicated person in the area. Would that be you?");
                        break;
                    case 2:
                        Game.DisplaySubtitle("Suspect: Ah get outta here..");
                        break;
                    case 3:
                        Game.DisplaySubtitle(Main.playerName + ": Do you have any ID on you?");
                        break;
                    case 4:
                        Game.DisplaySubtitle("Suspect: I told you to leave!");
                        suspect.Tasks.FightAgainst(Game.LocalPlayer.Character);
                        break;
                }
            }

            if (suspect.IsCuffed || suspect.IsDead || Game.LocalPlayer.Character.IsDead || !suspect.Exists())
                End();
        }

        public override void End()
        {
            base.End();

            if (suspect.Exists())
                suspect.Dismiss();

            if (blip.Exists())
                blip.Delete();

            Game.LogTrivial("TutorialCallouts - Intoxicated Person cleaned up.");
        }
    }
}
