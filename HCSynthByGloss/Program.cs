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
    class Program
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
            //string hcFile = "indo-hc.xml"; // sp-hc.xml
            //Language srcLang = XmlLanguageLoader.Load("../../../data/" + hcFile);
            var srcMorpher = new Morpher(hcTraceManager, synLang);

            // create list of morphemes to add to a new word analysis
            List<IMorpheme> morphemes = new List<IMorpheme>();
            IMorpheme morph1 = srcMorpher.Morphemes.FirstOrDefault(m => m.Gloss == "AV");
            morphemes.Add(morph1);
            IMorpheme morph2 = srcMorpher.Morphemes.FirstOrDefault(m => m.Gloss == "observe");
            morphemes.Add(morph2);
            IMorpheme morph3 = srcMorpher.Morphemes.FirstOrDefault(m => m.Gloss == "Cont");
            morphemes.Add(morph3);
            IMorpheme morph4 = srcMorpher.Morphemes.FirstOrDefault(m => m.Gloss == "LOC");
            morphemes.Add(morph4);

            WordAnalysis wordAnalysis = new WordAnalysis(morphemes, 1, "cat");

            var newSyntheses = srcMorpher.GenerateWords(wordAnalysis);
            foreach (string syn in newSyntheses)
            {
                Console.WriteLine("Synthesis from glosses:");
                Console.WriteLine("\t" + syn);
            }

        }
    }
}
