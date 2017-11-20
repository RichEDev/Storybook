//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

namespace System
{

    using System.Web.UI.WebControls;
    public static class CKEditorExtensions
    {
        public static void Configure(this CKEditor.NET.CKEditorControl editor, EditorMode mode = EditorMode.Short)
        {
            switch (mode)
            {
                case EditorMode.Full:
                    editor.config.toolbar = new object[]
			        {
				        new object[] { "Source", "-", "Preview", "-", "Templates" },
				        new object[] { "Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt" },
				        new object[] { "Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat" },
				        "/",
				        new object[] { "Bold", "Italic", "Underline", "Strike", "-", "Subscript", "Superscript" },
				        new object[] { "NumberedList", "BulletedList", "-", "Outdent", "Indent", "Blockquote", "CreateDiv" },
				        new object[] { "JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock" },
				        new object[] { "BidiLtr", "BidiRtl" },
				        new object[] { "Link", "Unlink", "Anchor" },
				        new object[] { "Image", "Table", "HorizontalRule", "Smiley", "SpecialChar", "PageBreak" },
				        "/",
				        new object[] { "Styles", "Format", "Font", "FontSize" },
				        new object[] { "TextColor", "BGColor" },
				        new object[] { "Maximize", "ShowBlocks" }
			        };
                    break;
                
                case EditorMode.Short:
                    editor.config.toolbar = new object[]
			        {
                        new object[] {"Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt" },
                        new object[] {"Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat"}, "/",
                        new object[] {"Bold", "Italic", "Underline"}, 
                        new object[] {"TextColor"}, 
                        new object[] {"Link", "Unlink"}, 
                        new object[] {"NumberedList", "BulletedList", "-", "Outdent", "Indent"}, 
                        new object[] {"JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"}, 
                        new object[] {"Format", "Font", "FontSize"},

			        };
                    
                    editor.Height = 200;
                    break;
                case EditorMode.ShortNoIFrame:
                    editor.config.extraPlugins = "divarea";
                    editor.config.toolbar = new object[]
                    {
                        new object[] {"Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt" },
                        new object[] {"Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat"}, "/",
                        new object[] {"Bold", "Italic", "Underline"},
                        new object[] {"TextColor"},
                        new object[] {"Link", "Unlink"},
                        new object[] {"NumberedList", "BulletedList", "-", "Outdent", "Indent"},
                        new object[] {"JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"},
                        new object[] {"Table"},
                        new object[] {"Format", "Font", "FontSize"},
                    };

                    editor.Height = 200;
                    break;
                case EditorMode.ShortNoIFrameSelAdmin:
                    editor.config.extraPlugins = "divarea,sourcedialog";
                    editor.config.toolbar = new object[]
                    {
                        new object[] {"Cut", "Copy", "Paste", "PasteText", "PasteFromWord", "-", "Print", "SpellChecker", "Scayt" },
                        new object[] {"Undo", "Redo", "-", "Find", "Replace", "-", "SelectAll", "RemoveFormat"}, "/",
                        new object[] {"Bold", "Italic", "Underline"},
                        new object[] {"TextColor"},
                        new object[] {"Link", "Unlink"},
                        new object[] {"NumberedList", "BulletedList", "-", "Outdent", "Indent"},
                        new object[] {"JustifyLeft", "JustifyCenter", "JustifyRight", "JustifyBlock"},
                        new object[] {"Table"},
                        new object[] {"Format", "Font", "FontSize"},
                        new object[] {"Source"}
                    };

                    editor.Height = 200;
                    break;

                case EditorMode.Shortest:
                    editor.config.removePlugins = "toolbar,elementspath,resize";
                    editor.config.extraPlugins = "divarea";
                    editor.Attributes.Add("title", "");
                    break;
            }

            editor.EnterMode = CKEditor.NET.EnterMode.BR;


        }
    }
    public enum EditorMode
    {
        Full,
        Short,
        Shortest,
        ShortNoIFrame,
        ShortNoIFrameSelAdmin
    }
}

