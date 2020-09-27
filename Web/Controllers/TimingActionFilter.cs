using Common;
using System.Diagnostics;
using System.Web.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// 监视 action执行 及 页面渲染 时间
    /// </summary>
    public class TimingActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        { 
            GetTimer(filterContext, "action").Start();
            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            GetTimer(filterContext, "action").Stop();
            base.OnActionExecuted(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var renderTimer = GetTimer(filterContext, "render");
            renderTimer.Stop();
            var actionTimer = GetTimer(filterContext, "action");
            if (actionTimer.ElapsedMilliseconds + renderTimer.ElapsedMilliseconds >= 1)
            {
                LogHelper.Debug(string.Format("运营监控： {0}/{1}/{2} ,执行:{3}ms,渲染:{4}ms",
                    filterContext.RouteData.DataTokens["area"],
                    filterContext.RouteData.Values["controller"],
                    filterContext.RouteData.Values["action"],
                    actionTimer.ElapsedMilliseconds, renderTimer.ElapsedMilliseconds));
            }
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            GetTimer(filterContext, "render").Start();
            base.OnResultExecuting(filterContext);
        }
        private Stopwatch GetTimer(ControllerContext context, string name)
        {
            string key = "__timer__" + name;
            if (context.HttpContext.Items.Contains(key))
            {
                return (Stopwatch)context.HttpContext.Items[key];
            }

            var result = new Stopwatch();
            context.HttpContext.Items[key] = result;
            return result;
        }
    }
}