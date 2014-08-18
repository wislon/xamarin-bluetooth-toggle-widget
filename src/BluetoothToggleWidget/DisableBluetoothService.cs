/*
  The MIT License (MIT)

  Copyright (c) 2014 John Wilson

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.
*/


using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Bluetooth;
using Android.Util;
using Android.Widget;

namespace BluetoothToggleWidget
{
  [Service]
  class DisableBluetoothService : IntentService
  {
    public DisableBluetoothService() : base("DisableBluetoothService")
    {
      
    }

    protected override void OnHandleIntent(Intent intent)
    {
      Log.Info(Constants.APP_NAME, "Received request to disable bluetooth");
      BluetoothAdapter.DefaultAdapter.Disable();
    }

  }

}