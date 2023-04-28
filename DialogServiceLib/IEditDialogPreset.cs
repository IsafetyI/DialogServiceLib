using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogServiceLib
{
    public interface IEditDialogPreset<TResult>
    {
        public Tuple<TResult?, bool> ShowDialog(TResult value);
    }
}
