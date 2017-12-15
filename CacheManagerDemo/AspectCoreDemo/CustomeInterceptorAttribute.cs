using AspectCore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCoreDemo
{
    public class CustomeInterceptorAttribute : InterceptorAttribute
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
            }
            catch (Exception)
            {
                Console.WriteLine("Server threw an exception!");
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }
    }
}
