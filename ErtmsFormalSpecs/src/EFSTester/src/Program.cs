// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using DataDictionary;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using DataDictionary.Tests.Runner.Events;
using Utils;

namespace EFSTester
{
    internal class Program
    {
        /// <summary>
        ///     Perform all functional tests defined in the .EFS file provided
        /// </summary>
        /// <param name="args"></param>
        /// <returns>the error code of the program</returns>
        private static int Main(string[] args)
        {
            int retVal = 0;

            EFSSystem efsSystem = EFSSystem.INSTANCE;
            try
            {
                Console.Out.WriteLine("EFS Tester");

                // Load the dictionaries provided as parameters
                Util.PleaseLockFiles = false;
                foreach (string arg in args)
                {
                    Console.Out.WriteLine("Loading dictionary " + arg);

                    Dictionary dictionary = Util.load(efsSystem, new Util.LoadParams(arg)
                    {
                        LockFiles = false,
                        Errors = null,
                        UpdateGuid = false,
                        ConvertObsolete = false
                    });
                    if (dictionary == null)
                    {
                        Console.Out.WriteLine("Cannot load dictionary " + arg);
                        return -1;
                    }
                }

                // Perform functional test for each loaded dictionary
                foreach (Dictionary dictionary in efsSystem.Dictionaries)
                {
                    Console.Out.WriteLine("Processing tests from dictionary " + dictionary.Name);
                    foreach (Frame frame in dictionary.Tests)
                    {
                        Console.Out.WriteLine("Executing frame " + frame.FullName);
                        foreach (SubSequence subSequence in frame.SubSequences)
                        {
                            Console.Out.WriteLine("Executing sub sequence " + subSequence.FullName);
                            if (subSequence.getCompleted())
                            {
                                if (dictionary.TranslationDictionary != null)
                                {
                                    Console.Out.WriteLine("  -> Translating sub sequence ");
                                    subSequence.Translate(dictionary.TranslationDictionary);
                                }

                                Runner runner = new Runner(subSequence, false, false, true);
                                runner.RunUntilStep(null);

                                bool failed = false;
                                foreach (ModelEvent evt in runner.FailedExpectations())
                                {
                                    Expect expect = evt as Expect;
                                    if (expect != null)
                                    {
                                        string message = expect.Message.Replace('\n', ' ');
                                        TestCase testCase = EnclosingFinder<TestCase>.find(expect.Expectation);
                                        if (testCase.ImplementationCompleted)
                                        {
                                            Console.Out.WriteLine(" failed (unexpected) :" + message);
                                            failed = true;
                                        }
                                        else
                                        {
                                            Console.Out.WriteLine(" failed (expected) : " + message);
                                        }
                                    }
                                    else
                                    {
                                        ModelInterpretationFailure modelInterpretationFailure =
                                            evt as ModelInterpretationFailure;
                                        if (modelInterpretationFailure != null)
                                        {
                                            Console.Out.WriteLine(" failed : " + modelInterpretationFailure.Message);
                                            failed = true;
                                        }
                                    }
                                }

                                if (failed)
                                {
                                    Console.Out.WriteLine("  -> Failed");
                                    retVal = -1;
                                }
                                else
                                {
                                    Console.Out.WriteLine("  -> Success");
                                }
                            }
                            else
                            {
                                Console.Out.WriteLine("  -> Not executed because it is not marked as completed");
                            }
                        }
                    }
                }
            }
            finally
            {
                Util.UnlockAllFiles();
                efsSystem.Stop();
            }

            return retVal;
        }
    }
}