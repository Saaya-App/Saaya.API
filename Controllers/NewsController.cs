using Microsoft.AspNetCore.Mvc;
using Saaya.API.Db;
using Saaya.API.Db.Models;

namespace Saaya.API.Controllers
{
    [Route("[controller]")]
    public class NewsController : Controller
    {
        private readonly ApiContext _db;

        public NewsController(ApiContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Info> GetRecentNews(string? platform)
        {
            if (platform != null)
                return _db.News.Where(x => x.Platform == platform).Take(5).ToList();

            return _db.News.Take(5).ToList();
        }
    }
}