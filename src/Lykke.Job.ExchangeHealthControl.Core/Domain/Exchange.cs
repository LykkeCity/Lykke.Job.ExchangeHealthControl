using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Newtonsoft.Json;

namespace Lykke.Job.ExchangeHealthControl.Core.Domain
{
    public class Exchange : IKeyedObject, ICloneable
    {
        public string Name { get; private set; }
        
        //TODO add metrics here
        
        [JsonIgnore]
        public string GetKey => Name;

        public Exchange(string name)
        {
            Name = name;
        }
        
        public object Clone()
        {
            return new Exchange(this.Name)
            {
            };
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
