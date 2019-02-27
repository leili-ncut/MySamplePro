using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ConsoleApp
{
    #region abstract

    /// <summary>
    /// 单一产品接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactory<T>
    {
        T Create();
    }

    /// <summary>
    /// 单一产品的工厂方法模板
    /// </summary>
    /// <typeparam name="TAbstractProduct"></typeparam>
    /// <typeparam name="TProduct"></typeparam>
    public class OpNewFactory<TAbstractProduct, TProduct> : IFactory<TAbstractProduct>
        where TProduct : TAbstractProduct, new()
    {
        public TAbstractProduct Create()
        {
            return new TProduct();
        }
    }

    /// <summary>
    /// 多个产品的抽象工厂接口了
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IAbstractFactory<T1, T2>
    {
        T1 Create(TypeToken<T1> token);
        T2 Create(TypeToken<T2> token);
    }

    public sealed class TypeToken<T>
    {
        public static TypeToken<T> Instance { get; } = new TypeToken<T>();

        private TypeToken() { }
    }


    #endregion

    #region 具体工厂

    /// <summary>
    /// 具体工厂就是利用生产每种产品的单一工厂来组合实现。因此你只要有每种类型的单一工厂就可以直接组合生成
    /// 抽象工厂，而无需定义一个类来做这件事。注意，对每种数目的抽象工厂接口都需要对应生成一个具体工厂的实现
    /// ，这里仅针对生成两个产品的演示
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ConcreteFactory<T1, T2> : IAbstractFactory<T1, T2>
    {
        private IFactory<T1> factory1;
        private IFactory<T2> factory2;

        public ConcreteFactory(IFactory<T1> f1, IFactory<T2> f2)
        {
            factory1 = f1;
            factory2 = f2;
        }

        public T1 Create(TypeToken<T1> token)
        {
            return factory1.Create();
        }

        public T2 Create(TypeToken<T2> token)
        {
            return factory2.Create();
        }
    }

    public static class ConcretFactory
    {
        public static ConcreteFactory<T1, T2> NewFactory<T1, T2>(IFactory<T1> f1, IFactory<T2> f2)
        {
            return new ConcreteFactory<T1, T2>(f1, f2);
        }
    }

    #endregion

    #region 抽象产品

    /// <summary>
    /// cpu
    /// </summary>
    public abstract class Processor
    {
        public abstract string Model { get; }
    }

    /// <summary>
    /// 内存
    /// </summary>
    public abstract class Ram
    {
        public abstract int Frequency { get; }
    }

    #endregion

    #region 具体产品实现

    public class PentiumProcessor : Processor
    {
        public override string Model
        {
            get { return "Pentium Extreme Edition 955"; }
        }
    }

    public class AthlonProcessor : Processor
    {
        public override string Model
        {
            get { return "Athlon 64 X2 FX-60"; }
        }
    }

    public class DDRRam : Ram
    {
        public override int Frequency
        {
            get { return 400; }
        }
    }

    public class DDR2Ram : Ram
    {
        public override int Frequency
        {
            get { return 533; }
        }
    }

    #endregion

    #region

    /// <summary>
    /// 随心所欲生成想要的抽象工厂接口以及快速从现有单一产品工厂组合成特定的具体工厂实现
    /// 
    /// </summary>
    public class Fac
    {
        public static IAbstractFactory<Processor, Ram> ComputerFactory(string type)
        {
            if (type == "Intel")
            {
                return ConcretFactory.NewFactory(
                    new OpNewFactory<Processor, PentiumProcessor>(),
                    new OpNewFactory<Ram, DDR2Ram>());
            }
            else if(type == "AMD")
            {
                return ConcretFactory.NewFactory(
                    new OpNewFactory<Processor, AthlonProcessor>(),
                    new OpNewFactory<Ram, DDRRam>());
            }

            return null;
        }

    }

    #endregion

    public class GenericAbstractFac
    {

    }
}
