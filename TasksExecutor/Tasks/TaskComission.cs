using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TasksExecutor.Tasks
{
    class TaskComission : TaskBase
    {
        public TaskComission(string url, IEnumerable<int> bookingIds, decimal comissionValue, Regions region) 
            : base (url, bookingIds, region)
        {
            ComissionValue = comissionValue;
            TagAction = "СменитьПроцент";
        }

        public decimal ComissionValue { get; }
    }
}
