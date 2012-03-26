using System;
using System.Collections.Generic;

namespace fastBinaryJSON
{
    internal class Getters
    {
        public string Name;
        public BJSON.GenericGetter Getter;
        public Type propertyType;
    }

    public class DatasetSchema
    {
        public List<string> Info { get; set; }
        public string Name { get; set; }
    }
}
