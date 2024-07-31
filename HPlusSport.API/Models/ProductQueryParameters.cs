namespace HPlusSport.API.Models
{
    public class ProductQueryParameters : QueryParameters
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }


        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        public string AllSearch { get; set; } = string.Empty;

        public int categoryId { get; set; } 

    }

}
