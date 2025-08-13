# 🚀 SharpPdfGen Publishing Guide

This guide provides step-by-step instructions for publishing the SharpPdfGen NuGet package and deploying the landing page.

## 📦 NuGet Package Publishing

### Prerequisites
1. .NET SDK 8.0 or later
2. NuGet account at [nuget.org](https://www.nuget.org)
3. API key from NuGet (Account Settings → API Keys)

### Step 1: Build and Test

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build --configuration Release

# Run all tests
dotnet test --configuration Release --verbosity normal

# Run sample application (optional)
cd samples/SharpPdfGen.ConsoleApp
dotnet run
cd ../..
```

### Step 2: Pack the NuGet Package

```bash
# Navigate to the library project
cd src/SharpPdfGen

# Create the NuGet package
dotnet pack --configuration Release --output ../../nupkg

# Verify package contents
dotnet nuget list source
```

### Step 3: Publish to NuGet

```bash
# Set your NuGet API key (replace YOUR_API_KEY)
dotnet nuget setapikey YOUR_API_KEY --source https://api.nuget.org/v3/index.json

# Push the package
dotnet nuget push ../../nupkg/SharpPdfGen.1.0.0.nupkg --source https://api.nuget.org/v3/index.json
```

### Step 4: Verify Publication

1. Visit [nuget.org/packages/SharpPdfGen](https://www.nuget.org/packages/SharpPdfGen)
2. Check that the package appears correctly
3. Test installation in a new project:

```bash
mkdir test-project
cd test-project
dotnet new console
dotnet add package SharpPdfGen
```

## 🌐 Landing Page Deployment

### Option 1: GitHub Pages

1. Create a GitHub repository for your project
2. Push all code to the repository
3. Enable GitHub Pages in repository settings
4. Set source to `docs/landing-page` folder
5. Your site will be available at `https://yourusername.github.io/SharpPdfGen`

### Option 2: Netlify

1. Sign up at [netlify.com](https://netlify.com)
2. Connect your GitHub repository
3. Set build directory to `docs/landing-page`
4. Deploy automatically on commits

### Option 3: Vercel

1. Sign up at [vercel.com](https://vercel.com)
2. Import your GitHub repository
3. Set output directory to `docs/landing-page`
4. Deploy with automatic domain

### Option 4: Azure Static Web Apps

1. Create Azure Static Web App resource
2. Connect to your GitHub repository
3. Set app location to `docs/landing-page`
4. Deploy through GitHub Actions

## 🔧 Configuration Updates

Before publishing, update these files with your actual information:

### 1. Project Metadata (`src/SharpPdfGen/SharpPdfGen.csproj`)

```xml
<PackageProjectUrl>https://github.com/YOUR_USERNAME/SharpPdfGen</PackageProjectUrl>
<RepositoryUrl>https://github.com/YOUR_USERNAME/SharpPdfGen</RepositoryUrl>
<Authors>Your Name</Authors>
<Company>Your Company</Company>
```

### 2. README.md URLs

Replace placeholder URLs with actual ones:
- GitHub repository URLs
- Documentation links
- Support contact information

### 3. Landing Page (`docs/landing-page/index.html`)

Update these elements:
- GitHub stars badge URL
- Repository links
- Contact information
- Support links

## 📊 Benchmarking

Run performance benchmarks before publishing:

```bash
cd benchmarks/SharpPdfGen.Benchmarks
dotnet run -c Release
```

This will generate detailed performance comparisons with other PDF libraries.

## 🧪 Testing Checklist

Before publishing, ensure all tests pass:

- [ ] Unit tests pass (`dotnet test`)
- [ ] Sample application runs without errors
- [ ] All target frameworks build successfully
- [ ] Package metadata is correct
- [ ] Documentation is complete
- [ ] Landing page displays correctly

## 📋 Release Process

### Version Updates

1. Update version in `src/SharpPdfGen/SharpPdfGen.csproj`
2. Update version in `README.md` installation examples
3. Create release notes in `PackageReleaseNotes`

### Git Tagging

```bash
# Create and push version tag
git tag v1.0.0
git push origin v1.0.0
```

### GitHub Release

1. Go to your GitHub repository
2. Create a new release with tag v1.0.0
3. Add release notes describing new features and fixes
4. Attach the NuGet package file

## 🔍 Post-Publication Tasks

### 1. Update Package Documentation

- Verify README displays correctly on NuGet
- Check that all links work properly
- Ensure examples are accurate

### 2. Monitor Package Stats

- Track download counts
- Monitor for issues and feedback
- Respond to questions in GitHub issues

### 3. Community Engagement

- Share on social media
- Post in relevant .NET communities
- Write blog posts about features

## 🐛 Troubleshooting

### Common Issues

**Package Push Fails:**
- Verify API key is correct
- Check package ID isn't already taken
- Ensure all required metadata is present

**Build Errors:**
- Check target framework compatibility
- Verify all dependencies are available
- Run `dotnet restore` to refresh packages

**Landing Page Issues:**
- Test locally with `python -m http.server` in the docs/landing-page directory
- Verify all links and resources load correctly
- Check responsive design on mobile devices

### Support Resources

- [NuGet Documentation](https://docs.microsoft.com/en-us/nuget/)
- [GitHub Pages Guide](https://pages.github.com/)
- [.NET Publishing Guide](https://docs.microsoft.com/en-us/dotnet/core/deploying/)

## 📞 Need Help?

If you encounter issues during publishing:

1. Check the troubleshooting section above
2. Review the official documentation links
3. Open an issue in the GitHub repository
4. Contact the community on Discord or Stack Overflow

---

**Good luck with your SharpPdfGen launch! 🚀**
