using System;
using System.Drawing;
using MonoTouch.CoreLocation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Estimote.iOS;

namespace Proximity
{
	public partial class ProximityViewController : UIViewController
	{
		#region Properties

		private UILabel distanceLabel;
		private ESTBeaconManager beaconManager;
		private ESTBeacon selectedBeacon;

		#endregion

		#region Constructor

		public ProximityViewController () : base ()
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
			distanceLabel = new UILabel (new RectangleF (37f, 232f, 247f, 73f));
			distanceLabel.TextAlignment = UITextAlignment.Center;
			distanceLabel.Lines = 0;
			distanceLabel.TextColor = UIColor.White;
			distanceLabel.Text = "Loading Beacons...";
			this.View.AddSubview (distanceLabel);

			// craete manager instance
			var myBeaconManager = new BeaconManager (this);

			beaconManager = new ESTBeaconManager ();
			beaconManager.Delegate = myBeaconManager;
			beaconManager.AvoidUnknownStateBeacons = true;

			// create sample region object (you can additionaly pass major / minor values)
			ESTBeaconRegion region = new ESTBeaconRegion("Estimote Sample Region");

			// start looking for estimote beacons in region
			// when beacon ranged beaconManager:didRangeBeacons:inRegion: invoked
			beaconManager.StartRangingBeacons (region);
		}

		#endregion

		#region Beacon Delegate Class

		class BeaconManager : ESTBeaconManagerDelegate
		{
			private ProximityViewController vc;

			public BeaconManager (ProximityViewController vc)
			{
				this.vc = vc;
			}

			public override void DidRangeBeacons(ESTBeaconManager manager, NSArray[] beacons, ESTBeaconRegion region)
			{
				if (beacons.Length > 0) {
					if (vc.selectedBeacon == null) {
						vc.selectedBeacon = (ESTBeacon)beacons.GetValue (0);
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

					// beacon array is sorted based on distance
					// closest beacon is the first one
					string labelText = string.Format ("Major: {0}, Minor: {1}\nRegion: ",
						vc.selectedBeacon.Ibeacon.Major.UnsignedIntegerValue,
						vc.selectedBeacon.Ibeacon.Minor.UnsignedIntegerValue);

					// calculate and set new y position
					switch (vc.selectedBeacon.Ibeacon.Proximity) {
					case CLProximity.Unknown:
						labelText += "Unknown";
						break;
					case CLProximity.Immediate:
						labelText += "Immediate";
						break;
					case CLProximity.Near:
						labelText += "Near";
						break;
					case CLProximity.Far:
						labelText += "Far";
						break;
					default:
						break;
					}

					vc.distanceLabel.Text = labelText;
				}
			}
		}

		#endregion
	}
}