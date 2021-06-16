using System;

using System.IO;

using System.Threading;



namespace LaB9OC
{
    class Example
    {
        public static Random rnd = new Random();
        public static Thread tw, tr1, tr2, tr3;
        // Создаем семафоры.
        // Семафор Sem1 защищает очередь от переполнения.
        // Семафор Sem2 доступ к очереди потоков-читателей.
        // Максимальное количество ресурсов у каждого семафора равно 3.
        // Для потока-писателя это означает максимальное число пустых
        // «ячеек» в очереди, куда можно помещать числа,
        // а для потоков-читателей – максимальное количество чисел в
        // очереди.
        // На момент создания значение счетчика ресурсов для Sem1 равно 3.
        // т.е. в очереди есть три свободных ячейки.

        // Значение счетчика ресурсов для sem2 равно 0. т.е. очередь пуста

        public static Semaphore Sem1 = new Semaphore(3, 3);

        public static Semaphore Sem2 = new Semaphore(0, 10);

        public static int index = 0;

        public static int[] arr = new int[3];

        public static void Put(int n)
        {
            lock (typeof(Example))
            {
                index++;
                arr[index - 1] = n;
            }
        }

        public static int Get()

        {
            lock (typeof(Example))

            {
                int Result = arr[index - 1];
                index--;
                return Result;
            }
        }

        //Тело потока-писателя
        public static void ThWriter()
        {
            while (true)

            {
                // Если очередь полна, поток писатель засыпает,
                // до тех пор,пока не освободится, хотя бы одно свободное место
                Sem1.WaitOne();
                // помещаем в очередь случайное число
                Put(rnd.Next(100));
                Console.WriteLine("Поместить" + " "+Thread.CurrentThread.Name);
                Thread.Sleep(2500);
                // Увеличиваем счетчик текущего числа ресурсов семафора sem2.
                // Это позволит проснуться одному из ожидающих
                // потоков-читателей
                Sem2.Release();
            }
        }
        public static void ThReader()
        {
            while (true)
            {
                // Если очередь пуста, поток-читатель засыпает,
                // до тех пор, пока в очереди не появится новое число
                Sem2.WaitOne();
                Get();
                Console.WriteLine("Извлеч " + " "  + Thread.CurrentThread.Name); //извлекаем число из очереди
                Thread.Sleep(1000);
                // Увеличиваем счетчик текущего числа ресурсов семафора Sem1.
                // Это позволит проснуться потоку-писателю}
                Sem1.Release();
            }
        }

        public static void Main()

        {
            tw = new Thread(ThWriter);
            tr1 = new Thread(ThReader);
            tr2 = new Thread(ThReader);
            tr3 = new Thread(ThReader);
            tw.Name = "|1 поток писатель";
            tr1.Name = "|1 Поток читатель";
            tr2.Name = "|2 Поток читатель";
            tr3.Name = "|3 Поток читатель";
            
            tw.Start();
            tr1.Start();
            tr2.Start();
            tr3.Start();
            Console.ReadLine();
        }

    }
}
