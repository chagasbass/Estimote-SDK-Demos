using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Estimote.iOS;

namespace Distance
{
	public partial class DistanceViewController : UIViewController
	{
		#region Properties

		private ESTBeaconManager beaconManager;
		public ESTBeacon selectedBeacon;
		public UIImageView positionDot;

		public float dotMinPos;
		public float dotRange;

		#endregion

		#region Constructors

		public DistanceViewController () : base ()
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
			SetupManager ();
			SetupView ();
		}

		#endregion

		#region Private Methods

		private void SetupManager()
		{
			// create estimote manager instance and assign it's delegates
			var myBeaconManager = new BeaconManager (this);

			beaconManager = new ESTBeaconManager ();
			beaconManager.Delegate = myBeaconManager;
			beaconManager.AvoidUnknownStateBeacons = true;

			// create sample region object (you can additionally pass major/minor values)
			ESTBeaconRegion region = new ESTBeaconRegion ("Estimote Sample Region");

			// start looking for estimote beacons in region
			// when beacon ranged beaconManager.DidRangeBeacons invoked
			beaconManager.StartRangingBeacons (region);
		}

		private void DidRangeBeacons(ESTBeaconManager manager, NSArray[] beacons, ESTBeaconRegion region)
		{
		}

		private void SetupView()
		{
			SetupBackgroundImage ();
			SetupDotImage ();
		}

		private void SetupBackgroundImage()
		{
			RectangleF screenRect = UIScreen.MainScreen.Bounds;
			float screenHeight = screenRect.Size.Height;
			UIImageView backgroundImage;

			if (screenHeight > 480) {
				backgroundImage = new UIImageView (UIImage.FromBundle ("backgroundBig"));
			} else {
				backgroundImage = new UIImageView (UIImage.FromBundle ("backgroundSmall"));
			}

			this.View.AddSubview (backgroundImage);
		}

		private void SetupDotImage()
		{
			positionDot = new UIImageView (UIImage.FromBundle ("dotImage"));
			positionDot.Center = this.View.Center;
			positionDot.Alpha = 1.0f;

			this.View.AddSubview (positionDot);

			dotMinPos = 150;
			dotRange = (this.View.Bounds.Size.Height - 220);
		}

		#endregion

		#region Beacon Delegate Class

		class BeaconManager : ESTBeaconManagerDelegate
		{
			private DistanceViewController vc;

			public BeaconManager (DistanceViewController vc)
			{
				this.vc = vc;
			}

			public override void DidRangeBeacons(ESTBeaconManager manager, NSArray[] beacons, ESTBeaconRegion region)
			{
				if (beacons.Length > 0) {
					if (vc.selectedBeacon == null) {
						// initially pick the closest beacon
						vc.selectedBeacon = (ESTBeacon)beacons.GetValue(0);
					} else {
						for (int i = 0; i < beacons.Length; i++) {
							ESTBeacon cBeacon = (ESTBeacon)beacons.GetValue (i);

							// update beacon if same as selected initially
							if ((vc.selectedBeacon.Ibeacon.Major.UnsignedIntegerValue == cBeacon.Ibeacon.Major.UnsignedIntegerValue)
								&& (vc.selectedBeacon.Ibeacon.Minor.UnsignedIntegerValue == cBeacon.Ibeacon.Minor.UnsignedIntegerValue)) {
								vc.selectedBeacon = cBeacon;
							}
						}
					}

					// based on observation rssi is not getting bigger than -30
					// so it changes from -30 to -100 so we normalize
					float distFactor = ((float)vc.selectedBeacon.Ibeacon.Rssi + 30) / -70;

					// calculate and set new y position
					float newYPos = (vc.dotMinPos + distFactor * vc.dotRange);
					vc.positionDot.Center = new PointF (vc.View.Bounds.Size.Width / 2, newYPos);
				}
			}
		}

		#endregion
	}
}