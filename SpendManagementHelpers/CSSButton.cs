using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SpendManagementHelpers
{
    public class CSSButton : Button
    {
        public CSSButtonSize ButtonSize { get { return (CSSButtonSize)ViewState["ButtonSize"]; } set { ViewState["ButtonSize"] = value; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (ViewState["ButtonSize"] == null)
                ButtonSize = CSSButtonSize.Standard;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string cssPrefix = "";
            if (ButtonSize == CSSButtonSize.Small)
                cssPrefix = "small";

            writer.Write("<span class=\"" + cssPrefix + "buttonContainer\">");
            base.CssClass = cssPrefix + "buttonInner";
            base.Render(writer);
            writer.Write("</span>");
        }
    }

    [DefaultProperty("Text")]
    [ToolboxData("<helpers:CSSSpanButton runat=\"server\" />")]
    public class CSSSpanButton : WebControl
    {
        public CSSSpanButton() : base(HtmlTextWriterTag.Span) { }

        public string Text { get; set; }
        public CSSButtonSize ButtonSize { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Text == null)
                Text = "ok";
        }

        protected override void Render(HtmlTextWriter writer)
        {
            string cssPrefix = "";
            if (ButtonSize == CSSButtonSize.Small)
                cssPrefix = "small";

            writer.Write("<span class=\"" + cssPrefix + "buttonContainer\">");
            base.CssClass = cssPrefix + "buttonInner";
            base.Render(writer);
            writer.Write("</span>");
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write(Text);
            base.RenderContents(writer);
        }
    }

    public enum CSSButtonSize
    {
        Standard,
        Small
    }
}
