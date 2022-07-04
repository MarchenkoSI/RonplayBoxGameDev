using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RonplayGameDev
{
    public class PercentCalculator
    {
        /// <summary>
        /// Returns value of 100% in float
        /// </summary>
        /// <param name="total_percent_value_">Value of 100%</param>
        /// <param name="num_">number</param>
        public static float ValueOfTotalInFloat(float total_percent_value_, float num_)
        {
            return GetCoefficient(total_percent_value_, 100) * num_;
        }

        /// <summary>
        /// Return value of 100% in percent
        /// </summary>
        /// <param name="total_percent_value_">Value of 100%</param>
        /// <param name="num_">number</param>

        public static float ValueOfTotalInPercent(float total_percent_value_, float num_)
        {
            return 100 / GetCoefficient(total_percent_value_, num_);
        }

        public static float IncreaseByPercent(float num_to_increase, float percent_of_increase)
        {
            return (GetCoefficient(num_to_increase, 100) * percent_of_increase) + num_to_increase;
        }

        public static float DecreaseByPercent(float num_to_decrease, float percent_of_decrease)
        {
            return num_to_decrease - (GetCoefficient(num_to_decrease, 100) * percent_of_decrease);
        }

        private static float GetCoefficient(float num_, float target_value_)
        {
            return num_ / target_value_;
        }
    }
}

