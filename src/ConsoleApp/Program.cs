using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            #region abstractfactory
            // 南昌工厂制作南昌的鸭脖和鸭架
            AbstractFactory nanChangFactory = new NanChangFactory();
            YaBo nanChangYabo = nanChangFactory.CreateYaBo();
            nanChangYabo.Print();
            YaJia nanChangeYaJia = nanChangFactory.CreateYaJia();
            nanChangeYaJia.Print();

            // 上海工厂制作上海的鸭脖和鸭架
            AbstractFactory shanghaiFactory = new ShangHaiFactory();
            shanghaiFactory.CreateYaBo().Print();
            shanghaiFactory.CreateYaJia().Print();


            #endregion


            // Yield a computer of Intel  
            IAbstractFactory<Processor, Ram> factory1 = Fac.ComputerFactory("Intel");
            Processor cpu1 = factory1.Create(TypeToken<Processor>.Instance);
            Ram ram1 = factory1.Create(TypeToken<Ram>.Instance);
            Console.WriteLine("An Intel Computer");
            Console.WriteLine("CPU Model: {0}", cpu1.Model);
            Console.WriteLine("Memory Frequency: {0} MHz", ram1.Frequency);

            Console.Read();
        }
    }
}
