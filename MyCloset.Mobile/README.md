# MyCloset Mobile App (.NET MAUI)

A cross-platform mobile application for iOS and Android built with .NET MAUI 7.0, featuring AI-powered outfit recommendations, closet management, and social media integration.

## 🌟 Features

### Core Features
- **Digital Closet Management** - Organize clothing items with photos, categories, colors, and tags
- **Outfit Creation & Tracking** - Build and save outfit combinations
- **AI-Powered Recommendations** - Get smart outfit suggestions based on occasion, weather, and season
- **Social Media Integration** - Connect accounts and track outfit history
- **Profile Management** - Manage your account and preferences

### Mobile-Specific Features
- **Native Performance** - True native apps for iOS and Android
- **Offline Capability** - Browse your closet even without internet
- **Swipe Gestures** - Swipe to delete items, analyze with AI, or record outfits worn
- **Pull to Refresh** - Refresh your closet and outfits with a simple pull gesture
- **Responsive Design** - Beautiful UI that adapts to all screen sizes

## 🏗️ Architecture

### MVVM Pattern
The app follows the Model-View-ViewModel (MVVM) pattern:
- **Models**: Data classes matching the backend DTOs
- **ViewModels**: Business logic and state management using CommunityToolkit.Mvvm
- **Views**: XAML-based UI pages

### Project Structure
```
MyCloset.Mobile/
├── Models/               # Data models matching backend
│   ├── AppModels.cs
│   └── RequestModels.cs
├── ViewModels/          # MVVM ViewModels
│   ├── ClosetViewModel.cs
│   ├── OutfitsViewModel.cs
│   ├── AIRecommendationsViewModel.cs
│   ├── SocialMediaViewModel.cs
│   ├── LoginViewModel.cs
│   └── ProfileViewModel.cs
├── Pages/               # XAML UI Pages
│   ├── LoginPage.xaml
│   ├── ClosetPage.xaml
│   ├── OutfitsPage.xaml
│   ├── AIRecommendationsPage.xaml
│   ├── SocialMediaPage.xaml
│   └── ProfilePage.xaml
├── Services/            # API and Auth services
│   ├── ApiService.cs
│   └── AuthService.cs
├── Converters/          # XAML value converters
├── Resources/           # Images, fonts, styles
│   └── Styles/
│       ├── Colors.xaml
│       └── Styles.xaml
├── Platforms/           # Platform-specific code
│   ├── Android/
│   ├── iOS/
│   ├── MacCatalyst/
│   └── Windows/
├── App.xaml             # Application entry
├── AppShell.xaml        # Shell navigation
└── MauiProgram.cs       # Dependency injection setup
```

## 🚀 Getting Started

### Prerequisites
- .NET 7.0 SDK or later
- Visual Studio 2022 (17.4+) for Windows or Visual Studio 2022 for Mac
- For iOS development: Xcode 14.0+ and a Mac
- For Android development: Android SDK 21+ (Android 5.0+)

### Installation

#### 1. Install .NET MAUI Workload

On **macOS**:
```bash
sudo dotnet workload install maui
```

On **Windows** (Run as Administrator):
```bash
dotnet workload install maui
```

#### 2. Update Backend API URL

Edit [ApiService.cs](Services/ApiService.cs) and update the `BaseUrl`:

```csharp
public string BaseUrl { get; set; } = "https://your-backend-url.azurewebsites.net/api";
```

For local development:
- **Android Emulator**: Use `http://10.0.2.2:5001/api`
- **iOS Simulator**: Use `http://localhost:5001/api`
- **Physical Device**: Use your computer's IP address (e.g., `http://192.168.1.100:5001/api`)

#### 3. Restore NuGet Packages

```bash
cd MyCloset.Mobile
dotnet restore
```

#### 4. Build the Project

```bash
dotnet build
```

### Running the App

#### Run on Android Emulator
```bash
dotnet build -t:Run -f:net7.0-android
```

#### Run on iOS Simulator (macOS only)
```bash
dotnet build -t:Run -f:net7.0-ios
```

#### Using Visual Studio
1. Open `MyCloset.sln` in Visual Studio
2. Set `MyCloset.Mobile` as the startup project
3. Select your target platform (Android/iOS)
4. Press F5 or click "Run"

## 📱 Supported Platforms

| Platform | Minimum Version | Supported |
|----------|----------------|-----------|
| **Android** | API 21 (Android 5.0) | ✅ Yes |
| **iOS** | iOS 11.0 | ✅ Yes |
| **macOS (Catalyst)** | macOS 10.15 (Catalina) | ✅ Yes |
| **Windows** | Windows 10 build 17763+ | ⚠️ Needs testing |

## 🎨 UI Pages

### 1. Login Page
- Email/Password authentication
- OAuth sign-in with Google, Facebook, Microsoft
- Secure token storage using `SecureStorage`

### 2. Closet Page
- View all clothing items in a scrollable list
- Search and filter items
- Swipe to delete or analyze with AI
- Pull to refresh
- Add new items (to be implemented)

### 3. Outfits Page
- View saved outfits
- Create new outfit combinations
- Swipe to record as worn or delete
- View outfit details and items

### 4. AI Recommendations Page
- Select occasion, weather, and season
- Get AI-powered outfit suggestions
- Identify wardrobe gaps
- View recommendation confidence scores

### 5. Social Media Page
- Link Instagram, Facebook, Twitter accounts
- View connected accounts
- Track outfit history (when worn, where, occasion)
- View analytics

### 6. Profile Page
- Edit profile information
- View app settings
- Logout functionality

