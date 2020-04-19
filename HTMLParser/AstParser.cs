using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Dom;

namespace HTMLParser
{
    public static class ExtensionMethods
    {
        public static bool IsDescenentOf(this Type t1, Type t2)
        {
            Queue<Type> queue = new Queue<Type>();
            queue.Enqueue(t1);
            while (queue.Count > 0)
            {
                var t = queue.Dequeue();
                if (t == t2) return true;
                else t.GetInterfaces().ToList().ForEach(i => queue.Enqueue(i));
            }
            return false;
        }
        public static U Cast<U>(this object t) where U : IElement
        {
            if (t is U u) return u;
            throw new InvalidCastException();
        }
    }
}
