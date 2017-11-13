
namespace CBHWA.Models
{
    public class State
    {
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public int? StateCountryKey { get; set; }
        public string StateCountry { get; set; }
        public string StateCountryCode { get; set; }
    }
}