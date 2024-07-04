using System;

namespace Gerencia_Reportes.Models
{
    public class CallsInQueues
    {
        public string SupportCallID { get; set; }
        public int Number { get; set; }

        public string Types { get; set; }

        public string Summary { get; set; }

        public string Queue { get; set; }

        public string Status { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DateAssignTo { get; set; }

        public string Priority { get; set; }

        public string Attribute { get; set; }

        public string Value { get; set; }

        public string EventSummary { get; set; }

        public string Product { get; set; }
        public string Detail { get; set; }

        public string Comments { get; set; }

        public CallsInQueues()
        {


        }

        public class CallsInQueuesBuilder
        {
            CallsInQueues supportCall;

            public CallsInQueuesBuilder()
            {
                supportCall = new CallsInQueues();
            }

            public CallsInQueuesBuilder WithSupportCallID(string supportCallID)
            {
                supportCall.SupportCallID = supportCallID;
                return this;
            }

            public CallsInQueuesBuilder WithNumber(int number)
            {
                supportCall.Number = number;
                return this;
            }

            public CallsInQueuesBuilder WithTypes(string types)
            {
                supportCall.Types = types;
                return this;
            }

            public CallsInQueuesBuilder WithSummary(string summary)
            {
                supportCall.Summary = summary;
                return this;
            }

            public CallsInQueuesBuilder WithQueue(string queue)
            {
                supportCall.Queue = queue;
                return this;
            }

            public CallsInQueuesBuilder WithStatus(string status)
            {
                supportCall.Status = status;
                return this;
            }

            public CallsInQueuesBuilder WithOpenDate(DateTime openDate)
            {
                supportCall.OpenDate = openDate;
                return this;
            }

            public CallsInQueuesBuilder WithDueDate(DateTime dueDate)
            {
                supportCall.DueDate = dueDate;
                return this;
            }

            public CallsInQueuesBuilder WithStartDate(DateTime startDate)
            {
                supportCall.StartDate = startDate;
                return this;
            }

            public CallsInQueuesBuilder WithDateAssignTo(DateTime dateAssignTo)
            {
                supportCall.DateAssignTo = dateAssignTo;
                return this;
            }

            public CallsInQueuesBuilder WithPriority(string priority)
            {
                supportCall.Priority = priority;
                return this;
            }

            public CallsInQueuesBuilder WithAttribute(string attribute)
            {
                supportCall.Attribute = attribute;
                return this;
            }

            public CallsInQueuesBuilder WithValue(string value)
            {
                supportCall.Value = value;
                return this;
            }

            public CallsInQueuesBuilder WithEventSummary(string eventSummary)
            {
                supportCall.EventSummary = eventSummary;
                return this;
            }

            public CallsInQueuesBuilder WithProduct(string product)
            {
                supportCall.Product = product;
                return this;
            }

            public CallsInQueuesBuilder WithDetail(string detail)
            {
                supportCall.Detail = detail;
                return this;
            }

            public CallsInQueuesBuilder WithComments(string comments)
            {
                supportCall.Comments = comments;
                return this;
            }





            public CallsInQueues Build()
            {
                return supportCall;
            }


        }

    }
}


