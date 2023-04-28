using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DialogServiceLib
{
    public abstract class DialogPreset<TResult>
    {
        protected Type? dialogViewModelType;

        protected Type? dialogViewType;

        public virtual DialogPreset<TResult> UseViewModel<TViewModel>()
            where TViewModel : DialogViewModelBase<TResult>, new()
        {
            dialogViewModelType = typeof(TViewModel);
            return this;
        }

        public virtual DialogPreset<TResult> UseView<TView>()
            where TView : Window, new()
        {
            dialogViewType = typeof(TView);
            return this;
        }
    }
}
