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
using System.Drawing;
using System.Windows.Forms;
using DataDictionary.Tests.Runner.Events;
using System.Drawing.Drawing2D;
using DataDictionary.Tests.Runner;
using DataDictionary;
using DataDictionary.Tests;

namespace GUI.TestRunnerView.TimeLineControl
{
    /// <summary>
    /// this control displays all execution events on a timeline
    /// </summary>
    public class TimeLineControl : Panel
    {
        /// <summary>
        /// Components for the tool tip
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// The enclosing window
        /// </summary>
        public Window Window
        {
            get { return FormsUtils.EnclosingIBaseForm(this) as Window; }
        }

        /// <summary>
        /// The time line handled by this window
        /// </summary>
        public EventTimeLine TimeLine
        {
            get
            {
                EventTimeLine retVal = null;

                Runner runner = DataDictionary.EFSSystem.INSTANCE.Runner;
                if (runner != null)
                {
                    retVal = runner.EventTimeLine;
                }

                return retVal;
            }
        }

        /// <summary>
        /// This label is used to allow auto scrolling by positionning it at the botton right bounds of the visible rectangle
        /// </summary>
        private Label AutoScrollEnabler { get; set; }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            AutoScroll = true;
            AutoSize = false;
            DoubleBuffered = true;

            Click += new EventHandler(TimeLineControl_Click);
            DoubleClick += new EventHandler(TimeLineControl_DoubleClick);

            AutoScrollEnabler = new Label();
            AutoScrollEnabler.Text = "";
            AutoScrollEnabler.Parent = this;
            AutoScrollEnabler.Location = new Point(0, 0);
            AutoScrollEnabler.Visible = true;
        }

        /// <summary>
        /// Provides the event under the mouse pointer
        /// </summary>
        /// <returns></returns>
        private ModelEvent GetEventUnderMouse(MouseEventArgs e)
        {
            ModelEvent retVal = null;

            Point position = new Point(e.X + HorizontalScroll.Value, e.Y + VerticalScroll.Value);
            foreach (KeyValuePair<ModelEvent, Rectangle> pair in PositionHandler.EventPositions)
            {
                if (pair.Value.Contains(position))
                {
                    retVal = pair.Key;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Handles a click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeLineControl_Click(object sender, EventArgs e)
        {
            ModelEvent evt = GetEventUnderMouse((MouseEventArgs)e);

            RuleFired ruleFired = evt as RuleFired;
            if (ruleFired != null)
            {
                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(ruleFired.RuleCondition);
                }
            }

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                BaseTreeNode treeNode = null;

                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    treeNode = GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(variableUpdate.Action);
                }
                if (treeNode == null)
                {
                    if (GUIUtils.MDIWindow.TestWindow != null)
                    {
                        GUIUtils.MDIWindow.TestWindow.TreeView.Select(variableUpdate.Action);
                    }
                }
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                if (GUIUtils.MDIWindow.TestWindow != null)
                {
                    GUIUtils.MDIWindow.TestWindow.TreeView.Select(expect.Expectation);
                }
            }

            ModelInterpretationFailure failure = evt as ModelInterpretationFailure;
            if (failure != null)
            {
                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    GUIUtils.MDIWindow.DataDictionaryWindow.TreeView.Select(failure.Instance as Utils.IModelElement);
                }
            }