## 🔧 Configuration

### API Service Configuration

The `ApiService` class handles all HTTP communication with the backend. Key configuration:

```csharp
// In ApiService.cs
public string BaseUrl { get; set; } = "https://localhost:5001/api";
```

### Authentication

The `AuthService` uses `SecureStorage` to store authentication tokens:

```csharp
await SecureStorage.SetAsync("auth_token", token);
var token = await SecureStorage.GetAsync("auth_token");
```

### Dependency Injection

Services and ViewModels are registered in `MauiProgram.cs`:

```csharp
builder.Services.AddSingleton<IApiService, ApiService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddTransient<ClosetViewModel>();
builder.Services.AddTransient<ClosetPage>();
```

## 🧪 Testing

### Test on Android Emulator
1. Open Android Device Manager in Visual Studio
2. Create an emulator (recommend Pixel 5 with Android 12+)
3. Start the emulator
4. Run the app

### Test on Physical Android Device
1. Enable Developer Options on your Android device
2. Enable USB Debugging
3. Connect device via USB
4. Select device in Visual Studio
5. Run the app

### Test on iOS Simulator (macOS only)
1. Select an iOS simulator in Visual Studio
2. Run the app

### Test on Physical iOS Device
1. Connect iPhone/iPad via USB
2. Set up provisioning profile in Visual Studio
3. Select device
4. Run the app

## 🐛 Troubleshooting

### Issue: "Unable to connect to API"
**Solution**: 
- Check `BaseUrl` in `ApiService.cs`
- For Android Emulator, use `http://10.0.2.2:5001/api`
- For iOS Simulator, use `http://localhost:5001/api`
- For physical devices, use your computer's local IP

### Issue: "MAUI workload not installed"
**Solution**:
```bash
sudo dotnet workload install maui
```

### Issue: "Build failed on iOS"
**Solution**:
- Ensure Xcode is installed and updated
- Run `xcode-select --install`
- Accept Xcode license: `sudo xcodebuild -license accept`

### Issue: "Android SDK not found"
**Solution**:
- Install Android SDK via Visual Studio Installer
- Or install Android Studio and set `ANDROID_HOME` environment variable

### Issue: "Images not displaying"
**Solution**:
- Add placeholder images to `Resources/Images/`
- Update image sources in XAML or use default system icons

## 📦 NuGet Packages Used

| Package | Version | Purpose |
|---------|---------|---------|
| `CommunityToolkit.Mvvm` | 8.2.1 | MVVM helpers (ObservableObject, RelayCommand) |
| `CommunityToolkit.Maui` | 5.3.0 | Additional MAUI controls and behaviors |
| `Newtonsoft.Json` | 13.0.3 | JSON serialization |
| `Microsoft.Extensions.Logging.Debug` | 7.0.0 | Debug logging |

## 🔐 Security Considerations

### Authentication Tokens
- Tokens are stored securely using `SecureStorage` API
- Tokens are encrypted on device
- Never hardcode tokens in source code

### API Communication
- Always use HTTPS in production
- Validate SSL certificates
- Implement certificate pinning for extra security

### OAuth Implementation
- Current implementation is a placeholder
- In production, implement proper OAuth flows:
  - Google: Use `Xamarin.Essentials.WebAuthenticator`
  - Facebook: Use Facebook SDK for .NET MAUI
  - Instagram/Twitter: Use respective OAuth libraries

## 🚀 Deploying to Production

### Android (Google Play Store)

1. **Update version in .csproj**:
```xml
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

2. **Create release build**:
```bash
dotnet publish -f:net7.0-android -c:Release
```

3. **Sign APK/AAB**:
- Generate keystore
- Sign with your certificate
- Upload to Google Play Console

### iOS (App Store)

1. **Update version in .csproj**:
```xml
<ApplicationDisplayVersion>1.0</ApplicationDisplayVersion>
<ApplicationVersion>1</ApplicationVersion>
```

2. **Create archive**:
- In Visual Studio, select Archive option
- Create distribution build

3. **Upload to App Store Connect**:
- Use Xcode or Application Loader
- Submit for review

## 🎯 Next Steps

### Immediate Todos
1. **Add Image Upload** - Implement camera/gallery picker for adding clothing items
2. **Complete OAuth Flows** - Implement real OAuth for Google, Facebook, Instagram, Twitter
3. **Offline Mode** - Add local SQLite database for offline support
4. **Push Notifications** - Notify users of new AI recommendations
5. **Add Loading States** - Better UX during API calls

### Future Enhancements
1. **AR Try-On** - Use AR to virtually try on clothing
2. **Barcode Scanner** - Scan clothing tags for quick add
3. **Weather Integration** - Auto-suggest outfits based on local weather
4. **Friends Feature** - Share outfits with friends
5. **Shopping List** - Create shopping lists from wardrobe gap analysis

## 📚 Resources

- [.NET MAUI Documentation](https://learn.microsoft.com/dotnet/maui/)
- [MVVM Community Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/)
- [MAUI Community Toolkit](https://learn.microsoft.com/dotnet/communitytoolkit/maui/)
- [Publishing to App Stores](https://learn.microsoft.com/dotnet/maui/deployment/)

## 🤝 Contributing

To contribute to the mobile app:
1. Follow MVVM pattern
2. Use CommunityToolkit.Mvvm for ViewModels
3. Keep ViewModels testable (no direct UI references)
4. Use async/await for API calls
5. Handle errors gracefully with try/catch

## 📄 License

[Your license here]

---

**Built with ❤️ using .NET MAUI and C#**
