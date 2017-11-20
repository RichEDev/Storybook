using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementUnitTests.Global_Objects
{
    class cColoursObject
    {
        public static string GetDefaultColour(Modules module, string key)
        {
            switch (module)
            {
                case Modules.contracts:
                    switch (key)
                    {
                        case "MBBG":
                            return "#006B51";
                        case "MBFG":
                            return "#97CE8B";
                        case "TBBG":
                            return "#97CE8B";
                        case "TBFG":
                            return "#FFFFFF";
                        case "R1BG":
                            return "#FFFFFF";
                        case "R1FG":
                            return "#333333";
                        case "R2BG":
                            return "#C6F0BF";
                        case "R2FG":
                            return "#333333";
                        case "FBG":
                            return "#A3D398";
                        case "FFG":
                            return "#FFFFFF";
                        case "H":
                            return "#006B51";
                        case "POFG":
                            return "#97CE8B";
                    }
                    break;
                case Modules.expenses:
                    switch (key)
                    {
                        case "MBBG":
                            return "#4A65A0";
                        case "MBFG":
                            return "#E0E0E0";
                        case "TBBG":
                            return "#4A65A0";
                        case "TBFG":
                            return "#FFFFFF";
                        case "R1BG":
                            return "#FFFFFF";
                        case "R1FG":
                            return "#333333";
                        case "R2BG":
                            return "#D2E4EE";
                        case "R2FG":
                            return "#333333";
                        case "FBG":
                            return "#4A65A0";
                        case "FFG":
                            return "#FFFFFF";
                        case "H":
                            return "#003768";
                        case "POFG":
                            return "#6280A7";
                    }
                    break;
                default:
                    switch (key)
                    {
                        case "MBBG":
                            return "#4A65A0";
                        case "MBFG":
                            return "#E0E0E0";
                        case "TBBG":
                            return "#4A65A0";
                        case "TBFG":
                            return "#FFFFFF";
                        case "R1BG":
                            return "#FFFFFF";
                        case "R1FG":
                            return "#333333";
                        case "R2BG":
                            return "#D2E4EE";
                        case "R2FG":
                            return "#333333";
                        case "FBG":
                            return "#4A65A0";
                        case "FFG":
                            return "#FFFFFF";
                        case "H":
                            return "#003768";
                        case "POFG":
                            return "#6280A7";
                    }
                    break;
            }

            return string.Empty;
        }
    }
}
