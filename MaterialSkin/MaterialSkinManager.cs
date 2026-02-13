namespace MaterialSkin;

public partial class MaterialSkinManager : IDisposable
{
    public static MaterialSkinManager Instance { get; set; } = new();

    public event EventHandler? ColorSchemeChanged;
    public event EventHandler? ThemeChanged;

    private readonly List<MaterialForm> _formsToManage = [];
    private readonly Dictionary<string, nint> _logicalFonts;
    private readonly Dictionary<string, FontFamily> _robotoFontFamilies;
    private readonly PrivateFontCollection _privateFontCollection = new();
    private ColorScheme _colorScheme;
    private bool _disposedValue;

    public MaterialSkinManager()
    {
        Theme = Themes.LIGHT;
        _colorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);

        // Create and cache Roboto fonts
        // Thanks https://www.codeproject.com/Articles/42041/How-to-Use-a-Font-Without-Installing-it
        // And https://www.codeproject.com/Articles/107376/Embedding-Font-To-Resources

        // Add font to system table in memory and save the font family
        AddFont(Resources.Roboto_Thin);
        AddFont(Resources.Roboto_Light);
        AddFont(Resources.Roboto_Regular);
        AddFont(Resources.Roboto_Medium);
        AddFont(Resources.Roboto_Bold);
        AddFont(Resources.Roboto_Black);

        _robotoFontFamilies = [];
        foreach (var ff in _privateFontCollection.Families)
        {
            _robotoFontFamilies.Add(ff.Name.Replace(' ', '_'), ff);
        }

