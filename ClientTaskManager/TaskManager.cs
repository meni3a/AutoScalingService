using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ClientTaskManager
{
    class TaskManager
    {

        #region /////Https client/////

        RestClient client;
        string serverAdress = "http://localhost:5000";
        string serverRoute = "/api/tasks";

        #endregion


        int IdCounter = 0;
        Random random = new Random();
        System.Timers.Timer taskGeneratorTimer;



        /// <summary>
        /// Initial http client and timer for generating tasks
        /// </summary>
        public void Init()
        {
            //Initial http client
            client = new RestClient(serverAdress);
            
            int numOfIntervalsPerMinute = random.Next(Configuration.MaxIntervalsPerMinute) + 1;

            //intial and start timer
            taskGeneratorTimer = new System.Timers.Timer(60000 / numOfIntervalsPerMinute);
            taskGeneratorTimer.Elapsed += Generate_tasks;
            taskGeneratorTimer.Start();

        }

        /// <summary>
        /// Generate tasks to send to the server
        /// </summary>
        private void Generate_tasks(object sender, ElapsedEventArgs e)
        {

            int numOfTasksPerRequest = random.Next(Configuration.MaxTasksPerRequest) + 1;
            int numOfRequestPerInterval = random.Next(Configuration.MaxRequestPerInterval) + 1;


            Parallel.For(0, numOfRequestPerInterval, i =>
            {
                //Generate request
                ComputeRequest computeRequest = new ComputeRequest
                {
                    Id = IdCounter++,
                    AmountOfTasks = numOfTasksPerRequest + 1,
                    IsDone = false
                };
                try
                {
                    //Send the request to the AutoScalingService and log the result
                    Console.WriteLine(SendPost(computeRequest));

                    //Pick a wating time between 30 to 120 sec
                    int TimeUntilTaskIsDone = random.Next(30000, 120001);

                    //Simulate the time used by cloud provider to execute all tasks
                    Thread.Sleep(TimeUntilTaskIsDone);
                    computeRequest.IsDone = true;

                    //Inform the AutoScalingService that tasks have been completed and log the result
                    Console.WriteLine(SendPost(computeRequest));

                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            });
        }

        /// <summary>
        /// Send http post message to the server
        /// </summary>
        /// <param name="req">request object to send</param>
        /// <returns>http result</returns>
        public string SendPost(ComputeRequest req)
        {

            var request = new RestRequest(serverRoute, Method.POST);

            request.AddJsonBody(new
            {
                Id = req.Id,
                AmountOfTasks = req.AmountOfTasks,
                IsDone = req.IsDone
            });

            IRestResponse response = client.Execute(request);
            return response.Content;

        }

    }
}
