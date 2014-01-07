using System;
using System.Drawing;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Estimote.iOS;

namespace Notification
{
	public partial class NotificationViewController : UIViewController
	{
		#region Properties

		private ESTBeaconManager beaconManager;
		private UIImageView productImage;

		#endregion

		#region Constructors

		public NotificationViewController () : base ()
		{
		}

		#endregion

		#region View Controller Events

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			// craete manager instance
			var myBeaconManager = new BeaconManager (this);
			beaconManager = new ESTBeaconManager ();
			beaconManager.Delegate = myBeaconManager;
			beaconManager.AvoidUnknownStateBeacons = true;

			// create sample region with major value defined
			Console.WriteLine("TODO: Update the ESTBeaconRegion with your major / minor number and enable background app refresh in the Settings on your device for the NotificationDemo to work correctly.");
			ESTBeaconRegion region = new ESTBeaconRegion (1, 1, "Estimote Sample Region");

			// start looking for estimote beacons in region
			// when beacon ranged beaconManager.DidRangeBeacons
			// and beconManager.DidExitRegion invoked
			beaconManager.StartMonitoring(region);
			beaconManager.RequestState(region);

			/////////////////////////////////////////////////////////////
			// setup view

			// background
			productImage = new UIImageView(UIScreen.MainScreen.Bounds);
			SetProductImage();
			this.View.AddSubview(productImage);
		}

		#endregion

		#region Private Methods

		public void SetProductImage()
		{
			// product image when user outside beacon zone
			if (UIScreen.MainScreen.Bounds.Size.Height > 480) {
				productImage.Image = UIImage.FromBundle ("beforeNotificationBig");
			} else {
				productImage.Image = UIImage.FromBundle ("beforeNotificationSmall");
			}
		}

		public void SetDiscountImage()
		{
			// product image when user inside beacon zone
			if (UIScreen.MainScreen.Bounds.Size.Height > 480) {
				productImage.Image = UIImage.FromBundle ("afterNotificationBig");
			} else {
				productImage.Image = UIImage.FromBundle ("afterNotificationSmall");
			}
		}

		#endregion

		#region Beacon Delegate Class

		class BeaconManager : ESTBeaconManagerDelegate
		{
			private NotificationViewController vc;

			public BeaconManager (NotificationViewController vc)
			{
				this.vc = vc;
			}

			public override void DidDetermineState(ESTBeaconManager manager, CLRegionState state, ESTBeaconRegion region)
			{
				if (state == CLRegionState.Inside) {
					vc.SetProductImage ();
				} else {
					vc.SetDiscountImage ();
				}
			}

			public override void DidEnterRegion(ESTBeaconManager manager, ESTBeaconRegion region)
			{
				// iPhone/iPad entered beacon zone
				vc.SetProductImage ();

				// present local notification
				UILocalNotification notification = new UILocalNotification ();
				notification.AlertBody = "You entered the region!";
				notification.SoundName = UILocalNotification.DefaultSoundName;

				UIApplication.SharedApplication.PresentLocationNotificationNow (notification);
			}

			public override void DidExitRegion(ESTBeaconManager manager, ESTBeaconRegion region)
			{
				// iPhone/iPad left beacon zone
				vc.SetDiscountImage ();

				// present local notification
				UILocalNotification notification = new UILocalNotification ();
				notification.AlertBody = "The shoes you'd tried on are now 20%% off for you with this coupon";
				notification.SoundName = UILocalNotification.DefaultSoundName;

				UIApplication.SharedApplication.PresentLocationNotificationNow (notification);
			}
		}

		#endregion
	}
}