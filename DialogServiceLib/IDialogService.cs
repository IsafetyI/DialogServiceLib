using CopyingServiceLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DialogServiceLib
{
    public interface IDialogService
    {
        public bool TryRegister<TResult>(CreateDialogPreset<TResult> dialogPreset);

        public bool TryUnregisterCreateDialogPreset<TResult>();

        public bool TryRegister<TResult>(EditDialogPreset<TResult> dialogPreset);

        public bool TryUnregisterEditDialogPreset<TResult>();

        public CreateDialogPreset<TResult> GetCreateDialogPreset<TResult>();

        public EditDialogPreset<TResult> GetEditDialogPreset<TResult>();
    }
}