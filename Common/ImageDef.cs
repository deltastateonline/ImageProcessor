namespace Common
{
    public class ImageDef
    {
        public required string Id { get; set; }
        public required string Filename { get; set; }
        public required string InputFolder { get; set; }
        public required string OutputFolder { get; set; }

        public decimal Resize { get; set; }
    }
}
