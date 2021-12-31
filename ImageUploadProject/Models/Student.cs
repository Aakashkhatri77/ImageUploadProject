using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageUploadProject.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Upload Image")]
        public string Profile { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}










