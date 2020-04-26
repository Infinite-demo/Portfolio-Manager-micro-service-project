using System.ComponentModel.DataAnnotations;

namespace StockManagement.Model
{
    public class StockDetailDto
    {
        [Required(ErrorMessage = "{0} is required")]
        public string StockName { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int Quantity { get; set; }
    }
}
