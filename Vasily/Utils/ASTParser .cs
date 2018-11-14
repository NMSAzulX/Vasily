using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Vasily
{
    public class ASTParser<T> : CSharpSyntaxWalker
    {
        public string operator_string;
        public string order_string;
        public LinkedList<SqlCondition<T>> conditions;
        public SqlCondition<T> order_condition;
        public (int, int) Page;
        public ASTParser()
        {
            Page.Item1 = -1;
            conditions = new LinkedList<SqlCondition<T>>();
            order_condition = new SqlCondition<T>();
            order_string = null;
            operator_string = null;
        }

        public void OrderCondition(string name)
        {
            if (operator_string != null)
            {
                switch (operator_string)
                {
                    case "+":
                        order_condition = order_condition + name;
                        break;
                    case "-":
                        order_condition = order_condition - name;
                        break;
                    default:
                        break;

                }
            }
        }

        public SqlCondition<T> Result
        {
            get
            {
                if (Page.Item1 == -1)
                {
                    return conditions.Last.Value ^ order_condition;
                }
                else
                {
                    return conditions.Last.Value ^ order_condition ^ Page;
                }
            }
        }


        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            string name = node.Identifier.ValueText;
            Console.WriteLine(name);
            if (name == "c")
            {
                return;
            }
            else
            {
                try
                {
                    name = MakerModel<T>.Column(name);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message},另有可能是构造注入语句，请检查传入信息：{name}");
                }
            }
            SqlCondition<T> temp = new SqlCondition<T>();
            if (operator_string != null)
            {
                switch (operator_string)
                {
                    case "==":
                        conditions.AddLast(temp == name);
                        break;
                    case ">":
                        conditions.AddLast(temp > name);
                        break;
                    case "!=":
                        conditions.AddLast(temp != name);
                        break;
                    case "<":
                        conditions.AddLast(temp < name);
                        break;
                    case ">=":
                        conditions.AddLast(temp >= name);
                        break;
                    case "<=":
                        conditions.AddLast(temp <= name);
                        break;
                    case "%":
                        conditions.AddLast(temp % name);
                        break;
                    default:
                        OrderCondition(name);
                        break;
                }
                operator_string = null;
            }

        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            operator_string = node.OperatorToken.ValueText;
            Console.WriteLine(operator_string);
            this.Visit(node.Left);
            operator_string = node.OperatorToken.ValueText;
            this.Visit(node.Right);
            switch (node.OperatorToken.ValueText)
            {
                case "&":
                    var temp2 = conditions.Last.Value;
                    conditions.RemoveLast();
                    var temp1 = conditions.Last.Value;
                    conditions.RemoveLast();
                    conditions.AddLast(temp1 & temp2);
                    break;
                case "|":
                    temp2 = conditions.Last.Value;
                    conditions.RemoveLast();
                    temp1 = conditions.Last.Value;
                    conditions.RemoveLast();
                    conditions.AddLast(temp1 | temp2);
                    break;
                default:
                    break;
            }
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {

            foreach (var item in node.Body.ChildNodes())
            {
                Console.WriteLine(item.Kind());
                this.Visit(node.Body);
            }
        }
        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            foreach (var item in node.ChildNodes())
            {
                Console.WriteLine(item.Kind());
                this.Visit(item);
            }
        }
        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            Page.Item1 = (int)(node.Arguments[0].Expression.GetFirstToken().Value);
            Page.Item2 = (int)(node.Arguments[1].Expression.GetFirstToken().Value);
        }

        public SqlCondition<T> GetCondition(string value) {
            value = $"public void A(){{{value}}}";
            SyntaxTree tree = CSharpSyntaxTree.ParseText(value);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            this.Visit(root);
            return Result;
        }
    }
}
