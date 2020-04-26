using System.Collections.Generic;

namespace StockReport.Domain
{
    public class FilteredResult<T>
    {
        public int TotalCount { get; set; }
        public List<T> Result { get; set; }
    }
}
