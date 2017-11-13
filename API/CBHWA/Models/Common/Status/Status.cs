
namespace CBHWA.Models
{
    public class Status
    {
        public int StatusKey { get; set; }
        public int StatusCategory { get; set; }
        public int StatusSort { get; set; }
        public string StatusText { get; set; }
        public bool StatusPublicDefault { get; set; }
        public bool StatusCustEntry { get; set; }
        public bool StatusClosed { get; set; }
        public bool StatusCompleted { get; set; }
        public int StatusStatusKey { get; set; }
    }


}