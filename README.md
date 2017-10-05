# anyline-ocr-examples-windows-uwp
Anyline - A powerful OCR SDK for Windows UWP


## File summary

* `Anyline Windows UWP Examples` - Example app source code
* `Anyline.dll` - Precompiled Anyline Core
* `Anyline.winmd` - Metadata for the Anyline Core
* `AnylineSDK.dll` - Precompiled library for the Anyline SDK
* `README.md` - This readme.
* `LICENSE.md` - The license file.


## Requirements

- Windows 10 + Visual Studio 2015 with the Windows 10 SDK
- An Windows 10 x86 device
- An integrated or external camera/webcam (recommended: 720p and focus capability)


## Quick Start


### 0. Clone or Download

* If you'd like to clone the repository you will have to use git-lfs. Use the following commands to install git-lfs.

Download git lfs from https://help.github.com/articles/installing-git-large-file-storage/

```
git lfs install
```


### 1. Integrate Anyline in your UWP App

Add `AnylineSDK.dll` and `Anyline.winmd` as reference to the project.
Make sure that `Anyline.dll` is in the same directory as the winmd file.


### 2. Configure the Project

Make sure to set The capabilities "Webcam", "Microphone" and "Internet (Client)" in Package.appxmanifest. Set the project build configuration to "x86". x64 and ARM are not supported.


### 3. License & Package Name

Make sure the Package Name in Package.appxmanifest matches the license.

To generate a license key for windows, you'll need the Package Name located under `Packaging` in the `Package.appxmanifest` file of your example project.

To claim a free developer / trial license, go to: [Anyline SDK Register Form](http://anyline.io/sdk-register?utm_source=githubios&utm_medium=readme&utm_campaign=examplesapp)

The software underlies the MIT License. As Anyline is a paid software for Commerical Projects, the License Agreement of Anyline GmbH apply, when used commercially. Please have a look at [Anyline License Agreement](https://anylinewebsiteresource.blob.core.windows.net/wordpressmedia/2015/12/ULA-AnylineSDK-August2015.pdf)


### 4. Add a View configuration as .json

Add a config.json for the view configuration as Asset to your project. 
Set the build action to "Content".

Example for **config** for MRZ:

```json
	{
	"captureResolution":"1080p",
	"cutout": {
		"style": "rect",
		"maxWidthPercent": "90%",
		"maxHeightPercent": "90%",
		"alignment": "center",
		"strokeWidth": 2,
		"cornerRadius": 4,
		"strokeColor": "FFFFFF",
		"outerColor": "000000",
		"outerAlpha": 0.3,
		"feedbackStrokeColor": "0099FF",
		"cropOffset": {
			"x": 10,
			"y": 20
		}
	},
	"flash": {
		"mode": "manual",
		"alignment": "bottom_right"
	},
	"beepOnResult": true,
	"vibrateOnResult": true,
	"blinkAnimationOnResult": true,
	"cancelOnResult": true,
	"visualFeedback": {
		"style": "RECT",
		"strokeColor": "0099FF",
		"fillColor": "220099FF",
		"animationDuration": 70
		}
	}
```


### 5. Follow the implementation below and enjoy scanning! :)

For a detailed implementation guide, follow the Windows implementation guide on the [Anyline documentation](https://documentation.anyline.io) website.


## License

See LICENSE file.