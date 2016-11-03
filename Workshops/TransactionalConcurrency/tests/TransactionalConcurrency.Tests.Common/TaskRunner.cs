using System;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionalConcurrency.Tests.Common
{
    public class TaskRunner
    {
        public static void Execute(params Action[] actions)
        {
            var tasks = actions.Select(a => Task.Factory.StartNew(a)).ToList();

            while (tasks.Any(t => !t.IsCompleted))
            {
            }
        }
    }
}