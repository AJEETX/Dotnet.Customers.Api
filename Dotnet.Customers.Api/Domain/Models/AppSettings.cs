namespace Dotnet.Customers.Api.Domain.Models
{
    internal class AppSettings
    {
        public CacheSettings CacheSettings { get; set; }
    }

    public class CacheSettings
    {
        public int AbsoluteExpiration { get; set; }
        public int SlidingExpiration { get; set; }
    }
}