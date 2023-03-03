// Copyright (c) 2023 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIL.Machine.Morphology;
using SIL.Machine.Morphology.HermitCrab;
using SIL.HCSynthByGloss;
using System.Reflection;
using System.IO;

namespace SIL.HCSynthByGlossTest
{
    public class HCSynthByGlossTest
    {
        Morpher morpher = null;
        Language synLang;
        TraceManager hcTraceManager;
        string glosses = "";
        string glossFile = "";
        string expectedWordFormsFile = "";

        [SetUp]
        public void Setup()
        {
            Uri uriBase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var rootdir = Path.GetDirectoryName(Uri.UnescapeDataString(uriBase.AbsolutePath));
            int i = rootdir.LastIndexOf("HCSynthByGloss");
            string basedir = rootdir.Substring(0, i);
            string testDataDir = Path.Combine(basedir, "HCSynthByGloss", "TestData");
            string hcConfig = Path.Combine(testDataDir, "indoHC4FLExrans.xml");
            glossFile = Path.Combine(testDataDir, "IndonesianAnalyses.txt");
            expectedWordFormsFile = Path.Combine(testDataDir, "expectedWordForms.txt");
            synLang = XmlLanguageLoader.Load(hcConfig);
            hcTraceManager = new TraceManager();
            morpher = new Morpher(hcTraceManager, synLang);
        }

        [Test]
        public void AnalysesCreatorTest()
        {
            var creator = AnalysesCreator.Instance;
            string analysis = "^<AV>ajar1.1<v>$";
            List<IMorpheme> morphemes = creator.ExtractMorphemes(analysis, morpher);
            Assert.AreEqual(2, morphemes.Count);
            Assert.AreEqual("AV", morphemes.ElementAt(0).Gloss);
            Assert.AreEqual("ajar1.1", morphemes.ElementAt(1).Gloss);
            Assert.AreEqual("v", creator.category);


        }

        [Test]
        public void SynthesizerTest()
        {
            var synthesizer = Synthesizer.Instance;
            glosses = "";
            string synthesizedWordForms = synthesizer.SynthesizeGlosses(glosses, morpher);
            Assert.AreEqual("", synthesizedWordForms);

            glosses = File.ReadAllText(glossFile, Encoding.UTF8);
            Assert.AreEqual(1089, glosses.Length);
            synthesizedWordForms = synthesizer.SynthesizeGlosses(glosses, morpher);
            string expectedWordForms = File.ReadAllText(expectedWordFormsFile, Encoding.UTF8);
            Assert.AreEqual(expectedWordForms, synthesizedWordForms);
        }

    }
}
