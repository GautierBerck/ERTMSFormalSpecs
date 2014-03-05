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
using System.IO;


namespace DataDictionary.Specification
{
    public class FunctionalBlockItem
    {
        public string Name { set; get; }
        public List<Paragraph> Paragraphs { set; get; }
        public int ApplicableParagraphs { set; get; }
        public int ImplementedParagraphs { set; get; }
        public double ModelingRate { set; get; } // paragraphs per day
        public double EstimatedTime { set; get; }


        /// <summary>
        /// Constructor
        /// </summary>
        public FunctionalBlockItem(Paragraph aParagraph)
        {
            Paragraphs = new List<Paragraph>();
            Paragraphs.Add(aParagraph);
            Name = aParagraph.getFunctionalBlockName();
            ApplicableParagraphs = 0;
            ImplementedParagraphs = 0;
            ModelingRate = 6;
            EstimatedTime = 0;
        }


        /// <summary>
        /// Adds a paragraph to the list of paragraphs of this functional block
        /// </summary>
        /// <param name="aParagraph"></param>
        public void AddParagraph(Paragraph aParagraph)
        {
            Paragraphs.Add(aParagraph);
        }

        /// <summary>
        /// Computes the stats of this functional block
        /// </summary>
        public void ComputeStats()
        {
            foreach (Paragraph aParagraph in Paragraphs)
            {
                ComputeStatsForParagraph(aParagraph);
            }
            EstimatedTime = Math.Round((ApplicableParagraphs - ImplementedParagraphs) / ModelingRate, 3);
        }


        private void ComputeStatsForParagraph(Paragraph aParagraph)
        {
            switch (aParagraph.getImplementationStatus())
            {
                case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_Implemented:
                    ApplicableParagraphs++;
                    ImplementedParagraphs++;
                    break;

                case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.Impl_NA:
                case DataDictionary.Generated.acceptor.SPEC_IMPLEMENTED_ENUM.defaultSPEC_IMPLEMENTED_ENUM:
                    ApplicableParagraphs++;
                    break;
            }
            foreach (Paragraph paragraph in aParagraph.SubParagraphs)
            {
                ComputeStatsForParagraph(paragraph);
            }
        }


        /// <summary>
        /// Provides the string representing the functional block information in CSV format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            double implementedPercentage = 0;
            if (ApplicableParagraphs != 0)
                implementedPercentage = ((double)ImplementedParagraphs / (double)ApplicableParagraphs) * 100;
            return Name + "\t" + ImplementedParagraphs + "\t" + ApplicableParagraphs + "\t" + Math.Round(implementedPercentage, 2) + "%\t" + EstimatedTime;
        }
    }

    public class FunctionalBlockExporter
    {
        private Specification specification;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aSpecification">The specification to be exported</param>
        public FunctionalBlockExporter(Specification aSpecification)
        {
            specification = aSpecification;
        }

        /// <summary>
        /// Exports the informations related to the functional blocks into a .cvs file
        /// </summary>
        public void Export(string path)
        {
            List<FunctionalBlockItem> functionalBlocks = CreateFunctionalBlocks();

            TextWriter tw = new StreamWriter(path);
            tw.WriteLine("Name\tImplemented paragraphs\tApplicableParagraphs\tImplementedPercentage\tEstimatedTime");

            foreach (FunctionalBlockItem fb in functionalBlocks)
            {
                fb.ComputeStats();
                tw.WriteLine(fb.ToString());
            }

            tw.Close();
        }


        /// <summary>
        /// Creates the list of functional blocks of the specification
        /// </summary>
        /// <returns></returns>
        private List<FunctionalBlockItem> CreateFunctionalBlocks()
        {
            List<FunctionalBlockItem> retVal = new List<FunctionalBlockItem>();
            List<Paragraph> paragraphs = CreateFunctionalBlockParagraphs();

            foreach (Paragraph paragraph in paragraphs)
            {
                FunctionalBlockItem aFB = null;
                for (int i = 0; i < retVal.Count && aFB == null; i++)
                {
                    if (retVal[i].Name.Equals(paragraph.getFunctionalBlockName()))
                    {
                        aFB = retVal[i];
                    }
                }
                if (aFB != null)
                {
                    aFB.AddParagraph(paragraph);
                }
                else
                {
                    retVal.Add(new FunctionalBlockItem(paragraph));
                }
            }

            return retVal;
        }


        /// <summary>
        /// Creates a list of paragraphs being marked as functional blocks
        /// </summary>
        /// <returns></returns>
        private List<Paragraph> CreateFunctionalBlockParagraphs()
        {
            List<Paragraph> retVal = new List<Paragraph>();

            foreach (Paragraph paragraph in specification.AllParagraphs)
            {
                if (paragraph.getFunctionalBlock())
                {
                    retVal.Add(paragraph);
                }
            }

            return retVal;
        }
    }
}
