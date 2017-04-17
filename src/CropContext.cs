
namespace CropScan
{
    public struct CropContext
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public int SrcWidthPx { get; set; }
        public int SrcHeightPx { get; set; }
        public decimal SrcWidthCm { get; set; }
        public decimal SrcHeightCm { get; set; }
        public int DestWidthPx { get; set; }
        public int DestHeightPx { get; set; }
        public decimal? DestWidthCm { get; set; }
        public decimal? DestHeightCm { get; set; }
        public decimal ResX { get; set; }
        public decimal ResY { get; set; }
        public string Message { get; set; }
    }
}
