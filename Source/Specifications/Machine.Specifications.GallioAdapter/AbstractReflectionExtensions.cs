using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gallio.Reflection;

namespace Machine.Specifications.GallioAdapter
{
  public static class AbstractReflectionExtensions
  {
    public static bool IsContext(this ITypeInfo type)
    {
      return !type.IsAbstract
       && type.IsPublic
       && type.GenericArguments.Count == 0
       && (type.GetSpecifications().Any() 
       //|| type.GetBehaviors().Any()
      );
    }

    public static bool IsIgnored(this ITypeInfo type)
    {
      return type.HasAttribute(Reflector.Wrap(typeof (IgnoreAttribute)), true);
    }

    public static ICollection<string> GetTags(this ITypeInfo type)
    {
      //var attributeInfos = type.GetAttributeInfos(Reflector.Wrap(typeof (TagsAttribute)), true);
      //foreach (var attributeInfo in attributeInfos)
      //{
      //  TagsAttribute instance = attributeInfo.Resolve(false) as TagsAttribute;
      //  if (instance == null)
      //  {
      //    return attributeInfos.SelectMany()
      //  }
      //}
      //return type.GetAttributeInfos(Reflector.Wrap(typeof (TagsAttribute)), true)
      //  .SelectMany(x => x.GetPropertyValue("Tags").Value as IEnumerable<string>)
      //  .Distinct().ToList();

      //var distinctListOfTags = new List<string>();
      //(type.GetAttributeInfos(Reflector.Wrap(typeof (TagsAttribute)), true)
      //  .Select(x => x.GetPropertyValue("Tags").Value as IEnumerable<string>)
      //  ).Flatten();
      //foreach (var attrib in type.GetAttributeInfos(Reflector.Wrap(typeof (TagsAttribute)), true)
      //{
      //  var 
      //}

      return new string[] {};
      ////return 
      ////  .Select(x => x.GetPropertyValue("Tags").Value as IEnumerable<string>).FirstOrDefault().ToList();
    }

    static IEnumerable<TResult> Flatten<TSource, TResult>(this IEnumerable<TSource> source,
      Func<TSource, TResult> singleResultSelector, Func<TSource, IEnumerable<TResult>> collectionResultSelector)
    {
      foreach (var s in source)
      {
        yield return singleResultSelector(s);

        foreach (var coll in collectionResultSelector(s))
        {
          yield return coll;
        }
      }
    }

    public static IEnumerable<IFieldInfo> GetSpecifications(this ITypeInfo type)
    {
      return type.GetPrivateFieldsOfType<It>();
    }

    public static IEnumerable<IFieldInfo> GetPrivateFields(this ITypeInfo type)
    {
      return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
        .Where(field => !field.IsStatic);
    }

    public static IEnumerable<IFieldInfo> GetPrivateFieldsOfType<T>(this ITypeInfo type)
    {
      return type.GetPrivateFieldsWith(typeof(T));
    }

    public static IEnumerable<IFieldInfo> GetPrivateFieldsWith(this ITypeInfo type, Type fieldType)
    {
      return type.GetPrivateFields()
        .Where(x => 
          {
            return x.ValueType.IsClass
              && x.ValueType.FullName.Equals(fieldType.FullName);
          }
      );
    }
  }

  public static class RandomExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
      if (enumerable == null)
      {
        return;
      }

      foreach (var item in enumerable)
      {
        action(item);
      }
    }
  }
}