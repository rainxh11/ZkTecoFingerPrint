using System.Reactive.Linq;
using ZkTecoFingerPrint;

ZkTecoFingerHost.Initialize();
ZkTecoFingerHost.ObserveDevice(0)
                .Where(x => x.IsSuccess)
                .Do(x => File.WriteAllBytes(Path.Combine(AppContext.BaseDirectory, $"{DateTime.Now.ToFileTime()}.bmp"), x.Value!.Bitmap))
                .Subscribe();

Console.ReadKey();