using System;


namespace ClientTaskManager
{
    class Program
    {

        static void Main(string[] args)
        {
            TaskManager manager = new TaskManager();
            manager.Init();

            Console.ReadKey();
        }
    }
}
