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
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
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
            bool doTracing = false;
            bool showTracing = false;
            int argCount = args.Count();
            if (argCount != 6 || args[0] != "-h" || args[2] != "-g" || args[4] != "-o")
            {
                if (argCount == 7 && args[6] == "-t")
                {
                    doTracing = true;
                }
                else if (argCount == 8 && args[6] == "-t" && args[7] == "-s")
                {
                    doTracing = true;
                    showTracing = true;
                }
                else
                {
                    Console.WriteLine("Usage:");
                    Console.WriteLine(
                        "HCSynthByGloss -h HC.xml_file -g gloss_file -o output (-t (-s))"
                    );
                    Console.WriteLine("\t-t = turn on tracing");
                    Console.WriteLine(
                        "\t-s = show the tracing result in the system default web browser; -s is only valid when also using -t"
                    );
                    Environment.Exit(1);
                }
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
            var hcTraceManager = new HcXmlTraceManager();
            hcTraceManager.IsTracing = doTracing;
            var srcMorpher = new Morpher(hcTraceManager, synLang);
            string glosses = File.ReadAllText(args[3], Encoding.UTF8);
            var synthesizer = Synthesizer.Instance;
            string synthesizedWordForms = synthesizer.SynthesizeGlosses(
                glosses,
                srcMorpher,
                synLang,
                hcTraceManager
            );
            File.WriteAllText(args[5], synthesizedWordForms, Encoding.UTF8);
            if (hcTraceManager.IsTracing)
            {
                // we want to create a temp XML file and stuff synthesizer.Trace into it
                // then transform it to an html file and show the html file
                var tempXMlResult = createXmlFile(synthesizer);
                string tempHtmResult = CreatHtmResult(tempXMlResult, synthesizer);
                if (showTracing)
                {
                    System.Diagnostics.Process.Start(tempHtmResult);
                }
            }
        }

        private static string CreatHtmResult(string xmlFile, Synthesizer synthesizer)
        {
            string tempHtmResult = Path.Combine(Path.GetTempPath(), "HCSynthTrace.htm");
            Uri uriBase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var rootdir = Path.GetDirectoryName(Uri.UnescapeDataString(uriBase.AbsolutePath));
            string basedir = rootdir;
            int i = rootdir.LastIndexOf("bin");
            if (i >= 0)
            {
                // rootdir is in development environment; adjust the value
                basedir = rootdir.Substring(0, i);
            }
            string iconPath = Path.Combine(
                basedir,
                "Language Explorer",
                "Configuration",
                "Words",
                "Analyses",
                "TraceParse"
            );
            var traceTransform = new XslCompiledTransform();
            XPathDocument doc = new XPathDocument(xmlFile);

            traceTransform.Load(Path.Combine(basedir, @"Transforms\FormatHCTrace.xsl"));
            StreamWriter result = new StreamWriter(tempHtmResult);
            XsltArgumentList argList = new XsltArgumentList();
            argList.AddParam("prmIconPath", "", iconPath);
            // we do not have access to any of the following; use defaults
            //argList.AddParam("prmAnalysisFont", "", m_language.NTFontFace);
            //argList.AddParam("prmAnalysisFontSize", "", m_language.NTFontSize.ToString());
            //argList.AddParam("prmVernacularFont", "", m_language.LexFontFace);
            //argList.AddParam("prmVernacularFontSize", "", m_language.LexFontSize.ToString());
            //argList.AddParam("prmVernacularRTL", "", m_language.NTColorName);
            argList.AddParam("prmShowTrace", "", "true");
            traceTransform.Transform(doc, argList, result);
            result.Close();
            return tempHtmResult;
        }

        private static string createXmlFile(Synthesizer synthesizer)
        {
            string tempXmlResult = Path.Combine(Path.GetTempPath(), "HCSynthTrace.xml");
            File.WriteAllText(tempXmlResult, synthesizer.Trace.ToString());
            return tempXmlResult;
        }
    }
}
