using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionMonitoring.Models
{
    //Auto-generated classes to parse JSON data
    public class Rootobject
    {
        public string schemaId { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string status { get; set; }
        public Context context { get; set; }
        public Properties1 properties { get; set; }
    }

    public class Context
    {
        public Activitylog activityLog { get; set; }
    }

    public class Activitylog
    {
        public Authorization authorization { get; set; }
        public string channels { get; set; }
        public string claims { get; set; }
        public string caller { get; set; }
        public string correlationId { get; set; }
        public string description { get; set; }
        public string eventSource { get; set; }
        public DateTime eventTimestamp { get; set; }
        public string httpRequest { get; set; }
        public string eventDataId { get; set; }
        public string level { get; set; }
        public string operationName { get; set; }
        public string operationId { get; set; }
        public Properties properties { get; set; }
        public string resourceId { get; set; }
        public string resourceGroupName { get; set; }
        public string resourceProviderName { get; set; }
        public string status { get; set; }
        public string subStatus { get; set; }
        public string subscriptionId { get; set; }
        public DateTime submissionTimestamp { get; set; }
        public string resourceType { get; set; }
    }

    public class Authorization
    {
        public string action { get; set; }
        public string scope { get; set; }
    }

    public class Properties
    {
        public string statusCode { get; set; }
        public string serviceRequestId { get; set; }
        public string responseBody { get; set; }
    }

    public class Properties1
    {
    }
}
