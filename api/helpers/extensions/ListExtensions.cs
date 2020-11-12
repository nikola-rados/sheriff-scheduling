﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SS.Api.helpers.extensions
{
    public static class ListExtensions
    {
        public static List<T2> SelectToList<T,T2>(this IEnumerable<T> target, Func<T, T2> lambda)
        {
            return target.Select(lambda).ToList();
        }

        public static List<T> WhereToList<T>(this IEnumerable<T> target, Func<T, bool> lambda)
        {
            return target.Where(lambda).ToList();
        }

        public static string ToStringWithPipes(this IEnumerable<string> contents) => string.Join("||", contents);
    }
}
