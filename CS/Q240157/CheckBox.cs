using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using System.ComponentModel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Skins;
using System;

namespace DXSample.Editors {
    public class MyCheckEdit : CheckEdit {
        static MyCheckEdit() { RepositoryItemMyCheckEdit.RegisterMyCheckEdit(); }
        public MyCheckEdit() : base() { }

        public override string EditorTypeName { get { return RepositoryItemMyCheckEdit.MyCheckEditName; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemMyCheckEdit Properties { get { return (RepositoryItemMyCheckEdit)base.Properties; } }
    }

    [UserRepositoryItem("RegisterMyCheckEdit")]
    public class RepositoryItemMyCheckEdit : RepositoryItemCheckEdit {
        static RepositoryItemMyCheckEdit() { RegisterMyCheckEdit(); }
        public RepositoryItemMyCheckEdit() : base() { }

        internal const string MyCheckEditName = "MyCheckEdit";

        public override string EditorTypeName { get { return MyCheckEditName; } }

        public static void RegisterMyCheckEdit() {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(MyCheckEditName, typeof(MyCheckEdit),
                typeof(RepositoryItemMyCheckEdit), typeof(MyCheckEditViewInfo), new CheckEditPainter(), true));
        }

        private VerticalGlyphAlignment fVerticalGlyphAlignment;
        public VerticalGlyphAlignment VerticalGlyphAlignment {
            get { return fVerticalGlyphAlignment; }
            set {
                if (VerticalGlyphAlignment == value) return;
                fVerticalGlyphAlignment = value;
                OnPropertiesChanged();
            }
        }

        public override void Assign(RepositoryItem item) {
            BeginUpdate();
            try {
                base.Assign(item);
                RepositoryItemMyCheckEdit source = item as RepositoryItemMyCheckEdit;
                if (source == null) return;
                VerticalGlyphAlignment = source.VerticalGlyphAlignment;
            }
            finally { EndUpdate(); }
        }
    }

    public class MyFlatCheckObjectPainter : FlatCheckObjectPainter, ICaptionCalculationMethodsProvider {
        public MyFlatCheckObjectPainter() : base() { }

        public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
            base.CalcObjectBounds(e);
            return ReCalcObjectBounds(e, this);
        }

        public static Rectangle ReCalcObjectBounds(ObjectInfoArgs e, ICaptionCalculationMethodsProvider instance) {
            MyCheckObjectInfoArgs ee = e as MyCheckObjectInfoArgs;
            if (ee != null) {
                Rectangle glyphRect = ee.GlyphRect;
                Rectangle captionRect = ee.CaptionRect;
                if (ee.GlyphAlignment == HorzAlignment.Center &&
                    ee.VerticalGlyphAlignment != VerticalGlyphAlignment.Center) {
                    Size cSize = instance.CalcCaptionSize(ee, e.Bounds.Width);
                    captionRect = e.Bounds;
                    captionRect.Width = Math.Min(cSize.Width, captionRect.Width);
                    ee.CaptionRect = captionRect;
                    if (ee.AllowHtmlString) instance.CalcHtmlCaptionBounds(ee);
                }
                captionRect.X = (ee.Bounds.Width - captionRect.Width) / 2;
                switch (ee.VerticalGlyphAlignment) {
                    case VerticalGlyphAlignment.Top:
                        glyphRect.Y = 2;
                        captionRect.Y = glyphRect.Height - 4;
                        break;
                    case VerticalGlyphAlignment.Bottom:
                        glyphRect.Y = ee.GlyphRect.Height + 2;
                        captionRect.Y = 2;
                        captionRect.Height -= glyphRect.Height + 2;
                        break;
                    default: return ee.Bounds;
                }
                ee.GlyphRect = glyphRect;
                ee.CaptionRect = captionRect;
            }
            return e.Bounds;
        }

        public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
            base.CalcObjectMinBounds(e);
            return ReCalcObjectMinBounds(e);
        }

        public static Rectangle ReCalcObjectMinBounds(ObjectInfoArgs e) {
            MyCheckObjectInfoArgs ee = e as MyCheckObjectInfoArgs;
            if (ee != null && ee.VerticalGlyphAlignment != VerticalGlyphAlignment.Center)
                ee.Bounds = new Rectangle(ee.Bounds.X, ee.Bounds.Y, ee.Bounds.Width, ee.Bounds.Height * 2 + 2);
            return ee.Bounds;
        }

        protected override bool CanDrawCaption(BaseCheckObjectInfoArgs e) {
            MyCheckObjectInfoArgs ee = e as MyCheckObjectInfoArgs;
            if (ee == null) return base.CanDrawCaption(e);
            else
                return CanDrawCaptionEx(ee);
        }

