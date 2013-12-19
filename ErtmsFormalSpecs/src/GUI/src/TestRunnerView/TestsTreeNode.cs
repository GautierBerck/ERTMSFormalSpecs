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
using System.Collections.Generic;
using System.Windows.Forms;
using DataDictionary;

namespace GUI.TestRunnerView
{
    public class TestsTreeNode : ModelElementTreeNode<DataDictionary.Dictionary>
    {
        private class ItemEditor : NamedEditor
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public ItemEditor()
                : base()
            {
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="item"></param>
        public TestsTreeNode(DataDictionary.Dictionary item)
            : base(item, null, true)
        {
        }

        protected override void BuildSubNodes()
        {
            base.BuildSubNodes();

            foreach (DataDictionary.Tests.Frame frame in Item.Tests)
            {
                Nodes.Add(new FrameTreeNode(frame));
            }
            SortSubNodes();
        }

        /// <summary>
        /// Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor createEditor()
        {
            return new ItemEditor();
        }

        /// <summary>
        /// Creates a new frame
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameTreeNode createFrame(string name)
        {
            FrameTreeNode retVal;

            DataDictionary.Tests.Frame frame = DataDictionary.Tests.Frame.createDefault(name);
            Item.appendTests(frame);

            retVal = new FrameTreeNode(frame);
            Nodes.Add(retVal);
            SortSubNodes();

            return retVal;
        }

        public void AddHandler(object sender, EventArgs args)
        {
            createFrame("Frame" + (GetNodeCount(false) + 1));
        }

        private void ClearAll()
        {
            TestTreeView treeView = TreeView as TestTreeView;
            if (treeView != null)
            {
                Window window = treeView.ParentForm as Window;
                window.Clear();
            }
        }

        #region Execute tests
        private class ExecuteTestsHandler : Utils.ProgressHandler
        {
            /// <summary>
            /// The window for which theses tests should be executed
            /// </summary>
            private Window Window { get; set; }

            /// <summary>
            /// The subsequence which should be executed
            /// </summary>
            private Dictionary Dictionary { get; set; }

            /// <summary>
            /// The EFS system 
            /// </summary>
            private EFSSystem EFSSystem { get { return Dictionary.EFSSystem; } }

            /// <summary>
            /// The number of failed tests
            /// </summary>
            public int Failed { get; private set; }

            /// <summary>
            /// The time span used to execute all tests
            /// </summary>
            public TimeSpan Span { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="window"></param>
            /// <param name="dictionary"></param>
            public ExecuteTestsHandler(Window window, Dictionary dictionary)
            {
                Window = window;
                Dictionary = dictionary;
            }

            /// <summary>
            /// Executes the tests in the background thread
            /// </summary>
            /// <param name="arg"></param>
            public override void ExecuteWork()
            {
                DateTime start = DateTime.Now;

                SynchronizerList.SuspendSynchronization();
                Failed = Dictionary.ExecuteAllTests();
                Dictionary.EFSSystem.Runner = null;
                SynchronizerList.ResumeSynchronization();

                Span = DateTime.Now.Subtract(start);
            }
        }

        /// <summary>
        /// Handles a run event on this test case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RunHandler(object sender, EventArgs args)
        {
            ClearAll();
            ClearMessages();

            ExecuteTestsHandler executeTestsHandler = new ExecuteTestsHandler(BaseForm as Window, Item);
            ProgressDialog dialog = new ProgressDialog("Executing test frames", executeTestsHandler);
            dialog.ShowDialog();

            System.Windows.Forms.MessageBox.Show(Item.Tests.Count + " test frame(s) executed, " + executeTestsHandler.Failed + " test frame(s) failed.\nTest duration : " + Math.Round(executeTestsHandler.Span.TotalSeconds) + " seconds", "Execution report");
        }
        #endregion


        /// <summary>
        /// Handles a run event on these tests and creates the associated report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReportHandler(object sender, EventArgs args)
        {
            Report.TestReport aReport = new Report.TestReport(Item);
            aReport.Show();
        }


        /// <summary>
        /// The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>();

            retVal.Add(new MenuItem("Add", new EventHandler(AddHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Import braking curves verification set", new EventHandler(ImportBrakingCurvesHandler)));
            retVal.Add(new MenuItem("-"));
            retVal.Add(new MenuItem("Execute", new EventHandler(RunHandler)));
            retVal.Add(new MenuItem("Create report", new EventHandler(ReportHandler)));

            return retVal;
        }


        /// <summary>
        /// Imports a test scenario from the ERA braking curves simulation tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ImportBrakingCurvesHandler(object sender, EventArgs args)
        {
            Window window = BaseForm as Window;
            if (window != null)
            {
                ExcelImport.Frm_ExcelImport excelImport = new ExcelImport.Frm_ExcelImport(this.Item);
                excelImport.ShowDialog();
                GUIUtils.MDIWindow.RefreshModel();
            }
        }

        /// <summary>
        /// Display the number of frames
        /// </summary>
        /// <param name="displayStatistics"></param>
        public override void SelectionChanged(bool displayStatistics)
        {
            base.SelectionChanged(false);

            GUIUtils.MDIWindow.SetStatus(Item.Tests.Count + " test frames");
        }
    }
}
