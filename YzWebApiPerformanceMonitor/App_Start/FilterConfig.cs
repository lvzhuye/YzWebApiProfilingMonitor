using System.Web;
using System.Web.Mvc;
using YzMiniProfiler;

namespace YzWebApiPerformanceMonitor
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ProfilingActionFilter());
        }
    }
}
