namespace MaterialSkin;

/// <summary>
/// Defines the <see cref="IMaterialControl" />
/// </summary>
public interface IMaterialControl
{
    /// <summary>
    /// Gets the SkinManager
    /// </summary>
    MaterialSkinManager SkinManager { get; }

    /// <summary>
    /// Gets or sets the MouseState
    /// </summary>
    MouseState MouseState { get; set; }
}
