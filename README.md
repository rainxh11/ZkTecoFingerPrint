# ZkTecoFingerPrint:
a robust and simple to use library to use ZkTeco Fingerprint Reader Device.

this library uses native ZkTeco dlls directly instead of the buggy `libzkfpcsharp` implementation shipped with the SDK

# Installation:
| Nuget Pacakge | Downloads |
|-|-|
|[![Latest version](https://img.shields.io/nuget/v/ZkTecoFingerPrint.svg)](https://www.nuget.org/packages/ZkTecoFingerPrint)|![Downloads](https://img.shields.io/nuget/dt/ZkTecoFingerPrint.svg)|

```powershell
dotnet add package ZkTecoFingerPrint
```
# Usage:
1. Initialize and Query devices  count:
```csharp
// Initialize the ZkTeco Library first
ZkTecoFingerHost.Initialize();

// Get how many ZkTeco devices are connected
var deviceCount = ZkTecoFingerHost.GetDeviceCount();
```
2. Using a device __(The recommended way)__:
```csharp

/*
Watching a device for fingerprints
PS: if device count is 1, deviceIndex is 0
*/
var deviceObservable = ZkTecoFingerHost.ObserveDevice(deviceIndex: 0)
                .Where(deviceResult => deviceResult.IsSuccess)
                .Select(deviceResult => deviceResult.Value)
                .Do(fingerPrintResult =>
                    {
                        var bitmapImage = fingerPrintResult.Bitmap;
                        var fingerPrintTemplate = fingerPrintResult.Template;
                        /*
                         * ...store in a db or whatever
                         */
                    })
                .Subscribe();
```

###### **_(open device is exclusive to the application using it and cannot be used elsewhere before closing it or terminating the application)_**

3. Closing a device:
```csharp
deviceObservable.Dispose();
ZkTecoFingerHost.Close();
```
4. Comparing a fingerprint template against another:
```csharp
var minimumSimilarity = 50;
var similarityRatio = fingerPrintResult.Match(template: anotherTemplate);

if(similarityRatio >= 50)
{
    /* ... */ 
}
```
* Example usage identifying a user from database:
```csharp
// db.GetAll<User>() is a hypothetical database query
var users = db.GetAll<User>();
var matchedUser = fingerPrintResult.Identify<User>(candidates: users,
                                    templateSelector: user => user.ingerPrintTemplate,
                                    threshold: 50);
```

5. Getting Device Information:
```csharp

```

### Polling for fingerprints manually:
```csharp
ZkTecoFingerHost.Initialize();

var deviceCount = ZkTecoFingerHost.GetDeviceCount();

if (deviceCount > 0)
{
    var deviceResult = ZkTecoFingerHost.OpenDevice(0);
    if (deviceResult.IsSuccess)
    {
        var device = device.Value;
        while (true)
        {
            var fingerprintResult = await AcquireFingerprintAsync();
            if (fingerprintResult.IsSuccess)
            {
                var fingerprint = fingerprintResult.Value;
                /*
                 * ....
                 */
            }
        }
    }
}
```

### Missing ZkTeco features found in SDK:
* This library uses `SourceAFIS` for fingerprint templates and matching instead of ZkTeco's
* DB functions from the SDK were deemed unnecessary, so they weren't implemented.

## Rant about ZkTeco
The ZkTeco SDK and C# library is the worst API I've ever had to deal with. Those dumb developers at ZkTeco should be ashamed of releasing an SDK in this state.

If you want to interface with a fingerprint device programmatically, avoid ZKTECO products.

After wasted hours and frustrating trials and errors, I ended up creating a more sane way to use the Abomination, which is the ZkTeco USB Fingerprint Reader, still not perfect but a dozen times better than their own SDK.

### Message to ZkTeco Developers or whoever is in charge of making SDKs:
Your C++ library interface sucks, with no documentation. Your.net library is just a half-ass attempt at wrapping your shitty dlls. Why the fuck don't you interface with your USB devices directly using the proper.net library?

I can make your SDK better and faster, and it does not make whoever uses it want to shoot themselves rather than spend any more seconds trying to decipher why and how things are done.

## Bugs and Issues:
If the issue can be fixed by tweaking the C# code, I'm open to suggestions and code contributions.
