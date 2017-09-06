using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace YzMiniProfiler
{

    public class ProfilingActionFilter:ActionFilterAttribute
    {
        //为什么是const?
        private const string profilingActionStartTimeKey = "ProfilingActionFilterStartTime";
        private const string profilingActionDesKey = "ProfilingActionDeskey";


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //获取当前时间，精度够吗？
            DateTime actionExecutingTime = DateTime.Now;
            //线程安全性？
            HttpContext.Current.Items[profilingActionStartTimeKey] = actionExecutingTime;
            //请求的Area,Controler,Action信息
            var ad = filterContext.ActionDescriptor;
            var area = filterContext.RouteData.DataTokens.TryGetValue("area", out object areaToken)
                ? areaToken as string + "."
                : null;
            var actionInfoDesc = "";
            if(area == null)
            {
                actionInfoDesc = "Controller:" + ad.ControllerDescriptor.ControllerName + ",Action:" + ad.ActionName;
            }
            else
            {
                actionInfoDesc= "Area:" + area + " Controller:" + ad.ControllerDescriptor.ControllerName + " Action:" + ad.ActionName;
            }
            HttpContext.Current.Items[profilingActionDesKey] = actionInfoDesc;
            base.OnActionExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if(HttpContext.Current.Items != null && HttpContext.Current.Items.Count> 0 && HttpContext.Current.Items[profilingActionStartTimeKey] != null)
            {
                var actionInvokeEndTime = DateTime.Now;
                var actionInvokeStartTime = (DateTime)HttpContext.Current.Items[profilingActionStartTimeKey];
                //方法结果准确吗？
                var invokeTime = actionInvokeEndTime.Subtract(actionInvokeStartTime).TotalMilliseconds;
                var profilingDesc = (string)HttpContext.Current.Items[profilingActionDesKey] + " 执行 "+invokeTime + " 毫秒";
                Debug.WriteLine(profilingDesc);
            }
            base.OnResultExecuted(filterContext);
        }
    }
}
