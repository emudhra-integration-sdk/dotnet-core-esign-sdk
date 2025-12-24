<<<<<<< HEAD
# dotnet-core-esign-sdk
=======
# eSign Library Kit (.NET)

A comprehensive .NET SDK for digital signature operations, providing PDF signing capabilities with support for multiple authentication methods and signature appearance customization.

## Table of Contents
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Building from Source](#building-from-source)
- [Quick Start](#quick-start)
- [Integration Guide](#integration-guide)
- [API Documentation](#api-documentation)
- [License](#license)
- [Security Considerations](#security-considerations)
- [Contributing](#contributing)

---

## Features

### Core Capabilities
- **PDF Digital Signing**: Sign PDF documents with digital certificates (X.509)
- **Multi-factor Authentication Support**:
  - OTP (One-Time Password)
  - Fingerprint
  - IRIS recognition
  - Face recognition
- **Flexible Signature Appearance**:
  - Standard signatures
  - Image-based signatures
  - One-liner signatures
  - Advanced signatures with custom styling
  - Colored graphics signatures
  - Background image signatures
- **Document Processing**:
  - Page-level signing control (All, Even, Odd, First, Last, or specify pages)
  - Hash-based or full document signing
  - Document encryption/decryption
  - Content search functionality
- **Enterprise Features**:
  - Bank KYC integration
  - Proxy server support
  - Transaction management and verification
  - Cross-platform support (.NET Standard 2.0)

---

## Requirements

### Runtime Requirements
- **.NET Runtime**: .NET Standard 2.0 compatible runtime
  - .NET Core 2.0+
  - .NET Framework 4.6.1+
  - .NET 5.0+
  - .NET 6.0+
  - .NET 7.0+
  - .NET 8.0+
- **Operating System**: Windows, Linux, macOS

### Build Requirements
- **.NET SDK**: 6.0 or higher (for building)
- **IDE** (optional):
  - Visual Studio 2019/2022
  - Visual Studio Code with C# extension
  - JetBrains Rider

---

## Installation

### Option 1: Use Pre-built DLL
The easiest way to use this library is with the pre-built DLL file:

1. Download `eSignASPLibrary.dll` from the releases page
2. Add it as a reference to your project

**NuGet Users**: See [Integration Guide](#integration-guide) below

### Option 2: Build from Source
See [Building from Source](#building-from-source) section below

---

## Building from Source

### Using .NET CLI (Recommended)

1. **Clone or download this repository**:
   ```bash
   git clone <repository-url>
   cd NetStandard_eSignLibKit
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the project**:
   ```bash
   dotnet build --configuration Release
   ```

4. **Locate the built DLL**:
   The compiled DLL will be in `bin/Release/netstandard2.0/eSignASPLibrary.dll`

### Using Visual Studio

1. Open Visual Studio
2. File → Open → Project/Solution
3. Select `eSignASPLibrary.sln`
4. Right-click on the project → Build
5. The DLL will be generated in `bin/Release/netstandard2.0/` folder

### Build Output
- **DLL File**: `bin/Release/netstandard2.0/eSignASPLibrary.dll`
- **NuGet Package**: Run `dotnet pack` to generate .nupkg file

---

## Quick Start

### Basic PDF Signing Example

```csharp
using eSign;
using eSign.Library;

public class QuickStart
{
    public static void Main(string[] args)
    {
        try
        {
            // 1. Configure eSign settings
            var settings = new eSignSettings
            {
                ASPID = "YOUR_ASP_ID",
                Gateway_URL = "https://gateway.example.com",
                ResponseURL = "https://yourapp.com/callback"
            };

            // 2. Build user information
            var userInfo = new UserInfoBuilder()
                .SeteMail("user@example.com")
                .SetFirstname("John")
                .SetLastname("Doe")
                .SetPhoneNumber("9876543210")
                .Build();

            // 3. Build eSign input
            var input = new eSignInputBuilder()
                .SeteSignSettings(settings)
                .SetUserInfo(userInfo)
                .SetPDFBase64("BASE64_ENCODED_PDF_CONTENT")
                .SetCoordinates(Coordinates.BOTTOMRIGHT)
                .SetAuthMode(AuthMode.OTP)
                .SetAppearanceType(AppearanceType.STANDARD)
                .Build();

            // 4. Initialize eSign and generate gateway parameters
            var esign = new eSign.eSign();
            var result = esign.GenerateGatewayParameters(input);

            // 5. Handle the response
            if (result.ReturnCode == "1")
            {
                Console.WriteLine($"Success! Gateway URL: {result.GatewayURL}");
                Console.WriteLine($"Response URL: {result.ResponseURL}");
                // Redirect user to gateway URL for signing
            }
            else
            {
                Console.WriteLine($"Error: {result.ErrorMsg}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
```

---

## Integration Guide

### NuGet Package Manager

```bash
# Install from local package
dotnet add package eSignASPLibrary --source ./path/to/packages
```

Or add to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="eSignASPLibrary" Version="2.0.0.17" />
</ItemGroup>
```

### Direct DLL Reference

Add to your `.csproj` file:

```xml
<ItemGroup>
  <Reference Include="eSignASPLibrary">
    <HintPath>lib\eSignASPLibrary.dll</HintPath>
  </Reference>
</ItemGroup>
```

### ASP.NET Core Integration

```csharp
// Startup.cs or Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure eSign settings
    services.Configure<eSignSettings>(Configuration.GetSection("eSignSettings"));

    // Register eSign service
    services.AddScoped<eSign.eSign>();
}
```

In `appsettings.json`:
```json
{
  "eSignSettings": {
    "ASPID": "YOUR_ASP_ID",
    "Gateway_URL": "https://gateway.example.com",
    "ResponseURL": "https://yourapp.com/callback",
    "Timeout": 30000
  }
}
```

### Web Application Integration

#### Step 1: Generate Gateway Parameters
```csharp
[HttpPost]
[Route("api/signing/initiate")]
public async Task<IActionResult> InitiateSigning([FromBody] SigningRequest request)
{
    var input = BuildESignInput(request);
    var esign = new eSign.eSign();
    var result = esign.GenerateGatewayParameters(input);

    return Ok(new
    {
        gatewayURL = result.GatewayURL,
        responseURL = result.ResponseURL
    });
}
```

#### Step 2: Handle Callback
```csharp
[HttpPost]
[Route("api/signing/callback")]
public async Task<IActionResult> HandleCallback([FromForm] string response)
{
    var esign = new eSign.eSign();
    var result = esign.CheckTransactionStatus(
        "YOUR_ASP_ID",
        "TRANSACTION_ID",
        "GATEWAY_URL"
    );

    if (result.ReturnCode == "1")
    {
        // Retrieve signed document
        string signedPdfBase64 = result.SignedDoc;
        // Process signed document
        return Ok("Document signed successfully");
    }

    return BadRequest("Signing failed");
}
```

---

## API Documentation

### Main Classes

#### `eSign`
The main facade class for all signing operations.

**Key Methods**:
- `GenerateGatewayParameters(eSignInput input)` - Generate parameters for signing gateway
- `CheckTransactionStatus(string aspId, string txnId, string gatewayUrl)` - Check signing status
- `VerifySignature(string signedPdfBase64)` - Verify digital signature
- `SignDocument(eSignInput input, string pkcs12Path, string password)` - Direct document signing

#### `eSignInputBuilder`
Builder pattern for creating `eSignInput` objects.

**Example**:
```csharp
var input = new eSignInputBuilder()
    .SeteSignSettings(settings)
    .SetUserInfo(userInfo)
    .SetPDFBase64(pdfBase64)
    .SetCoordinates(Coordinates.BOTTOMRIGHT)
    .SetAuthMode(AuthMode.OTP)
    .SetAppearanceType(AppearanceType.STANDARD)
    .SetPageNo(PageNo.ALL)
    .Build();
```

#### `eSignSettings`
Configuration holder for eSign service settings.

**Properties**:
- `ASPID` - Application Service Provider ID (required)
- `Gateway_URL` - eSign gateway URL (required)
- `ResponseURL` - Callback URL for async responses (required)
- `Timeout` - Connection timeout in milliseconds (default: 30000)
- `ProxyHost`, `ProxyPort`, `ProxyUserID`, `ProxyUserPassword` - Proxy configuration

#### `UserInfoBuilder`
Builder for creating user information objects.

**Example**:
```csharp
var user = new UserInfoBuilder()
    .SeteMail("user@example.com")
    .SetFirstname("John")
    .SetLastname("Doe")
    .SetPhoneNumber("9876543210")
    .SetCity("Bangalore")
    .SetState("Karnataka")
    .SetCountry("India")
    .Build();
```

### Enumerations

#### `AuthMode`
Authentication methods:
- `OTP` - One-Time Password
- `FP` - Fingerprint
- `IRIS` - IRIS recognition
- `FACE` - Face recognition

#### `AppearanceType`
Signature appearance types:
- `STANDARD` - Standard signature
- `SIGNATUREIMG` - Image-based signature
- `ONELINER` - Single line signature
- `ADVANCED` - Advanced signature with custom styling
- `COLOREDGRAPHIC` - Colored graphics signature
- `BGIMG` - Background image signature

#### `Coordinates`
9-point positioning system:
- `TOPLEFT`, `TOPMIDDLE`, `TOPRIGHT`
- `CENTERLEFT`, `CENTERMIDDLE`, `CENTERRIGHT`
- `BOTTOMLEFT`, `BOTTOMMIDDLE`, `BOTTOMRIGHT`

#### `PageNo`
Page selection for signing:
- `ALL` - All pages
- `EVEN` - Even pages only
- `ODD` - Odd pages only
- `LAST` - Last page only
- `FIRST` - First page only
- `SPECIFY` - Specify custom pages

---

## Advanced Features

### Custom Signature Appearance

```csharp
var advSig = new AdvanceSignature
{
    SignerName = "John Doe",
    SignerLocation = "Bangalore",
    SignerReason = "Document Approval",
    SignerContactInfo = "+91-9876543210"
};

var style = new CustomStyle
{
    BackgroundColor = "#FFFFFF",
    TextColor = "#000000",
    FontSize = 12
};

var input = new eSignInputBuilder()
    // ... other settings
    .SetAppearanceType(AppearanceType.ADVANCED)
    .SetAdvanceSignature(advSig)
    .SetCustomStyle(style)
    .Build();
```

### Colored Graphics Signature

```csharp
var graphicInputs = new ColoredGraphicInputs
{
    BackgroundColor = "#E8F4F8",
    BorderColor = "#0066CC",
    TextColor = "#000000",
    LogoBase64 = "BASE64_ENCODED_LOGO"
};

var input = new eSignInputBuilder()
    // ... other settings
    .SetAppearanceType(AppearanceType.COLOREDGRAPHIC)
    .SetColoredGraphicInputs(graphicInputs)
    .Build();
```

### Bank KYC Integration

```csharp
var kycInfo = new BankKYCInfo
{
    AccountNumber = "1234567890",
    BankName = "Sample Bank",
    IfscCode = "SBIN0001234"
};

var input = new eSignInputBuilder()
    // ... other settings
    .SetBankKYCInfo(kycInfo)
    .Build();
```

### Proxy Configuration

```csharp
var settings = new eSignSettings
{
    ASPID = "YOUR_ASP_ID",
    Gateway_URL = "https://gateway.example.com",
    ProxyHost = "proxy.company.com",
    ProxyPort = 8080,
    ProxyUserID = "proxyuser",
    ProxyUserPassword = "proxypass"
};
```

---

## License

This project is licensed under the **GNU Affero General Public License v3.0 (AGPL-3.0)**.

### Important Licensing Notes:

1. **AGPL-3.0 Requirements**:
   - Any modifications to this library must be open-sourced
   - Applications using this library over a network must provide source code to users
   - Commercial use is allowed, but derivative works must remain open source

2. **Embedded Libraries**:
   - **iText PDF Library**: This library embeds iText, which is also AGPL-licensed. For commercial use without open-sourcing your application, you may need a commercial license from iText Software.
   - **BouncyCastle**: Licensed under MIT-style license (permissive)

3. **Commercial Considerations**:
   - If you cannot comply with AGPL requirements (e.g., closed-source SaaS application), contact the library maintainers for commercial licensing options
   - Alternative: Replace iText with a different PDF library with a permissive license

See [LICENSE.txt](LICENSE.txt) for full license text.

---

## Security Considerations

### Important Security Warnings

1. **SSL/TLS Certificate Validation**:
   - Review SSL/TLS implementation to ensure proper certificate validation
   - Never disable certificate validation in production environments
   - This makes the library vulnerable to Man-in-the-Middle (MITM) attacks

2. **Credential Handling**:
   - Never hardcode credentials (ASPID, passwords, proxy credentials) in source code
   - Use environment variables or secure configuration management (Azure Key Vault, AWS Secrets Manager)
   - Rotate credentials regularly

3. **Input Validation**:
   - Always validate and sanitize user inputs before passing to the library
   - Validate PDF content before processing
   - Sanitize email addresses and phone numbers

4. **Logging**:
   - Review log configurations to avoid logging sensitive data
   - Transaction IDs and user information may appear in logs

### Recommended Security Practices

```csharp
// Use environment variables for sensitive configuration
var settings = new eSignSettings
{
    ASPID = Environment.GetEnvironmentVariable("ESIGN_ASP_ID"),
    Gateway_URL = Environment.GetEnvironmentVariable("ESIGN_GATEWAY_URL")
};

// Or use configuration with secret management
var settings = new eSignSettings
{
    ASPID = Configuration["eSignSettings:ASPID"]
};
```

---

## Troubleshooting

### Common Issues

**Issue**: `FileNotFoundException` when loading the DLL
**Solution**: Ensure the DLL and all its dependencies are in the same directory or properly referenced

**Issue**: SSL handshake failures
**Solution**: Check proxy settings and certificate validation configuration

**Issue**: Gateway timeout errors
**Solution**: Increase timeout value in `eSignSettings`:
```csharp
settings.Timeout = 60000; // 60 seconds
```

**Issue**: "Invalid ASPID" errors
**Solution**: Verify your ASPID is correctly registered with the eSign service provider

---

## Support and Contact

For issues, questions, or contributions, please:
- Open an issue on GitHub (if repository is public)
- Contact: [your-contact@example.com]
- Documentation: [Link to detailed docs]

---

## Changelog

### Version 2.0.0.17
- Current release version
- Comprehensive digital signature support
- Multi-factor authentication
- Advanced signature appearance options
- Bank KYC integration
- Cross-platform support with .NET Standard 2.0

---

## Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

### Development Setup
1. Fork the repository
2. Clone your fork
3. Create a feature branch
4. Make your changes
5. Write/update tests
6. Submit a pull request

---

## Acknowledgments

- **iText PDF Library** - PDF manipulation capabilities
- **BouncyCastle** - Cryptographic operations
- **System.Security.Cryptography.Xml** - XML signature support

---

**Built with .NET | Powered by Digital Signatures**
>>>>>>> f249d54 (Initial commit: eSign Library Kit v2.0.0.17)
