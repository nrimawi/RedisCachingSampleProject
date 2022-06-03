using System.Threading.Tasks;

namespace DAL.Helpers
{
    public interface ICasheServices
    {
        public Task<string> GetCachValueAsync(string key);
        public Task SetCachValueAsync(string key, string value);



    }
}
