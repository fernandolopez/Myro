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
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using W3C.Soap;
using Myro.Utilities;

namespace Myro.Services.Generic.Vector
{
    /// <summary>
    /// This is a base class for building services that can represent their
    /// data as a Vector, or a set of key-value pairs.  This class makes
    /// building new services easy, since it takes care of all get and set
    /// requests (get/set by index, by key, or all elements at once), and also
    /// handles subscribers.
    /// 
    /// To build a sensor service, you have to implement some way of updating
    /// the state.  This can either be with a subscription to another service,
    /// a periodic timer, or with a callback (by overriding GetCallback, which
    /// will be called whenever any part of the state is requested).
    /// 
    /// To build an actuator service, your options are basically the same.  If
    /// another service subscribes to this one, you don't have to do anything.
    /// Otherwise, you can add a callback (by overriding SetCallback), or you
    /// can read the state periodically.
    /// 
    /// When overriding GetCallback() and SetCallback(), you can throw an
    /// exception to indicate an error.  If you do, subscribers will not be
    /// notified, and the Vector class will encapsulate the exception with a
    /// Fault (using RSUtils.FaultOfException()), which you can either
    /// retrieve on the other end using RSUtils.ExceptionOfFault(), or if you
    /// use RSUtils.SyncReceive<>(), this method will automatically retrieve
    /// and re-throw the exception.  NOTE:  If a Fault is generated by the DSS
    /// system, and does not encapsulate an exception,
    /// RSUtils.ExceptionOfFault() will instead throw a FaultReceivedException,
    /// which encapsulates the Fault.
    /// 
    /// You do not need to actually modify the state in SetCallback(), it will
    /// already have been modified by the Vector base class.  Likewise, you do
    /// not need to throw exceptions from the above callbacks for index
    /// out-of-bounds, invalid key, etc, these will be handled automatically.
    /// The callbacks simply allow you to communicate with the hardware or with
    /// another service.
    /// 
    /// Although the state properties are public (this is necessary for
    /// serialization), DO NOT modify them directly, because there is an
    /// internal cache of key-index mappings, which must be rebuilt if a key
    /// changes, or if the number of keys or values changes.
    /// 
    /// The key and value lists do not have to have the same lengths, if there
    /// are more values than keys, the end values will only be accessible by
    /// index, and if there are more keys than values, those keys will simply
    /// by unused (and throw UnknownKeyExceptions).  This flexibility implies
    /// that you do not even have to use the keys at all if clients will
    /// always access elements by index.
    /// 
    /// If all else fails, see the Scribbler services for examples :)
    /// </summary>
    [DisplayName("Vector")]
    [Description("A Generic Vector Service")]
    [Contract(Contract.Identifier)]
    public class VectorService : DsspServiceBase
    {

        /// <summary>
        /// _state
        /// </summary>
        [ServiceState()]
        protected VectorState _state = new VectorState();

        /// <summary>
        /// _main Port
        /// </summary>
        [ServicePort("/vector", AllowMultipleInstances = false)]
        protected VectorOperations _operationsPort = new VectorOperations();
        protected VectorOperations OperationsPort { get { return _operationsPort; } private set { _operationsPort = value; } }

        [Partner("SubMgr",
            Contract = submgr.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.CreateAlways,
            Optional = false)]
        private submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        /// <summary>
        /// Default Service Constructor
        /// </summary>
        public VectorService(DsspServiceCreationPort creationPort) : base(creationPort) { }

        /// <summary>
        /// Service Start
        /// </summary>
        protected override void Start()
        {
            base.Start();
        }

        /// <summary>
        /// Callback giving you the opportunity to set the state before it is
        /// retrieved due to a request.  The requestInfo parameter will be
        /// either a GetElementRequestInfo class, or a GetAllRequestInfo class.
        /// Use the "is" keyword to find out which one (and thus what type the
        /// request was).  This class, once casted to the right type, contains
        /// information about the specific request.  See the Vector class
        /// description for more information.
        /// </summary>
        protected virtual void GetCallback(GetRequestInfo request)
        {
        }

