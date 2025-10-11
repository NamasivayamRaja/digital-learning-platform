namespace DigitalLearningPlatform.BuildingBlocks.Common.Helper
{
    public class PaginationHeader(int currentPage, int itemPerPage, int totalPages, int totalItems)
    {
        public int CurrentPage { get; set; } = currentPage;
        public int ItemPerPage { get; set; } = itemPerPage;
        public int TotalItems { get; set; } = totalItems;
        public int TotalPages { get; set; } = totalPages;
    }
}