        // create and save font handles for GDI
        _logicalFonts = new Dictionary<string, nint>(18)
        {
            { "H1", AreateLogicalFont("Roboto Light", 96, LogFontWeight.FW_LIGHT) },
            { "H2", AreateLogicalFont("Roboto Light", 60, LogFontWeight.FW_LIGHT) },
            { "H3", AreateLogicalFont("Roboto", 48, LogFontWeight.FW_REGULAR) },
            { "H4", AreateLogicalFont("Roboto", 34, LogFontWeight.FW_REGULAR) },
            { "H5", AreateLogicalFont("Roboto", 24, LogFontWeight.FW_REGULAR) },
            { "H6", AreateLogicalFont("Roboto Medium", 20, LogFontWeight.FW_MEDIUM) },
            { "Subtitle1", AreateLogicalFont("Roboto", 16, LogFontWeight.FW_REGULAR) },
            { "Subtitle2", AreateLogicalFont("Roboto Medium", 14, LogFontWeight.FW_MEDIUM) },
            { "SubtleEmphasis", AreateLogicalFont("Roboto", 12, LogFontWeight.FW_NORMAL, 1) },
            { "Body1", AreateLogicalFont("Roboto", 16, LogFontWeight.FW_REGULAR) },
            { "Body2", AreateLogicalFont("Roboto", 14, LogFontWeight.FW_REGULAR) },
            { "Button", AreateLogicalFont("Roboto Medium", 14, LogFontWeight.FW_MEDIUM) },
            { "Caption", AreateLogicalFont("Roboto", 12, LogFontWeight.FW_REGULAR) },
            { "Overline", AreateLogicalFont("Roboto", 10, LogFontWeight.FW_REGULAR) },
            // Logical fonts for textbox animation
            { "textBox16", AreateLogicalFont("Roboto", 16, LogFontWeight.FW_REGULAR) },
            { "textBox15", AreateLogicalFont("Roboto", 15, LogFontWeight.FW_REGULAR) },
            { "textBox14", AreateLogicalFont("Roboto", 14, LogFontWeight.FW_REGULAR) },
            { "textBox13", AreateLogicalFont("Roboto Medium", 13, LogFontWeight.FW_MEDIUM) },
            { "textBox12", AreateLogicalFont("Roboto Medium", 12, LogFontWeight.FW_MEDIUM) }
        };
    }

    /// <summary>
    /// Set this property to false to stop enforcing the backcolor on non-materialSkin components
    /// </summary>
    public virtual bool EnforceBackcolorOnAllComponents { get; set; } = true;

    public virtual int FormPadding { get; set; } = 14;
    public virtual int FormBorderWidth { get; set; } = 7;
    public virtual int FormStatusBarButtonWidth { get; set; } = 24;
    public virtual int FormStatusBarHeight { get; set; } = 24;
    public virtual int FormIconSize { get; set; } = 24;
    public virtual int FormPaddingMinimum { get; set; } = 3;
    public virtual int FormTitleLeftPadding { get; set; } = 72;
    public virtual int FormActionBarPadding { get; set; } = 16;
    public virtual int FormActionBarHeight { get; set; } = 40;

    public virtual Themes Theme
    {
        get;
        set
        {
            field = value;
            UpdateBackgrounds();
            ThemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public virtual ColorScheme ColorScheme
    {
        get => _colorScheme;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _colorScheme = value;
            UpdateBackgrounds();
            ColorSchemeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    // Text
#pragma warning disable IDE1006 // Naming Styles
    private static readonly Color TEXT_HIGH_EMPHASIS_LIGHT = Color.FromArgb(222, 255, 255, 255); // Alpha 87%
    private static readonly Brush TEXT_HIGH_EMPHASIS_LIGHT_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_LIGHT);
    private static readonly Color TEXT_HIGH_EMPHASIS_DARK = Color.FromArgb(222, 0, 0, 0); // Alpha 87%
    private static readonly Brush TEXT_HIGH_EMPHASIS_DARK_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_DARK);

    private static readonly Color TEXT_HIGH_EMPHASIS_LIGHT_NOALPHA = Color.FromArgb(255, 255, 255, 255); // Alpha 100%
    private static readonly Brush TEXT_HIGH_EMPHASIS_LIGHT_NOALPHA_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_LIGHT_NOALPHA);
    private static readonly Color TEXT_HIGH_EMPHASIS_DARK_NOALPHA = Color.FromArgb(255, 0, 0, 0); // Alpha 100%
    private static readonly Brush TEXT_HIGH_EMPHASIS_DARK_NOALPHA_BRUSH = new SolidBrush(TEXT_HIGH_EMPHASIS_DARK_NOALPHA);

    private static readonly Color TEXT_MEDIUM_EMPHASIS_LIGHT = Color.FromArgb(153, 255, 255, 255); // Alpha 60%
    private static readonly Brush TEXT_MEDIUM_EMPHASIS_LIGHT_BRUSH = new SolidBrush(TEXT_MEDIUM_EMPHASIS_LIGHT);
    private static readonly Color TEXT_MEDIUM_EMPHASIS_DARK = Color.FromArgb(153, 0, 0, 0); // Alpha 60%
    private static readonly Brush TEXT_MEDIUM_EMPHASIS_DARK_BRUSH = new SolidBrush(TEXT_MEDIUM_EMPHASIS_DARK);

    private static readonly Color TEXT_DISABLED_OR_HINT_LIGHT = Color.FromArgb(97, 255, 255, 255); // Alpha 38%
    private static readonly Brush TEXT_DISABLED_OR_HINT_LIGHT_BRUSH = new SolidBrush(TEXT_DISABLED_OR_HINT_LIGHT);
    private static readonly Color TEXT_DISABLED_OR_HINT_DARK = Color.FromArgb(97, 0, 0, 0); // Alpha 38%
    private static readonly Brush TEXT_DISABLED_OR_HINT_DARK_BRUSH = new SolidBrush(TEXT_DISABLED_OR_HINT_DARK);

    // Dividers and thin lines
    private static readonly Color DIVIDERS_LIGHT = Color.FromArgb(30, 255, 255, 255); // Alpha 30%
    private static readonly Brush DIVIDERS_LIGHT_BRUSH = new SolidBrush(DIVIDERS_LIGHT);
    private static readonly Color DIVIDERS_DARK = Color.FromArgb(30, 0, 0, 0); // Alpha 30%
    private static readonly Brush DIVIDERS_DARK_BRUSH = new SolidBrush(DIVIDERS_DARK);
    private static readonly Color DIVIDERS_ALTERNATIVE_LIGHT = Color.FromArgb(153, 255, 255, 255); // Alpha 60%
    private static readonly Brush DIVIDERS_ALTERNATIVE_LIGHT_BRUSH = new SolidBrush(DIVIDERS_ALTERNATIVE_LIGHT);
    private static readonly Color DIVIDERS_ALTERNATIVE_DARK = Color.FromArgb(153, 0, 0, 0); // Alpha 60%
    private static readonly Brush DIVIDERS_ALTERNATIVE_DARK_BRUSH = new SolidBrush(DIVIDERS_ALTERNATIVE_DARK);

    // Checkbox / Radio / Switches
    private static readonly Color CHECKBOX_OFF_LIGHT = Color.FromArgb(138, 0, 0, 0);
    private static readonly Brush CHECKBOX_OFF_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_LIGHT);
    private static readonly Color CHECKBOX_OFF_DARK = Color.FromArgb(179, 255, 255, 255);
    private static readonly Brush CHECKBOX_OFF_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DARK);
    private static readonly Color CHECKBOX_OFF_DISABLED_LIGHT = Color.FromArgb(66, 0, 0, 0);
    private static readonly Brush CHECKBOX_OFF_DISABLED_LIGHT_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_LIGHT);
    private static readonly Color CHECKBOX_OFF_DISABLED_DARK = Color.FromArgb(77, 255, 255, 255);
    private static readonly Brush CHECKBOX_OFF_DISABLED_DARK_BRUSH = new SolidBrush(CHECKBOX_OFF_DISABLED_DARK);

    // Switch specific
    private static readonly Color SWITCH_OFF_THUMB_LIGHT = Color.FromArgb(255, 255, 255, 255);
    private static readonly Color SWITCH_OFF_THUMB_DARK = Color.FromArgb(255, 190, 190, 190);
    private static readonly Color SWITCH_OFF_TRACK_LIGHT = Color.FromArgb(100, 0, 0, 0);
    private static readonly Color SWITCH_OFF_TRACK_DARK = Color.FromArgb(100, 255, 255, 255);
    private static readonly Color SWITCH_OFF_DISABLED_THUMB_LIGHT = Color.FromArgb(255, 230, 230, 230);
    private static readonly Color SWITCH_OFF_DISABLED_THUMB_DARK = Color.FromArgb(255, 150, 150, 150);

    // Generic back colors - for user controls
    private static readonly Color BACKGROUND_LIGHT = Color.FromArgb(255, 255, 255, 255);
    private static readonly Brush BACKGROUND_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
    private static readonly Color BACKGROUND_DARK = Color.FromArgb(255, 80, 80, 80);
    private static readonly Brush BACKGROUND_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);
    private static readonly Color BACKGROUND_ALTERNATIVE_LIGHT = Color.FromArgb(10, 0, 0, 0);
    private static readonly Brush BACKGROUND_ALTERNATIVE_LIGHT_BRUSH = new SolidBrush(BACKGROUND_ALTERNATIVE_LIGHT);
    private static readonly Color BACKGROUND_ALTERNATIVE_DARK = Color.FromArgb(10, 255, 255, 255);
    private static readonly Brush BACKGROUND_ALTERNATIVE_DARK_BRUSH = new SolidBrush(BACKGROUND_ALTERNATIVE_DARK);
    private static readonly Color BACKGROUND_HOVER_LIGHT = Color.FromArgb(20, 0, 0, 0);
    private static readonly Brush BACKGROUND_HOVER_LIGHT_BRUSH = new SolidBrush(BACKGROUND_HOVER_LIGHT);
    private static readonly Color BACKGROUND_HOVER_DARK = Color.FromArgb(20, 255, 255, 255);
    private static readonly Brush BACKGROUND_HOVER_DARK_BRUSH = new SolidBrush(BACKGROUND_HOVER_DARK);
    private static readonly Color BACKGROUND_HOVER_RED = Color.FromArgb(255, 255, 0, 0);
    private static readonly Brush BACKGROUND_HOVER_RED_BRUSH = new SolidBrush(BACKGROUND_HOVER_RED);
    private static readonly Color BACKGROUND_DOWN_RED = Color.FromArgb(255, 255, 84, 54);
    private static readonly Brush BACKGROUND_DOWN_RED_BRUSH = new SolidBrush(BACKGROUND_DOWN_RED);
    private static readonly Color BACKGROUND_FOCUS_LIGHT = Color.FromArgb(30, 0, 0, 0);
    private static readonly Brush BACKGROUND_FOCUS_LIGHT_BRUSH = new SolidBrush(BACKGROUND_FOCUS_LIGHT);
    private static readonly Color BACKGROUND_FOCUS_DARK = Color.FromArgb(30, 255, 255, 255);
    private static readonly Brush BACKGROUND_FOCUS_DARK_BRUSH = new SolidBrush(BACKGROUND_FOCUS_DARK);
    private static readonly Color BACKGROUND_DISABLED_LIGHT = Color.FromArgb(25, 0, 0, 0);
    private static readonly Brush BACKGROUND_DISABLED_LIGHT_BRUSH = new SolidBrush(BACKGROUND_DISABLED_LIGHT);
    private static readonly Color BACKGROUND_DISABLED_DARK = Color.FromArgb(25, 255, 255, 255);
    private static readonly Brush BACKGROUND_DISABLED_DARK_BRUSH = new SolidBrush(BACKGROUND_DISABLED_DARK);

    //Expansion Panel colors
    private static readonly Color EXPANSIONPANEL_FOCUS_LIGHT = Color.FromArgb(255, 242, 242, 242);
    private static readonly Brush EXPANSIONPANEL_FOCUS_LIGHT_BRUSH = new SolidBrush(EXPANSIONPANEL_FOCUS_LIGHT);
    private static readonly Color EXPANSIONPANEL_FOCUS_DARK = Color.FromArgb(255, 50, 50, 50);
    private static readonly Brush EXPANSIONPANEL_FOCUS_DARK_BRUSH = new SolidBrush(EXPANSIONPANEL_FOCUS_DARK);

    // Backdrop colors - for containers, like forms or panels
    private static readonly Color BACKDROP_LIGHT = Color.FromArgb(255, 242, 242, 242);
    private static readonly Brush BACKDROP_LIGHT_BRUSH = new SolidBrush(BACKGROUND_LIGHT);
    private static readonly Color BACKDROP_DARK = Color.FromArgb(255, 50, 50, 50);
    private static readonly Brush BACKDROP_DARK_BRUSH = new SolidBrush(BACKGROUND_DARK);

    //Other colors
    private static readonly Color CARD_BLACK = Color.FromArgb(255, 42, 42, 42);
    private static readonly Color CARD_WHITE = Color.White;
