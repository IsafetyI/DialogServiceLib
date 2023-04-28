using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogServiceLib
{
    public class DialogViewModelBase<TResult> : ViewModelBase
    {
        public TResult InputValue { get; set; }

        public AddEditDialogMode Mode { get; set;}

        public Tuple<TResult?, bool>? DialogResult { get; protected set; }
    }
}
