using System;
using Microsoft.Xrm.Sdk;

namespace BasicPlugin
{
    internal class AttrWrapper<T>
    {
        private readonly string _name;
        private readonly bool _useDefault;

        public string Name => _name;

        public AttrWrapper(string name, bool useDefault = true)
        {
            _name = name;
            _useDefault = useDefault;
        }

        public T Get(Entity entity)
        {
            if (entity.TryGetAttributeValue<T>(_name, out var tData))
            {
                return tData;
            }
            if (_useDefault)
            {
                return default;
            }
            throw new ArgumentOutOfRangeException(_name, "Corresponding value does not exist");
        }

        public void Set(Entity entity, T val)
        {
            entity.Attributes[_name] = val;
        }
    }
}
