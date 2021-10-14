using System;
using System.Collections.Generic;
using System.Linq;

namespace N.Analysis
{
    public sealed class Executor
        {
            private readonly Expression _root;
            public Executor(SyntaxTree _tree)
            {
                _root = _tree.root;
            }

            public int Eval()
            {
                return EvalExpr(_root);
            }

            private int EvalExpr(Expression _expr)
            {
                if (_expr is NumberExpr num)
                    return (int)num.numberToken.value;
                if (_expr is BinaryExpr bin)
                {
                    var left = EvalExpr(bin.left);
                    var right = EvalExpr(bin.right);
                    if (bin.binaryOperator.type == TokenType.MathAdd)
                        return left + right;
                    else if (bin.binaryOperator.type == TokenType.MathSubtract)
                        return left - right;
                    else if (bin.binaryOperator.type == TokenType.MathMultiply)
                        return left * right;
                    else if (bin.binaryOperator.type == TokenType.MathDivide)
                        return left / right;
                    else throw new Exception($"Unexpected token {bin.binaryOperator.type}");
                }

                if (_expr is ParenthesisExpr par)
                    return EvalExpr(par.expression);

                throw new Exception($"Unexpected node {_expr.type}");
            }
        }
}