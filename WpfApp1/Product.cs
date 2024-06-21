using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows.Documents;

namespace WpfApp1
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public required byte[] Description { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}