using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MyCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CodeDomProvider

            string res = string.Format(CultureInfo.CurrentCulture, GetString("ViewComponent_AmbiguousTypeMatch_Item"), 100, "well");
        }

        private static string GetString(string name, params string[] formatterNames)
        {
            var value = "Type: '{0}' - Name: '{1}'";// _resourceManager.GetString(name);

            System.Diagnostics.Debug.Assert(value != null);

            if (formatterNames != null)
            {
                for (var i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
