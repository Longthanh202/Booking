namespace Container_App.Model.Bannners
{
    public class InsertBannerDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public int IsActive { get; set; }
        public IFormFile File { get; set; }
    }
}
