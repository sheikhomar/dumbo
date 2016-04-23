using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dumbo.Compiler;
using dumbo.Compiler.AST;
using dumbo.Compiler.Interpreter;
using dumbo.Compiler.TypeChecking;

namespace dumbo.WpfApp
{
    class HappyCProcessor
    {
        private class RunnerArgument {
            public EventReporter Reporter { get; }
            public IInteractiveShell Shell { get; }
            public RootNode Root { get; }

            public RunnerArgument(EventReporter reporter, IInteractiveShell shell, RootNode root)
            {
                Reporter = reporter;
                Shell = shell;
                Root = root;
            }
        }

        public event EventHandler<ProcessorResult> Success;

        public event EventHandler<IEnumerable<Exception>> Failure;
        
        public void Start(EventReporter reporter, IInteractiveShell shell, RootNode root)
        {
            var args = new RunnerArgument(reporter, shell, root);
            var task = Task.Factory.StartNew(Run, args);

            TaskScheduler currentContext = TaskScheduler.FromCurrentSynchronizationContext();

            task.ContinueWith(TaskComplete, currentContext);
            task.ContinueWith(TaskFaulted, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, currentContext);
        }

        public static ProcessorResult Run(object args)
        {
            RunnerArgument arguments = args as RunnerArgument;
            var scopeChecker = new ScopeCheckVisitor(arguments.Reporter);
            var typeChecker = new TypeCheckVisitor(arguments.Reporter);
            var interpreter = new InterpretationVisitor(arguments.Reporter, arguments.Shell);
            arguments.Root.Accept(scopeChecker, new VisitorArgs());
            arguments.Root.Accept(typeChecker, new VisitorArgs());
            if (!arguments.Reporter.HasErrors)
            {
                arguments.Root.Accept(interpreter, new VisitorArgs());
            }

            return new ProcessorResult(arguments.Reporter);
        }

        protected virtual void OnSuccess(ProcessorResult result)
        {
            var handler = Success;
            handler?.Invoke(this, result);
        }

        protected virtual void OnFailure(IEnumerable<Exception> arg)
        {
            var handler = Failure;
            handler?.Invoke(this, arg);
        }

        private void TaskComplete(Task<ProcessorResult> task)
        {
            OnSuccess(task.Result);
        }

        private void TaskFaulted(Task<ProcessorResult> task)
        {
            if (task.Exception != null)
                OnFailure(task.Exception.InnerExceptions);
        }
    }
}
