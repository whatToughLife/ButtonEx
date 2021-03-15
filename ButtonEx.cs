using CycloneFlashShell.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CycloneFlashShell.Tool
{
    public partial class ButtonEx : Button
    {
        /// <summary>
        /// 必需的设计器变量。System.ComponentModel 
        /// 命名空间提供用于实现组件和控件的运行时和设计时行为的类。
        /// 此命名空间包括用于特性和类型转换器的实现、数据源绑定和组件授权的基类和接口。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        ControlState paintControlState = ControlState.Normal;


        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
        public Color EnterBeginColor { get; set; }
        public Color PressBeginColor { get; set; }
        public Color NormalBeginColor { get; private set; }

        public Color NormalEndColor { get; set; }
        public Color TextColor { get; set; }
        public Color TextSuccColor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Color TextFailColor { get; set; }
        /// <summary>
        /// 圆角半径
        /// </summary>
        public int Radius { get; set; }
        /// <summary>
        /// 是否点击
        /// </summary>
        private bool Clicked { get; set; } = false;


        public ButtonEx()
        {
            InitializeComponent();
            //这些得带上，不然会有黑边
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0, 0);
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;

            SetDefaultColor();


        }
        public void SetDefaultColor()
        {//给个初始值
            EnterBeginColor = Color.Blue;
            PressBeginColor = Color.Yellow;
            NormalBeginColor = Color.Gray;

            NormalEndColor = Color.White;

            TextColor = Color.Black;
            TextSuccColor = Color.Green;
            TextFailColor = Color.Red;
            Radius = 18;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);//这个不能去，而且得放在前面，不然会有黑框之类的莫名其妙的东西
            var colorbegin = NormalBeginColor;
            var colorend = NormalEndColor;
            switch (paintControlState)
            {
                case ControlState.Enter:
                    colorbegin = EnterBeginColor;
                    colorend = NormalEndColor;
                    break;
                case ControlState.Pressed:
                    colorbegin = PressBeginColor;
                    colorend = NormalEndColor;
                    break;
                case ControlState.Enable:
                    colorbegin = Color.LightGray;
                    colorend = Color.DarkGray;
                    break;
                case ControlState.Normal:
                    colorbegin = NormalBeginColor;
                    colorend = NormalEndColor;
                    break;
                default:
                    colorbegin = NormalBeginColor;
                    colorend = NormalEndColor;
                    break;
            }
            DrawButton(e.ClipRectangle, e.Graphics, false, colorend, colorbegin);
            DrawText(e.ClipRectangle, e.Graphics, TextColor);
            DrawIcon(new Rectangle(3, e.ClipRectangle.Height / 4, 24, 24), e.Graphics);
        }


        #region 鼠标事件
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            paintControlState = ControlState.Enter;
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            paintControlState = ControlState.Normal;
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Clicked = !Clicked;
            paintControlState = ControlState.Pressed;

        }
        protected override void OnEnabledChanged(EventArgs e)
        {
            paintControlState = Enabled ? ControlState.Normal : ControlState.Enable;
            Invalidate();//false 转换为true的时候不会刷新 这里强制刷新下
            base.OnEnabledChanged(e);
        }
        #endregion

        void DrawButton(Rectangle rectangle, Graphics g, bool cusp, Color begin_color, Color? end_colorex = null)
        {
            Color end_color = end_colorex == null ? begin_color : (Color)end_colorex;
            int span = 2;
            //抗锯齿
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //渐变填充
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush(rectangle, begin_color, end_color, LinearGradientMode.Vertical);
            //画尖角
            if (cusp)
            {
                span = 10;
                PointF p1 = new PointF(rectangle.Width - 12, rectangle.Y + 10);
                PointF p2 = new PointF(rectangle.Width - 12, rectangle.Y + 30);
                PointF p3 = new PointF(rectangle.Width, rectangle.Y + 20);
                PointF[] ptsArray = { p1, p2, p3 };
                g.FillPolygon(myLinearGradientBrush, ptsArray);
            }
            //填充
            g.FillPath(myLinearGradientBrush, DrawRoundRect(rectangle.X, rectangle.Y, rectangle.Width - span, rectangle.Height - 1, Radius));

        }
        void DrawText(Rectangle rectangle, Graphics g, Color color)
        {
            SolidBrush sbr = new SolidBrush(color);
            var rect = new RectangleF();
            switch (TextAlign)
            {
                case ContentAlignment.MiddleRight:
                    rect = getTextRec(rectangle, g);
                    break;
                default:
                    rect = getTextRec(rectangle, g);
                    break;
            }
            g.DrawString(Text, Font, sbr, rect);
        }
        void DrawIcon(Rectangle clipRectangle, Graphics g)
        {
            if (Image != null)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.DrawImage(Image, clipRectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            }

        }
        RectangleF getTextRec(Rectangle rectangle, Graphics g)
        {
            var rect = new RectangleF();
            var size = g.MeasureString(Text, Font);
            if (size.Width > rectangle.Width || size.Height > rectangle.Height)
            {
                rect = rectangle;
            }
            else
            {
                rect.Size = size;
                rect.Location = new PointF(rectangle.X + 27, rectangle.Y + (rectangle.Height - size.Height) / 2+5 );
            }
            return rect;
        }
        GraphicsPath DrawRoundRect(int x, int y, int width, int height, int radius)
        {
            //四边圆角
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(x, y, radius, radius, 180, 90);
            gp.AddArc(width - radius, y, radius, radius, 270, 90);
            gp.AddArc(width - radius, height - radius, radius, radius, 0, 90);
            gp.AddArc(x, height - radius, radius, radius, 90, 90);
            gp.CloseAllFigures();
            return gp;
        }
    }
}
