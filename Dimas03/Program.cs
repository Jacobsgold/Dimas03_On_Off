using System;
using System.Runtime.InteropServices;

namespace PowerManagement
{
    class Program
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;
        }

        static void Main(string[] args)
        {
            // Установите время включения и выключения ПК
            int turnOnHour = 8;
            int turnOnMinute = 0;
            int turnOffHour = 22;
            int turnOffMinute = 0;

            // Проверка диапазона значений
            if (turnOnHour < 0 || turnOnHour > 23 ||
                turnOnMinute < 0 || turnOnMinute > 59 ||
                turnOffHour < 0 || turnOffHour > 23 ||
                turnOffMinute < 0 || turnOffMinute > 59)
            {
                Console.WriteLine("Недопустимые значения времени.");
                return;
            }

            // Получение текущего времени
            DateTime currentTime = DateTime.Now;

            // Создание структуры SYSTEMTIME для установки времени
            SYSTEMTIME sysTime = new SYSTEMTIME();
            sysTime.Year = (ushort)currentTime.Year;
            sysTime.Month = (ushort)currentTime.Month;
            sysTime.Day = (ushort)currentTime.Day;
            sysTime.Hour = (ushort)0;
            sysTime.Minute = (ushort)0;
            sysTime.Second = (ushort)0;

            // Установка времени включения ПК
            sysTime.Hour = (ushort)turnOnHour;
            sysTime.Minute = (ushort)turnOnMinute;
            SetSystemTime(ref sysTime);

            Console.WriteLine("Время включения ПК установлено на " + turnOnHour + ":" + turnOnMinute);

            // Ожидание до времени выключения ПК
            TimeSpan turnOffTime = new TimeSpan(turnOffHour, turnOffMinute, 0) - currentTime.TimeOfDay;
            int turnOffMilliseconds = (int)turnOffTime.TotalMilliseconds;
            Console.WriteLine("Ожидание до времени выключения ПК: " + turnOffTime);

            System.Threading.Thread.Sleep(turnOffMilliseconds);

            // Выключение ПК (можно заменить на вызов специальной команды или утилиты)
            IntPtr hWnd = FindWindow(null, "classname");
            ShowWindow(hWnd, 0); // Свернуть окно перед выключением
            SetForegroundWindow(hWnd);
            System.Diagnostics.Process.Start("shutdown", "/s /t 0");

            Console.WriteLine("ПК выключен.");
        }
    }
}