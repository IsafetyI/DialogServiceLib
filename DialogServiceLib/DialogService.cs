using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogServiceLib
{
    public class DialogService : IDialogService
    {
        protected Dictionary<Type, object> createDialogPresets = new();

        protected Dictionary<Type, object> editDialogPresets = new();

        public CreateDialogPreset<TResult> GetCreateDialogPreset<TResult>()
        {
            if (createDialogPresets[typeof(TResult)] != null)
                return (CreateDialogPreset<TResult>)createDialogPresets[typeof(TResult)];
            throw new Exception("Create dialog preset for this type wasn't registered");
        }

        public EditDialogPreset<TResult> GetEditDialogPreset<TResult>()
        {
            if (editDialogPresets[typeof(TResult)] != null)
                return (EditDialogPreset<TResult>)editDialogPresets[typeof(TResult)];
            throw new Exception("Edit dialog preset for this type wasn't registered");
        }

        public bool TryRegister<TResult>(CreateDialogPreset<TResult> dialogPreset)
        {
            if (createDialogPresets.ContainsKey(typeof(TResult)))
                return false;
            createDialogPresets.Add(typeof(TResult), dialogPreset);
            return true;
        }

        public bool TryRegister<TResult>(EditDialogPreset<TResult> dialogPreset)
        {
            if (editDialogPresets.ContainsKey(typeof(TResult)))
                return false;
            editDialogPresets.Add(typeof(TResult), dialogPreset);
            return true;
        }

        public bool TryUnregisterCreateDialogPreset<TResult>()
        {
            if (!createDialogPresets.ContainsKey(typeof(TResult)))
                return false;
            createDialogPresets.Remove(typeof(TResult));
            return true;
        }

        public bool TryUnregisterEditDialogPreset<TResult>()
        {
            if (!editDialogPresets.ContainsKey(typeof(TResult)))
                return false;
            editDialogPresets.Remove(typeof(TResult));
            return true;
        }
    }
}
