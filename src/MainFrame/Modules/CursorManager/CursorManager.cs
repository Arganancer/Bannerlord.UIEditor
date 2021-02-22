using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Bannerlord.UIEditor.Core;

namespace Bannerlord.UIEditor.MainFrame
{
    public class CursorManager : Module, ICursorManager
    {
        public static CursorManager? Instance { get; private set; }

        private readonly Dictionary<CursorIcon, Cursor> m_Cursors = new();

        public void SetCursor(CursorIcon _cursorIcon)
        {
            var cursor = m_Cursors[_cursorIcon];
            if (cursor is not null)
            {
                Mouse.SetCursor(cursor);
            }
        }

        public Cursor GetCursor(CursorIcon _cursorIcon)
        {
            return m_Cursors[_cursorIcon]!;
        }

        public void Initialize(MainWindow _mainWindow)
        {
            var cursorResourceDictionary = _mainWindow.Resources.FindMergedDictionaryWithKeys(Enum.GetName(typeof(CursorIcon), CursorIcon.MoveIcon)!);
            foreach (var cursorVisual in Enum.GetValues(typeof( CursorIcon )).Cast<CursorIcon>())
            {
                Cursor? cursor = null;
                try
                {
                    object? cursorElement;
                    _mainWindow.Dispatcher.Invoke(() =>
                    {
                        cursorElement = cursorResourceDictionary?[cursorVisual.ToString()];
                        if (cursorElement is CursorViewbox cursorViewbox)
                        {
                            cursor = CursorCreator.CreateCursor(cursorViewbox);
                            Disposing += (_, _) => cursor.Dispose();
                        }
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (cursor is null)
                {
                    cursor = cursorVisual switch
                    {
                        CursorIcon.NoIcon => Cursors.No,
                        CursorIcon.SizeNWSEIcon => Cursors.SizeNWSE,
                        CursorIcon.SizeWEIcon => Cursors.SizeWE,
                        CursorIcon.InsertIcon => Cursors.SizeWE,
                        CursorIcon.SizeNESWIcon => Cursors.SizeNESW,
                        CursorIcon.SizeNSIcon => Cursors.SizeNS,
                        CursorIcon.MoveIcon => Cursors.Cross,
                        CursorIcon.AddIcon => Cursors.Cross,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                m_Cursors.Add(cursorVisual, cursor);
            }
        }

        public override void Create(IPublicContainer _publicContainer)
        {
            base.Create(_publicContainer);
            Instance = this;
            RegisterModule<ICursorManager>();
        }
    }
}
