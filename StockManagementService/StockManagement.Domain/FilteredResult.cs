using System.Collections.Generic;

namespace StockManagement.Domain
{
    public class FilteredResult
    {
        public int TotalCount { get; set; }
        public List<StockDetail> Result { get; set; }
    }
}
