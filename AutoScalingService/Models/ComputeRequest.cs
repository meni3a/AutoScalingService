using System;
using System.Collections.Generic;
using System.Text;

namespace AutoScalingService
{
    public class ComputeRequest
    {
        public int Id { get; set; }
        public int AmountOfTasks { get; set; }
        public bool IsDone { get; set; }
    }
}
