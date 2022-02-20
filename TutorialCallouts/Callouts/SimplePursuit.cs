using LSPD_First_Response.Mod.API;
using LSPD_First_Response.Mod.Callouts;
using Rage;
using System.Drawing;

namespace TutorialCallouts.Callouts
{
    [CalloutInfo("SimplePursuit", CalloutProbability.High)]
    public class SimplePursuit : Callout
    {
        private Vector3 spawnPoint;
        private Blip blip;
        private Ped suspect;
        private Vehicle suspectVehicle;
        private LHandle pursuit;
        private bool pursuitCreated;
        
        public override bool OnBeforeCalloutDisplayed()
        {
            spawnPoint = World.GetRandomPositionOnStreet();
            ShowCalloutAreaBlipBeforeAccepting(spawnPoint, 30f);
            AddMinimumDistanceCheck(30f, spawnPoint);

            CalloutMessage = "Pursuit!";
            CalloutPosition = spawnPoint;
            CalloutAdvisory = "There is a pursuit happening!";
            Functions.PlayScannerAudioUsingPosition("WE_HAVE CRIME_RESISTING_ARREST_02 IN_OR_ON_POSITION", spawnPoint);

            return base.OnBeforeCalloutDisplayed();
        }

        public override bool OnCalloutAccepted()
        {
            suspectVehicle = new Vehicle("WASHINGTON", spawnPoint);
            suspectVehicle.IsPersistent = true;

            suspect = new Ped(suspectVehicle.GetOffsetPositionFront(5f));
            suspect.IsPersistent = true;
            suspect.BlockPermanentEvents = true;
            suspect.WarpIntoVehicle(suspectVehicle, -1);

            blip = suspect.AttachBlip();
            blip.Color = Color.Red;
            blip.IsRouteEnabled = true;

            pursuitCreated = false;

            return base.OnCalloutAccepted();
        }

        public override void Process()
        {
            base.Process();

            if (!pursuitCreated && Game.LocalPlayer.Character.DistanceTo(suspectVehicle) <= 20f)
            {
                blip.Delete();
                pursuit = Functions.CreatePursuit();
                Functions.AddPedToPursuit(pursuit, suspect);
                Functions.SetPursuitIsActiveForPlayer(pursuit, true);
                pursuitCreated = true;
            }

            if (pursuitCreated && !Functions.IsPursuitStillRunning(pursuit))
            {
                End();
            }
        }

        public override void End()
        {
            base.End();

            if (suspect.Exists())
                suspect.Dismiss();

            if (blip.Exists())
                blip.Delete();

            if (suspectVehicle.Exists())
                suspectVehicle.Dismiss();

            Game.LogTrivial("TutorialCallouts - Simple Pursuit cleaned up.");
        }
    }
}