        /// <summary>
        /// Callback giving you the opportunity to take action after the state
        /// is modified by a request.  The requestInfo parameter will be
        /// either a SetElementRequestInfo class, or a SetAllRequestInfo class.
        /// Use the "is" keyword to find out which one (and thus what type the
        /// request was).  This class, once casted to the right type, contains
        /// information about the specific request.  See the Vector class
        /// description for more information.
        /// </summary>
        protected virtual void SetCallback(SetRequestInfo request)
        {
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReplaceHandler(Replace replace)
        {
            _state = replace.Body;
            try
            {
                SetCallback(new SetAllRequestInfo()
                {
                    Timestamp = replace.Body.Timestamp,
                    Values = replace.Body.Values
                });
                replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
                SendNotification<Replace>(replace);
            }
            catch (Exception e)
            {
                replace.ResponsePort.Post(RSUtils.FaultOfException(e));
            }
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetByIndexHandler(GetByIndex get)
        {
            try
            {
                GetElementResponseType response = new GetElementResponseType()
                {
                    Value = _state.Get(get.Body.Index),
                    Timestamp = _state.Timestamp
                };
                GetCallback(new GetElementRequestInfo()
                {
                    RequestType = RequestType.ByIndex,
                    Index = get.Body.Index,
                    Key = ((_state.Keys.Count >= (get.Body.Index + 1)) ? _state.Keys[get.Body.Index] : "")
                });
                get.ResponsePort.Post(response);
            }
            catch (Exception e)
            {
                get.ResponsePort.Post(RSUtils.FaultOfException(e));
            }
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetByKeyHandler(GetByKey get)
        {
            try
            {
                GetElementResponseType response = new GetElementResponseType()
                {
                    Value = _state.Get(get.Body.Key),
                    Timestamp = _state.Timestamp
                };
                GetCallback(new GetElementRequestInfo()
                {
                    RequestType = RequestType.ByKey,
                    Index = _state.indexCache[get.Body.Key],
                    Key = get.Body.Key
                });
                get.ResponsePort.Post(response);
            }
            catch (Exception e)
            {
                get.ResponsePort.Post(RSUtils.FaultOfException(e));
            }
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetAllHandler(GetAllElements get)
        {
            // No exception check here - none can be thrown
            GetCallback(new GetAllRequestInfo() { });
            get.ResponsePort.Post(new GetAllResponseType()
                {
                    Values = _state.Values,
                    Timestamp = _state.Timestamp
                });
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetByIndexHandler(SetByIndex set)
        {
            try
            {
                _state.Set(set.Body.Index, set.Body.Value);
                _state.Timestamp = set.Body.Timestamp;
                SetCallback(new SetElementRequestInfo()
                {
                    RequestType = RequestType.ByIndex,
                    Index = set.Body.Index,
                    Key = ((_state.Keys.Count >= (set.Body.Index + 1)) ? _state.Keys[set.Body.Index] : ""),
                    Timestamp = set.Body.Timestamp,
                    Value = set.Body.Value
                });
                set.ResponsePort.Post(DefaultUpdateResponseType.Instance);
                SendNotification<SetByIndex>(set);
            }
            catch (Exception e)
            {
                set.ResponsePort.Post(RSUtils.FaultOfException(e));
            }
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetByKeyHandler(SetByKey set)
        {
            try
            {
                _state.Set(set.Body.Key, set.Body.Value);
                _state.Timestamp = set.Body.Timestamp;
                SetCallback(new SetElementRequestInfo()
                {
                    RequestType = RequestType.ByKey,
                    Index = _state.indexCache[set.Body.Key],
                    Key = set.Body.Key,
                    Timestamp = set.Body.Timestamp,
                    Value = set.Body.Value
                });
                set.ResponsePort.Post(DefaultUpdateResponseType.Instance);
                SendNotification<SetByKey>(set);
            }
            catch (Exception e)
            {
                set.ResponsePort.Post(RSUtils.FaultOfException(e));
            }
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SetAllHandler(SetAllElements setAll)
        {
            _state.Values = setAll.Body.Values;
            _state.Timestamp = setAll.Body.Timestamp;
            SetCallback(new SetAllRequestInfo() { Values = setAll.Body.Values, Timestamp = setAll.Body.Timestamp });
            setAll.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            SendNotification<SetAllElements>(setAll);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    base.SendNotification<Replace>(_subMgrPort, subscribe.Body.Subscriber, _state);
                },
                delegate(Exception error)
                {
                    base.LogError("Error adding subscriber " + subscribe.Body.Subscriber, error);
                });
        }

        protected void SendNotification<T>(T message) where T : DsspOperation
        {
            base.SendNotification<T>(_subMgrPort, message);
        }
    }

    /// <summary>
    /// For the RequestType member of GetElementRequestInfo and
    /// SetElementRequestInfo, this indicates whether the service request was
    /// (Get/Set)ByIndex, or (Get/Set)ByKey.
    /// </summary>
    public enum RequestType { ByIndex, ByKey }

    /// <summary>
    /// The abstract base class for GetElementRequestInfo and GetAllRequestInfo
    /// </summary>
    public abstract class GetRequestInfo
    {
    }

    /// <summary>
    /// Passed into GetCallback, provides information about the actual service
    /// request, if the request was either GetByIndex and GetByKey.  The
    /// RequestType property indicates whether the request was GetByIndex or
    /// GetByKey.
    /// </summary>
    public class GetElementRequestInfo : GetRequestInfo
    {
        public RequestType RequestType { get; set; }
        public int Index { get; set; }
        public string Key { get; set; }
    }

    /// <summary>
    /// Passed into GetCallback, if the request was GetState.  This does not
    /// contain any members, but only indicates that the service request was
    /// GetState.
    /// </summary>
    public class GetAllRequestInfo : GetRequestInfo
    {
    }

    /// <summary>
    /// The abstract base class for SetElementRequestInfo and SetAllRequestInfo
    /// </summary>
    public abstract class SetRequestInfo
    {
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Passed into SetCallback, provides information about the actual service
    /// request, if the request was either SetByIndex and SetByKey.  The
    /// RequestType property indicates whether the request was SetByIndex or
    /// SetByKey.
    /// </summary>
    public class SetElementRequestInfo : SetRequestInfo
    {
        public RequestType RequestType { get; set; }
        public int Index { get; set; }
        public string Key { get; set; }
        public double Value { get; set; }
    }

    /// <summary>
    /// Passed into SetCallback, if the request was SetAll.  This contains the
    /// values that were set in the state.
    /// </summary>
    public class SetAllRequestInfo : SetRequestInfo
    {
        public List<double> Values { get; set; }
    }
}
