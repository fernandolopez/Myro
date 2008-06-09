//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     DSS Runtime Version: 2.0.730.3
//     CLR Runtime Version: 2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using System;
using System.Linq;
using System.Collections.Generic;
using W3C.Soap;

namespace Myro.Services.Generic.Vector
{
    /// <summary>
    /// Vector Contract class
    /// </summary>
    public sealed class Contract
    {

        /// <summary>
        /// The Dss Service contract
        /// </summary>
        [DataMember()]
        public const String Identifier = "http://schemas.tempuri.org/2008/06/vector.html";
    }

    /// <summary>
    /// The Vector State
    /// </summary>
    [DataContract()]
    public class VectorState
    {
        [DataMember()]
        public List<double> Values { get; set; }
        [DataMember()]
        public List<string> Keys { get; set; }
        [DataMember()]
        public DateTime Timestamp { get; set; }
        [DataMember()]
        public Dictionary<string, int> indexCache { get; set; }

        public VectorState() :
            this(null, null, DateTime.Now) { }
        public VectorState(List<Double> values, DateTime timestamp) :
            this(values, null, timestamp) { }
        public VectorState(List<Double> values) :
            this(values, null, DateTime.Now) { }
        public VectorState(bool[] values, DateTime timestamp) :
            this(from v in values select (v ? 1.0 : 0.0), new List<string>(), timestamp) { }

        public VectorState(IEnumerable<double> values, IEnumerable<string> keys, DateTime timestamp)
        {
            Values = (values == null ? new List<double>() : new List<double>(values));
            Keys = (keys == null ? new List<string>() : new List<string>(keys));
            Timestamp = (timestamp == null ? DateTime.Now : timestamp);
            RebuildIndexCache();
        }

        public double Get(int index) { return Values[index]; }
        public double Get(string key) { return Values[indexCache[key]]; }
        public IList<double> GetValues() { return Values; }
        public IList<bool> GetValuesBool() { return new List<bool>(from v in Values select (v >= 0.5 ? true : false)); }
        public void Set(int index, double value) { Values[index] = value; }
        public void Set(string key, double value) { Set(indexCache[key], value); }
        public void SetAll(IEnumerable<bool> values) { SetAll(from v in values select (v ? 1.0 : 0.0)); }
        public void SetAll(IEnumerable<double> values)
        {
            List<double> newValues = new List<double>(values);
            // If the length of the vector changes, rebuild the index cache
            // because the number of elements accessible by key is the lesser
            // of the number of elements and the number of keys.
            if (Values.Count != newValues.Count)
            {
                Values = newValues;
                RebuildIndexCache();
            }
            else
                Values = new List<double>(values);
        }

        private void RebuildIndexCache()
        {
            indexCache = new Dictionary<string, int>(Keys.Count);
            int max = Keys.Count > Values.Count ? Values.Count : Keys.Count;
            for (int i = 0; i < max; i++)
                indexCache.Add(Keys[i], i);
        }
    }

    /// <summary>
    /// Vector Main Operations Port
    /// </summary>
    [ServicePort()]
    public class VectorOperations : PortSet<DsspDefaultLookup, DsspDefaultDrop,
        HttpGet, Get, Replace, GetByIndex, GetByKey, SetByIndex, SetByKey, SetAll, Subscribe>
    {
    }

    public class Get : Get<GetRequestType, PortSet<VectorState, Fault>>
    {
        public Get() : base() { }
        public Get(GetRequestType body) : base(body) { }
        public Get(GetRequestType body, PortSet<VectorState, Fault> responsePort) : base(body, responsePort) { }
    }

    public class Subscribe : Subscribe<SubscribeRequestType, DsspResponsePort<SubscribeResponseType>>
    {
        public Subscribe() : base() { }
        public Subscribe(SubscribeRequestType body) : base(body) { }
        public Subscribe(SubscribeRequestType body, DsspResponsePort<SubscribeResponseType> responsePort) : base(body, responsePort) { }
    }

    public class Replace : Replace<VectorState, DsspResponsePort<DefaultReplaceResponseType>>
    {
        public Replace()
            : base()
        {
        }
        public Replace(VectorState body)
            : base(body)
        {
        }
        public Replace(VectorState body, DsspResponsePort<DefaultReplaceResponseType> responsePort)
            : base(body, responsePort)
        {
        }
    }

    [DataContract]
    [DataMemberConstructor]
    public class GetElementResponseType
    {
        [DataMember]
        public double Value { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        public GetElementResponseType()
        {
            Value = 0.0;
            Timestamp = DateTime.Now;
        }
    }


    [DataContract]
    [DataMemberConstructor]
    public class GetByIndexRequestType
    {
        [DataMember]
        public int Index { get; set; }
        public GetByIndexRequestType()
        {
            Index = 0;
        }
    }
    public class GetByIndex : Update<GetByIndexRequestType, DsspResponsePort<GetElementResponseType>>
    {
        public GetByIndex() : base() { }
        public GetByIndex(GetByIndexRequestType body) : base(body) { }
        public GetByIndex(GetByIndexRequestType body, DsspResponsePort<GetElementResponseType> responsePort) : base(body, responsePort) { }
    }

    [DataContract]
    [DataMemberConstructor]
    public class GetByKeyRequestType
    {
        [DataMember]
        public String Key { get; set; }
        public GetByKeyRequestType()
        {
            Key = "";
        }
    }
    public class GetByKey : Update<GetByKeyRequestType, DsspResponsePort<GetElementResponseType>>
    {
        public GetByKey() : base() { }
        public GetByKey(GetByKeyRequestType body) : base(body) { }
        public GetByKey(GetByKeyRequestType body, DsspResponsePort<GetElementResponseType> responsePort) : base(body, responsePort) { }
    }

    [DataContract]
    [DataMemberConstructor]
    public class SetByIndexRequestType
    {
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public double Value { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        public SetByIndexRequestType()
        {
            Index = 0;
            Value = 0.0;
            Timestamp = DateTime.Now;
        }
    }

    public class SetByIndex : Update<SetByIndexRequestType, DsspResponsePort<DefaultUpdateResponseType>>
    {
        public SetByIndex() : base() { }
        public SetByIndex(SetByIndexRequestType body) : base(body) { }
        public SetByIndex(SetByIndexRequestType body, DsspResponsePort<DefaultUpdateResponseType> responsePort) : base(body, responsePort) { }
    }

    [DataContract]
    [DataMemberConstructor]
    public class SetByKeyRequestType
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public double Value { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        public SetByKeyRequestType()
        {
            Key = "";
            Value = 0.0;
            Timestamp = DateTime.Now;
        }
    }

    public class SetByKey : Update<SetByKeyRequestType, DsspResponsePort<DefaultUpdateResponseType>>
    {
        public SetByKey() : base() { }
        public SetByKey(SetByKeyRequestType body) : base(body) { }
        public SetByKey(SetByKeyRequestType body, DsspResponsePort<DefaultUpdateResponseType> responsePort) : base(body, responsePort) { }
    }

    [DataContract]
    [DataMemberConstructor]
    public class SetAllRequestType
    {
        [DataMember]
        public List<double> Values { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        public SetAllRequestType()
        {
            Values = new List<double>();
            Timestamp = DateTime.Now;
        }
    }

    public class SetAll : Update<SetAllRequestType, DsspResponsePort<DefaultUpdateResponseType>>
    {
        public SetAll() : base() { }
        public SetAll(SetAllRequestType body) : base(body) { }
        public SetAll(SetAllRequestType body, DsspResponsePort<DefaultUpdateResponseType> responsePort) : base(body, responsePort) { }
    }

}
