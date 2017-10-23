using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;

namespace Sprint.Linq
{  
    public static class Mapper
    {
        private static readonly IDictionary<string, IBuildMap> Maps = new Dictionary<string, IBuildMap>();
 
        public static IInitIExpressionMapper<TSource, TDestination> CreateMap<TSource, TDestination>()
            where TSource : class
            where TDestination : class
        {
            var map=new ExpressionMapper<TSource, TDestination>();

            var key = typeof (TSource).FullName + typeof (TDestination).FullName;

            key = GetHashed(key);

            Maps.Add(key, map);

            return map;
        }

        public static Expression<Func<TSource, TDestination>> Map<TSource, TDestination>(params string[] includes)
        {
            var key = typeof(TSource).FullName + typeof(TDestination).FullName;

            key = GetHashed(key);

            var map = Maps[key];

            return map.Build(includes) as Expression<Func<TSource, TDestination>>;
        }

        public static Expression<Func<TSource, TDestination>> MapAll<TSource, TDestination>()
        {
            var key = typeof(TSource).FullName + typeof(TDestination).FullName;

            key = GetHashed(key);

            var map = Maps[key];

            return map.BuildAll() as Expression<Func<TSource, TDestination>>;
        }

        public static Expression<Func<TSource, TDestination>> MapAll<TSource, TDestination>(params string[] excludeColumns)
        {
            var key = typeof(TSource).FullName + typeof(TDestination).FullName;

            key = GetHashed(key);

            var map = Maps[key];

            return map.BuildAll(excludeColumns) as Expression<Func<TSource, TDestination>>;
        }

        internal static string GetHashed(string text)
        {
            var ue = new UnicodeEncoding();
            var message = ue.GetBytes(text);

            var hashString = new SHA512Managed();
            var hex = String.Empty;

            var hashValue = hashString.ComputeHash(message);
            return hashValue.Aggregate(hex, (current, x) => current + String.Format("{0:x2}", x));
        }              
    }
}
