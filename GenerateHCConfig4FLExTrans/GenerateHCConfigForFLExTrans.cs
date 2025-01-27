// Copyright (c) 2016-2023 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

using System;
using System.IO;
using SIL.FieldWorks.Common.FwUtils;
using SIL.FieldWorks.WordWorks.Parser;
using SIL.LCModel;
using SIL.LCModel.Utils;
using SIL.Machine.Annotations;
using SIL.Machine.Morphology.HermitCrab;
using SIL.WritingSystems;

namespace SIL.GenerateHCConfigForFLExTrans
{
    public class GenerateHCConfigForFLExTrans
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                WriteHelp();
                return 0;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("The FieldWorks project file could not be found.");
                return 1;
            }
            return Generate(args[0], args[1]);
        }

        public static int Generate(string flexDB, string hcXML)
        {
            FwRegistryHelper.Initialize();
            FwUtils.InitializeIcu();
            Sldr.Initialize();
            var synchronizeInvoke = new SingleThreadedSynchronizeInvoke();

            var projectId = new ProjectIdentifier(flexDB);
            var logger = new ConsoleLogger(synchronizeInvoke);
            var dirs = new NullFdoDirectories();
            var settings = new LcmSettings { DisableDataMigration = true };
            var progress = new NullThreadedProgress(synchronizeInvoke);
            Console.WriteLine("Loading FieldWorks project...");
            try
            {
                using (
                    LcmCache cache = LcmCache.CreateCacheFromExistingData(
                        projectId,
                        "en",
                        logger,
                        dirs,
                        settings,
                        progress
                    )
                )
                {
                    Language language = HCLoaderForFLExTrans.Load(cache, logger);
                    Console.WriteLine("Loading completed.");
                    Console.WriteLine("Writing HC configuration file...");
                    XmlLanguageWriter.Save(language, hcXML);
                    Console.WriteLine("Checking for duplicate glosses.");
                    var dupChecker = new DuplicateGlossChecker(hcXML);
                    dupChecker.ReportAnyDuplicateGlosses();
                    Console.WriteLine("Writing completed.");
                }
                return 0;
            }
            catch (LcmFileLockedException)
            {
                Console.WriteLine("Loading failed.");
                Console.WriteLine(
                    "The FieldWorks project is currently open in another application."
                );
                Console.WriteLine("Close the application and try to run this command again.");
                return 2;
            }
            catch (LcmDataMigrationForbiddenException)
            {
                Console.WriteLine("Loading failed.");
                Console.WriteLine(
                    "The FieldWorks project was created with an older version of FLEx."
                );
                Console.WriteLine(
                    "Migrate the project to the latest version by opening it in FLEx."
                );
                return 3;
            }
        }

        private static void WriteHelp()
        {
            Console.WriteLine(
                "Generates a HermitCrab configuration file from a FieldWorks project, suitable for using HermitCrab synthesis with FLExTrans."
            );
            Console.WriteLine();
            Console.WriteLine("generatehcconfig <input-project> <output-config>");
            Console.WriteLine();
            Console.WriteLine("  <input-project>  Specifies the FieldWorks project path.");
            Console.WriteLine("  <output-config>  Specifies the HC configuration path.");
        }
    }
}
