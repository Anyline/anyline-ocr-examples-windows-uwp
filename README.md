# anyline-ocr-examples-windows-uwp
Anyline - A powerful OCR SDK for Windows UWP


## File summary

* `Anyline Windows UWP Examples` - Example app source code
* `Anyline.dll` - Precompiled Anyline Core
* `Anyline.winmd` - Metadata for the Anyline Core
* `AnylineSDK.dll` - Precompiled library for the Anyline SDK
* `README.md` - This readme.
* `LICENSE.md` - The license file.


## API Reference

The API reference for the Anyline SDK for Windows UWP can be found here: https://documentation.anyline.com/api/windows/index.html


## Requirements

- Windows 10 + Visual Studio 2015 with the Windows 10 SDK
- A Windows 10 x86 device
- An integrated camera or external webcam (recommended: 1080 and focus capability)


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

To claim a free trial license, go to the [Anyline Customer Portal](https://customer.anyline.com/login)

The software underlies the MIT License. As Anyline is a paid software for Commerical Projects, the License Agreement of Anyline GmbH apply, when used commercially. Please have a look at [Anyline License Agreement](https://anylinewebsiteresource.blob.core.windows.net/wordpressmedia/2015/12/ULA-AnylineSDK-August2015.pdf)


### 4. Add a View configuration as .json

Add a config.json for the view configuration as Asset to your project. 
Set the build action to "Content".

Example for **config** for MRZ:

```json
	{
	  "camera": {
		"captureResolution": "1080p"
	  },
	  "flash": {
		"mode": "manual",
		"alignment": "bottom_right"
	  },
	  "viewPlugin": {
		"plugin": {
		  "id": "ID_PLUGIN",
		  "idPlugin": {
			"mrzConfig": {
			  "mrzFieldScanOptions": {
				"vizAddress": "optional",
				"vizDateOfIssue": "optional"
			  }
			}
		  }
		},
		"cutoutConfig": {
		  "style": "rect",
		  "maxWidthPercent": "90%",
		  "maxHeightPercent": "90%",
		  "alignment": "center",
		  "strokeWidth": 2,
		  "cornerRadius": 4,
		  "strokeColor": "FFFFFF",
		  "outerColor": "000000",
		  "outerAlpha": 0.3,
		  "cropPadding": {
			"x": -30,
			"y": -90
		  },
		  "cropOffset": {
			"x": 0,
			"y": 90
		  },
		  "feedbackStrokeColor": "0099FF"
		},
		"scanFeedback": {
		  "style": "rect",
		  "visualFeedbackRedrawTimeout": 100,
		  "strokeColor": "0099FF",
		  "fillColor": "220099FF",
		  "beepOnResult": true,
		  "vibrateOnResult": true,
		  "strokeWidth": 2
		},
		"cancelOnResult": true
	  }
	}
```


### 5. Follow the implementation below and enjoy scanning! :)

For a detailed implementation guide, follow the Windows implementation guide on the [Anyline documentation](https://documentation.anyline.com) website.


## Get Help (Support)

We don't actively monitor the Github Issues, please raise a support request using the [Anyline Helpdesk](https://anyline.atlassian.net/servicedesk/customer/portal/2/group/6).
When raising a support request based on this Github Issue, please fill out and include the following information:

```
Support request concerning Anyline Github Repository: anyline-ocr-flutterexamples-windows-uwp
```

Thank you!



## License

See LICENSE file.
