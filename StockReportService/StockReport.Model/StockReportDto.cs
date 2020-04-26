using System.ComponentModel.DataAnnotations;

namespace StockReport.Model
{
    public class StockReportDto
    {
        [Required(ErrorMessage = "{0} is required")]
        public string StockName { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int Quantity { get; set; }
    }
}
