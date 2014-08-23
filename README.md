xamarin-bluetooth-toggle-widget
===============================

This is a simple tutorial/how-to on building a widget for an Android device, using Xamarin. 

![bluetooth logo](https://github.com/wislon/xamarin-bluetooth-toggle-widget/blob/master/src/BluetoothToggleWidget/Resources/drawable-xhdpi/bluetooth_on.png)
![bluetooth logo](https://github.com/wislon/xamarin-bluetooth-toggle-widget/blob/master/src/BluetoothToggleWidget/Resources/drawable-xhdpi/bluetooth_connecting.png)
![bluetooth logo](https://github.com/wislon/xamarin-bluetooth-toggle-widget/blob/master/src/BluetoothToggleWidget/Resources/drawable-xhdpi/bluetooth_connected.png)
![bluetooth logo](https://github.com/wislon/xamarin-bluetooth-toggle-widget/blob/master/src/BluetoothToggleWidget/Resources/drawable-xhdpi/bluetooth_off.png)

I've used an example of enabling/disabling Bluetooth because 
* It's slightly more complex than a pointless "hello world!" widget that doesn't actually show you anything.
* I needed a widget to do this, so I built one. Yes, I could have just downloaded one, but where's the fun in that? And this way you may benefit too.
* It's reproducible enough that it can be used to build a similar one for wi-fi, or location, or NFC or something else.

As it stands, this widget does the following:

* Reacts and changes its display based on power-state and connectivity-state changes in your device's built-in adapter.
* Toggles the enabling/disabling of the default Bluetooth adapter by tapping the widget.

It is based on the (simpler) [xamarin-bluetooth-status-widget](https://github.com/wislon/xamarin-bluetooth-status-widget), which I built a couple of weeks ago, and promised to extend to allow toggling and monitoring of connectivity status too. And to show you how it all fits together, there's a post on my blog over at [wislon.io](http://wislon.io/posts/2014/08/23/make-a-xamarin-android-widget-to-enable-disable-bluetooth)

####License
Copyright (c) 2014, John Wilson.

_This code is released under the [MIT license](LICENSE). However one of the Bluetooth symbol images is provided by a third-party, and was distributed as 'freeware, not for commercial use'. So if you plan on using the code in this repo as the basis for a commercial product (go right ahead!), you'll need to source your Bluetooth symbol image(s) from elsewhere_
