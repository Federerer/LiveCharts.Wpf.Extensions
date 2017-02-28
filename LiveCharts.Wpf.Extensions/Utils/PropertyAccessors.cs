using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiveCharts.Wpf.Extensions.Utils
{
    public static class PropertyAccessors
    {
        public static Func<object, object> CreateGetter(string propertyName, object target)
        {
            return CreateGetter<object>(propertyName, target.GetType());
        }

        public static Func<object, object> CreateGetter(string propertyName, Type type)
        {
            return CreateGetter<object>(propertyName, type);
        }

        public static Func<object, T> CreateGetter<T>(string propertyName, object target)
        {
            return CreateGetter<T>(propertyName, target.GetType());
        }

        public static Func<object, T> CreateGetter<T>(string propertyName, Type type)
        {
            var property = type.GetProperty(propertyName);
            if (property == null || !typeof(T).IsAssignableFrom(property.PropertyType))
            {
                return null;
            }

            var helper = typeof(PropertyAccessorsHelper<,,>).MakeGenericType(type, property.PropertyType, typeof(T));

            var method = helper.GetMethod(nameof(PropertyAccessorsHelper<object, object, object>.CreateGetterInternal), BindingFlags.NonPublic | BindingFlags.Static);
            return (Func<object, T>)method.Invoke(null, new object[] { property });

        }

        public static Action<object> CreateSetter<TData>(string propertyName)
        {
            return o => new object();
        }
    }

    internal class PropertyAccessorsHelper<TData, TValue, TReturn> where TValue : TReturn
    {
        delegate TValue RefFunc(ref TData o);

        internal static Func<object, TReturn> CreateGetterInternal(PropertyInfo property)
        {
            var info = property.GetGetMethod();
            if (info.DeclaringType != null && info.DeclaringType.IsValueType)
            {
                var g = (RefFunc)Delegate.CreateDelegate(typeof(RefFunc), info);
                return o =>
                {
                    var data = (TData)o;
                    return g(ref data);
                };
            }

            var getter = (Func<TData, TValue>)Delegate.CreateDelegate(typeof(Func<TData, TValue>), info);
            return o =>
            {
                if (!(o is TData))
                {
                    throw new ArgumentException();
                }
                return getter((TData) o);
            };
        }
    }
}
