using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleRosyln
{
    public class Program2
    {
        static void Main(string[] args)
        {
            People p = new ConsoleRosyln.People();
            SetDesc sd = new ConsoleRosyln.SetDesc(typeof(People), typeof(People).GetProperty("Name"));
            sd.SetValue<People>(p, "Jackie");
            Console.WriteLine(p);

            sd = new SetDesc(typeof(People), typeof(People).GetProperty("Address"));
            var instance = Expression.Constant(sd);
            ConstantExpression ceAddress = Expression.Constant("Gd.Sz");
            MethodInfo mi = typeof(SetDesc).GetMethod("SetValue");
            mi = mi.MakeGenericMethod(typeof(People));
            MethodCallExpression mcSetValue = Expression.Call(instance, mi , new Expression[] { Expression.Constant(p), ceAddress });
            var lm = Expression.Lambda<Action>(mcSetValue);
            lm.Compile()();
            Console.WriteLine(p);

            Console.Read();
        }


    }

    class People
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Age},{Address}\n";
        }
    }


    class SetDesc
    {
        public Type ClassType { get; private set; }
        public PropertyInfo Property { get; private set; }

        public SetDesc(Type classType, PropertyInfo pi)
        {
            ClassType = classType;
            Property = pi;
        }

        public void SetValue<T>(object instanse, object value)
        {
            if (instanse == null)
                throw new ArgumentNullException(nameof(instanse));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Action<T, object> _setValueAction = null;
            if (_setValueAction == null)
            {
                var instanseParameterExpression = Expression.Parameter(typeof(T));
                var valueParameterExpression = Expression.Parameter(typeof(object));
                var convertTypeExpression = Expression.Convert(instanseParameterExpression, ClassType);
                var convertValueExpression = Expression.Convert(valueParameterExpression, Property.PropertyType);
                var propertyExpression = Expression.Property(convertTypeExpression, Property);
                var propertyAssginExpression = Expression.Assign(propertyExpression, convertValueExpression);
                var lambdaExpression = Expression.Lambda<Action<T, object>>(propertyAssginExpression, instanseParameterExpression, valueParameterExpression);
                _setValueAction = lambdaExpression.Compile();
            }
            _setValueAction((T)instanse, value);
        }
    }
}
