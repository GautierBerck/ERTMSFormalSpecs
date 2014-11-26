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
using System;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using DataDictionary;
using DataDictionary.Tests;


namespace GUI.ExcelImport
{
    public partial class Frm_ExcelImport : Form
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Importers.ExcelImporter.BrakingCurvesImporter brakingCurvesImporter = new Importers.ExcelImporter.BrakingCurvesImporter();


        public Frm_ExcelImport(Dictionary aDictionary)
        {
            InitializeComponent();
            brakingCurvesImporter.TheDictionary = aDictionary;

            TB_FrameName.Text = String.Format("Frame__{0}_{1}_{2}__{3}s_{4}m_{5}h", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Second, DateTime.Now.Minute, DateTime.Now.Hour);

            CBB_SpeedInterval.Items.Add("0.1");
            CBB_SpeedInterval.Items.Add("0.2");
            CBB_SpeedInterval.Items.Add("0.5");
            CBB_SpeedInterval.Items.Add("1.0");
            CBB_SpeedInterval.Items.Add("5.0");
            CBB_SpeedInterval.Items.Add("10.0");
        }


        public Frm_ExcelImport(Step aStep)
        {
            InitializeComponent();
            brakingCurvesImporter.TheStep = aStep;

            CBB_SpeedInterval.Items.Add("0.1");
            CBB_SpeedInterval.Items.Add("0.2");
            CBB_SpeedInterval.Items.Add("0.5");
            CBB_SpeedInterval.Items.Add("1.0");
            CBB_SpeedInterval.Items.Add("5.0");
            CBB_SpeedInterval.Items.Add("10.0");
        }


        /// <summary>
        /// Gives the list of all the controls of the form
        /// (situated on the main form or on its group box)
        /// </summary>
        public ArrayList AllControls
        {
            get
            {
                System.Collections.ArrayList retVal = new System.Collections.ArrayList();
                retVal.AddRange(this.Controls);
                return retVal;
            }
        }


        /// <summary>
        /// Allows to select the excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_SelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select excel file...";
            openFileDialog.Filter = "Microsof Excel (.xlsm)|*.xlsm";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                brakingCurvesImporter.FileName = openFileDialog.FileName;
                TB_FileName.Text = openFileDialog.FileName;
            }
        }


        /// <summary>
        /// Launches the command of importing excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Import_Click(object sender, EventArgs e)
        {
            Hide();
            Double.TryParse(CBB_SpeedInterval.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out brakingCurvesImporter.SpeedInterval);

            brakingCurvesImporter.FrameName = TB_FrameName.Text;
            brakingCurvesImporter.FileName = TB_FileName.Text;
            brakingCurvesImporter.FillEBD = CB_EBD.Checked;
            brakingCurvesImporter.FillSBD = CB_SBD.Checked;
            brakingCurvesImporter.FillEBI = CB_EBI.Checked;
            brakingCurvesImporter.FillSBI1 = CB_SBI1.Checked;
            brakingCurvesImporter.FillSBI2 = CB_SBI2.Checked;
            brakingCurvesImporter.FillWarning = CB_Warning.Checked;
            brakingCurvesImporter.FillPermitted = CB_Permitted.Checked;
            brakingCurvesImporter.FillIndication = CB_Indication.Checked;

            ProgressDialog dialog = new ProgressDialog("Importing excel file....", brakingCurvesImporter);
            dialog.ShowDialog(Owner);
        }


        private void CB_Select_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox cb = sender as System.Windows.Forms.CheckBox;
            if (cb.Tag.Equals("GLOBAL_FILTER"))
            {
                if (cb.Checked)
                {
                    cb.Text = "Deselect all";
                    foreach (Control control in AllControls)
                    {
                        if (control is System.Windows.Forms.CheckBox)
                        {
                            System.Windows.Forms.CheckBox checkBox = control as System.Windows.Forms.CheckBox;
                            if (checkBox.Tag.Equals("FILTER"))
                            {
                                checkBox.Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    cb.Text = "Select all";
                    foreach (Control control in AllControls)
                    {
                        if (control is System.Windows.Forms.CheckBox)
                        {
                            System.Windows.Forms.CheckBox checkBox = control as System.Windows.Forms.CheckBox;
                            if (checkBox.Tag.Equals("FILTER"))
                            {
                                checkBox.Checked = false;
                            }
                        }
                    }
                }
            }
        }
    }
}
