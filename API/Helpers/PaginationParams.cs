namespace API.Helpers
{
    public class PaginationParams
    {
        private const int MaxPageSize=50;
        //bellow are listed the values that are gonna be returned
        public int PageNumber { get; set; }=1;
        private int _pageSize=10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value>MaxPageSize) ? MaxPageSize:value ;
        }
        
    }
}