#pragma warning restore IDE1006 // Naming Styles

    // Getters - Using these makes handling the dark theme switching easier
    // Text
    public virtual Color TextHighEmphasisColor => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK : TEXT_HIGH_EMPHASIS_LIGHT;
    public virtual Brush TextHighEmphasisBrush => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK_BRUSH : TEXT_HIGH_EMPHASIS_LIGHT_BRUSH;
    public virtual Color TextHighEmphasisNoAlphaColor => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK_NOALPHA : TEXT_HIGH_EMPHASIS_LIGHT_NOALPHA;
    public virtual Brush TextHighEmphasisNoAlphaBrush => Theme == Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK_NOALPHA_BRUSH : TEXT_HIGH_EMPHASIS_LIGHT_NOALPHA_BRUSH;
    public virtual Color TextMediumEmphasisColor => Theme == Themes.LIGHT ? TEXT_MEDIUM_EMPHASIS_DARK : TEXT_MEDIUM_EMPHASIS_LIGHT;
    public virtual Brush TextMediumEmphasisBrush => Theme == Themes.LIGHT ? TEXT_MEDIUM_EMPHASIS_DARK_BRUSH : TEXT_MEDIUM_EMPHASIS_LIGHT_BRUSH;
    public virtual Color TextDisabledOrHintColor => Theme == Themes.LIGHT ? TEXT_DISABLED_OR_HINT_DARK : TEXT_DISABLED_OR_HINT_LIGHT;
    public virtual Brush TextDisabledOrHintBrush => Theme == Themes.LIGHT ? TEXT_DISABLED_OR_HINT_DARK_BRUSH : TEXT_DISABLED_OR_HINT_LIGHT_BRUSH;

    // Divider
    public virtual Color DividersColor => Theme == Themes.LIGHT ? DIVIDERS_DARK : DIVIDERS_LIGHT;
    public virtual Brush DividersBrush => Theme == Themes.LIGHT ? DIVIDERS_DARK_BRUSH : DIVIDERS_LIGHT_BRUSH;
    public virtual Color DividersAlternativeColor => Theme == Themes.LIGHT ? DIVIDERS_ALTERNATIVE_DARK : DIVIDERS_ALTERNATIVE_LIGHT;
    public virtual Brush DividersAlternativeBrush => Theme == Themes.LIGHT ? DIVIDERS_ALTERNATIVE_DARK_BRUSH : DIVIDERS_ALTERNATIVE_LIGHT_BRUSH;

    // Checkbox / Radio / Switch
    public virtual Color CheckboxOffColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT : CHECKBOX_OFF_DARK;
    public virtual Brush CheckboxOffBrush => Theme == Themes.LIGHT ? CHECKBOX_OFF_LIGHT_BRUSH : CHECKBOX_OFF_DARK_BRUSH;
    public virtual Color CheckBoxOffDisabledColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT : CHECKBOX_OFF_DISABLED_DARK;
    public virtual Brush CheckBoxOffDisabledBrush => Theme == Themes.LIGHT ? CHECKBOX_OFF_DISABLED_LIGHT_BRUSH : CHECKBOX_OFF_DISABLED_DARK_BRUSH;

    // Switch
    public virtual Color SwitchOffColor => Theme == Themes.LIGHT ? CHECKBOX_OFF_DARK : CHECKBOX_OFF_LIGHT; // yes, I re-use the checkbox color, sue me
    public virtual Color SwitchOffThumbColor => Theme == Themes.LIGHT ? SWITCH_OFF_THUMB_LIGHT : SWITCH_OFF_THUMB_DARK;
    public virtual Color SwitchOffTrackColor => Theme == Themes.LIGHT ? SWITCH_OFF_TRACK_LIGHT : SWITCH_OFF_TRACK_DARK;
    public virtual Color SwitchOffDisabledThumbColor => Theme == Themes.LIGHT ? SWITCH_OFF_DISABLED_THUMB_LIGHT : SWITCH_OFF_DISABLED_THUMB_DARK;

    // Control Back colors
    public virtual Color BackgroundColor => Theme == Themes.LIGHT ? BACKGROUND_LIGHT : BACKGROUND_DARK;
    public virtual Brush BackgroundBrush => Theme == Themes.LIGHT ? BACKGROUND_LIGHT_BRUSH : BACKGROUND_DARK_BRUSH;
    public virtual Color BackgroundAlternativeColor => Theme == Themes.LIGHT ? BACKGROUND_ALTERNATIVE_LIGHT : BACKGROUND_ALTERNATIVE_DARK;
    public virtual Brush BackgroundAlternativeBrush => Theme == Themes.LIGHT ? BACKGROUND_ALTERNATIVE_LIGHT_BRUSH : BACKGROUND_ALTERNATIVE_DARK_BRUSH;
    public virtual Color BackgroundDisabledColor => Theme == Themes.LIGHT ? BACKGROUND_DISABLED_LIGHT : BACKGROUND_DISABLED_DARK;
    public virtual Brush BackgroundDisabledBrush => Theme == Themes.LIGHT ? BACKGROUND_DISABLED_LIGHT_BRUSH : BACKGROUND_DISABLED_DARK_BRUSH;
    public virtual Color BackgroundHoverColor => Theme == Themes.LIGHT ? BACKGROUND_HOVER_LIGHT : BACKGROUND_HOVER_DARK;
    public virtual Brush BackgroundHoverBrush => Theme == Themes.LIGHT ? BACKGROUND_HOVER_LIGHT_BRUSH : BACKGROUND_HOVER_DARK_BRUSH;
    public virtual Color BackgroundHoverRedColor => Theme == Themes.LIGHT ? BACKGROUND_HOVER_RED : BACKGROUND_HOVER_RED;
    public virtual Brush BackgroundHoverRedBrush => Theme == Themes.LIGHT ? BACKGROUND_HOVER_RED_BRUSH : BACKGROUND_HOVER_RED_BRUSH;
    public virtual Brush BackgroundDownRedBrush => Theme == Themes.LIGHT ? BACKGROUND_DOWN_RED_BRUSH : BACKGROUND_DOWN_RED_BRUSH;
    public virtual Color BackgroundFocusColor => Theme == Themes.LIGHT ? BACKGROUND_FOCUS_LIGHT : BACKGROUND_FOCUS_DARK;
    public virtual Brush BackgroundFocusBrush => Theme == Themes.LIGHT ? BACKGROUND_FOCUS_LIGHT_BRUSH : BACKGROUND_FOCUS_DARK_BRUSH;

    // Other color
    public virtual Color CardsColor => Theme == Themes.LIGHT ? CARD_WHITE : CARD_BLACK;

    // Expansion Panel color/brush
    public virtual Brush ExpansionPanelFocusBrush => Theme == Themes.LIGHT ? EXPANSIONPANEL_FOCUS_LIGHT_BRUSH : EXPANSIONPANEL_FOCUS_DARK_BRUSH;

    // SnackBar
    public virtual Color SnackBarTextHighEmphasisColor => Theme != Themes.LIGHT ? TEXT_HIGH_EMPHASIS_DARK : TEXT_HIGH_EMPHASIS_LIGHT;
    public virtual Color SnackBarBackgroundColor => Theme != Themes.LIGHT ? BACKGROUND_LIGHT : BACKGROUND_DARK;
    public virtual Color SnackBarTextButtonNoAccentTextColor => Theme != Themes.LIGHT ? ColorScheme.PrimaryColor : ColorScheme.LightPrimaryColor;

    // Backdrop color
    public virtual Color BackdropColor => Theme == Themes.LIGHT ? BACKDROP_LIGHT : BACKDROP_DARK;
    public virtual Brush BackdropBrush => Theme == Themes.LIGHT ? BACKDROP_LIGHT_BRUSH : BACKDROP_DARK_BRUSH;

    public virtual Font GetFontByType(FontType type) => type switch
    {
        FontType.H1 => new Font(_robotoFontFamilies["Roboto_Light"], 96f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.H2 => new Font(_robotoFontFamilies["Roboto_Light"], 60f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.H3 => new Font(_robotoFontFamilies["Roboto"], 48f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.H4 => new Font(_robotoFontFamilies["Roboto"], 34f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.H5 => new Font(_robotoFontFamilies["Roboto"], 24f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.H6 => new Font(_robotoFontFamilies["Roboto_Medium"], 20f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.Subtitle1 => new Font(_robotoFontFamilies["Roboto"], 16f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.Subtitle2 => new Font(_robotoFontFamilies["Roboto_Medium"], 14f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.SubtleEmphasis => new Font(_robotoFontFamilies["Roboto"], 12f, FontStyle.Italic, GraphicsUnit.Pixel),
        FontType.Body1 => new Font(_robotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.Body2 => new Font(_robotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.Button => new Font(_robotoFontFamilies["Roboto"], 14f, FontStyle.Bold, GraphicsUnit.Pixel),
        FontType.Caption => new Font(_robotoFontFamilies["Roboto"], 12f, FontStyle.Regular, GraphicsUnit.Pixel),
        FontType.Overline => new Font(_robotoFontFamilies["Roboto"], 10f, FontStyle.Regular, GraphicsUnit.Pixel),
        _ => new Font(_robotoFontFamilies["Roboto"], 14f, FontStyle.Regular, GraphicsUnit.Pixel),
    };

    /// <summary>
    /// Get the font by size - used for textbox label animation, try to not use this for anything else
    /// </summary>
    /// <param name="size">font size, ranges from 12 up to 16</param>
    /// <returns></returns>
    public virtual nint GetTextBoxFontBySize(int size)
    {
        var name = "textBox" + Math.Min(16, Math.Max(12, size)).ToString();
        return _logicalFonts[name];
    }

    /// <summary>
    /// Gets a Material Skin Logical Roboto Font given a standard material font type
    /// </summary>
    /// <param name="type">material design font type</param>
    /// <returns></returns>
    public virtual nint GetLogFontByType(FontType type) => _logicalFonts[Enum.GetName(type)!];

    protected void AddFont(byte[] fontdata)
    {
        // Add font to system table in memory
        var dataLength = fontdata.Length;

        var ptrFont = Marshal.AllocCoTaskMem(dataLength);
        Marshal.Copy(fontdata, 0, ptrFont, dataLength);

        // GDI Font
        uint num = 0;
        Functions.AddFontMemResourceEx(ptrFont, dataLength, 0, num);

        // GDI+ Font
        _privateFontCollection.AddMemoryFont(ptrFont, dataLength);
    }

    private static nint AreateLogicalFont(string fontName, int size, LogFontWeight weight, byte lfItalic = 0)
    {
        var lfont = new LOGFONTW
        {
            lfFaceName = fontName,
            lfHeight = -size,
            lfWeight = (int)weight,
            lfItalic = lfItalic
        };
        return Functions.CreateFontIndirectW(lfont);
    }

    // Dyanmic Themes
    public virtual void AddFormToManage(MaterialForm materialForm)
    {
        _formsToManage.Add(materialForm);
        UpdateBackgrounds();

        // Set background on newly added controls
        materialForm.ControlAdded += (sender, e) =>
        {
            UpdateControlBackColor(e.Control, BackdropColor);
        };
    }

    public virtual void RemoveFormToManage(MaterialForm materialForm) => _formsToManage.Remove(materialForm);
    private void UpdateBackgrounds()
    {
        var newBackColor = BackdropColor;
        foreach (var materialForm in _formsToManage)
        {
            materialForm.BackColor = newBackColor;
            UpdateControlBackColor(materialForm, newBackColor);
        }
    }

    private void UpdateControlBackColor(Control? controlToUpdate, Color newBackColor)
    {
        if (controlToUpdate == null)
            return;

        // Control's Context menu
        if (controlToUpdate.ContextMenuStrip != null)
        {
            UpdateToolStrip(controlToUpdate.ContextMenuStrip, newBackColor);
        }

        // Material Tabcontrol pages
        if (controlToUpdate is TabPage page)
        {
            page.BackColor = newBackColor;
        }

        // Material Divider
        else if (controlToUpdate is MaterialDivider)
        {
            controlToUpdate.BackColor = DividersColor;
        }

        // Other Material Skin control
        else if (controlToUpdate is IMaterialControl)
        {
            controlToUpdate.BackColor = newBackColor;
            controlToUpdate.ForeColor = TextHighEmphasisColor;
        }

        // Other Generic control not part of material skin
        else if (EnforceBackcolorOnAllComponents && controlToUpdate is not IMaterialControl && controlToUpdate.Parent != null)
        {
            controlToUpdate.BackColor = controlToUpdate.Parent.BackColor;
            controlToUpdate.ForeColor = TextHighEmphasisColor;
            controlToUpdate.Font = GetFontByType(FontType.Body1);
        }

        // Recursive call to control's children
        foreach (Control control in controlToUpdate.Controls)
        {
            UpdateControlBackColor(control, newBackColor);
        }
    }

    private static void UpdateToolStrip(ToolStrip? toolStrip, Color newBackColor)
    {
        if (toolStrip == null)
            return;

        toolStrip.BackColor = newBackColor;
        foreach (ToolStripItem control in toolStrip.Items)
        {
            control.BackColor = newBackColor;
            if (control is MaterialToolStripMenuItem item && item.HasDropDown)
            {
                //recursive call
                UpdateToolStrip(item.DropDown, newBackColor);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            foreach (var handle in _logicalFonts.Values)
            {
                Functions.DeleteObject(handle);
            }
            _disposedValue = true;
        }
    }

    ~MaterialSkinManager() { Dispose(disposing: false); }
    public void Dispose() { Dispose(disposing: true); GC.SuppressFinalize(this); }
}
