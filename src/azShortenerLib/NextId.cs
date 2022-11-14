using Microsoft.Azure.Cosmos.Table;

namespace Cloud5mins.AzShortener
{
    public class NextId : TableEntity 
    {
        public int Id { get; set; }
    }
}