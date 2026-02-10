namespace MaterialSkin.Controls;

public class MaterialCard : Panel, IMaterialControl
{
    private Control? _oldParent;
    private bool _shadowDrawEventSubscribed;

    public MaterialCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        Paint += PaintControl;
        BackColor = SkinManager.BackgroundColor;
        ForeColor = SkinManager.TextHighEmphasisColor;
        Margin = new Padding(SkinManager.FORM_PADDING);
        Padding = new Padding(SkinManager.FORM_PADDING);
    }

    [Browsable(false)]
    public MaterialSkinManager SkinManager => MaterialSkinManager.Instance;

    [Browsable(false)]
    public MouseState MouseState { get; set; }

    private void DrawShadowOnParent(object? sender, PaintEventArgs e)
    {
        if (Parent == null)
        {
            RemoveShadowPaintEvent((Control)sender!, DrawShadowOnParent);
            return;
        }

        // paint shadow on parent
        var gp = e.Graphics;
        var rect = new Rectangle(Location, ClientRectangle.Size);
        gp.SmoothingMode = SmoothingMode.AntiAlias;
        DrawHelper.DrawSquareShadow(gp, rect);
    }

    protected override void InitLayout()
    {
        LocationChanged += (sender, e) => { Parent?.Invalidate(); };
        ForeColor = SkinManager.TextHighEmphasisColor;
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent != null)
        {
            AddShadowPaintEvent(Parent, DrawShadowOnParent);
        }

        if (_oldParent != null)
        {
            RemoveShadowPaintEvent(_oldParent, DrawShadowOnParent);
        }

        _oldParent = Parent;
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Parent == null)
            return;

        if (Visible)
        {
            AddShadowPaintEvent(Parent, DrawShadowOnParent);
        }
        else
        {
            RemoveShadowPaintEvent(Parent, DrawShadowOnParent);
        }
    }

    private void AddShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
    {
        if (_shadowDrawEventSubscribed)
            return;

        control.Paint += shadowPaintEvent;
        control.Invalidate();
        _shadowDrawEventSubscribed = true;
    }

    private void RemoveShadowPaintEvent(Control control, PaintEventHandler shadowPaintEvent)
    {
        if (!_shadowDrawEventSubscribed)
            return;

        control.Paint -= shadowPaintEvent;
        control.Invalidate();
        _shadowDrawEventSubscribed = false;
    }

    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        BackColor = SkinManager.BackgroundColor;
    }

    private void PaintControl(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        if (Parent != null)
        {
            g.Clear(Parent.BackColor);
        }

        // card rectangle path
        var cardRectF = new RectangleF(ClientRectangle.Location, ClientRectangle.Size);
        cardRectF.X -= 0.5f;
        cardRectF.Y -= 0.5f;
        var cardPath = DrawHelper.CreateRoundRect(cardRectF, 4);

        // button shadow (blend with form shadow)
        DrawHelper.DrawSquareShadow(g, ClientRectangle);

        // Draw card
        using var normalBrush = new SolidBrush(BackColor);
        g.FillPath(normalBrush, cardPath);
    }
}