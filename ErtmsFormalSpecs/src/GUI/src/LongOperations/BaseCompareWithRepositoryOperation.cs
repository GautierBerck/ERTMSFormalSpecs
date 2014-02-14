﻿// ------------------------------------------------------------------------------
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
namespace GUI.LongOperations
{
    using DataDictionary;
    using System.IO;
    using LibGit2Sharp;
    using System.Diagnostics;
    using System;
    using System.Windows.Forms;

    public abstract class BaseCompareWithRepositoryOperation : BaseLongOperation
    {
        /// <summary>
        /// The dictionary on which the comparison operation is performed
        /// </summary>
        protected Dictionary Dictionary { get; private set; }

        /// <summary>
        /// The dictionary working directory
        /// </summary>
        protected string WorkingDir { get { return Path.GetDirectoryName(Dictionary.FilePath); } }

        /// <summary>
        /// The dictionary file name
        /// </summary>
        protected string DictionaryFileName { get { return Path.GetFileName(Dictionary.FilePath); } }

        /// <summary>
        /// Provides the path of the repository
        /// </summary>
        protected string RepositoryPath { get; private set; }

        /// <summary>
        /// Provides the repository related to the Dictionary directory
        /// </summary>
        /// <returns></returns>
        protected Repository getRepository()
        {
            Repository retVal = null;

            RepositoryPath = WorkingDir;
            while (retVal == null && !String.IsNullOrEmpty(RepositoryPath))
            {
                try
                {
                    retVal = new Repository(RepositoryPath);
                }
                catch (Exception)
                {
                    RepositoryPath = Path.GetDirectoryName(RepositoryPath);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary">The dictionary on which the operation should be performed</param>
        public BaseCompareWithRepositoryOperation(Dictionary dictionary)
            : base()
        {
            Dictionary = dictionary;
        }

        /// <summary>
        /// Retrieves a specific version of a data dictionary
        /// </summary>
        /// <param name="dictionary">The dictionary from which the version should be found</param>
        /// <param name="commit">The specific version to be found</param>
        /// <returns>The specific version of the dictionary provided as parameter</returns>
        protected Dictionary DictionaryByVersion(Commit commit)
        {
            DataDictionary.Dictionary retVal = null;

            if (commit != null)
            {
                string workingDir = Path.GetDirectoryName(Dictionary.FilePath);

                // Create the temp directory to store alternate version of the subset file
                string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(tempDirectory);
                try
                {
                    // Retrieve the archive of the selected version

                    ProcessStartInfo _processStartInfo = new ProcessStartInfo();
                    _processStartInfo.WorkingDirectory = workingDir;
                    _processStartInfo.FileName = "git";
                    _processStartInfo.Arguments = "archive -o " + tempDirectory + "\\specs.zip " + commit.Id.Sha + " .";
                    _processStartInfo.CreateNoWindow = true;
                    _processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    Process myProcess = Process.Start(_processStartInfo);
                    myProcess.WaitForExit();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        "Exception raised during operation " + exception.Message + "\nPlease make sure that git is available in your path",
                        "Cannot perform operation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    // Unzip the archive
                    ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
                    zip.ExtractZip(tempDirectory + "\\specs.zip", tempDirectory, null);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Exception raised during operation " + exception.Message, "Cannot perform operation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    // Open the dictionary but do not store it in the EFS System
                    bool allowErrors = true;
                    bool updateGuid = false;
                    OpenFileOperation openFileOperation = new OpenFileOperation(tempDirectory + Path.DirectorySeparatorChar + DictionaryFileName, null, allowErrors, updateGuid);
                    openFileOperation.ExecuteWork();
                    retVal = openFileOperation.Dictionary;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Exception raised during operation " + exception.Message, "Cannot perform operation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    Directory.Delete(tempDirectory, true);
                }
                catch (Exception exception2)
                {
                    MessageBox.Show("Exception raised during operation " + exception2.Message, "Cannot perform operation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

            return retVal;
        }
    }
}
