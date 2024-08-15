using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombEscape
{
    internal class AssetHolder<T>
    {
        Dictionary<string, T> assets;

        public AssetHolder()
        {
            assets = new();
        }

        public void Add(string key, T value)
        {
            assets.Add(key, value);
        }

        public T Get(string key)
        {
            return assets[key];
        }
    }
}
