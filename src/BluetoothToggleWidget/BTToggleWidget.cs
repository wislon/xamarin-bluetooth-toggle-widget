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
using System.Linq;

namespace BluetoothToggleWidget
{


  [BroadcastReceiver(Label = "Bluetooth Toggle Widget")]
  [IntentFilter(new string[]
  { "android.appwidget.action.APPWIDGET_UPDATE", 
    BluetoothAdapter.ActionStateChanged, 
    BluetoothAdapter.ActionConnectionStateChanged
  })]
  [MetaData("android.appwidget.provider", Resource = "@xml/bt_widget")]
  public class BTToggleWidget : AppWidgetProvider
  {

    /// <summary>
    /// This event fires for every intent you're filtering for. There can be lots of them,
    /// and they can arrive very quickly, so spend as little time as possible processing them
    /// on the UI thread.
    /// </summary>
    /// <param name="context">The Context in which the receiver is running.</param>
    /// <param name="intent">The Intent being received.</param>
    public override void OnReceive(Context context, Intent intent)
    {
      Log.Info(Constants.APP_NAME, "OnReceive received intent: {0}", intent.Action);

      if(intent.Action == "android.appwidget.action.APPWIDGET_UPDATE")
      {
        Log.Info(Constants.APP_NAME, "Received AppWidget Update");
        var currentState = Android.Bluetooth.BluetoothAdapter.DefaultAdapter.State;
        Log.Info(Constants.APP_NAME, "BT adapter state currently {0}", currentState);
        UpdateWidgetDisplay(context, (int)currentState);

        if(currentState == State.On)
        {
          Log.Debug(Constants.APP_NAME, "Checking bluetooth connection state...");
          ProfileState currentConnectedState = GetBluetoothConnectionState();
          UpdateWidgetDisplay(context, (int)currentConnectedState);
        }
        return;
      }

      if(intent.Action == Android.Bluetooth.BluetoothAdapter.ActionStateChanged)
      {
        Log.Info(Constants.APP_NAME, "Received BT Action State change message");
        ProcessBTStateChangeMessage(context, intent);
        return;
      }

      if(intent.Action == Android.Bluetooth.BluetoothAdapter.ActionConnectionStateChanged)
      {
        Log.Info(Constants.APP_NAME, "Received BT Connection State change message");
        ProcessBTConnectionStateChangeMessage(context, intent);
        return;
      }
    }

    /// <summary>
    /// Clunky implementation of getting connection state for the different profiles.
    /// Sufficient for our purposes though. Could expand it to include what we're connected
    /// to as well.
    /// </summary>
    /// <returns>The bluetooth connection state.</returns>
    private ProfileState GetBluetoothConnectionState()
    {
      var profileTypes = new ProfileType[] { ProfileType.A2dp, ProfileType.Gatt, ProfileType.GattServer, ProfileType.Headset, ProfileType.Health };
      bool connected = profileTypes.Any(pt => BluetoothAdapter.DefaultAdapter.GetProfileConnectionState(pt) == ProfileState.Connected);
      return connected ? ProfileState.Connected : ProfileState.Disconnected;
    }

    private void ProcessBTStateChangeMessage(Context context, Intent intent)
    {
      int prevState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousState, -1);
      int newState = intent.GetIntExtra(BluetoothAdapter.ExtraState, -1);
      string message = string.Format("Bluetooth Adapter state change from {0} to {1}", (State)prevState, (State)newState);
      Log.Info(Constants.APP_NAME, message);

      UpdateWidgetDisplay(context, newState);
    }

    private void ProcessBTConnectionStateChangeMessage(Context context, Intent intent)
    {
      int prevState = intent.GetIntExtra(BluetoothAdapter.ExtraPreviousConnectionState, -1);
      int newState = intent.GetIntExtra(BluetoothAdapter.ExtraConnectionState, -1);
      string message = string.Format("Bluetooth Connection state change from {0} to {1}", (State)prevState, (State)newState);
      Log.Info(Constants.APP_NAME, message);

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
      // Log.Debug(Constants.APP_NAME, "this.GetType().ToString(): {0}", this.GetType().ToString());

      var thisWidget = new ComponentName(context, this.Class);
      // Log.Debug(Constants.APP_NAME, thisWidget.FlattenToString());
      // Log.Debug(Constants.APP_NAME, "remoteViews: {0}", remoteViews.ToString());

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

      switch(currentState)
      {
        case State.Off:
          {
            Log.Info(Constants.APP_NAME, "Adapter state is 'off', adding click delegate to turn on BT ");
            Intent enableBluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, enableBluetoothIntent, PendingIntentFlags.UpdateCurrent);
            remoteViews.SetOnClickPendingIntent(Resource.Id.imgBluetooth, pendingIntent);
            break;
          }
        default:
          {
            Log.Info(Constants.APP_NAME, string.Format("Adapter state is {0}, adding click delegate to turn off BT", currentState.ToString()));
            Intent disableBluetoothIntent = new Intent(context, typeof(DisableBluetoothService));
            PendingIntent pendingIntent = PendingIntent.GetService(context, 0, disableBluetoothIntent, PendingIntentFlags.UpdateCurrent);
            remoteViews.SetOnClickPendingIntent(Resource.Id.imgBluetooth, pendingIntent);
            break;
          }
      }

      appWidgetManager.UpdateAppWidget(thisWidget, remoteViews);
    }
  }
}