namespace MaterialSkinExample;

public partial class MainForm : MaterialForm
{
    private readonly MaterialSkinManager _materialSkinManager;
    private int _colorSchemeIndex;

    public MainForm()
    {
        InitializeComponent();

        menuIconList.TransparentColor = Color.Transparent;
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_assessment_white_24dp.png")!);
        menuIconList.Images.SetKeyName(0, "round_assessment_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_backup_white_24dp.png")!);
        menuIconList.Images.SetKeyName(1, "round_backup_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_bluetooth_white_24dp.png")!);
        menuIconList.Images.SetKeyName(2, "round_bluetooth_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_bookmark_white_24dp.png")!);
        menuIconList.Images.SetKeyName(3, "round_bookmark_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_build_white_24dp.png")!);
        menuIconList.Images.SetKeyName(4, "round_build_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_gps_fixed_white_24dp.png")!);
        menuIconList.Images.SetKeyName(5, "round_gps_fixed_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_http_white_24dp.png")!);
        menuIconList.Images.SetKeyName(6, "round_http_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_report_problem_white_24dp.png")!);
        menuIconList.Images.SetKeyName(7, "round_report_problem_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_swap_vert_white_24dp.png")!);
        menuIconList.Images.SetKeyName(8, "round_swap_vert_white_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_phone_black_24dp.png")!);
        menuIconList.Images.SetKeyName(9, "round_phone_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_push_pin_black_24dp.png")!);
        menuIconList.Images.SetKeyName(10, "round_push_pin_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_mail_outline_black_24dp.png")!);
        menuIconList.Images.SetKeyName(11, "round_mail_outline_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_person_black_24dp.png")!);
        menuIconList.Images.SetKeyName(12, "round_person_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_add_a_photo_black_24dp.png")!);
        menuIconList.Images.SetKeyName(13, "round_add_a_photo_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_alternate_email_black_24dp.png")!);
        menuIconList.Images.SetKeyName(14, "round_alternate_email_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_cancel_black_24dp.png")!);
        menuIconList.Images.SetKeyName(15, "round_cancel_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_error_black_24dp.png")!);
        menuIconList.Images.SetKeyName(16, "round_error_black_24dp.png");
        menuIconList.Images.Add(MaterialSinkExtensions.GetBitmapFromResource("round_event_black_24dp.png")!);
        menuIconList.Images.SetKeyName(17, "round_event_black_24dp.png");

        materialButton23.Icon = MaterialSinkExtensions.GetBitmapFromResource("baseline_fingerprint_black_24dp.png");
        materialFloatingActionButton1.Icon = MaterialSinkExtensions.GetBitmapFromResource("plus.png");
        materialButton24.Icon = MaterialSinkExtensions.GetBitmapFromResource("baseline_bluetooth_black_24dp.png");
        materialButton22.Icon = MaterialSinkExtensions.GetBitmapFromResource("baseline_favorite_border_black_24dp.png");
        materialMaskedTextBox1.LeadingIcon = MaterialSinkExtensions.GetBitmapFromResource("round_phone_black_24dp.png");
        materialTextBox2.LeadingIcon = MaterialSinkExtensions.GetBitmapFromResource("baseline_fingerprint_black_24dp.png");
        materialFlatButton2.Icon = MaterialSinkExtensions.GetBitmapFromResource("minus.png");
        MaterialButton2.Icon = MaterialSinkExtensions.GetBitmapFromResource("round_add_black_24dp.png");
        item1ToolStripMenuItem.Image = MaterialSinkExtensions.GetBitmapFromResource("minus.png");

        materialLabel8.Text = "Here is a list of every variant a Material Button can be. Contained button's shadows are only drawn at run-time.\r\nClick on them and checkout those sweet animations. Oh yeah, the buttons follow the theme and colors, try changing those too.\r\nNormally the buttons should be AutoSize = true, but for the sake of my OCD, it's set to false here\r\nIf any of the buttons looks weird while designing, change the tab background color from transparent to white.";

        // Initialize MaterialSkinManager
        _materialSkinManager = MaterialSkinManager.Instance;

        // Set this to false to disable backcolor enforcing on non-materialSkin components
        // This HAS to be set before the AddFormToManage()
        _materialSkinManager.EnforceBackcolorOnAllComponents = true;

        // MaterialSkinManager properties
        _materialSkinManager.AddFormToManage(this);
        _materialSkinManager.Theme = Themes.LIGHT;
        _materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);

        // Add dummy data to the listview
        SeedListView();
        materialCheckedListBox1.Items.Add("Item1", false);
        materialCheckedListBox1.Items.Add("Item2", true);
        materialCheckedListBox1.Items.Add("Item3", true);
        materialCheckedListBox1.Items.Add("Item4", false);
        materialCheckedListBox1.Items.Add("Item5", true);
        materialCheckedListBox1.Items.Add("Item6", false);
        materialCheckedListBox1.Items.Add("Item7", false);

        materialComboBox6.SelectedIndex = 0;

        materialListBoxFormStyle.Clear();
        foreach (var FormStyleItem in Enum.GetNames<FormStyles>())
        {
            materialListBoxFormStyle.AddItem(FormStyleItem);
            if (FormStyleItem == FormStyle.ToString())
            {
                materialListBoxFormStyle.SelectedIndex = materialListBoxFormStyle.Items.Count - 1;
            }
        }

        materialListBoxFormStyle.SelectedIndexChanged += (sender, args) =>
        {
            if (args.Text == null)
                return;

            var SelectedStyle = Enum.Parse<FormStyles>(args.Text);
            if (FormStyle != SelectedStyle)
            {
                FormStyle = SelectedStyle;
            }
        };

        materialMaskedTextBox1.ValidatingType = typeof(short);
    }

    private void SeedListView()
    {
        //Define
        var data = new[]
        {
            new []{"Lollipop", "392", "0.2", "0"},
            ["KitKat", "518", "26.0", "7"],
            ["Ice cream sandwich", "237", "9.0", "4.3"],
            ["Jelly Bean", "375", "0.0", "0.0"],
            ["Honeycomb", "408", "3.2", "6.5"]
        };

        //Add
        foreach (var version in data)
        {
            var item = new ListViewItem(version);
            materialListView1.Items.Add(item);
        }
    }

    private void MaterialButton7_Click(object sender, EventArgs e)
    {
        _materialSkinManager.Theme = _materialSkinManager.Theme == Themes.DARK ? Themes.LIGHT : Themes.DARK;
        UpdateColor();
    }

    private void MaterialButton4_Click(object sender, EventArgs e)
    {
        _colorSchemeIndex++;
        if (_colorSchemeIndex > 2)
            _colorSchemeIndex = 0;
        UpdateColor();
    }

    private void UpdateColor()
    {
        //These are just example color schemes
        switch (_colorSchemeIndex)
        {
            case 0:
                _materialSkinManager.ColorScheme = new ColorScheme(
                    _materialSkinManager.Theme == Themes.DARK ? Primary.Teal500 : Primary.Indigo500,
                    _materialSkinManager.Theme == Themes.DARK ? Primary.Teal700 : Primary.Indigo700,
                    _materialSkinManager.Theme == Themes.DARK ? Primary.Teal200 : Primary.Indigo100,
                    Accent.Pink200,
                    TextShade.WHITE);
                break;

            case 1:
                _materialSkinManager.ColorScheme = new ColorScheme(
                    Primary.Green600,
                    Primary.Green700,
                    Primary.Green200,
                    Accent.Red100,
                    TextShade.WHITE);
                break;

            case 2:
                _materialSkinManager.ColorScheme = new ColorScheme(
                    Primary.BlueGrey800,
                    Primary.BlueGrey900,
                    Primary.BlueGrey500,
                    Accent.LightBlue200,
                    TextShade.WHITE);
                break;
        }
        Invalidate();
    }

    private void MaterialButton2_Click(object sender, EventArgs e) => materialProgressBar1.Value = Math.Min(materialProgressBar1.Value + 10, 100);
    private void MaterialFlatButton4_Click(object sender, EventArgs e) => materialProgressBar1.Value = Math.Max(materialProgressBar1.Value - 10, 0);
    private void MaterialSwitch4_CheckedChanged(object sender, EventArgs e) => DrawerUseColors = materialSwitch4.Checked;
    private void MaterialSwitch5_CheckedChanged(object sender, EventArgs e) => DrawerHighlightWithAccent = materialSwitch5.Checked;
    private void MaterialSwitch6_CheckedChanged(object sender, EventArgs e) => DrawerBackgroundWithAccent = materialSwitch6.Checked;
    private void MaterialSwitch8_CheckedChanged(object sender, EventArgs e) => DrawerShowIconsWhenHidden = materialSwitch8.Checked;

    private void MaterialButton3_Click(object sender, EventArgs e)
    {
        var builder = new StringBuilder("Batch operation report:\n\n");
        var random = new Random();
        int result;
        for (var i = 0; i < 200; i++)
        {
            result = random.Next(1000);

            if (result < 950)
            {
                builder.AppendFormat(" - Task {0}: Operation completed sucessfully.\n", i);
            }
            else
            {
                builder.AppendFormat(" - Task {0}: Operation failed! A very very very very very very very very very very very very serious error has occured during this sub-operation. The errorcode is: {1}).\n", i, result);
            }
        }

        var batchOperationResults = "Simple text";
        MaterialMessageBox.Show(batchOperationResults, "Batch Operation", MessageBoxButtons.YesNoCancel, ButtonsPosition.Center);
        materialComboBox1.Items.Add("this is a very long string");
    }

    private void MaterialSwitch9_CheckedChanged(object sender, EventArgs e) => DrawerAutoShow = materialSwitch9.Checked;
    private void MaterialTextBox2_LeadingIconClick(object sender, EventArgs e) => new MaterialSnackBar("Leading Icon Click").Show(this);
    private void MaterialButton6_Click(object sender, EventArgs e) => new MaterialSnackBar("SnackBar started succesfully", "OK", true).Show(this);
    private void MaterialSwitch10_CheckedChanged(object sender, EventArgs e) => materialTextBox21.UseAccent = materialSwitch10.Checked;
    private void MaterialSwitch11_CheckedChanged(object sender, EventArgs e) => materialTextBox21.UseTallSize = materialSwitch11.Checked;
    private void MaterialSwitch12_CheckedChanged(object sender, EventArgs e)
    {
        if (materialSwitch12.Checked)
        {
            materialTextBox21.Hint = "Hint text";
        }
        else
        {
            materialTextBox21.Hint = string.Empty;
        }
    }

    private void MaterialComboBox7_SelectionChangeCommitted(object sender, EventArgs e)
    {
        if (materialComboBox7.SelectedIndex == 1)
        {
            materialTextBox21.PrefixSuffix = PrefixSuffixTypes.Prefix;
        }
        else if (materialComboBox7.SelectedIndex == 2)
        {
            materialTextBox21.PrefixSuffix = PrefixSuffixTypes.Suffix;
        }
        else
        {
            materialTextBox21.PrefixSuffix = PrefixSuffixTypes.None;
        }
    }

    private void MaterialSwitch13_CheckedChanged(object sender, EventArgs e) => materialTextBox21.UseSystemPasswordChar = materialSwitch13.Checked;
    private void MaterialSwitch14_CheckedChanged(object sender, EventArgs e)
    {
        if (materialSwitch14.Checked)
        {
            materialTextBox21.LeadingIcon = MaterialSinkExtensions.GetBitmapFromResource("baseline_fingerprint_black_24dp.png");
        }
        else
        {
            materialTextBox21.LeadingIcon = null;
        }
    }

    private void MaterialSwitch15_CheckedChanged(object sender, EventArgs e)
    {
        if (materialSwitch15.Checked)
        {
            materialTextBox21.TrailingIcon = MaterialSinkExtensions.GetBitmapFromResource("baseline_build_black_24dp.png");
        }
        else
        {
            materialTextBox21.TrailingIcon = null;
        }
    }

    private void MaterialTextBox21_LeadingIconClick(object sender, EventArgs e) => new MaterialSnackBar("Leading Icon Click").Show(this);
    private void MaterialTextBox21_TrailingIconClick(object sender, EventArgs e) => new MaterialSnackBar("Trailing Icon Click").Show(this);
    private void MaterialSwitch16_CheckedChanged(object sender, EventArgs e) => materialTextBox21.ShowAssistiveText = materialSwitch16.Checked;
    private void MsReadOnly_CheckedChanged(object sender, EventArgs e) => materialCheckbox1.ReadOnly = msReadOnly.Checked;
    private void MaterialButton25_Click(object sender, EventArgs e)
    {
        var materialDialog = new MaterialDialog(this, "Dialog Title", "Dialogs inform users about a task and can contain critical information, require decisions, or involve multiple tasks.", "OK", true, "Cancel");
        var result = materialDialog.ShowDialog(this);

        var SnackBarMessage = new MaterialSnackBar(result.ToString(), 750);
        SnackBarMessage.Show(this);
    }
}
