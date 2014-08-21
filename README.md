xamarin-bluetooth-toggle-widget
===============================

This is a simple tutorial/how-to on building a widget for an Android device, using Xamarin. 

I've used an example of enabling/disabling Bluetooth because 
* it's complex enough that it demonstrates use of an `ImageView`, `IntentService`, intents, broadcasts, and intent filtering (beats a simple, pointless "hello world!" widget that doesn't actually show you anything), and 
* I needed a widget to do this, so I built one. Yes, I could have just downloaded one, but where's the fun in that? And this way, you may benefit too.

As it stands, this widget does the following:

* Reacts and changes its display based on power-state and connectivity-state changes in your device's built-in adapter.
* Toggles the enabling/disabling of the default Bluetooth adapter by tapping the widget.

It is based on the (simpler) [xamarin-bluetooth-status-widget](https://github.com/wislon/xamarin-bluetooth-status-widget), which I built a couple of weeks ago, and promised to extend to allow toggling and monitoring of connectivity status too.

####License
Copyright (c) 2014, John Wilson.
_This code is released under the [MIT license](LICENSE). However one of the bluetooth symbol images is provided by a third-party, and was distributed as 'freeware, not for commercial use', so if you plan on using the code in this repo as the basis for a commercial product (go right ahead!), you'll need to source your bluetooth symbol image(s) from elsewhere._
