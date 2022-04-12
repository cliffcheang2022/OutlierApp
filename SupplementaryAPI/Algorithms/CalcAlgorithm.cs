using System;
using System.Collections.Generic;
using System.Linq;

namespace SupplementaryAPI.Algorithms
{
    /// <summary>
    /// <para>Author      : Cliff Cheang</para>
    /// <para>Description : CalcAlgorithm Class is the one stored all calculation related algorithm</para>
    /// </summary>
    public class CalcAlgorithm
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CalcAlgorithm() { }

        /// <summary>
        /// Simple Std Dev Logic
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public double GetStandardDeviation(IEnumerable<double> sequence)
        {
            double average = sequence.Average();
            double sum = sequence.Sum(d => Math.Pow(d - average, 2));
            return Math.Sqrt((sum) / sequence.Count());
        }
    }
}
