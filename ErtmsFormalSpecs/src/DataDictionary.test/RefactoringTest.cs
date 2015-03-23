﻿using DataDictionary.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = DataDictionary.Rules.Action;
using NameSpace = DataDictionary.Types.NameSpace;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;

namespace DataDictionary.test
{
    [TestClass]
    public class RefactoringTest : BaseModelTest
    {
        [TestMethod]
        public void TestRefactorStructureName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N1.S1 { E1 => False }");

            Refactor(s1, "NewS1");
            Assert.AreEqual("NewS1", el2.TypeName);
            Assert.AreEqual("N1.NewS1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- N1.NewS1 { E1 => False }", a.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorElementStructureName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            RuleCondition rc1 = CreateRuleAndCondition(s2, "Rule1");
            Action action = CreateAction(rc1, "E2.E1 <- False");

            Variable variable = CreateVariable(n1, "A", "S1");
            Action action2 = CreateAction(rc1, "A <- S1 { E1 -> False }");

            Refactor(el1, "NewE1");
            Assert.AreEqual("E2.NewE1 <- False", action.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorElementStructureName2()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Variable variable = CreateVariable(n1, "A", "S1");
            RuleCondition rc1 = CreateRuleAndCondition(n1, "Rule1");
            Action action = CreateAction(rc1, "A <- S1 { E1 => False }");

            Refactor(el1, "NewE1");
            Assert.AreEqual("A <- S1 { NewE1 => False }", action.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorNameSpaceName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n1 = CreateNameSpace(test, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N1.S1 { E1 => False }");

            Refactor(n1, "NewN1");
            Assert.AreEqual("NewN1.S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewN1.S1 { E1 => False }", a.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorNameSpaceName2()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("N0.N1.S1 { E1 => True }");
            RuleCondition rc = CreateRuleAndCondition(n1, "Rule1");
            Action a = CreateAction(rc, "V <- N0.N1.S1 { E1 => False }");

            Refactor(n0, "NewN0");
            Assert.AreEqual("NewN0.N1.S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewN0.N1.S1 { E1 => False }", a.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorNameSpaceName3()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StructureElement el1 = CreateStructureElement(s1, "E1", "Boolean");
            Structure s2 = CreateStructure(n1, "S2");
            StructureElement el2 = CreateStructureElement(s2, "E2", "S1");
            Variable v = CreateVariable(n1, "V", "S1");
            v.setDefaultValue("S1 { E1 => True }");
            Function f = CreateFunction(n1, "f", "S2");

            NameSpace n2 = CreateNameSpace(n0, "N2");
            RuleCondition rc = CreateRuleAndCondition(n2, "Rule1");
            PreCondition p = CreatePreCondition(rc, "N1.f().E2.E1");
            Action a = CreateAction(rc, "N1.V <- N1.S1 { E1 => False }");

            Refactor(n1, "NewN1");
            Assert.AreEqual("S1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("NewN1.V <- NewN1.S1 { E1 => False }", a.ExpressionText);
            Assert.AreEqual("NewN1.f().E2.E1", p.ExpressionText);

            RefactorAndRelocate(p);
            Assert.AreEqual("NewN1.f().E2.E1", p.ExpressionText);
        }

        [TestMethod]
        public void TestRefactorNameSpaceName4()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            NameSpace n1 = CreateNameSpace(n0, "N1");
            Structure s1 = CreateStructure(n1, "S1");
            StateMachine stataMachine = CreateStateMachine(s1, "StateMachine");
            StructureElement el1 = CreateStructureElement(s1, "E1", "StateMachine");

            NameSpace n2 = CreateNameSpace(n0, "N2");

            Refactor(n2, "NewN2");
            Assert.AreEqual("StateMachine", el1.TypeName);
        }
    }
}