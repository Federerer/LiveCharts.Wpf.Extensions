using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiveCharts.Wpf.Extensions.PropertyAccessors
{
    public static class PropertyAccessors
    {
        public static Func<object, object> CreateGetter<TData>(string propertyName)
        {
            var property = typeof(TData).GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }
            var helper = typeof(PropertyAccessorsHelper<,>).MakeGenericType(typeof(TData), property.PropertyType);

            var method = helper.GetMethod(nameof(PropertyAccessorsHelper<object,object>.CreateGetterInternal), BindingFlags.NonPublic | BindingFlags.Static);
            return (Func<object, object>)method.Invoke(null, new object[] { property });
        }

        public static Action<object> CreateSetter<TData>(string propertyName)
        {
            return o => new object();
        }
    }

    internal class PropertyAccessorsHelper<TData, TValue>
    {
        delegate TValue RefFunc(ref TData o);

        internal static Func<object, object> CreateGetterInternal(PropertyInfo property)
        {
            var info = property.GetGetMethod();
            if (info.DeclaringType.IsValueType)
            {
                var g = (RefFunc)Delegate.CreateDelegate(typeof(RefFunc), info);
                return o =>
                {
                    var data = (TData)o;
                    return g(ref data);
                };
            }
            var getter = (Func<TData, TValue>)Delegate.CreateDelegate(typeof(Func<TData, TValue>), info);
            return o => getter((TData)o);
        }
    }
}
