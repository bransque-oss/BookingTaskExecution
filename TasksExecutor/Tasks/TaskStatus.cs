using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TasksExecutor.Tasks
{
    class TaskStatus : TaskBase
    {
        internal TaskStatus(string url, IEnumerable<int> bookingIds, Statuses status, Regions region) 
            : base (url, bookingIds, region)
        {
            Status = status;
            TagAction = "СменитьСтатус";
        }

        public Statuses Status { get; }
    }
}
