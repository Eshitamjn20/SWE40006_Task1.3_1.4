# MathMate Calculator Deployment – Task 1.3 & Task 1.4  

This repository contains the solution for:  
- **Task 1.3**: Packaging a Calculator application with DLL dependencies and creating both MSI and MSIX (sideloading) installers.  
- **Task 1.4**: Extending the Calculator (MathMate) for **Microsoft Store publication**, including certificate signing, validation, and store submission.  

---

## 📌 Environment Setup  

Before starting, ensure the following environment is configured:  

- **Visual Studio Community 2022**  
  - Installed with **.NET Desktop Development** workload.  
  - Added **.NET Framework SDKs/Targeting Packs** (4.7.2, 4.8, 4.8.1).  
  - Enabled **MSIX Packaging Tools**.  

- **WiX Toolset v3.11.2**  
  - Installed from [GitHub Release](https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm).  
  - Added **WiX v3 Extension** for Visual Studio 2022.  
  - Verified installation by running `candle.exe` in Command Prompt.  

- **Microsoft Partner Center Account** (Task 1.4)  
  - Required to reserve app name (MathMate) and submit the app to the Microsoft Store.  

---

## 📂 Repository Structure

The solution contains five projects: the main calculator, two DLL libraries, a WiX MSI setup, and a UWP/MSIX packaging project.

```text
CalculatorApp.sln
│
├── AdvancedMath/                       # DLL project (advanced math ops)
│   ├── Properties/
│   ├── References/
│   └── Class1.cs
│
├── StatsLibrary/                       # DLL project (statistics ops)
│   ├── Properties/
│   ├── References/
│   └── Class1.cs
│
├── CalculatorApp/                      # WinForms calculator (uses both DLLs)
│   ├── Properties/
│   ├── References/
│   ├── App.config
│   ├── Form1.cs
│   ├── Form1.Designer.cs
│   └── Program.cs
│
├── CalculatorWixSetup/                 # WiX v3 setup project (MSI)
│   └── (Product.wxs, *.wixproj, bin/Release/CalculatorWixSetup.msi)
│
└── AppCalculatorPackage/               # Windows Application Packaging (MSIX)
    ├── Images/                         # Visual assets / logos
    ├── AppCalculatorPackage_TemporaryKey.pfx
    ├── Package.appxmanifest
    └── Package.StoreAssociation.xml    # (present if store-associated)
```
# ▶️ How to Build and Run

## 🔹 Task 1.3 – DLL-based Calculator with MSI & MSIX

### 1) Build the Calculator with DLLs
1. Open the solution in **Visual Studio 2022**.  
2. Right-click **CalculatorApp → Set as Startup Project**.  
3. Select **Release** configuration.  
4. Build → **Build Solution** (`Ctrl+Shift+B`).  
5. Confirm these files exist in `CalculatorApp/bin/Release/`:
   - `CalculatorApp.exe`
   - `AdvancedMath.dll`
   - `StatsLibrary.dll`

Run `CalculatorApp.exe` and verify:
- **Scientific ops:** powers, logs, trig, factorials.  
- **Statistical ops:** mean, median, standard deviation.

---

### 2) Package as MSI (WiX)
1. Right-click **CalculatorWixSetup → Build**.  
2. Go to `CalculatorWixSetup/bin/Release/` and run `CalculatorWixSetup.msi`.  
3. Complete the wizard (defaults to `C:\Program Files (x86)\MathMate Calculator`).  
4. Launch from **Start Menu** (search *MathMate* or *Calculator*).

**Notes:**  
- In `Product.wxs`, each `<Component>` (EXE + both DLLs) must have a unique **GUID**.  
- Generate GUIDs: **Tools → Create GUID**. No duplicates.  
- Use `<MediaTemplate EmbedCab="yes" />` so DLLs and EXE are bundled in the MSI.

---

### 3) Package as MSIX (UWP Sideloading)
1. Right-click **AppCalculatorPackage → Publish → Create App Packages**.  
2. Select **Sideloading**.  
3. After packaging, locate the output folder (e.g.):  AppCalculatorPackage/AppPackages/AppCalculatorPackage_1.0.0.0_Test/  by right click AppCalculatorPackage in Solution explorer -> Open in File explorer -> AppPackages -> AppCalculatorPackage_1.0.0.0_Test

Inside: `.msixbundle` + `.cer` certificate.

**Install certificate:**
- Double-click `.cer` → *Install Certificate*.  
- Choose **Local Machine** → place in **Trusted Root Certification Authorities**.

**Install package:**
- Double-click `.msixbundle` → App Installer → *Install*.  
- Launch from **Start Menu** (search *MathMate*).

---

### 🧩 DLL Components (Overview)
- **AdvancedMath.dll** – powers, logs, trig, factorials (validates inputs like factorial on non-negative integers).  
- **StatsLibrary.dll** – mean, median, stdev (safe handling of empty inputs).  

Both DLLs are compiled automatically into the output folder during **Build Solution**.  
No manual linking is required — cloning the repo and building will recreate them.

---

## 🔹 Task 1.4 – Microsoft Store Publication

### 1) Associate Project with Microsoft Store
- Reserve app name **MathMate** in [Partner Center](https://partner.microsoft.com/).  
- In Visual Studio: right-click **AppCalculatorPackage → Publish → Associate App with Store**.  
- Select the reserved app → links project with store entry.

### 2) Create Store-ready Package
- Right-click **AppCalculatorPackage → Publish → Create App Packages**.  
- Choose **Microsoft Store distribution**.  
- Ensure *Identity / Version / Publisher* match store association.  
- Visual Studio generates `.msixupload` for submission.

### 3) Validate with Windows App Certification Kit
- After build, run **App Certification Kit**.  
- Fix issues like placeholder logos:  
- Open **Package.appxmanifest → Visual Assets**.  
- Replace defaults with branded icons (e.g., 1024×1024 MathMate logo).  
- Rebuild and re-validate.

### 4) Submit to Microsoft Store
- Upload `.msixupload` and metadata (description, screenshots, keywords).  
- Complete **Age Rating**.  
- Set **Pricing & Availability** (MathMate → Free, Global).  
- Submit for review.  
- After approval → MathMate live on **Microsoft Store**.

---

## ❌ Common Issues & Fixes
- **Unresolved namespaces:** wrong DLL refs → re-add AdvancedMath + StatsLibrary.  
- **Shadowed variables:** fix LINQ scope conflicts in `StatsLibrary`.  
- **Designer mismatch:** `PlaceholderText` not in .NET 4.7.2 → simulate with focus event.  
- **Duplicate GUIDs in WiX:** regenerate per `<Component>`.  
- **Store validation errors:** replace placeholder logos, rebuild.  

---

## ✅ Outcomes
- **Task 1.3:** Calculator packaged as MSI (WiX) + MSIX (sideload). Verified DLL-based functions.  
- **Task 1.4:** Rebranded as **MathMate**, Store-associated, passed validation, published on Microsoft Store.  

📌 [MathMate on Store](https://apps.microsoft.com/detail/9NMF37GP3W15?hl=en-us&gl=AU&ocid=pdpshare)

---

## 🔗 Useful Links
- [WiX Toolset v3.11.2](https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm)  
- [WiX Extension (VS2022)](https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2022Extension)  
- [Microsoft Partner Center](https://partner.microsoft.com/)  

---

👩‍💻 **Author:**  
Eshita Mahajan (104748964)  
SWE40006 – Software Deployment and Evolution (Semester II, 2025)
