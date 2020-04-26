using System.Collections.Generic;

namespace UserManagement.Domain
{
    public class FilteredResult
    {
        public int TotalCount { get; set; }
        public List<User> Result { get; set; }
    }
}
