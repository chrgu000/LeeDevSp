using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefineDynamicAddControllerAttribue
{
    using HttpGet = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
    using HttpPost = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
    using Route = Microsoft.AspNetCore.Mvc.RouteAttribute;

    internal class ActionModelConvention : IActionModelConvention
    {
        private readonly Type serviceType;

        public ActionModelConvention(Type serviceType)
        {
            this.serviceType = serviceType;
        }

        public void Apply(ActionModel action)
        {
            // 判断是否为指定接口类型的实现类
            if (!serviceType.IsAssignableFrom(action.Controller.ControllerType)) return;

            var actionParams = action.ActionMethod.GetParameters();
            // 这串linq是查询出接口类型中与当前action相对应方法，从中获取特性
            var method = serviceType.GetMethods().FirstOrDefault(mth =>
            {
                var mthParams = mth.GetParameters();
                return action.ActionMethod.Name == mth.Name
                && actionParams.Length == mthParams.Length
                && actionParams.Any(x => mthParams.Where(o => x.Name == o.Name).Any(o => x.GetType() == o.GetType()));
            });

            var attrs = method.GetCustomAttributes(false);
            var actionAttrs = new List<object>();

            foreach (var att in attrs)
            {
                //下面的HttpMethodAttribute是我们自己写的特性类型
                if (att is HttpMethodAttribute methodAttr)
                {
                    var httpMethod = methodAttr.Method;
                    var path = methodAttr.Path;
                    if (httpMethod == HttpMethod.Get)
                    {
                        //添加的HttpGet和HttpPost使用了命名空间别名
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpGet), path));
                    }
                    else if (httpMethod == HttpMethod.Post)
                    {
                        actionAttrs.Add(Activator.CreateInstance(typeof(HttpPost), path));
                    }
                }
                //下面的RouteAttribute是我们自己写的特性类型
                if (att is RouteAttribute routeAttr)
                {
                    actionAttrs.Add(Activator.CreateInstance(typeof(Route), routeAttr.Template));
                }
            }
            if (actionAttrs.Any())
            {
                action.Selectors.Clear();
                //AddRange静态方法就是从源码中复制过来的
                ModelConventionHelper.AddRange(action.Selectors, ModelConventionHelper.CreateSelectors(actionAttrs));
            }
        }
    }
}
