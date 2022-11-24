namespace Cloud5mins.AzShortener
{
    public class UrlClickStatsRequest
    {
        public string Vanity { get; set; }

        public UrlClickStatsRequest(string vanity)
        {
            Vanity = vanity;
        }
    }
}