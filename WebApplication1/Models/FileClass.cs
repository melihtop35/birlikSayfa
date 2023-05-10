namespace WebApplication1.Models
{
    public class FileClass
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public List<FileClass> Files { get; set; }=new List<FileClass>();
    }
}
