ZkTecoFingerPrint:
The ZkTeco SDK and C# library is the worst API I've ever had to deal with. Those dumb developers at ZkTeco should be ashamed of releasing an SDK in this state.

If you want to interface with a fingerprint device programmatically, avoid ZKTECO products.

After wasted hours and frustrating trials and errors, I ended up creating a more sane way to use the Abomination, which is the ZkTeco USB Fingerprint Reader, still not perfect but a dozen times better than their own SDK.

Message to ZkTeco Developers or whoever is in charge of making SDKs:
Your C++ library interface sucks, with no documentation. Your.net library is just a half-ass attempt at wrapping your shitty dlls. Why the fuck don't you interface with your USB devices directly using the proper.net library?

I can make your SDK better and faster, and it does not make whoever uses it want to shoot themselves rather than spend any more seconds trying to decipher why and how things are done.

Bugs and Issues:
If the issue can be fixed by tweaking the C# code, I'm open to suggestions and code contributions.