        public static bool CanDrawCaptionEx(MyCheckObjectInfoArgs ee) {
            return !(ee.GlyphAlignment == HorzAlignment.Center &&
                ee.VerticalGlyphAlignment == VerticalGlyphAlignment.Center);
        }

        #region ICaptionCalculationMethodsProvider Members

        Size ICaptionCalculationMethodsProvider.CalcCaptionSize(CheckObjectInfoArgs e, int width) {
            return CalcCaptionSize(e, width);
        }

        Rectangle ICaptionCalculationMethodsProvider.CalcCaptionRect(BaseCheckObjectInfoArgs e, Rectangle bounds, Rectangle caption, Rectangle glyph) {
            return CalcCaptionRect(e, bounds, caption, glyph);
        }

        void ICaptionCalculationMethodsProvider.CalcHtmlCaptionBounds(CheckObjectInfoArgs e) {
            CalcHtmlCaptionBounds(e);
        }

        #endregion
    }

    public class MySkinCheckObjectPainter : SkinCheckObjectPainter, ICaptionCalculationMethodsProvider {
        public MySkinCheckObjectPainter(ISkinProvider provider) : base(provider) { }

        public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
            base.CalcObjectBounds(e);
            return MyFlatCheckObjectPainter.ReCalcObjectBounds(e, this);
        }

        public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
            base.CalcObjectMinBounds(e);
            return MyFlatCheckObjectPainter.ReCalcObjectMinBounds(e);
        }

        protected override bool CanDrawCaption(BaseCheckObjectInfoArgs e) {
            MyCheckObjectInfoArgs ee = e as MyCheckObjectInfoArgs;
            if (ee == null) return base.CanDrawCaption(e);
            else return MyFlatCheckObjectPainter.CanDrawCaptionEx(ee);
        }

        public override void DrawCaption(ObjectInfoArgs e) {
            base.DrawCaption(e);
        }

        #region ICaptionCalculationMethodsProvider Members

        Size ICaptionCalculationMethodsProvider.CalcCaptionSize(CheckObjectInfoArgs e, int width) {
            return CalcCaptionSize(e, width);
        }

        Rectangle ICaptionCalculationMethodsProvider.CalcCaptionRect(BaseCheckObjectInfoArgs e, Rectangle bounds, Rectangle caption, Rectangle glyph) {
            return CalcCaptionRect(e, bounds, caption, glyph);
        }

        void ICaptionCalculationMethodsProvider.CalcHtmlCaptionBounds(CheckObjectInfoArgs e) {
            CalcHtmlCaptionBounds(e);
        }

        #endregion
    }

    public class MyCheckObjectInfoArgs : CheckObjectInfoArgs {
        public MyCheckObjectInfoArgs(AppearanceObject style) : base(style) { }

        private VerticalGlyphAlignment fVerticalGlyphAlignment;
        public VerticalGlyphAlignment VerticalGlyphAlignment {
            get { return fVerticalGlyphAlignment; }
            set { fVerticalGlyphAlignment = value; }
        }

        public override void Assign(ObjectInfoArgs info) {
            base.Assign(info);
            MyCheckObjectInfoArgs source = info as MyCheckObjectInfoArgs;
            if (source == null) return;
            VerticalGlyphAlignment = source.VerticalGlyphAlignment;
        }
    }

    public enum VerticalGlyphAlignment { Top, Center, Bottom }

    public class MyCheckEditViewInfo : CheckEditViewInfo {
        public MyCheckEditViewInfo(RepositoryItem item) : base(item) { }


        protected override void UpdateCheckProperties(BaseCheckObjectInfoArgs e) {
            base.UpdateCheckProperties(e);
            MyCheckObjectInfoArgs ee = e as MyCheckObjectInfoArgs;
            if (ee == null) return;
            ee.VerticalGlyphAlignment = Item.VerticalGlyphAlignment;
        }

        public new RepositoryItemMyCheckEdit Item { get { return (RepositoryItemMyCheckEdit)base.Item; } }

        protected override BaseCheckObjectInfoArgs CreateCheckArgs() {
            return new MyCheckObjectInfoArgs(PaintAppearance);
        }

        protected override BaseCheckObjectPainter CreateCheckPainter() {
            if (IsPrinting) return new MyFlatCheckObjectPainter();
            return new MySkinCheckObjectPainter(LookAndFeel);
        }
    }

    public interface ICaptionCalculationMethodsProvider {
        Size CalcCaptionSize(CheckObjectInfoArgs e, int width);
        Rectangle CalcCaptionRect(BaseCheckObjectInfoArgs e, Rectangle bounds, Rectangle caption, Rectangle glyph);
        void CalcHtmlCaptionBounds(CheckObjectInfoArgs e);
    }
}