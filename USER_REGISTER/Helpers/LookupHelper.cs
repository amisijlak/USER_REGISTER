using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using USER_REGISTER.DAL.Interfaces;

namespace USER_REGISTER
{
    public static class LookupHelper
    {
        #region INameMapped Lookups

        public static IEnumerable<object> ConvertNamedModelToEnumerable<T, O>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<O>, INamedModel
        {
            return query.OrderBy(r => r.Name).ToList().OrderBy(r=>r.Id).Select(r => new { Key = r.Name, Value = r.Id }).ToList();
        }

        private static SelectList ConvertNamedModelToSelectList<T, O>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<O>, INamedModel
        {
            var data = ConvertNamedModelToEnumerable<T,O>(query);

            return new SelectList(data, "Value", "Key");
        }

        public static IEnumerable<Tuple<object, object>> ConvertToKeyValueTuples<T>(this IEnumerable<T> Collection, Func<T, object> KeyExpression
           , Func<T, object> ValueExpression)
        {
            return (Collection ?? new List<T>()).Select(r => new Tuple<object, object>(KeyExpression(r), ValueExpression(r)));
        }

        public static SelectList GetSelectList(this IEnumerable<Tuple<object, object>> KeyValueTuples, object selectedValue = null)
        {
            return new SelectList(KeyValueTuples ?? new List<Tuple<object, object>>(), "Item2", "Item1", selectedValue);
        }

        public static SelectList ConvertNamedNumericModelToSelectList<T>(this IQueryable<T> query) where T : class, INumericPrimaryKey, INamedModel
        {
            return ConvertNamedModelToSelectList<T, long>(query);
        }

        public static SelectList ConvertNamedGuidModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<Guid>, INamedModel
        {
            return ConvertNamedModelToSelectList<T, Guid>(query);
        }

        public static SelectList ConvertNamedStringModelToSelectList<T>(this IQueryable<T> query) where T : class, IPrimaryKeyEnabled<string>, INamedModel
        {
            return ConvertNamedModelToSelectList<T, string>(query);
        }

        public static SelectList GetSelectList<T>(this IEnumerable<T> Collection, Func<T, object> KeyExpression, Func<T, object> ValueExpression,
           object selectedValue = null)
        {
            return Collection.ConvertToKeyValueTuples(KeyExpression, ValueExpression).GetSelectList(selectedValue);
        }

        #endregion
    }
}
