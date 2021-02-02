using System;
using System.Windows;

namespace Bannerlord.UIEditor.AppContext
{
    public interface IApplicationManager
    {
        Window? MainWindow { get; set; }
        void Dispatch(Action _action, bool _invokeSynchronously = true);
    }
}
