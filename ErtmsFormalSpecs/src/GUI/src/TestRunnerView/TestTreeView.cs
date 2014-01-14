
namespace GUI.TestRunnerView
{
    public class TestTreeView : TypedTreeView<DataDictionary.EFSSystem>
    {
        /// <summary>
        /// The tests tree node
        /// </summary>
        TestsTreeNode tests;

        /// <summary>
        /// Builds the tree model according to the root node
        /// </summary>
        protected override void BuildModel()
        {
            Nodes.Clear();
            foreach (DataDictionary.Dictionary dictionary in Root.Dictionaries)
            {
                tests = new TestRunnerView.TestsTreeNode(dictionary, true);
                Nodes.Add(tests);
            }
        }

        /// <summary>
        /// Creates a new frame
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameTreeNode createFrame(string name)
        {
            return tests.createFrame(name);
        }

        /// <summary>
        /// Finds the frame which corresponds to the name provided
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FrameTreeNode findFrame(string name)
        {
            FrameTreeNode retVal = (FrameTreeNode)tests.findSubNode(name);

            return retVal;
        }
    }
}
