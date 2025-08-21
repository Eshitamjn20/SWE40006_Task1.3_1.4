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
## ▶️ How to Build and Run


And here’s the **How to Build & Run** with the **correct project names**:


## ▶️ How to Build and Run

### 🔹 Task 1.3 – DLL-based Calculator with MSI & MSIX

#### 1) Build the Calculator with DLLs
1. Open the solution in **Visual Studio 2022**.
2. Right‑click **`CalculatorApp`** → **Set as Startup Project**.
3. Select **Release** configuration.
4. **Build → Build Solution** (`Ctrl+Shift+B`).
5. Confirm these files in `CalculatorApp/bin/Release/`:
   - `CalculatorApp.exe`
   - `AdvancedMath.dll`
   - `StatsLibrary.dll`
6. Run `CalculatorApp.exe` and verify:
   - Scientific operations (powers, logs, trig)
   - Statistical ops (mean, median, stdev)

---

#### 2) Create & Install the MSI (WiX)
1. Right‑click **`CalculatorWixSetup`** → **Build**.
2. Open `CalculatorWixSetup/bin/Release/` → run **`CalculatorWixSetup.msi`**.
3. Finish the wizard (defaults to **`C:\Program Files (x86)\MathMate Calculator`**).
4. Launch from **Start Menu** (search **MathMate**/**Calculator**).

**Notes**
- In `Product.wxs`, each `<Component>` (exe + both DLLs) must have a **unique GUID**.
- Generate via **Tools → Create GUID** (no duplicates).

---

#### 3) Create & Install the MSIX (UWP Sideloading)
1. Right‑click **`AppCalculatorPackage`** → **Publish → Create App Packages**.
2. Choose **Sideloading**.
3. After packaging, open the generated folder (similar to):AppCalculatorPackage\AppPackages\AppCalculatorPackage_1.0.0.0_Test\

4. You’ll see a **`.msixbundle`** and a **`.cer`** (or `.pfx`) certificate.

4. **Install the certificate**
- Double‑click the **`.cer`** → **Install Certificate**.
- **Local Machine** → **Trusted Root Certification Authorities**.

5. **Install the package**
- Double‑click the **`.msixbundle`** → **App Installer** → **Install**.

6. Launch from **Start Menu** (search **AppCalculatorPackage / MathMate**).

---

### 🔹 Task 1.4 – Microsoft Store Publication

#### 1) Associate Project with Microsoft Store
1. Reserve app name **MathMate** in **Partner Center**.  
2. In Visual Studio: right‑click **`CalculatorPackage`** → **Publish → Associate App with Store**.  
3. Select the reserved app to link the project with the store entry.

---

#### 2) Create Store‑ready Package
1. Right‑click **`CalculatorPackage`** → **Publish → Create App Packages**.  
2. Choose **Microsoft Store** distribution.  
3. Ensure **Identity / Version / Publisher** match the store association.  
4. Visual Studio generates a **`.msixupload`** for submission.

---

#### 3) Validate with Windows App Certification Kit
1. Run **App Certification Kit** after build (prompted automatically).  
2. If errors (e.g., placeholder logos), fix and rebuild:
- Open **`Package.appxmanifest`** → **Visual Assets** tab.  
- Replace default logos with branded assets (e.g., 1024×1024 **MathMate** logo).  
- Rebuild package and re‑run validation.

---

#### 4) Submit to Microsoft Store
1. Upload **`.msixupload`** and metadata (description, screenshots, keywords).  
2. Complete **Age Rating** questionnaire.  
3. Set **Pricing & Availability** (MathMate → **Free**, **Global**).  
4. Submit for review.  
5. After approval, **MathMate** is published to the Microsoft Store.

---

## ❌ Common Issues & Fixes

- **Unresolved types/namespaces**  
*Cause:* DLLs referenced incorrectly (circular or missing references).  
*Fix:* Remove wrong references; re‑add **AdvancedMath** and **StatsLibrary** to **CalculatorApp**.

- **Shadowed variables in StatsLibrary (CS0136)**  
*Cause:* Variable reuse in LINQ (e.g., Median).  
*Fix:* Rename inner variables to resolve scope conflicts.

- **Designer/API mismatch (CS0117)**  
*Cause:* `TextBox.PlaceholderText` not available in .NET 4.7.2.  
*Fix:* Use a focus event + default text to simulate placeholder.

- **Duplicate GUIDs in WiX**  
*Fix:* Generate a **unique GUID per `<Component>`** (Tools → Create GUID).

- **Store submission validation errors**  
*Cause:* Placeholder logos or missing assets.  
*Fix:* Replace with branded icons; regenerate assets; rebuild and re‑validate.

---

## ✅ Outcomes

**Task 1.3**
- DLL‑based calculator packaged as **MSI (WiX)** and **MSIX (sideload)**.  
- Verified advanced (scientific) and statistical functions.

**Task 1.4**
- Calculator rebranded as **MathMate**.  
- Store‑associated package created and **passed App Certification Kit**.  
- **Published** on the Microsoft Store.

**MathMate on Store:** ([Math Mate Link ](https://apps.microsoft.com/detail/9NMF37GP3W15?hl=en-us&gl=AU&ocid=pdpshare)

---

## 🔗 Useful Links
- WiX Toolset v3.11.2 (Release): https://github.com/wixtoolset/wix3/releases/tag/wix3112rtm  
- WiX v3 Extension (VS2022): https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2022Extension  
- Microsoft Partner Center: https://partner.microsoft.com/
- Math Mate: https://apps.microsoft.com/detail/9NMF37GP3W15?hl=en-us&gl=AU&ocid=pdpshare

---

## 👩‍💻 Author
**Eshita Mahajan (104748964)**  
SWE40006 – Software Deployment and Evolution (Semester II, 2025)
