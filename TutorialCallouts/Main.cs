using LSPD_First_Response.Mod.API;
using Rage;
using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TutorialCallouts
{
    public class Main : Plugin
    {
        private IniFile iniFile;
        public static string playerName;
        public static Keys interactionKey;

        public override void Initialize()
        {
            iniFile = new IniFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\plugins\\LSPDFR\\TutorialCallouts.ini");
            playerName = iniFile.IniReadValue("Identifier", "Name");
            interactionKey = (Keys)Enum.Parse(typeof(Keys), iniFile.IniReadValue("Keys", "Interact"));

            Game.LogTrivial("TutorialCallouts version 1.0.0 by SneakySteve has been initialized!");
        }

        public override void Finally()
        {
            Game.LogTrivial("TutorialCallouts version 1.0.0 by SneakySteve has been cleaned up!");
        }

        private static void OnOnDutyStateChangedHandler(bool OnDuty)
        {
            if (!OnDuty) return;
            RegisterCallouts();
            Game.DisplayNotification("TutorialCallouts version 1.0.0 has loaded successfully!");
        }

        private static void RegisterCallouts()
        {
            Functions.RegisterCallout(typeof(Callouts.SimplePursuit));
            Functions.RegisterCallout(typeof(Callouts.IntoxicatedPerson));
        }

        public static Assembly LSPDFRResolveEventHandler(object sender, ResolveEventArgs args)
        {
            return Functions.GetAllUserPlugins().FirstOrDefault(assembly => args.Name.ToLower().Contains(assembly.GetName().Name.ToLower()));
        }

        public static bool IsLSPDFRPluginRunning(string Plugin, Version minversion = null)
        {
            return Functions.GetAllUserPlugins()
                .Select(assembly => assembly.GetName())
                .Where(an => an.Name.ToLower() == Plugin.ToLower())
                .Any(an => minversion == null || an.Version.CompareTo(minversion) >= 0);
        }
    }
}