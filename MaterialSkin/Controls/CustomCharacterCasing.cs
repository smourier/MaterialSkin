namespace MaterialSkin.Controls;

public enum CustomCharacterCasing
{
    [Description("Text will be used as user inserted, no alteration")]
    Normal,

    [Description("Text will be converted to UPPER case")]
    Upper,

    [Description("Text will be converted to lower case")]
    Lower,

    [Description("Text will be converted to Proper case (aka Title case)")]
    Proper
}
