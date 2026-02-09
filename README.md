# MaterialSkin 2 for .NET WinForms 10+, AOT publishing compatible

Theming .NET WinForms to Google's Material Design principles.

This project was initially developed here from multiple forks https://github.com/leocb/MaterialSkin.

From there, I have done the following:

* ported the project to .NET Core 10+
* removed the MaterialSkinCore project, as it's not needed anymore
* removed .NET Framework projects (use previous forks if you need them)
* added AOT publishing support (changed interop code, remove some dependencies on resx, etc.)
* cleaned up C# code so it's more aligned with "modern" C#
  
  * consolidate `using` usage in csproj
  * enable `<nullable>`
  * enforce usual naming convention
  * enforce 1 public type => 1 file
  * removed all warnings
  * etc.

Note: The project is not Hi-DPI aware, as this require some amount of work since many values are currently hardcoded.

<img width="1029" height="622" src="https://github.com/user-attachments/assets/4150c2f4-02c4-4b71-a0ed-97112a294fdc" />
<br/><br/>
<img width="1029" height="622" src="https://github.com/user-attachments/assets/d1d915d4-263f-42c6-b482-52c344e4ff1f" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/80b8f2db-30e0-4eee-a122-3efa1bf8b314" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/6da6645a-50c7-46cb-9895-a2cb9646c94d" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/da098698-bdcf-4f8c-96bf-1b48bb04f05d" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/0404ac32-c0da-41e0-8c81-56712afb7d72" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/44cb42c8-f907-4be1-b1bc-1d8eccdadd65" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/4fac51b6-f784-474f-b2a6-9b5f45c66220" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/5e400f2c-754d-4435-95f6-ce7636e0d2ea" />
<br/><br/>
<img width="1029" height="558" src="https://github.com/user-attachments/assets/a44e5c6c-5a5f-43cd-95b3-993bd3aac16a" />
