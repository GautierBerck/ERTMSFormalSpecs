﻿using DataDictionary.Generated;
using NUnit.Framework;
using Action = DataDictionary.Rules.Action;
using NameSpace = DataDictionary.Types.NameSpace;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using Structure = DataDictionary.Types.Structure;
using StructureElement = DataDictionary.Types.StructureElement;

namespace DataDictionary.test
{
    [TestFixture]
    public class RefactoringTest : BaseModelTest
    {
        [Test]
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
            Assert.AreEqual("NewS1 { E1 => True }", v.getDefaultValue());
            Assert.AreEqual("V <- NewS1 { E1 => False }", a.ExpressionText);
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
        public void TestRefactorInterfaceName()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            Structure i1 = CreateStructure(n0, "I1");
            i1.setIsAbstract(true);
            Structure s1 = CreateStructure(n0, "S1");
            StructureRef sr = CreateStructureRef(s1, "I1");

            Refactor(i1, "NewI1");
            Assert.AreEqual("NewI1", sr.ExpressionText);
        }

        [Test]
        public void TestRefactorInterfaceField()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace n0 = CreateNameSpace(test, "N0");
            Structure i1 = CreateStructure(n0, "I1");
            i1.setIsAbstract(true);
            StructureElement el1 = CreateStructureElement(i1, "E1", "Boolean");
            Structure s1 = CreateStructure(n0, "S1");
            StructureRef sr = CreateStructureRef(s1, "I1");
            StructureElement el2 = CreateStructureElement(s1, "E1", "Boolean");

            Refactor(el1, "NewE1");
            Assert.AreEqual("NewE1", el2.Name);
        }

        [Test]
        public void TestMoveFromDefaultNameSpace()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace defaultNameSpace = CreateNameSpace(test, "Default");
            Types.Enum e1 = CreateEnum(defaultNameSpace, "E1");
            Constants.EnumValue v1 = CreateEnumValue(e1, "X");
 
            NameSpace n1 = CreateNameSpace (test, "N1");
            Variables.Variable var1 = CreateVariable(n1, "V1", "Default.E1");
            RuleCondition ruleCondition = CreateRuleAndCondition(n1, "R");
            Action action = CreateAction(ruleCondition, "N1 <- Default.E1.X");

            MoveToNameSpace(e1, n1);
            Assert.AreEqual("N1 <- N1.E1.X", action.ExpressionText);
            Assert.AreEqual("E1", var1.TypeName);
        }

        [Test]
        public void TestMoveBugReport555()
        {
            Dictionary test = CreateDictionary("Test");
            NameSpace MANamespace = CreateNameSpace(test, "MA");
            NameSpace ModeProfileNamespace = CreateNameSpace(MANamespace, "ModeProfile");
            Variables.Variable TackVariable = CreateVariable(ModeProfileNamespace, "Tack", "Boolean");

            NameSpace KernelNameSpace = CreateNameSpace(test, "Kernel");
            RuleCondition ruleCondition = CreateRuleAndCondition(KernelNameSpace, "R");
            Action action = CreateAction(ruleCondition, "MA.ModeProfile.Tack <- False");

            NameSpace DMINamespace = CreateNameSpace(test, "DMI");
            NameSpace InNamespace = CreateNameSpace(DMINamespace, "IN");
            NameSpace DisplayNamespace = CreateNameSpace(InNamespace, "Display");

            MoveToNameSpace(TackVariable, DisplayNamespace);
            Assert.AreEqual("DMI.IN.Display.Tack <- False", action.ExpressionText);
        }
    
    }
}