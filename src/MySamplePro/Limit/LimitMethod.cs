using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySamplePro.Limit
{
    /// <summary>
    /// https://mp.weixin.qq.com/s/xIEN_IvR8h-Yc7oZoojTpw
    /// 总体思路是这样：
    ///1.  用一个环形来代表通过的请求容器。
    /// 2.  用一个指针指向当前请求所到的位置索引，来判断当前请求时间和当前位置上次请求的时间差，依此来判断是否被限制。
    ///3.  如果请求通过，则当前指针向前移动一个位置，不通过则不移动位置
    ///4.  重复以上步骤 直到永远.......
    /// </summary>
    public class LimitService
    {
        // 当前指针的位置
        private int currentIndex = 0;
        //限制的时间的秒数，即：x秒允许多少请求
        private int _limitTimeSencond = 1;
        // 请求环的容器数组
        private DateTime?[] requestRing = null;
        // 容器改变或者移动指针时候的锁
        private object objLock = new object();

        public LimitService(int countPerSecond,int limitTimeSencond)
        {
            requestRing = new DateTime?[countPerSecond];
            _limitTimeSencond = limitTimeSencond;
        }

        public bool IsContinue()
        {
            lock (objLock)
            {
                var currentNode = requestRing[currentIndex];
                //如果当前节点的值加上设置的秒 超过当前时间，说明超过限制
                if (currentNode != null && currentNode.Value.AddSeconds(_limitTimeSencond) > DateTime.Now)
                {
                    return false;
                }
                //当前节点设置为当前时间
                requestRing[currentIndex] = DateTime.Now;
                // //指针往前移动一个位置
                MoveNextIndex(ref currentIndex);
            }

            return true;
        }

        //指针往前移动一个位置
        private void MoveNextIndex(ref int currentIndex)
        {
            if (currentIndex != requestRing.Length -1)
            {
                currentIndex = this.currentIndex + 1;
            }
            else
            {
                currentIndex = 0;
            }
        }

        //改变每秒可以通过的请求数
        public bool ChangeCountPerSecond(int countPerSecond)
        {
            lock (objLock)
            {
                requestRing = new DateTime?[countPerSecond];
                currentIndex = 0;
            }
            return true;
        }
    }
}
