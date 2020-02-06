using System;
using Datadog.Trace.Abstractions;

namespace Datadog.Trace
{
    /// <summary>
    /// A scope is a handle used to manage the concept of an active span.
    /// Meaning that at a given time at most one span is considered active and
    /// all newly created spans that are not created with the ignoreActiveSpan
    /// parameter will be automatically children of the active span.
    /// </summary>
    public class Scope : IScope
    {
        private static ICoreLogger _log = null;

        private readonly IScopeManager _scopeManager;
        private readonly bool _finishOnClose;

        internal Scope(Scope parent, Span span, IScopeManager scopeManager, bool finishOnClose)
        {
            Parent = parent;
            Span = span;
            _scopeManager = scopeManager;
            _finishOnClose = finishOnClose;

            if (_log == null)
            {
                _log = CoreLogStrategy.For<Scope>();
            }
        }

        /// <summary>
        /// Gets the active span wrapped in this scope
        /// </summary>
        public Span Span { get; }

        /// <summary>
        /// Gets the active span wrapped in this scope
        /// Proxy to Span without concrete return value
        /// </summary>
        ISpan IScope.Span => Span;

        internal Scope Parent { get; }

        /// <summary>
        /// Closes the current scope and makes its parent scope active
        /// </summary>
        public void Close()
        {
            _scopeManager.Close(this);

            if (_finishOnClose)
            {
                Span.Finish();
            }
        }

        /// <summary>
        /// Closes the current scope and makes its parent scope active
        /// </summary>
        public void Dispose()
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                // Ignore disposal exceptions here...
                _log.Error(ex, "Error when closing scope.");
            }
        }
    }
}
