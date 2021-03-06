// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.UnitTests;
using Xunit;

namespace Microsoft.QualityGuidelines.Analyzers.UnitTests
{
    public class PreferJaggedArraysOverMultidimensionalTests : DiagnosticAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetBasicDiagnosticAnalyzer()
        {
            return new PreferJaggedArraysOverMultidimensionalAnalyzer();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PreferJaggedArraysOverMultidimensionalAnalyzer();
        }

        [Fact]
        public void CSharpSimpleMembers()
        {
            VerifyCSharp(@"
public class Class1
{
    public int[,] MultidimensionalArrayField;
    
    public int[,] MultidimensionalArrayProperty
    {
        get { return null; }
    }

    public int[,] MethodReturningMultidimensionalArray()
    {
        return null;
    }

    public void MethodWithMultidimensionalArrayParameter(int[,] multidimensionalParameter) { }

    public void MethodWithMultidimensionalArrayCode()
    {
        int[,] multiDimVariable = new int[5, 5];
        multiDimVariable[1, 1] = 3;
    }

    public int[,][] JaggedMultidimensionalField;
}

public interface IInterface
{
    int[,] InterfaceMethod(int[,] array);
}
",
            GetCSharpDefaultResultAt(4, 19, "MultidimensionalArrayField"),
            GetCSharpDefaultResultAt(6, 19, "MultidimensionalArrayProperty"),
            GetCSharpReturnResultAt(11, 19, "MethodReturningMultidimensionalArray", "int[*,*]"),
            GetCSharpDefaultResultAt(16, 65, "multidimensionalParameter"),
            GetCSharpBodyResultAt(20, 35, "MethodWithMultidimensionalArrayCode", "int[*,*]"),
            GetCSharpDefaultResultAt(24, 21, "JaggedMultidimensionalField"),
            GetCSharpReturnResultAt(29, 12, "InterfaceMethod", "int[*,*]"),
            GetCSharpDefaultResultAt(29, 35, "array"));
        }

        [Fact]
        public void BasicSimpleMembers()
        {
            VerifyBasic(@"
Public Class Class1
    Public MultidimensionalArrayField As Integer(,)

    Public ReadOnly Property MultidimensionalArrayProperty As Integer(,)
        Get
            Return Nothing
        End Get
    End Property

    Public Function MethodReturningMultidimensionalArray() As Integer(,)
        Return Nothing
    End Function

    Public Sub MethodWithMultidimensionalArrayParameter(multidimensionalParameter As Integer(,))
    End Sub

    Public Sub MethodWithMultidimensionalArrayCode()
        Dim multiDimVariable(5, 5) As Integer
        multiDimVariable(1, 1) = 3
    End Sub

    Public JaggedMultidimensionalField As Integer(,)()
End Class

Public Interface IInterface
    Function InterfaceMethod(array As Integer(,)) As Integer(,)
End Interface
",
            GetBasicDefaultResultAt(3, 12, "MultidimensionalArrayField"),
            GetBasicDefaultResultAt(5, 30, "MultidimensionalArrayProperty"),
            GetBasicReturnResultAt(11, 21, "MethodReturningMultidimensionalArray", "Integer(*,*)"),
            GetBasicDefaultResultAt(15, 57, "multidimensionalParameter"),
            GetBasicBodyResultAt(19, 13, "MethodWithMultidimensionalArrayCode", "Integer(*,*)"),
            GetBasicDefaultResultAt(23, 12, "JaggedMultidimensionalField"),
            GetBasicReturnResultAt(27, 14, "InterfaceMethod", "Integer(*,*)"),
            GetBasicDefaultResultAt(27, 30, "array"));
        }

        [Fact]
        public void CSharpNoDiagostics()
        {
            VerifyCSharp(@"
public class Class1
{
    public int[][] JaggedArrayField;
    
    public int[][] JaggedArrayProperty
    {
        get { return null; }
    }

    public int[][] MethodReturningJaggedArray()
    {
        return null;
    }

    public void MethodWithJaggedArrayParameter(int[][] jaggedParameter) { }
}
");
        }

        [Fact]
        public void BasicNoDiangnostics()
        {
            VerifyBasic(@"
Public Class Class1
    Public JaggedArrayField As Integer()()

    Public ReadOnly Property JaggedArrayProperty As Integer()()
        Get
            Return Nothing
        End Get
    End Property

    Public Function MethodReturningJaggedArray() As Integer()()
        Return Nothing
    End Function

    Public Sub MethodWithJaggedArrayParameter(jaggedParameter As Integer()())
    End Sub
");
        }

        [Fact]
        public void CSharpOverridenMembers()
        {
            VerifyCSharp(@"
public class Class1
{
    public virtual int[,] MultidimensionalArrayProperty
    {
        get { return null; }
    }

    public virtual int[,] MethodReturningMultidimensionalArray()
    {
        return null;
    }
}

public class Class2 : Class1
{
    public override int[,] MultidimensionalArrayProperty
    {
        get { return null; }
    }

    public override int[,] MethodReturningMultidimensionalArray()
    {
        return null;
    }
}
",
            GetCSharpDefaultResultAt(4, 27, "MultidimensionalArrayProperty"),
            GetCSharpReturnResultAt(9, 27, "MethodReturningMultidimensionalArray", "int[*,*]"));
        }

        [Fact]
        public void BasicOverriddenMembers()
        {
            VerifyBasic(@"
Public Class Class1
    Public Overridable ReadOnly Property MultidimensionalArrayProperty As Integer(,)
        Get
            Return Nothing
        End Get
    End Property

    Public Overridable Function MethodReturningMultidimensionalArray() As Integer(,)
        Return Nothing
    End Function
End Class

Public Class Class2
    Inherits Class1
    Public Overrides ReadOnly Property MultidimensionalArrayProperty As Integer(,)
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides Function MethodReturningMultidimensionalArray() As Integer(,)
        Return Nothing
    End Function
End Class
",
            GetBasicDefaultResultAt(3, 42, "MultidimensionalArrayProperty"),
            GetBasicReturnResultAt(9, 33, "MethodReturningMultidimensionalArray", "Integer(*,*)"));
        }

        private DiagnosticResult GetCSharpDefaultResultAt(int line, int column, string symbolName)
        {
            return GetCSharpResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.DefaultRule, symbolName);
        }

        private DiagnosticResult GetCSharpReturnResultAt(int line, int column, string symbolName, string typeName)
        {
            return GetCSharpResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.ReturnRule, symbolName, typeName);
        }
        private DiagnosticResult GetCSharpBodyResultAt(int line, int column, string symbolName, string typeName)
        {
            return GetCSharpResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.BodyRule, symbolName, typeName);
        }

        private DiagnosticResult GetBasicDefaultResultAt(int line, int column, string symbolName)
        {
            return GetBasicResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.DefaultRule, symbolName);
        }

        private DiagnosticResult GetBasicReturnResultAt(int line, int column, string symbolName, string typeName)
        {
            return GetBasicResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.ReturnRule, symbolName, typeName);
        }
        private DiagnosticResult GetBasicBodyResultAt(int line, int column, string symbolName, string typeName)
        {
            return GetBasicResultAt(line, column, PreferJaggedArraysOverMultidimensionalAnalyzer.BodyRule, symbolName, typeName);
        }
    }
}