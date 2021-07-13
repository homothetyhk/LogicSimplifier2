using System;
using System.Collections.Generic;

namespace LogicSimplifier2
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlLoader.Load(out string[] settings, out Dictionary<string, string> macros,
            out Dictionary<string, string> waypointLogic, out Dictionary<string, string> locationLogic);
            var settingsBools = Prompts.SelectSettingsPrompt(settings);
            var lp = new LogicProcessor(macros, settingsBools);
            var lm = lp.GetLogicManager(locationLogic, waypointLogic);
            Prompts.SelectTermOrderPrompt(lm);
            var ws = lm.GetSolver();
            Prompts.StartSolverPrompt(ws);
            ws.Solve();
            ws.ApplyToLocations(lm.relLocationLogic);
            Console.ReadLine();
        }
    }
}
