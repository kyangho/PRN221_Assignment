namespace SignalR.Helper
{
    public class PageHelper
    {
        public int pageCurrent { get; set; }
        public int totalPages { get; set; }

        public int categoryChoose { get; set; }

        public string search { get; set; }

        public Func<int?, string> generateUrl { get; set; }
    }
}
