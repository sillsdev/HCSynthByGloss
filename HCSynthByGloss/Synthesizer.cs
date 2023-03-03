// Copyright (c) 2023 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using SIL.Machine.Morphology;
using SIL.Machine.Morphology.HermitCrab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIL.HCSynthByGloss
{
    public class Synthesizer
    {
        private static readonly Synthesizer instance = new Synthesizer();

        public static Synthesizer Instance
        {
            get
            {
                return instance;
            }
        }

        public string SynthesizeGlosses(string glosses, Morpher morpher)
        {
            StringBuilder sb = new StringBuilder();
            var analysesCreator = AnalysesCreator.Instance;
            var formatter = SynthesizedWordFomatter.Instance;
            int indexCaret = glosses.IndexOf("^");
            int indexBeg = glosses.IndexOf("^");
            int indexEnd = glosses.IndexOf("$");
            while (indexCaret >= 0 && indexBeg >= 0 && indexEnd >= 0)
            {
                string analysis = glosses.Substring(indexBeg, (indexEnd - indexBeg) + 1);
                List<IMorpheme> morphemes = analysesCreator.ExtractMorphemes(analysis, morpher);
                WordAnalysis wordAnalysis = new WordAnalysis(morphemes, 1, analysesCreator.category);
                var newSyntheses = morpher.GenerateWords(wordAnalysis);
                string result = formatter.Format(newSyntheses, analysis);
                sb.Append(result);
                sb.Append("\n");
                int lastIndexEnd = indexEnd;
                indexCaret = glosses.Substring(lastIndexEnd).IndexOf("^");
                indexBeg = indexCaret + lastIndexEnd;
                indexEnd = glosses.Substring(lastIndexEnd + 1).IndexOf("$") + lastIndexEnd + 1;
            }
            return sb.ToString();
        }
    }
}
