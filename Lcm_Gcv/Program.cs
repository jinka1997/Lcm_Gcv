using System;
using System.Collections.Generic;
using System.Linq;

namespace Lcm_Gcv
{
    class Program
    {
        static void Main(string[] args)
        {
            var nums = args.Select(v => int.Parse(v));

            if (nums.Count() < 2) { throw new Exception("要素数が2以上の条件に反しています。"); }
            if (nums.Any(n => n < 1)) { throw new Exception("自然数(1以上)でない値が含まれています。"); }
            if (nums.Count() != nums.Distinct().Count()) { throw new Exception("要素内に重複する値が含まれています。"); }

            //１．各要素の値ごとに、素因数分解
            var primeFactorizes = nums.Select(n => PrimeFactorize(n));

            #region ★debug1
            Console.WriteLine("debug1:start");
            Enumerable.Range(0, nums.Count()).ToList().ForEach(n =>
            {
                var pf = primeFactorizes.ElementAt(n);
                var list = new List<string>();
                pf.ToList().ForEach(kvp => list.Add($"{kvp.Key}^{kvp.Value}"));
                Console.WriteLine($"{nums.ElementAt(n),2} = {string.Join(" × ", list)}");
            });
            Console.WriteLine("debug1:end");
            #endregion

            //２．素数リストの各値について、素因数分解した結果から指数を並べる
            var dic = LineUpPowerIndex(primeFactorizes);

            #region ★debug
            Console.WriteLine("debug2:start");
            dic.ToList().ForEach(kvp => Console.WriteLine($"{kvp.Key,2}の指数→{string.Join(",", kvp.Value)}"));
            Console.WriteLine("debug2:end");
            #endregion

            //３．最小公倍数・最大公約数の計算
            var result = Calc(dic);

            #region ★debug
            Console.WriteLine("debug3:start");
            var lcmExpList = new List<string>();
            var gcdExpList = new List<string>();
            dic.ToList().ForEach(kvp =>
            {
                lcmExpList.Add($"{kvp.Key,2}^{kvp.Value.Max()}");
                gcdExpList.Add($"{kvp.Key,2}^{kvp.Value.Min()}");

            });
            Console.WriteLine($"最小公倍数の計算式 {string.Join(" × ", lcmExpList)}");
            Console.WriteLine($"最大公約数の計算式 {string.Join(" × ", gcdExpList)}");
            Console.WriteLine("debug3:end");
            #endregion

            Console.WriteLine($"{string.Join(",", nums)}の最小公倍数は{result.Item1}");
            Console.WriteLine($"{string.Join(",", nums)}の最大公約数は{result.Item2}");

            Console.ReadLine();
        }

        /// <summary>
        /// 素因数分解
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static Dictionary<int, int> PrimeFactorize(int num)
        {
            var primeDic = new Dictionary<int, int>();
            var tmp = num;
            var n = 2;

            while (tmp > 1)
            {
                //指数の初期化
                var i = 0;
                while (tmp % n == 0)
                {
                    //割ったら余りが0の時、指数を+1し、除算
                    i++;
                    tmp = tmp / n;
                }
                //指数決定
                if (i > 0) { primeDic.Add(n, i); }

                n++;
            }
            return primeDic;
        }

        /// <summary>
        /// 各値の素因数と指数を並べる
        /// </summary>
        /// <param name="primeFactorizes">配列各要素の素因数分解結果</param>
        /// <returns></returns>
        private static SortedDictionary<int, int[]> LineUpPowerIndex(IEnumerable<Dictionary<int, int>> primeFactorizes)
        {
            var dic = new SortedDictionary<int, int[]>();

            //素因数のリスト作成
            var plist = new List<int>();
            primeFactorizes.ToList().ForEach(pf => plist.AddRange(pf.Keys));
            var primes = plist.Distinct();

            //素因数のリストと、それに対応する指数の配列作成(初期値0)
            primes.ToList().ForEach(p => dic.Add(p, Enumerable.Repeat<int>(0, primeFactorizes.Count()).ToArray()));

            //素因数分解の結果を反映
            var idx = 0;
            foreach (var pf in primeFactorizes)
            {
                foreach (var kvp in pf)
                {
                    dic[kvp.Key][idx] = kvp.Value;
                }
                idx++;
            }
            return dic;
        }

        /// <summary>
        /// 最大公約数、最小公倍数の計算
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        private static Tuple<int, int> Calc(SortedDictionary<int, int[]> dic)
        {
            var gcd = 1;
            var lcm = 1;
            dic.ToList().ForEach(kvp =>
            {
                //素因数の指数乗を計算
                var max_index = kvp.Value.Max();
                var min_index = kvp.Value.Min();

                //最大値の積→最小公倍数
                lcm = lcm * (int)(Math.Pow(kvp.Key, max_index));

                //最小値の積→最大公約数
                gcd = gcd * (int)(Math.Pow(kvp.Key, min_index));
            });

            //結果をタプルで返す
            return new Tuple<int, int>(lcm, gcd);
        }
    }
}
