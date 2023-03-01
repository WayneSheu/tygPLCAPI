using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYG.HoleDetect
{
    /// <summary>Hole Detection Utility for Remote I/O</summary>
    public class HoleDetection
    {
        /// <summary>Base Resistance (KΩ)</summary>
        private static readonly float BaseResistance = 100.0f;
        /// <summary>Max. voltage (V)</summary>
        private static readonly float MaxVoltage = 7.5f;
        /// <summary>Min. voltage (V)</summary>
        private static readonly float MinVoltage =  4.5f;
        /// <summary>Hole KOHM List</summary>
        private static readonly float[] HoleKOHM = new float[] {500.0f,
                                                                374.0f, 332.0f, 300.0f, 274.0f, 249.0f, 205.0f, 187.0f, 174.0f, 162.0f, 150.0f,
                                                                130.0f, 121.0f, 113.0f, 105.0f,  97.6f,  84.5f,  78.7f,  73.2f,  68.0f,  63.4f,
                                                                 54.9f,  51.0f,  47.0f,  43.2f,  39.2f,  33.0f,  30.0f,  27.0f,  24.3f,  21.5f,
                                                                 9.09f,   1.0f};
        /// <summary>Hole ID List</summary>
        private static readonly string[] HoleID = new string[] {"  ",
                                                                "A1", "A2", "A3", "A4", "A5", "B1", "B2", "B3", "B4", "B5",
                                                                "C1", "C2", "C3", "C4", "C5", "D1", "D2", "D3", "D4", "D5",
                                                                "E1", "E2", "E3", "E4", "E5", "F1", "F2", "F3", "F4", "F5",
                                                                "**", "XX"};
        /// <summary>
        /// Convert channel voltage to Hole ID.
        /// </summary>
        /// <param name="paramChannelVoltages">Channel voltage array</param>
        /// <param name="paramHoleIDs">Channel voltage array</param>
        /// <returns>Channel Hole Id array</returns>
        public static void HoleConverter(ushort[] paramChannelVoltages, ref string[] paramHoleIDs)
        {
            int index, RNo;
            float channelVoltage = paramChannelVoltages[0] * 10.0f / 32767.0f;
            float baseVoltage = channelVoltage * 2;
            float resisValue = 0;
            if ((channelVoltage >= MaxVoltage) || (channelVoltage < MinVoltage))
            {
                for (index = 0; index < paramHoleIDs.Length; index++)
                    paramHoleIDs[index] = "XX";
            }
            else
            {
                paramHoleIDs[0] = "MB";
                
                for (index = 1;index < paramHoleIDs.Length; index++)
                {
                    // Scale AI Input to Voltage Value
                    channelVoltage = paramChannelVoltages[index] * 10.0f / 32767.0f;
                    // Convert Channel Voltage to Resistance
                    resisValue = channelVoltage / ((baseVoltage - channelVoltage) / BaseResistance);
                    // Search Hole HoleKOHM List to find Index Value
                    RNo = SearchRNO(resisValue);
                    // Get Hole ID from Hole ID List.
                    paramHoleIDs[index] = HoleID[RNo];
                }
            }
        }

        /// <summary>
        /// Quick search for resistance comparision
        /// </summary>
        /// <param name="paramResistance">Resistance value</param>
        /// <returns>Resistance No.</returns>
        private static int SearchRNO(float paramResistance)
        {
            int RNO = 0;
            int RidxHi = 0;
            int RidxLo = (HoleKOHM.Length - 1); 
            int index;
            do
            {
                index = ((RidxLo + RidxHi) / 2);
                if (HoleKOHM[index] > paramResistance)
                    RidxHi = index;
                else
                    RidxLo = index;
            } while ((RidxHi + 1) < RidxLo);
            if ((HoleKOHM[RidxHi] - paramResistance) < (paramResistance - HoleKOHM[RidxLo]))
                RNO = RidxHi;
            else
                RNO = RidxLo;

            return RNO;
        }
    }
}
