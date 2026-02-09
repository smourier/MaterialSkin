# MaterialSkin 2 for .NET WinForms 10+, AOT publishing compatible

Theming .NET WinForms to Google's Material Design Principles.

This project was initially developed here from multiple forks https://github.com/leocb/MaterialSkin.

I have done the following:

* ported the project to .NET Core 10+
* removed .NET Framework projects (use previous forks if you need it)
* added AOT publishing support (changed interop code, remove some dependencies on resx, etc.)
* cleaned up C# code so it's more aligned with "modern" C#
  
  * consolidate `using` usage in csproj
  * enable `<nullable>`
  * enforce usual naming convention
  * enforce 1 public type => 1 file
  * removed all warnings
  * etc.
