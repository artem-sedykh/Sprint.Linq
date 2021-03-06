﻿using System;
using System.Linq.Expressions;
// ReSharper disable once CheckNamespace

namespace Sprint.Linq
{
    public interface IExpressionMapper<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        IExpressionMapper<TSource, TDestination> Include(string key, Expression<Func<TSource, TDestination>> projection);
    }
}
