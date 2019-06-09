using System;
using System.Collections.Generic;
using System.Text;

namespace VueNote.Core.Util
{
    public class Pageable<T>
    {
        public IEnumerable<T> PageRecords { get; set; }
        public int TotalCount { get; set; }
    }
}
