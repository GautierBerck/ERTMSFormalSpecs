﻿using DataDictionary.Functions;
using DataDictionary.Generated;
using DataDictionary.Interpreter;
using DataDictionary.Tests;
using DataDictionary.Tests.Runner;
using DataDictionary.Values;
using NUnit.Framework;
using NameSpace = DataDictionary.Types.NameSpace;
using Variable = DataDictionary.Variables.Variable;
using Rule = DataDictionary.Rules.Rule;
using RuleCondition = DataDictionary.Rules.RuleCondition;
using SubSequence = DataDictionary.Tests.SubSequence;

namespace DataDictionary.test.updateModel
{
    [TestFixture]
    class UpdateRuleTest : BaseModelTest
    {
        [Test]
        public void TestUpdateRule()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "NameSpace");

            Variable var = CreateVariable(nameSpace, "TheVariable", "Integer");
            var.setDefaultValue("0");

            RuleCondition condition1 = CreateRuleAndCondition(nameSpace, "Rule1");
            Rule rule = condition1.EnclosingRule;
            Action action = CreateAction(condition1, "TheVariable <- 1");


            Compiler.Compile_Synchronous(true);
            System.Runner = new Runner(false, false);

            Expression expression = Parser.Expression(dictionary, "NameSpace.TheVariable");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(value.LiteralName, "0");

            System.Runner.Cycle();

            value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(value.LiteralName, "1");


            Dictionary update = CreateDictionary("TestUpdate");
            update.setUpdates(dictionary.Guid);
            NameSpace updNameSpace = CreateNameSpace(update, "NameSpace");
            updNameSpace.setUpdates(nameSpace.Guid);

            RuleCondition updCondition = CreateRuleAndCondition(updNameSpace, "Rule1");
            Rule updRule = updCondition.EnclosingRule;
            Action updAction = CreateAction(updCondition, "TheVariable <- 2");
            updRule.setUpdates(rule.Guid);

            System.Runner.Cycle();

            value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(value.LiteralName, "2");
        }


        // Change rule in structure, state machine
    }
}