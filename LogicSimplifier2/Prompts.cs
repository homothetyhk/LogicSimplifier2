using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier2
{
    public static class Prompts
    {
        public static Dictionary<string, bool?> SelectSettingsPrompt(string[] settings)
        {
            Console.WriteLine("Would you like to select settings? [y/n]");
            while (true)
            {
                string c = Console.ReadLine();
                if (c == "n") return new();
                if (c == "y") break;
            }

            Dictionary<string, bool?> settingBools = settings
                .ToDictionary<string, string, bool?>(s => s, s => false);
            void Print()
            {
                Console.Clear();
                for (int i = 0; i < settings.Length; i++)
                {
                    Console.WriteLine($"[{i}] {settings[i]}: {settingBools[settings[i]]?.ToString() ?? "Variable"}");
                }
                Console.WriteLine("Enter the number in brackets to toggle the corresponding setting");
                Console.WriteLine("Enter a blank line to finish.");
            }

            void Toggle(string setting)
            {
                switch (settingBools[setting])
                {
                    case null:
                        settingBools[setting] = false;
                        break;
                    case false:
                        settingBools[setting] = true;
                        break;
                    case true:
                        settingBools[setting] = null;
                        break;
                }
            }

            while (true)
            {
                Print();
                string s = Console.ReadLine();
                if (s == "") return settingBools;
                if (int.TryParse(s, out int i) && 0 <= i && i < settings.Length)
                {
                    Toggle(settings[i]);
                }
            }
        }

        public static void SelectTermOrderPrompt(LogicManager lm)
        {
            Console.Clear();
            Console.WriteLine("Select ordering for printed logic:");
            foreach (TermOrder value in Enum.GetValues<TermOrder>())
            {
                Console.WriteLine($"[{(int)value}] {value}");
            }
            Console.WriteLine("(Enter the number in brackets)");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int value))
                {
                    TermOrder to = (TermOrder)value;
                    if (Enum.IsDefined(to))
                    {
                        lm.termOrder = to;
                        return;
                    }
                }
            }
        }

        public static void StartSolverPrompt(WaypointSolver ws)
        {
            Console.Clear();
            Console.WriteLine("Please select an initial waypoint.");
            for (int i = 0; i < ws.Waypoints.Length; i++)
            {
                Console.WriteLine($"[{i}] {ws.Waypoints[i].name}");
            }
            Console.WriteLine("(Enter the number in brackets)");

            int value;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out value) && value < ws.Waypoints.Length)
                {
                    break;
                }
            }
            ws.GiveWaypoint(ws.Waypoints[value]);
        }
    }
}
