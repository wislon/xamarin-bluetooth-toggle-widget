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


  [BroadcastReceiver(Label = "Bluetooth Toggle Widget")]
  [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE", BluetoothAdapter.ActionStateChanged, BluetoothAdapter.ActionConnectionStateChanged })]
  [MetaData("android.appwidget.provider", Resource = "@xml/bt_widget")]
  public class BTToggleWidget : AppWidgetProvider
  {
    private const string APP_NAME = "BTToggleWidget";

    /// <summary>
    /// This event fires for every intent you're filtering for. There can be lots of them,
    /// and they can arrive very quickly, so spend as little time as possible processing them
    /// on the UI thread.
    /// </summary>
    /// <param name="context">The Context in which the receiver is running.</param>
    /// <param name="intent">The Intent being received.</param>
    public override void OnReceive(Context context, Intent intent)
    {
      Log.Info(APP_NAME, "OnReceive received intent: {0}", intent.Action);

      if(intent.Action == "android.appwidget.action.APPWIDGET_UPDATE")
      {
        Log.Info(APP_NAME, "Received AppWidget Update");
        var currentState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.State;
        Log.Info(APP_NAME, "BT adapter state currently {0}", currentState);
        UpdateWidgetDisplay(context, (int)currentState);

        if(currentState == State.On)
        {
          Log.Debug(APP_NAME, "Checking bluetooth connection state");
          ProfileState currentConnectedState = GetBluetoothConnectionState();
          UpdateWidgetDisplay(context, (int)currentConnectedState);
        }
        return;
      }

      if(intent.Action == Android.Bluetooth.BluetoothAdapter.ActionStateChanged)
      {
        Log.Info(APP_NAME, "Received BT Action State change message");
        ProcessBTStateChangeMessage(context, intent);
        return;
      }

      if(intent.Action == Android.Bluetooth.BluetoothAdapter.ActionConnectionStateChanged)
      {
        Log.Info(APP_NAME, "Received BT Action State change message");
        ProcessBTConnectionStateChangeMessage(context, intent);
        return;
      }
    }

    private ProfileState GetBluetoothConnectionState()
    {
      ProfileState connectionState = ProfileState.Disconnected;
      connectionState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(ProfileType.A2dp);
      if(connectionState == ProfileState.Connected)
      {
        Log.Info(APP_NAME, "Connected to A2DP");
        return connectionState;
      }

      connectionState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(ProfileType.Gatt);
      if(connectionState == ProfileState.Connected)
      {
        Log.Info(APP_NAME, "Connected to Gatt");
        return connectionState;
      }

      connectionState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(ProfileType.GattServer);
      if(connectionState == ProfileState.Connected)
      {
        Log.Info(APP_NAME, "Connected to Gatt Server");
        return connectionState;
      }

      connectionState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(ProfileType.Headset);
      if(connectionState == ProfileState.Connected)
      {
        Log.Info(APP_NAME, "Connected to Headset");
        return connectionState;
      }

      connectionState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(ProfileType.Health);
      if(connectionState == ProfileState.Connected)
      {
        Log.Info(APP_NAME, "Connected to a health device");
        return connectionState;
      }

      Log.Info(APP_NAME, "Not connected to a device matching any of the known profiles");
      return connectionState;
    }

    private void ProcessBTStateChangeMessage(Context context, Intent intent)
    {
      int prevState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousState, -1);
      int newState = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);
      string message = string.Format("Bluetooth State Change from {0} to {1}", prevState, newState);
      Log.Info(APP_NAME, message);

      UpdateWidgetDisplay(context, newState);
    }

    private void ProcessBTConnectionStateChangeMessage(Context context, Intent intent)
    {
      int prevState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousConnectionState, -1);
      int newState = intent.GetIntExtra(BluetoothAdapter.ExtraConnectionState, -1);
      string message = string.Format("Bluetooth Connection State Change from {0} to {1}", prevState, newState);
      Log.Info(APP_NAME, message);

      UpdateWidgetDisplay(context, newState);
    }

    /// <summary>
    /// Updates the widget display image based on the new state
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="newState">New state.</param>
    private void UpdateWidgetDisplay(Context context, int newState)
    {
      var appWidgetManager = AppWidgetManager.GetInstance(context);
      var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.initial_layout);
      // Log.Debug(APP_NAME, "this.GetType().ToString(): {0}", this.GetType().ToString());

      var thisWidget = new ComponentName(context, this.Class);
      // Log.Debug(APP_NAME, thisWidget.FlattenToString());
      // Log.Debug(APP_NAME, "remoteViews: {0}", remoteViews.ToString());

      int imgResource = Resource.Drawable.bluetooth_off;
      State currentState = (Android.Bluetooth.State)newState;
      switch(currentState)
      {
        case Android.Bluetooth.State.Off:
        case Android.Bluetooth.State.TurningOn:
          {
            imgResource = Resource.Drawable.bluetooth_off;
            break;
          }

        case Android.Bluetooth.State.On:
        case Android.Bluetooth.State.TurningOff:
          {
            imgResource = Resource.Drawable.bluetooth_on;
            break;
          }

        case Android.Bluetooth.State.Connecting:
        case Android.Bluetooth.State.Disconnecting:
          {
            imgResource = Resource.Drawable.bluetooth_connecting;
            break;
          }

        case Android.Bluetooth.State.Connected:
          {
            imgResource = Resource.Drawable.bluetooth_connected;
            break;
          }

        case Android.Bluetooth.State.Disconnected:
          {
            imgResource = Resource.Drawable.bluetooth_on;
            break;
          }

        default:
          {
            imgResource = Resource.Drawable.bluetooth_off;
            break;
          }
      }

      remoteViews.SetImageViewResource(Resource.Id.imgBluetooth, imgResource);
      // depending on current State of the adapter (on or off), allow a tap of the widget to 
      // toggle it. we do this by hooking up a pending intent to the imageButton's OnClick event
      if(currentState == State.Off)
      {
        Log.Info(APP_NAME, "State is off, adding click delegate to turn on BT ");
        Intent enableBluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
        PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, enableBluetoothIntent, PendingIntentFlags.OneShot);
        remoteViews.SetOnClickPendingIntent(Resource.Id.imgBluetooth, pendingIntent);
      }

      appWidgetManager.UpdateAppWidget(thisWidget, remoteViews);
    }
  }
}