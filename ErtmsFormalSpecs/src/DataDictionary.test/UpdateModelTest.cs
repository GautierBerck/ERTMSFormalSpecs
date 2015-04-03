﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataDictionary.Functions;
using DataDictionary.Interpreter;
using DataDictionary.Types;
using DataDictionary.Values;
using NUnit.Framework;

namespace DataDictionary.test
{
    [TestFixture]
    public class UpdateModelTest : BaseModelTest
    {
        [Test]
        public void TestUpdateFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");
            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case) updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q()";


            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value);
        }

        // Further tests:
        //  - Replace both f and q              [  ]
        //      change the parameters for q     [  ]
        //  - Replace q (with f calling it)     [OK]
        // Other model elements                 [  ]


        [Test]
        public void TestUpdateMultipleFunctions()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");

            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);
            
            Function updatedFunction = function.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case)updatedFunction.Cases[0];
            cas3.ExpressionText = "NOT q(param => 3)";
            
            Function updatedFunction2 = function2.CreateFunctionUpdate(dictionary2);
            Parameter intParameter = CreateFunctionParameter(updatedFunction2, "param", "Integer");
            Case cas4 = (Case) updatedFunction2.Cases[0];
            cas4.ExpressionText = "False";
            

            Compiler.Compile_Synchronous(true);


            Expression expression = Parser.Expression(dictionary, "N1.f()");
            IValue value = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.True, value);
        }


        [Test]
        public void TestUpdateCalledFunction()
        {
            Dictionary dictionary = CreateDictionary("Test");
            NameSpace nameSpace = CreateNameSpace(dictionary, "N1");

            Function function = CreateFunction(nameSpace, "f", "Bool");
            Case cas1 = CreateCase(function, "Case 1", "q()");

            Function function2 = CreateFunction(nameSpace, "q", "Bool");
            Case cas2 = CreateCase(function2, "Case 1", "True");


            Dictionary dictionary2 = CreateDictionary("TestUpdate");
            dictionary2.setUpdates(dictionary.Guid);

            Function updatedFunction = function2.CreateFunctionUpdate(dictionary2);
            Case cas3 = (Case)updatedFunction.Cases[0];
            cas3.ExpressionText = "False";


            Compiler.Compile_Synchronous(true);

            Expression expression = Parser.Expression(dictionary, "N1.q()");
            IValue value = expression.GetValue(new InterpretationContext(), null);

            Expression expression2 = Parser.Expression(dictionary, "N1.f()");
            IValue value2 = expression.GetValue(new InterpretationContext(), null);
            Assert.AreEqual(System.BoolType.False, value2);
        }

        // Further further tests:
        //  Tests for other model elements (Variables, Types, procedures, rules)


    }
}
