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
using System.Collections;
using System.IO;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using SourceTextComment = DataDictionary.Tests.Translations.SourceTextComment;
using DataDictionary.Tests.DBElements;

namespace DataDictionary.Tests
{
    public class Step : Generated.Step, ICommentable, TextualExplain
    {
        public override string Name
        {
            get
            {
                string retVal = base.Name;

                if (getTCS_Order() != 0)
                {
                    retVal = "Step " + getTCS_Order() + ": " + getDescription();
                }
                else
                {
                    if (Utils.Utils.isEmpty(retVal))
                    {
                        retVal = getDescription();
                    }
                }

                return retVal;
            }
        }

        public ArrayList SubSteps
        {
            get
            {
                if (allSubSteps() == null)
                {
                    setAllSubSteps(new ArrayList());
                }
                return allSubSteps();
            }
        }


        /// <summary>
        /// The messages associated to this step
        /// </summary>
        public System.Collections.ArrayList StepMessages
        {
            get
            {
                if (allMessages() == null)
                {
                    setAllMessages(new System.Collections.ArrayList());
                }
                return allMessages();
            }
        }

        /// <summary>
        /// The enclosing test case
        /// </summary>
        public TestCase TestCase
        {
            get { return Enclosing as TestCase; }
        }

        /// <summary>
        /// The collection which encloses this step
        /// </summary>
        public override ArrayList EnclosingCollection
        {
            get { return TestCase.Steps; }
        }

        /// <summary>
        /// The explanation of this step, as RTF pseudo code
        /// </summary>
        /// <returns></returns>
        public override string getExplain()
        {
            string retVal = "";

            foreach (SubStep subStep in SubSteps)
            {
                retVal += subStep.getExplain();
            }

            return retVal;
        }

        /// <summary>
        /// Provides the sub sequence for this step
        /// </summary>
        public SubSequence SubSequence
        {
            get { return Utils.EnclosingFinder<SubSequence>.find(this); }
        }

        /// <summary>
        /// Indicates if this step contains some actions or expectations
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            bool retVal = false;
            if (SubSteps.Count == 0)
            {
                retVal = true;
            }
            else
            {
                retVal = true;
                foreach (SubStep subStep in SubSteps)
                {
                    if (!subStep.IsEmpty())
                    {
                        retVal = false;
                        break;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Translates the current step according to the translation dictionary
        /// Removes all preconditions, actions and expectations
        /// </summary>
        /// <param name="translationDictionary"></param>
        public void Translate(Translations.TranslationDictionary translationDictionary)
        {
            if (getTranslationRequired())
            {
                SubSteps.Clear();

                Translations.Translation translation = null;
                if (translationDictionary != null)
                {
                    translation = translationDictionary.findTranslation(getDescription(), Comment);
                }

                if (translation != null)
                {
                    translation.UpdateStep(this);
                    setTranslated(true);
                }
                else
                {
                    AddWarning("Cannot find translation for this step");
                }
            }
        }

        /// <summary>
        /// Adds a model element in this model element
        /// </summary>
        /// <param name="copy"></param>
        public override void AddModelElement(Utils.IModelElement element)
        {
            SubStep item = element as SubStep;
            if (item != null)
            {
                appendSubSteps(item);
            }
        }


        /// <summary>
        /// Fills the actual step with information of another test case
        /// </summary>
        /// <param name="oldTestCase"></param>
        public void Merge(Step aStep)
        {
            setAllSubSteps(aStep.SubSteps);

            setGuid(aStep.getGuid());
            setComment(aStep.Comment);
            setTranslated(aStep.getTranslated());
            setTranslationRequired(aStep.getTranslationRequired());

            int cnt = 0;
            foreach (DBMessage message in StepMessages)
            {
                if (cnt < aStep.StepMessages.Count )
                {
                    message.Merge((DBMessage)aStep.StepMessages[cnt]);
                }
                cnt += 1;
            }

            foreach (ReqRef reqRef in aStep.Requirements)
            {
                appendRequirements(reqRef);
            }
        }

        /// <summary>
        /// Adds a new message
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(DBElements.DBMessage message)
        {
            allMessages().Add(message);
        }

        /// <summary>
        /// Creates a step and sets its default values
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Step createDefault(string name)
        {
            Step retVal = (Step)DataDictionary.Generated.acceptor.getFactory().createStep();
            retVal.Name = name;

            retVal.appendSubSteps(SubStep.createDefault("Sub-step1"));

            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the step's behaviour
        /// </summary>
        /// <param name="indentLevel">the number of white spaces to add at the beginning of each line</param>
        /// <returns></returns>
        public string getExplain(int indentLevel, bool explainSubElements)
        {
            string retVal = TextualExplainUtilities.Header(this, indentLevel);

            foreach (SubStep subStep in SubSteps)
            {
                retVal += subStep.getExplain(indentLevel + 2, explainSubElements) + "\\par";

            }
            return retVal;
        }

        /// <summary>
        /// Provides an explanation of the step's behaviour
        /// </summary>

        /// <param name="explainSubElements">Precises if we need to explain the sub elements (if any)</param>
        /// <returns></returns>
        public string getExplain(bool explainSubElements)
        {
            string retVal = "";

            retVal = getExplain(0, explainSubElements);

            return retVal;
        }

        /// <summary>
        /// Creates the source text which corresponds to this step
        /// </summary>
        /// <returns></returns>
        public SourceText createSourceText()
        {
            SourceText retVal = (SourceText) Generated.acceptor.getFactory().createSourceText();
            retVal.Name = getDescription();

            if (!string.IsNullOrEmpty(Comment) && Comment.Trim() != "-")
            {
                SourceTextComment comment = (SourceTextComment) Generated.acceptor.getFactory().createSourceTextComment();
                comment.Name = Comment;
                retVal.appendComments(comment);
            }

            return retVal;
        }

        /// <summary>
        /// !!! Clean HacK !!! 
        /// Do not save the substeps when the step requires automatic translation
        /// !!! Clean HaCk !!!
        /// </summary>
        /// <param name="pw"></param>
        /// <param name="typeId"></param>
        /// <param name="headingTag"></param>
        /// <param name="endingTag"></param>
        public override void unParse(TextWriter pw, bool typeId, string headingTag, string endingTag)
        {
            if (getTranslationRequired())
            {
                ArrayList tmp = allSubSteps();
                bool translated = getTranslated();

                setAllSubSteps(null);
                setTranslated(false);

                base.unParse(pw, typeId, headingTag, endingTag);

                setAllSubSteps(tmp);
                setTranslated(translated);
            }
            else
            {
                base.unParse(pw, typeId, headingTag, endingTag);
            }
        }

        /// <summary>
        /// Provides the previous step (if any) in the subsequence
        /// </summary>
        public Step PreviousStep
        {
            get
            {
                Step retVal = null;

                bool found = false;
                foreach (TestCase testCase in SubSequence.TestCases)
                {
                    foreach (Step step in testCase.Steps)
                    {
                        if (step == this)
                        {
                            found = true;
                            break;
                        }
                        else
                        {
                            retVal = step;
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }

                if (!found)
                {
                    retVal = null;
                }

                return retVal;
            }
        }
    }
}
