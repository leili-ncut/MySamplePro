using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySamplePro.Model;

namespace MySamplePro.Filter
{
    /// <summary>
    /// 定义一个ActionFilter，统一处理模型验证
    /// </summary>
    public class XcActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                XcHttpResult result = new XcHttpResult() { Result = false };

                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        result.Msg += error.ErrorMessage + "|";
                    }
                }

                context.Result = new JsonResult(result);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