            SubStepActivated subStepActivated = evt as SubStepActivated;
            if (subStepActivated != null)
            {
                if (GUIUtils.MDIWindow.DataDictionaryWindow != null)
                {
                    GUIUtils.MDIWindow.TestWindow.TreeView.Select(subStepActivated.SubStep);
                }
            }
        }

        /// <summary>
        /// Handles a double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimeLineControl_DoubleClick(object sender, EventArgs e)
        {
            ModelEvent evt = GetEventUnderMouse((MouseEventArgs)e);

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                if (variableUpdate != null)
                {
                    DataDictionary.Interpreter.ExplanationPart explain = variableUpdate.Explanation;
                    ExplainBox explainTextBox = new ExplainBox();
                    explainTextBox.setExplanation(explain);
                    GUIUtils.MDIWindow.AddChildWindow(explainTextBox);
                }
            }

            Expect expect = evt as Expect;
            if (expect != null)
            {
                DataDictionary.Tests.Expectation expectation = expect.Expectation;

                if (expectation != null)
                {
                    DataDictionary.Interpreter.ExplanationPart explain = expectation.Expression.Explain();
                    ExplainBox explainTextBox = new ExplainBox();
                    explainTextBox.setExplanation(explain);
                    GUIUtils.MDIWindow.AddChildWindow(explainTextBox);
                }
            }

        }

        /// <summary>
        /// The position handler for the events
        /// </summary>
        private EventPositionHandler PositionHandler { get; set; }

        /// <summary>
        /// The images used by this time line control
        /// </summary>
        private ImageList Images { get; set; }

        /// <summary>
        /// The image indexes used to retrieve images
        /// </summary>
        private const int ToolsImageIndex = 0;
        private const int ErrorImageIndex = 1;
        private const int SuccessImageIndex = 2;
        private const int QuestionMarkImageIndex = 3;
        private const int AntennaImageIndex = 4;
        private const int BalanceImageIndex = 5;
        private const int SpeedControlImageIndex = 6;
        private const int TrainImageIndex = 7;
        private const int WheelImageIndex = 8;
        private const int InImageIndex = 9;
        private const int OutImageIndex = 10;
        private const int InOutImageIndex = 11;
        private const int InternalImageIndex = 12;
        private const int CallImageIndex = 13;


        /// <summary>
        /// Constructor
        /// </summary>
        public TimeLineControl()
        {
            InitializeComponent();

            FilterConfiguration = new FilterConfiguration();
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("Configure filter...", new EventHandler(OpenFilter)));
            PositionHandler = new EventPositionHandler(this);

            Images = new ImageList();
            Images.Images.Add(GUI.Properties.Resources.tools);
            Images.Images.Add(GUI.Properties.Resources.error);
            Images.Images.Add(GUI.Properties.Resources.success);
            Images.Images.Add(GUI.Properties.Resources.question_mark);
            Images.Images.Add(GUI.Properties.Resources.antenna);
            Images.Images.Add(GUI.Properties.Resources.balance);
            Images.Images.Add(GUI.Properties.Resources.speed_control);
            Images.Images.Add(GUI.Properties.Resources.train);
            Images.Images.Add(GUI.Properties.Resources.wheel);
            Images.Images.Add(GUI.Properties.Resources.in_icon);
            Images.Images.Add(GUI.Properties.Resources.out_icon);
            Images.Images.Add(GUI.Properties.Resources.in_out_icon);
            Images.Images.Add(GUI.Properties.Resources.internal_icon);
            Images.Images.Add(GUI.Properties.Resources.call_icon);
        }

        /// <summary>
        /// The time line filtering configuration
        /// </summary>
        public FilterConfiguration FilterConfiguration { get; private set; }

        /// <summary>
        /// Opens the filtering dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFilter(object sender, EventArgs e)
        {
            Filtering filtering = new Filtering();
            filtering.Configure(GUIUtils.MDIWindow.EFSSystem, FilterConfiguration);
            filtering.ShowDialog(this);
            filtering.UpdateConfiguration(FilterConfiguration);
            CleanEventPositions();
            Refresh();
        }

        /// <summary>
        /// Handles the position of the events displayed by the time line
        /// </summary>
        private class EventPositionHandler
        {
            /// <summary>
            /// Provides the cycle time of the engine
            /// </summary>
            public TimeLineControl TimeLine { get; private set; }

            /// <summary>
            /// The allocated positios alongs with their corresponding event
            /// The first list represents the X axis
            /// The second list represents the Y axis
            /// This is related to EventPositions
            /// </summary>
            public List<List<ModelEvent>> AllocatedPositions { get; private set; }

            /// <summary>
            /// The position of each event. 
            /// This is related to AllocatedPositions.
            /// </summary>
            public Dictionary<ModelEvent, Rectangle> EventPositions { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="timeLine">The time line for which this position handler is built</param>
            public EventPositionHandler(TimeLineControl timeLine)
            {
                TimeLine = timeLine;
                AllocatedPositions = new List<List<ModelEvent>>();
                EventPositions = new Dictionary<ModelEvent, Rectangle>();
                CleanPositions();
            }

            /// <summary>
            /// Provides the column of a single event
            /// </summary>
            /// <param name="evt"></param>
            /// <returns></returns>
            private int EventColumn(ModelEvent evt)
            {
                int retVal = (int)(evt.Time / EFSSystem.INSTANCE.Runner.Step);

                return retVal;
            }

            /// <summary>
            /// Clears the position of all events
            /// </summary>
            public void CleanPositions()
            {
                NextY = 0;
                LastActivationTime = -1.0;
                AllocatedPositions.Clear();
                EventPositions.Clear();
                LastSubStepActivation = null;
            }

            /// <summary>
            /// The activation time of the last event
            /// </summary>
            private double LastActivationTime { get; set; }

            /// <summary>
            /// The last sub step activation
            /// </summary>
            SubStepActivated LastSubStepActivation { get; set; }

            /// <summary>
            /// The next Y position to place an event
            /// </summary>
            private int NextY { get; set; }

            /// <summary>
            /// Registers an event according to its column
            /// </summary>
            /// <param name="evt"></param>
            public void RegisterEvent(ModelEvent evt)
            {
                if (evt.Time > LastActivationTime)
                {
                    LastActivationTime = evt.Time;
                    AllocatedPositions.Add(new List<ModelEvent>());
                    NextY = 0;

                    if (LastSubStepActivation != null)
                    {
                        SubStepActivated currentSubStepActivation = evt as SubStepActivated;
                        if (currentSubStepActivation == null)
                        {
                            AllocatedPositions[AllocatedPositions.Count - 1].Add(LastSubStepActivation);
                        }
                    }
                }

                List<ModelEvent> events = AllocatedPositions[AllocatedPositions.Count - 1];
                if (!events.Contains(evt))
                {
                    SubStepActivated currentSubStepActivation = evt as SubStepActivated;
                    if (currentSubStepActivation != null)
                    {
                        NextY += STEP_BOX_HEIGHT;
                        if (LastSubStepActivation != null && LastSubStepActivation.SubStep.Step == currentSubStepActivation.SubStep.Step)
                        {
                            // Increase the sub step activation size it is belongs to the same step
                            Rectangle lastRectangle = EventPositions[LastSubStepActivation];
                            lastRectangle.Width = lastRectangle.Width + (EVENT_MARGING.Width + 1) / 2;
                            EventPositions[LastSubStepActivation] = lastRectangle;

                            events.Add(evt);
                            Point location = new Point((AllocatedPositions.Count - 1) * (EVENT_SIZE.Width + EVENT_MARGING.Width) - (EVENT_MARGING.Width / 2), NextY);
                            EventPositions.Add(evt, new Rectangle(location, new Size(STEP_ACTIVATION_EVENT_SIZE.Width + EVENT_MARGING.Width / 2, STEP_ACTIVATION_EVENT_SIZE.Height)));
                        }
                        else
                        {
                            events.Add(evt);
                            Point location = new Point((AllocatedPositions.Count - 1) * (STEP_ACTIVATION_EVENT_SIZE.Width + EVENT_MARGING.Width), NextY);
                            EventPositions.Add(evt, new Rectangle(location, STEP_ACTIVATION_EVENT_SIZE));
                        }
                        NextY += STEP_ACTIVATION_EVENT_SIZE.Height + EVENT_MARGING.Height;
                    }
                    else
                    {
                        events.Add(evt);
                        Point location = new Point((AllocatedPositions.Count - 1) * (EVENT_SIZE.Width + EVENT_MARGING.Width), NextY);
                        EventPositions.Add(evt, new Rectangle(location, EVENT_SIZE));
                        NextY += EVENT_SIZE.Height + EVENT_MARGING.Height;
                    }
                }
                else
                {
                    if (evt == LastSubStepActivation)
                    {
                        // Extent the sub step activation
                        Rectangle position = EventPositions[evt];
                        position.Width = position.Width + EVENT_SIZE.Width + EVENT_MARGING.Width;
                        EventPositions[evt] = position;
                    }
                }

                if (evt is SubStepActivated)
                {
                    LastSubStepActivation = evt as SubStepActivated;
                }
            }

            /// <summary>
            /// Provides the bottom right position of the panel
            /// </summary>
            public Point BottomRightPosition
            {
                get
                {
                    int right = 0;
                    int bottom = 0;

                    foreach (Rectangle rectangle in EventPositions.Values)
                    {
                        right = Math.Max(right, rectangle.Right);
                        bottom = Math.Max(bottom, rectangle.Bottom);
                    }

                    return new Point(right, bottom);
                }
            }
        }

        /// <summary>
        /// Cleans the event positions
        /// </summary>
        public void CleanEventPositions()
        {
            HandledEvents = -1;
            PositionHandler.CleanPositions();
        }

        /// <summary>
        /// The number of events that were handled
        /// </summary>
        private int HandledEvents = -1;

        public override void Refresh()
        {
            if (TimeLine != null && TimeLine.Events.Count != HandledEvents)
            {
                UpdatePositionHandler();
                UpdatePanelSize();
                HandledEvents = TimeLine.Events.Count;
                base.Refresh();
            }
        }

        /// <summary>
        /// Update the information stored in the position handler
        /// </summary>
        private void UpdatePositionHandler()
        {
            PositionHandler.CleanPositions();
            if (TimeLine != null)
            {
                foreach (ModelEvent evt in TimeLine.Events)
                {
                    if (FilterConfiguration.VisibleEvent(evt) || evt is SubStepActivated)
                    {
                        PositionHandler.RegisterEvent(evt);
                    }
                }
            }
        }

        /// <summary>
        /// The size of an event
        /// </summary>
        private static Size EVENT_SIZE = new Size(71, 44);

        /// <summary>
        /// The size of an event
        /// </summary>
        private static Size STEP_ACTIVATION_EVENT_SIZE = new Size(71, 33);

        /// <summary>
        /// The marging between events
        /// </summary>
        private static Size EVENT_MARGING = new Size(5, 2);

        /// <summary>
        /// Updates the size of the panel according to the number of events to handle
        /// </summary>
        private void UpdatePanelSize()
        {
            AutoScrollEnabler.Location = PositionHandler.BottomRightPosition;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                pe.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
                drawEvents(pe);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Draws the event for the panel
        /// </summary>
        /// <param name="pe"></param>
        private void drawEvents(PaintEventArgs pe)
        {
            foreach (KeyValuePair<ModelEvent, Rectangle> pair in PositionHandler.EventPositions)
            {
                drawEvent(pe, pair.Key, pair.Value);
            }

            drawStep(pe);
        }

        /// <summary>
        /// Attributes used to display a single event
        /// </summary>
        private class EventDisplayAttributes
        {
            /// <summary>
            /// The event fill color
            /// </summary>
            public Color FillColor { get; private set; }

            /// <summary>
            /// The pen used to draw the form
            /// </summary>
            public Pen DrawPen { get; private set; }

            /// <summary>
            /// The text to display at the bottom of the event
            /// </summary>
            public string BottomText { get; private set; }

            /// <summary>
            /// The image index of the left icon
            /// </summary>
            public int LeftIconImageIndex { get; private set; }

            /// <summary>
            /// The image index of the Right icon
            /// </summary>
            public int RightIconImageIndex { get; private set; }

            /// <summary>
            /// The icon to display near the RightIcon
            /// </summary>
            public int RightIconModifierImageIndex { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="fillColor"></param>
            /// <param name="drawPen"></param>
            /// <param name="bottomText"></param>
            /// <param name="leftIcon"></param>
            /// <param name="rightIcon"></param>
            /// <param name="rightIconModifier"></param>
            public EventDisplayAttributes(Color fillColor, Pen drawPen, string bottomText, int leftIcon, int rightIcon, int rightIconModifier)
            {
                FillColor = fillColor;
                DrawPen = drawPen;
                BottomText = bottomText;
                LeftIconImageIndex = leftIcon;
                RightIconImageIndex = rightIcon;
                RightIconModifierImageIndex = rightIconModifier;
            }
        }

        /// <summary>
        /// Provides the display attributes for a model event
        /// </summary>
        /// <param name="evt"></param>
        /// <returns></returns>
        private EventDisplayAttributes GetDisplayAttributes(Graphics graphics, ModelEvent evt)
        {
            EventDisplayAttributes retVal = new EventDisplayAttributes(Color.White, new Pen(Color.Black), "<undefined>", -1, -1, -1); ;

            Expect expect = evt as Expect;
            if (expect != null)
            {
                string name = AdjustForDisplay(graphics, ShortName(expect.Expectation.Name), EVENT_SIZE.Width - 4, BOTTOM_FONT);

                switch (expect.State)
                {
                    case Expect.EventState.Active:
                        retVal = new EventDisplayAttributes(Color.Silver, new Pen(Color.Black), name, QuestionMarkImageIndex, -1, -1);
                        break;
                    case Expect.EventState.Fullfilled:
                        retVal = new EventDisplayAttributes(Color.LightGreen, new Pen(Color.Green), name, SuccessImageIndex, -1, -1);
                        break;
                    case Expect.EventState.TimeOut:
                        retVal = new EventDisplayAttributes(Color.Red, new Pen(Color.DarkRed), name, ErrorImageIndex, -1, -1);
                        break;
                }
            }

            RuleFired ruleFired = evt as RuleFired;
            if (ruleFired != null)
            {
                string name = AdjustForDisplay(graphics, ShortName(ruleFired.RuleCondition.Name), EVENT_SIZE.Width - 4, BOTTOM_FONT);

                retVal = new EventDisplayAttributes(Color.LightBlue, new Pen(Color.Blue), name, ToolsImageIndex, GetImageIndex(ruleFired.RuleCondition), -1);
            }

            VariableUpdate variableUpdate = evt as VariableUpdate;
            if (variableUpdate != null)
            {
                string name = AdjustForDisplay(graphics, ShortName(variableUpdate.Action.Statement.ShortShortDescription()), EVENT_SIZE.Width - 4, BOTTOM_FONT);

                int rightIcon = GetImageIndex(variableUpdate.Action.Statement.AffectedElement());
                int rightModifier = -1;
                switch (variableUpdate.Action.Statement.UsageDescription())
                {
                    case DataDictionary.Interpreter.Statement.Statement.ModeEnum.Call:
                        rightModifier = CallImageIndex;
                        break;
                    case DataDictionary.Interpreter.Statement.Statement.ModeEnum.In:
                        rightModifier = InImageIndex;
                        break;

                    case DataDictionary.Interpreter.Statement.Statement.ModeEnum.InOut:
                        rightModifier = InOutImageIndex;
                        break;

                    case DataDictionary.Interpreter.Statement.Statement.ModeEnum.Internal:
                        rightModifier = InternalImageIndex;
                        break;

                    case DataDictionary.Interpreter.Statement.Statement.ModeEnum.Out:
                        rightModifier = OutImageIndex;
                        break;
                }

                TestCase testCase = Utils.EnclosingFinder<TestCase>.find(variableUpdate.Action);
                if (testCase != null)
                {
                    retVal = new EventDisplayAttributes(Color.LightGray, new Pen(Color.Black), name, -1, rightIcon, rightModifier);
                }
                else
                {
                    retVal = new EventDisplayAttributes(Color.BlanchedAlmond, new Pen(Color.Black), name, -1, rightIcon, rightModifier);
                }
            }

            SubStepActivated subStepActivated = evt as SubStepActivated;
            if (subStepActivated != null)
            {
                retVal = new EventDisplayAttributes(Color.LightGray, new Pen(Color.Black), "SubStep", -1, -1, -1);
            }

            return retVal;
        }

        /// <summary>
        /// Provides the image index of the element provided
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private int GetImageIndex(ModelElement element)
        {
            int retVal = -1;

            DataDictionary.Types.NameSpace nameSpace = DataDictionary.EnclosingNameSpaceFinder.find(element);
            while (nameSpace != null && nameSpace.EnclosingNameSpace != null)
            {
                nameSpace = nameSpace.EnclosingNameSpace;
            }

            // TODO : Allow to define the image in the namespace itself
            if (nameSpace != null)
            {
                if (nameSpace.Name == "BTM" || nameSpace.Name == "EURORADIO")
                {
                    retVal = AntennaImageIndex;
                }
                else if (nameSpace.Name == "JRU")
                {
                    retVal = BalanceImageIndex;
                }
                else if (nameSpace.Name == "DMI")
                {
                    retVal = SpeedControlImageIndex;
                }
                else if (nameSpace.Name == "Odometry")
                {
                    retVal = WheelImageIndex;
                }
                else
                {
                    retVal = TrainImageIndex;
                }
            }

            return retVal;
        }

        /// <summary>
        /// The font used to display the top text
        /// </summary>
        private static Font TOP_FONT = new Font(FontFamily.GenericSansSerif, 8.0f, FontStyle.Bold);

        /// <summary>
        /// The font used to display the bottom text
        /// </summary>
        private static Font BOTTOM_FONT = new Font(FontFamily.GenericSansSerif, 8.0f);

        /// <summary>
        /// The first an last substep activation currently being displayed
        /// </summary>
        private SubStepActivated FirstSubStep;
        private SubStepActivated LastSubStep;

        /// <summary>
        /// Draws a single event
        /// </summary>
        /// <param name="pe"></param>
        /// <param name="evt"></param>
        /// <param name="X">The column in which the event should be drawn</param>
        /// <param name="Y">The line in which the event should be drawn</param>
        private void drawEvent(PaintEventArgs pe, ModelEvent evt, Rectangle bounds)
        {
            EventDisplayAttributes attributes = GetDisplayAttributes(pe.Graphics, evt);
            int CornerRadius = 5;

            SubStepActivated subStepActivation = evt as SubStepActivated;
            if (subStepActivation != null)
            {
                if (FirstSubStep == null)
                {
                    FirstSubStep = subStepActivation;
                    LastSubStep = subStepActivation;
                }
                else
                {
                    if (FirstSubStep.SubStep.Step == subStepActivation.SubStep.Step)
                    {
                        LastSubStep = subStepActivation;
                    }
                    else
                    {
                        // This is a new Step,draw the preceding one
                        drawStep(pe);
                        FirstSubStep = subStepActivation;
                        LastSubStep = subStepActivation;
                    }
                }

                pe.Graphics.FillRectangle(new SolidBrush(attributes.FillColor), bounds);
                int index = subStepActivation.SubStep.EnclosingCollection.IndexOf(subStepActivation.SubStep) + 1;
                if (index == 1)
                {
                    pe.Graphics.DrawString("Substep", BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color), new Point(bounds.Left + 2, bounds.Top + 2));
                }
                pe.Graphics.DrawString("" + index, BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color), new Point(bounds.Left + bounds.Width / 2, bounds.Bottom - 2 - BOTTOM_FONT.Height));
            }
            else
            {
                int strokeOffset = Convert.ToInt32(Math.Ceiling(attributes.DrawPen.Width));
                bounds = Rectangle.Inflate(bounds, -strokeOffset, -strokeOffset);

                attributes.DrawPen.EndCap = attributes.DrawPen.StartCap = LineCap.Round;

                GraphicsPath gfxPath = new GraphicsPath();
                gfxPath.AddArc(bounds.X, bounds.Y, CornerRadius, CornerRadius, 180, 90);
                gfxPath.AddArc(bounds.X + bounds.Width - CornerRadius, bounds.Y, CornerRadius, CornerRadius, 270, 90);
                gfxPath.AddArc(bounds.X + bounds.Width - CornerRadius, bounds.Y + bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gfxPath.AddArc(bounds.X, bounds.Y + bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                gfxPath.CloseAllFigures();

                pe.Graphics.FillPath(new SolidBrush(attributes.FillColor), gfxPath);
                pe.Graphics.DrawPath(attributes.DrawPen, gfxPath);

                pe.Graphics.DrawString(attributes.BottomText, BOTTOM_FONT, new SolidBrush(attributes.DrawPen.Color), new Point(bounds.Left + 2, bounds.Bottom - 2 - BOTTOM_FONT.Height));

                if (attributes.LeftIconImageIndex >= 0)
                {
                    pe.Graphics.DrawImage(Images.Images[attributes.LeftIconImageIndex], bounds.Left + 4, bounds.Top + 4, 20, 20);
                }

                if (attributes.RightIconImageIndex >= 0)
                {
                    pe.Graphics.DrawImage(Images.Images[attributes.RightIconImageIndex], bounds.Right - 4 - 20, bounds.Top + 4, 20, 20);
                }

                if (attributes.RightIconImageIndex >= 0 && attributes.RightIconModifierImageIndex >= 0)
                {
                    pe.Graphics.DrawImage(Images.Images[attributes.RightIconModifierImageIndex], bounds.Right - 4 - 30, bounds.Top + 10, 16, 16);
                }
            }
        }

        /// <summary>
        /// The height of the step box
        /// </summary>
        private const int STEP_BOX_HEIGHT = 20;

        /// <summary>
        /// The color used to display a step box
        /// </summary>
        private static Color STEP_BOX_COLOR = Color.Black;

        /// <summary>
        /// The pen used to display a step box
        /// </summary>
        private static Brush STEP_BOX_PEN = new SolidBrush(Color.LightGray);

        /// <summary>
        /// Draws the step according to the FirstSubStep and LastSubStep values
        /// </summary>
        /// <param name="pe"></param>
        private void drawStep(PaintEventArgs pe)
        {
            if (FirstSubStep != null && PositionHandler.EventPositions.ContainsKey(FirstSubStep))
            {
                Rectangle first = PositionHandler.EventPositions[FirstSubStep];
                Rectangle last = PositionHandler.EventPositions[LastSubStep];
                Rectangle bounds = new Rectangle(new Point(first.X, 0), new Size(last.X + last.Width - first.X, STEP_BOX_HEIGHT));

                string name = AdjustForDisplay(pe.Graphics, FirstSubStep.SubStep.Step.Name, bounds.Width - 4, TOP_FONT);
                pe.Graphics.FillRectangle(STEP_BOX_PEN, bounds);
                pe.Graphics.DrawString(
                    name,
                    TOP_FONT,
                    new SolidBrush(STEP_BOX_COLOR),
                    new Rectangle(new Point(bounds.Left + 2, bounds.Top + 2), new Size(bounds.Width - 4, Bounds.Height - 4)));
                pe.Graphics.DrawLine(new Pen(STEP_BOX_COLOR), bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
            }
        }

        /// <summary>
        /// Adjust the text size according to the display size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <param name="font"></param>
        private string AdjustForDisplay(Graphics graphics, string text, int width, Font font)
        {
            string retVal = text;

            if (graphics.MeasureString(text, font).Width > width)
            {
                width = (int)(width - graphics.MeasureString("...", font).Width);
                int i = text.Length;
                int step = i / 2;
                while (graphics.MeasureString(text.Substring(0, i), font).Width > width)
                {
                    i = i - step;
                    step = step / 2;
                    while (graphics.MeasureString(text.Substring(0, i), font).Width < width && step > 0)
                    {
                        i = i + step;
                    }
                }
                retVal = text.Substring(0, i) + "...";
            }

            return retVal;
        }

        /// <summary>
        /// Reduces the string to the most important thing in it
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ShortName(string text)
        {
            string retVal = text.Trim();

            // Remove leading prefixes
            string prefix = "";
            int parentCount = 0;
            for (int i = 0; i < retVal.Length; i++)
            {
                if (retVal[i] == '(')
                {
                    parentCount += 1;
                }
                if (retVal[i] == ')')
                {
                    parentCount -= 1;
                }

                if (parentCount == 0)
                {
                    if (retVal[i] == '.')
                    {
                        prefix = "";
                    }
                    else
                    {
                        if (char.IsSeparator(retVal, i))
                        {
                            break;
                        }
                        else
                        {
                            prefix = prefix + retVal[i];
                        }
                    }
                }
                else
                {
                    prefix = prefix + retVal[i];
                }
            }
            retVal = prefix;

            return retVal;
        }
    }
}
