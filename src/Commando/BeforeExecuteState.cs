using System;

namespace Commando
{
    public class BeforeExecutionResult
    {
        public bool SkipExecution { get; set; }

        public Action<ICommand> AfterAction { get; set; }
    }
}
