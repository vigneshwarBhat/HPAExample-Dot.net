using System.ComponentModel.DataAnnotations;

namespace HPAExample.Model
{
    public class AddCartRequest
    {
        [Required]
        public Guid CartId { get; set; }
        public int CartLine { get; set; }

    }
}
