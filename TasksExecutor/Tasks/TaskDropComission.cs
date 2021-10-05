using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TasksExecutor.Tasks
{
    class TaskDropComission : TaskBase
    {
        public TaskDropComission(string url, IEnumerable<int> bookingIds, Regions region) 
            : base (url, bookingIds, region)
        {
            TagAction = "Другое";
        }

        public decimal ComissionValue { get; }
    }
}
