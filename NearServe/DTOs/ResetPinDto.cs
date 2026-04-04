namespace NearServe.DTOs
{
    public class ResetPinDto
    {
        public int CID { get; set; }
        public string OldPin { get; set; } = string.Empty;
        public string NewPin { get; set; } = string.Empty;
    }
}
