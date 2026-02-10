namespace MaterialSkin.Controls;

[ToolboxItem(false)]
public class MaterialContextMenuStrip : ContextMenuStrip, IMaterialControl
{
    internal AnimationManager _animationManager;
    internal Point _animationSource;
    private ToolStripItemClickedEventArgs? _delayedArgs;

    public event EventHandler<ToolStripItemClickedEventArgs>? OnItemClickStart;

    public MaterialContextMenuStrip()
    {
        Renderer = new MaterialToolStripRender();

        _animationManager = new AnimationManager(false)
        {
            Increment = 0.07,
            AnimationType = AnimationType.Linear
        };

        _animationManager.OnAnimationProgress += (sender, e) => Invalidate();
        _animationManager.OnAnimationFinished += (sender, e) => OnItemClicked(_delayedArgs);

        BackColor = MaterialSkinManager.Instance.BackdropColor;
    }

    //Properties for managing the material design properties
    [Browsable(false)]
    public MouseState MouseState { get; set; }

    protected override void OnMouseUp(MouseEventArgs mea)
    {
        base.OnMouseUp(mea);
        _animationSource = mea.Location;
    }

    protected override void OnItemClicked(ToolStripItemClickedEventArgs? e)
    {
        if (e == null)
            return;

        if (e.ClickedItem != null && e.ClickedItem is not ToolStripSeparator)
        {
            if (e == _delayedArgs)
            {
                //The event has been fired manualy because the args are the ones we saved for delay
                base.OnItemClicked(e);
            }
            else
            {
                //Interrupt the default on click, saving the args for the delay which is needed to display the animaton
                _delayedArgs = e;

                //Fire custom event to trigger actions directly but keep cms open
                OnItemClickStart?.Invoke(this, e);

                //Start animation
                _animationManager.StartNewAnimation(AnimationDirection.In);
            }
        }
    }
}

internal sealed class MaterialToolStripRender : ToolStripProfessionalRenderer, IMaterialControl
{
    private const int _leftPadding = 16;

    //Properties for managing the material design properties
    public MouseState MouseState { get; set; }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        if (e.Text == null)
            return;

        var g = e.Graphics;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        var itemRect = GetItemRect(e.Item);
        var textRect = new Rectangle(_leftPadding, itemRect.Y, itemRect.Width - _leftPadding, itemRect.Height);

        using var NativeText = new NativeTextRenderer(g);
        NativeText.DrawTransparentText(e.Text, MaterialSkinManager.Instance.GetLogFontByType(FontType.Body2),
            e.Item.Enabled ? MaterialSkinManager.Instance.TextHighEmphasisColor : MaterialSkinManager.Instance.TextDisabledOrHintColor,
            textRect.Location,
            textRect.Size,
            TextAlignFlags.Left | TextAlignFlags.Middle);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(MaterialSkinManager.Instance.BackgroundColor);

        //Draw background
        var itemRect = GetItemRect(e.Item);
        g.FillRectangle(e.Item.Selected && e.Item.Enabled ? MaterialSkinManager.Instance.BackgroundFocusBrush : MaterialSkinManager.Instance.BackgroundBrush, itemRect);

        //Ripple animation
        if (e.ToolStrip is MaterialContextMenuStrip toolStrip)
        {
            var animationManager = toolStrip._animationManager;
            var animationSource = toolStrip._animationSource;
            if (toolStrip._animationManager.IsAnimating() && e.Item.Bounds.Contains(animationSource))
            {
                for (var i = 0; i < animationManager.GetAnimationCount(); i++)
                {
                    var animationValue = animationManager.GetProgress(i);
                    var rippleBrush = new SolidBrush(Color.FromArgb((int)(51 - (animationValue * 50)), Color.Black));
                    var rippleSize = (int)(animationValue * itemRect.Width * 2.5);
                    g.FillEllipse(rippleBrush, new Rectangle(animationSource.X - rippleSize / 2, itemRect.Y - itemRect.Height, rippleSize, itemRect.Height * 3));
                }
            }
        }
    }

    protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
    {
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        var g = e.Graphics;

        g.FillRectangle(MaterialSkinManager.Instance.BackgroundBrush, e.Item.Bounds);
        g.DrawLine(
            new Pen(MaterialSkinManager.Instance.DividersColor),
            new Point(e.Item.Bounds.Left, e.Item.Bounds.Height / 2),
            new Point(e.Item.Bounds.Right, e.Item.Bounds.Height / 2));
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) => e.ToolStrip.BackColor = MaterialSkinManager.Instance.BackgroundColor;
    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        var g = e.Graphics;
        const int arrowSize = 4;

        var arrowMiddle = new Point(e.ArrowRectangle.X + e.ArrowRectangle.Width / 2, e.ArrowRectangle.Y + e.ArrowRectangle.Height / 2);
        var arrowBrush = e.Item?.Enabled == true ? MaterialSkinManager.Instance.TextHighEmphasisBrush : MaterialSkinManager.Instance.TextDisabledOrHintBrush;
        using var arrowPath = new GraphicsPath();
        arrowPath.AddLines(
        [
            new Point(arrowMiddle.X - arrowSize, arrowMiddle.Y - arrowSize),
            new Point(arrowMiddle.X, arrowMiddle.Y),
            new Point(arrowMiddle.X - arrowSize, arrowMiddle.Y + arrowSize)
        ]);
        arrowPath.CloseFigure();
        g.FillPath(arrowBrush, arrowPath);
    }

    private static Rectangle GetItemRect(ToolStripItem item) => new(0, item.ContentRectangle.Y, item.ContentRectangle.Width, item.ContentRectangle.Height);
}
