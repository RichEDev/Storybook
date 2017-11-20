using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Tailoring.Colours
{
    class Colours
    {
        internal string MenubarBGColour { get; set; }
        internal string MenubarFGColour { get; set; }
        internal string TitlebarBGColour { get; set; }
        internal string TitlebarFGColour { get; set; }
        internal string RowColourFG { get; set; }
        internal string RowColourBG { get; set; }
        internal string AlternateRowColourFG { get; set; }
        internal string AlternateRowColourBG { get; set; }
        internal string FieldBG { get; set; }
        internal string FieldFG { get; set; }
        internal string HoverColour { get; set; }
        internal string PageOptionFGColour { get; set; }
        internal string TooltipBGColour { get; set; }
        internal string TooltipTextColour { get; set; }
        internal string GreenLightFieldColour { get; set; }
        internal string GreenLightSectionTextColour { get; set; }
        internal string GreenLightSectionBackgroundColour { get; set; }
        internal string GreenLightSectionUnderlineColour { get; set; }

        internal Colours() { }

        public static Colours SetDefaultColours(ProductType executingProduct)
        {
            Colours productColours = new Colours();
            switch (executingProduct)
            {
                case ProductType.framework:
                    productColours.MenubarBGColour = "00A0AF".ToUpper();
                    productColours.MenubarFGColour = "97ce8b".ToUpper();

                    productColours.TitlebarBGColour = "97ce8b".ToUpper();
                    productColours.TitlebarFGColour = "ffffff".ToUpper();

                    productColours.RowColourBG = "FFFFFF".ToUpper();
                    productColours.RowColourFG = "333333".ToUpper();
                    productColours.AlternateRowColourBG = "c6f0bf".ToUpper();
                    productColours.AlternateRowColourFG = "333333".ToUpper();

                    productColours.FieldBG = "a3d398".ToUpper();
                    productColours.FieldFG = "FFFFFF".ToUpper();

                    productColours.HoverColour = "00A0AF".ToUpper();
                    productColours.PageOptionFGColour = "97ce8b".ToUpper();

                    productColours.TooltipBGColour = "4A65A0".ToUpper();
                    productColours.TooltipTextColour = "FFFFFF".ToUpper();

                    productColours.GreenLightFieldColour = "00A0AF".ToUpper();
                    productColours.GreenLightSectionTextColour = "00A0AF".ToUpper();
                    productColours.GreenLightSectionBackgroundColour = "FFFFFF".ToUpper();
                    productColours.GreenLightSectionUnderlineColour = "00A0AF".ToUpper();
                    break;
                case ProductType.expenses:
                    productColours.MenubarBGColour = "4A65A0".ToUpper();
                    productColours.MenubarFGColour = "E0E0E0".ToUpper();

                    productColours.TitlebarBGColour = "4A65A0".ToUpper();
                    productColours.TitlebarFGColour = "FFFFFF".ToUpper();

                    productColours.RowColourBG = "FFFFFF".ToUpper();
                    productColours.RowColourFG = "333333".ToUpper();
                    productColours.AlternateRowColourBG = "D2E4EE".ToUpper();
                    productColours.AlternateRowColourFG = "333333".ToUpper();

                    productColours.FieldBG = "4A65A0".ToUpper();
                    productColours.FieldFG = "FFFFFF".ToUpper();

                    productColours.HoverColour = "003768".ToUpper();
                    productColours.PageOptionFGColour = "6280a7".ToUpper();

                    productColours.TooltipBGColour = "4A65A0".ToUpper();
                    productColours.TooltipTextColour = "FFFFFF".ToUpper();

                    productColours.GreenLightFieldColour = "4A65A0".ToUpper();
                    productColours.GreenLightSectionTextColour = "4A65A0".ToUpper();
                    productColours.GreenLightSectionBackgroundColour = "FFFFFF".ToUpper();
                    productColours.GreenLightSectionUnderlineColour = "4A65A0".ToUpper();
                    break;
                #region colours for SmartD and Greenlight
                //case SmartDiligence:
                //    productColours.MenubarBGColour = "1E2326".ToUpper();
                //    productColours.MenubarFGColour = "E0E0E0".ToUpper();

                //    productColours.TitlebarBGColour = "C7114A".ToUpper();
                //    productColours.TitlebarFGColour = "FFFFFF".ToUpper();

                //    productColours.RowColourBG = "FFFFFF".ToUpper();
                //    productColours.RowColourFG = "333333".ToUpper();
                //    productColours.AlternateRowColourBG = "C7114A".ToUpper();
                //    productColours.AlternateRowColourFG = "FFFFFF".ToUpper();

                //    productColours.FieldBG = "1E2326".ToUpper();
                //    productColours.FieldFG = "FFFFFF".ToUpper();

                //    productColours.HoverColour = "663399".ToUpper();
                //    productColours.PageOptionFGColour = "C7114A".ToUpper();

                //    productColours.TooltipBGColour = "4A65A0".ToUpper();
                //    productColours.TooltipTextColour = "FFFFFF".ToUpper();

                //    productColours.GreenLightFieldColour = "1E2326".ToUpper();
                //    productColours.GreenLightSectionTextColour = "1E2326".ToUpper();
                //    productColours.GreenLightSectionBackgroundColour = "FFFFFF".ToUpper();
                //    productColours.GreenLightSectionUnderlineColour = "1E2326".ToUpper();
                //    break;
                //case Greenlight:
                //    productColours.MenubarBGColour = "E46C0A".ToUpper();
                //    productColours.MenubarFGColour = "E0E0E0".ToUpper();

                //    productColours.TitlebarBGColour = "E46C0A".ToUpper();
                //    productColours.TitlebarFGColour = "FFFFFF".ToUpper();

                //    productColours.RowColourBG = "FFFFFF".ToUpper();
                //    productColours.RowColourFG = "333333".ToUpper();
                //    productColours.AlternateRowColourBG = "E9893B".ToUpper();
                //    productColours.AlternateRowColourFG = "333333".ToUpper();

                //    productColours.FieldBG = "E46C0A".ToUpper();
                //    productColours.FieldFG = "FFFFFF".ToUpper();

                //    productColours.HoverColour = "A04C07".ToUpper();
                //    productColours.PageOptionFGColour = "EC9854".ToUpper();

                    //productColours.TooltipBGColour = "4A65A0".ToUpper();
                //    productColours.TooltipTextColour = "FFFFFF".ToUpper();


                //    productColours.GreenLightFieldColour = "E46C0A".ToUpper();
                //    productColours.GreenLightSectionTextColour = "E46C0A".ToUpper();
                //    productColours.GreenLightSectionBackgroundColour = "FFFFFF".ToUpper();
                //    productColours.GreenLightSectionUnderlineColour = "E46C0A".ToUpper();
                //    break;
                #endregion
                default:
                    productColours.MenubarBGColour = "4A65A0".ToUpper();
                    productColours.MenubarFGColour = "E0E0E0".ToUpper();

                    productColours.TitlebarBGColour = "4A65A0".ToUpper();
                    productColours.TitlebarFGColour = "FFFFFF".ToUpper();

                    productColours.RowColourBG = "FFFFFF".ToUpper();
                    productColours.RowColourFG = "333333".ToUpper();
                    productColours.AlternateRowColourBG = "D2E4EE".ToUpper();
                    productColours.AlternateRowColourFG = "333333".ToUpper();

                    productColours.FieldBG = "4A65A0".ToUpper();
                    productColours.FieldFG = "FFFFFF".ToUpper();

                    productColours.HoverColour = "003768".ToUpper();
                    productColours.PageOptionFGColour = "6280a7".ToUpper();

                    productColours.TooltipBGColour = "4A65A0".ToUpper();
                    productColours.TooltipTextColour = "FFFFFF".ToUpper();

                    productColours.GreenLightFieldColour = "4A65A0".ToUpper();
                    productColours.GreenLightSectionTextColour = "4A65A0".ToUpper();
                    productColours.GreenLightSectionBackgroundColour = "FFFFFF".ToUpper();
                    productColours.GreenLightSectionUnderlineColour = "4A65A0".ToUpper();
                    break;
            }
                    return productColours;
        }
    }
}
