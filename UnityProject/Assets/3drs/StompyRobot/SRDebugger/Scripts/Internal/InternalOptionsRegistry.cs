using System;
using System.Collections.Generic;
using SRF.Service;

namespace SRDebugger.Internal
{
    /// <summary>
    /// Workaround for the debug panel not being initialized on startup.
    /// SROptions needs to register itself but not cause auto-initialization.
    /// This class buffers requests to register contains until there is a handler in place to deal with them.
    /// Once the handler is in place, all buffered requests are passed in and future requests invoke the handler directly.
    /// </summary>
    [Service(typeof(InternalOptionsRegistry))]
    public sealed class InternalOptionsRegistry
    {
        private List<object> _registeredContainers = new List<object>();
        private Action<object> _handler;
        private Action<object> _removeHandler;

        public void AddOptionContainer(object obj)
        {
            if (!_registeredContainers.Contains(obj))
            {
                _registeredContainers.Add(obj);
            }
            
            if (_handler != null)
            {
                _handler(obj);
            }
        }

        public void SetHandler(Action<object> action, Action<object> removeAction)
        {
            _handler = action;
            _removeHandler = removeAction;
            
            foreach (object o in _registeredContainers)
            {
                _handler(o);
            }
            
            //_registeredContainers = null;
        }

        public void RemoveOptionContainer(Type type)
        {
            for (int i = _registeredContainers.Count - 1; i >= 0; --i)
            {
                if (_registeredContainers[i].GetType() == type)
                {
                    _removeHandler(_registeredContainers[i]);
                    _registeredContainers.Remove(_registeredContainers[i]);
                }
            }
        }
    }
}