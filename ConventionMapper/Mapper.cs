using System;
using System.Reflection;

namespace ConventionMapper
{
    public static class Mapper<TSource, TOutput>
        where TOutput : class
        where TSource : class
    {
        [ThreadStatic]
        private static TOutput _existingShape;

        [ThreadStatic]
        private static TSource _source;

        public static TOutput MapByPropertyConvention(TSource source, Func<PropertyInfo, PropertyInfo, bool> propertyInfoConvention = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            _source = source;

            var mappedResult = Activator.CreateInstance<TOutput>();

            FillOutput(mappedResult, propertyInfoConvention);

            return mappedResult;
        }

        public static void MapByPropertyConventionModifiyingExisting(TSource source, TOutput existingOutputToModify, Func<PropertyInfo, PropertyInfo, bool> propertyInfoConvention = null)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (existingOutputToModify == null) throw new ArgumentNullException("existingOutputToModify");

            _source = source;
            _existingShape = existingOutputToModify;

            FillOutput(_existingShape, propertyInfoConvention);
        }

        private static void FillOutput(TOutput mappedResult, Func<PropertyInfo, PropertyInfo, bool> propertyInfoConvention)
        {
            if (propertyInfoConvention == null)
                propertyInfoConvention = (sType, tType)
                                        => sType.Name == tType.Name
                                        && sType.PropertyType == tType.PropertyType;

            foreach (var sourcePropertyInfo in typeof(TSource).GetProperties())
            {
                FillMatchedProperties(_source, sourcePropertyInfo, mappedResult, propertyInfoConvention);
            }
        }

        private static void FillMatchedProperties(TSource source, PropertyInfo sourcePropertyInfo, TOutput result, Func<PropertyInfo, PropertyInfo, bool> propertyInfoConvention)
        {
            var sourceValue = sourcePropertyInfo.GetValue(source, null);

            PropertyInfo[] targetPropertyInfos = typeof(TOutput).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.SetProperty);

            foreach (var targetPropertyInfo in targetPropertyInfos)
            {
                if (propertyInfoConvention(sourcePropertyInfo, targetPropertyInfo))
                {
                    if (_existingShape != null)
                    {
                        object targetValue = targetPropertyInfo.GetValue(_existingShape, null);
                        if (sourceValue != targetValue)
                        {
                            targetPropertyInfo.SetValue(result, sourceValue ?? targetValue, null);
                        }
                    }
                    else
                    {
                        targetPropertyInfo.SetValue(result, sourceValue, null);
                    }
                }
            }
        }
    }

}
