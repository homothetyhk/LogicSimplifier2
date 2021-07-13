using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicSimplifier2
{
    public class LogicProcessor
    {
        Dictionary<string, string[]> macros = new();
        Dictionary<string, bool> settings;

        public LogicProcessor(Dictionary<string, string> macros, Dictionary<string, bool> settings)
        {
            this.settings = settings;
            foreach (var kvp in macros)
            {
                try
                {
                    this.macros.Add(kvp.Key, Shunt(kvp.Value));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Invalid logic for macro {kvp.Key}:\n{e}");
                    throw;
                }
            }
        }

        public LogicManager GetLogicManager(Dictionary<string, string> locationLogic,
            Dictionary<string, string> waypointLogic)
        {
            Dictionary<string, string[][]> processedLocs = new();
            foreach (var (name, logic) in locationLogic)
            {
                processedLocs[name] = ApplySettings(Distribute(Shunt(logic)));
            }

            Dictionary<string, string[][]> processedWays = new();
            foreach (var (name, logic) in waypointLogic)
            {
                processedWays[name] = ApplySettings(Distribute(Shunt(logic)));
            }

            return new LogicManager(processedLocs, processedWays);
        }

        public string[][] ApplySettings(string[][] logic)
        {
            return logic.Where(l => l.All(t => !settings.ContainsKey(t) || settings[t]))
                .Select(l => l.Where(t => !settings.ContainsKey(t)).ToArray()).ToArray();
        }

        public string[][] Distribute(string[] shuntedLogic)
        {
            if (shuntedLogic.Length == 0) return new string[][] { shuntedLogic };

            Stack<string[][]> sets = new();
            foreach (string t in shuntedLogic)
            {
                switch (t)
                {
                    default:
                        sets.Push(new[] { new[] { t } });
                        break;
                    case "+":
                        {
                            var aa = sets.Pop();
                            var bb = sets.Pop();
                            var cc = from a in aa
                                     from b in bb
                                     select a.Concat(b).ToArray();
                            sets.Push(cc.ToArray());
                        }
                        break;
                    case "|":
                        sets.Push(sets.Pop().Concat(sets.Pop()).ToArray());
                        break;
                }
            }

            return sets.Pop();
        }

        public string[] Shunt(string infix)
        {
            int i = 0;
            Stack<string> stack = new Stack<string>();
            List<string> postfix = new List<string>();

            while (i < infix.Length)
            {
                string op = GetNextOperator(infix, ref i);

                // Easiest way to deal with whitespace between operators
                if (op.Trim() == string.Empty)
                {
                    continue;
                }

                if (op == "+" || op == "|")
                {
                    while (stack.Count != 0 && (op == "|" || op == "+" && stack.Peek() != "|") && stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }

                    stack.Push(op);
                }
                else if (op == "(")
                {
                    stack.Push(op);
                }
                else if (op == ")")
                {
                    while (stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }

                    stack.Pop();
                }
                else
                {
                    // Parse macros
                    if (macros.TryGetValue(op, out string[] macro))
                    {
                        postfix.AddRange(macro);
                    }
                    else
                    {
                        postfix.Add(op);
                    }
                }
            }

            while (stack.Count != 0)
            {
                postfix.Add(stack.Pop());
            }

            return postfix.ToArray();
        }

        private static string GetNextOperator(string infix, ref int i)
        {
            int start = i;

            if (infix[i] == '(' || infix[i] == ')' || infix[i] == '+' || infix[i] == '|')
            {
                i++;
                return infix[i - 1].ToString();
            }

            while (i < infix.Length && infix[i] != '(' && infix[i] != ')' && infix[i] != '+' && infix[i] != '|')
            {
                i++;
            }

            return infix.Substring(start, i - start).Trim();
        }

    }
}
