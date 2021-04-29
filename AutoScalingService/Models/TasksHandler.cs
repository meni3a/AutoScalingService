using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AutoScalingService.Models
{
    public sealed class TasksHandler
    {

        #region ///// Singelton pattren /////

        private static readonly TasksHandler instance = new TasksHandler();

        static TasksHandler()
        {
        }
        private TasksHandler()
        {
        }
        public static TasksHandler Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion


        public int numOfTasksToExecute = 0;
        public int activeIinstances = 0;
        double instanceSpaceLeft = 0;

        private object _token = new Object();

        /// <summary>
        /// Control the anount of tasks ans calculate instances
        /// </summary>
        /// <param name="request"></param>
        public void HandleInstances(ComputeRequest request)
        {
            lock (_token)
            {
                if (!request.IsDone)
                {
                    numOfTasksToExecute += request.AmountOfTasks;
                }
                else
                {
                    numOfTasksToExecute -= request.AmountOfTasks;
                }
                var amountOfinstances = CalcAmountOfinstances();
                LogToFile($"Number of active machines: {amountOfinstances} - {DateTime.Now}");
            }


        }

        /// <summary>
        /// Calc the total amount of instances needs to be active
        /// </summary>
        /// <returns>The total amount</returns>
        public int CalcAmountOfinstances()
        {
            int avaibaleInstances = 300 - activeIinstances;
            double requiredInstances = numOfTasksToExecute / 100;

            //Subtract the space left in the last instance
            if (requiredInstances>= instanceSpaceLeft) {
                requiredInstances -= instanceSpaceLeft;
                instanceSpaceLeft = 0;
            }

            //Since we can only ask the cloud provider for an integer
            int roundedRequiredInstances = Convert.ToInt32(Math.Ceiling(requiredInstances));

            //Save the space left for the next round
            instanceSpaceLeft += requiredInstances- Math.Floor(requiredInstances);


            if (requiredInstances <= avaibaleInstances)
            {
                activeIinstances = roundedRequiredInstances;

            }
            else
            {
                activeIinstances += avaibaleInstances;
            }

            return activeIinstances;
        }

        /// <summary>
        /// Log to a Text file
        /// </summary>
        /// <param name="str"></param>
        private void LogToFile(string str)
        {
            try
            {
                File.AppendAllText("log.txt", str + Environment.NewLine);
            }
            catch(Exception err)
            {
                Console.WriteLine($"Failed log to the file - Error: {err.Message}");
            }
            
        }
        
    }
}
