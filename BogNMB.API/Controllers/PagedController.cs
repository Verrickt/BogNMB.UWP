namespace BogNMB.API.Controllers
{
    public abstract class PagedController : ControllerBase
    {
        public int PageSize { get; private set; }
        public PagedController(ApiConfig config) : base(config)
        {
            PageSize = config.ConfigStore.GetConfigForController<int>(GetType());
        }
    }
}
