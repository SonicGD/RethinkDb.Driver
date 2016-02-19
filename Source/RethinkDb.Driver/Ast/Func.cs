using System.Collections.Generic;
using System.Threading;
using RethinkDb.Driver.Model;

namespace RethinkDb.Driver.Ast
{
    public partial class Func
    {
        private static int varId = 0;

        internal static int NextVarId()
        {
            return Interlocked.Increment(ref varId);
        }

        internal static Func MakeFunc(IList<int> varIds, ReqlExpr appliedFunction)
        {
            return new Func(Arguments.Make(
                new MakeArray(varIds),
                Util.ToReqlAst(appliedFunction)
                ));
        }
    }
}