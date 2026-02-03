namespace Core.Reports.ViewModel
{
    public class ProductGraphViewModel
    {
        public string Bullet { get; set; }
        public string Title { get; set; }
        public string ValueField { get; set; }
        public string ValueAxis { get; set; }
        public string LineColor { get; set; }
        public string BalloonText { get; set; }
        public int LineThickness { get; set; }
        public string type { get; set; }
        public double ColumnWidth { get; set; }
        public double FillAlphas { get; set; }
        public double LineAlpha { get; set; }
        public ProductGraphViewModel()
        {
            Bullet = "round";
            LineAlpha = 1;
            FillAlphas = 0;
        }
    }
}
