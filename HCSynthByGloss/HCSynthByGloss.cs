// Copyright (c) 2022-2023 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SIL.Machine.Morphology;
using SIL.Machine.Morphology.HermitCrab;
using SIL.Machine.Morphology.HermitCrab.MorphologicalRules;
using SIL.Machine.Translation;

namespace SIL.HCSynthByGloss
{
    class HCSynthByGloss
    {
        static void Main(string[] args)
        {
            if (args.Count() != 6 || args[0] != "-h" || args[2] != "-g" || args[4] != "-o")
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("HCSynthByGloss -h HC.xml_file -g gloss_file -o output");
                Environment.Exit(1);
            }
            if (!File.Exists(args[1]))
            {
                Console.WriteLine("Could not find file '" + args[1] + "'.");
                Environment.Exit(2);
            }
            if (!File.Exists(args[3]))
            {
                Console.WriteLine("Could not find file '" + args[3] + "'.");
                Environment.Exit(3);
            }

            Language synLang = XmlLanguageLoader.Load(args[1]);
            var hcTraceManager = new TraceManager();
            //hcTraceManager.IsTracing = true;
            var srcMorpher = new Morpher(hcTraceManager, synLang);
            string glosses = File.ReadAllText(args[3], Encoding.UTF8);
            var synthesizer = Synthesizer.Instance;
            string synthesizedWordForms = synthesizer.SynthesizeGlosses(glosses, srcMorpher);
            File.WriteAllText(args[5], synthesizedWordForms, Encoding.UTF8);
        }
    }
}
