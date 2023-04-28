using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyingServiceLib
{
    public class ValueCopyingService : ICopyingService
    {
        public object? Copy(object? source, object? target)
        {
            return source;
        }
    }

    public class ValueCopyingService<TItem> : ValueCopyingService, ICopyingService<TItem>
    {
        public TItem? Copy(TItem? source, TItem? target)
        {
            return source;
        }
    }
}