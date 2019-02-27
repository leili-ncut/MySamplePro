using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            #region 抽象工厂
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

            // 套用泛型工厂实现 抽象工作中的 鸭脖鸭架 
            // 由此对比得出，泛型抽象工厂的优势，不针对特定具体的产品。当新增一种产品，比如：黑龙江口味的，
            // 抽象工厂模式要新增加黑龙江的具体实现类，但泛型抽象工厂就不需要，只需要增加具体产品的实现。
            //
            IAbstractFactory<YaBo, YaJia> factory2 = Fac.ZhouheiyaFactory("南昌");
            YaBo nanChangYabo2 = factory2.Create(TypeToken<YaBo>.Instance);
            nanChangYabo2.Print();
            YaJia nanChangYajia2 = factory2.Create(TypeToken<YaJia>.Instance);
            nanChangYajia2.Print();

            Console.Read();
        }
    }
}
