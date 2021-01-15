using System.Threading.Tasks;
using Nuka.MVC.Web.Models;
using Refit;

namespace Nuka.MVC.Web.Refit
{
    public interface ISampleApi
    {
        [Get("/Sample/Item/{id}")]
        Task<SampleItemModel> GetItemById(int id);
    }
}