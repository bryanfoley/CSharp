using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OrderSystem
{
	class Model :EventArgs          //This is a class which models the interactions of ACME, the Manufacturer and 20 Customers over 365 days.
	{
		public int DayCount = 0;
		public int YearCount = 0;
		public int day = 0;
		public int LowerLimit;

		public static event ACMEHandler EventACME;
		public static event CustomerHandler EventCUS;
		public static event ManufacturerHandler EventMAN;
		public static event CustomerHandler EventCancelOrder;


		bool OpenForBusiness = true;

		Random r = new Random();
		Random rand = new Random();

		CustomerEventArgs[] Customers = new CustomerEventArgs[20];
		ManufacturerEventArgs b = new ManufacturerEventArgs();
		ACMEEventArgs a = new ACMEEventArgs();

		public void Run(int StepSize, int ReorderPoint)   //Default Constructor
		{
			StreamWriter sw1 = new StreamWriter(@"/home/bryan/Profits.txt",true);
			StreamWriter sw2 = new StreamWriter(@"/home/bryan/Stock.txt",true);
			StreamWriter sw3 = new StreamWriter(@"/home/bryan/DataPoints.xls", true);

			YearCount = 0;

			OpenForBusiness = true;
			a.StdOrder = StepSize;
			LowerLimit = ReorderPoint;


			EventCUS += new CustomerHandler(a.HeardIt);
			EventACME += new ACMEHandler(b.HeardIt);
			EventMAN += new ManufacturerHandler(a.UpdateStock);
			//EventCancelOrder += new CustomerHandler(a.CancelOrder);

			for (day = 1; day <= 365; day++)
			{
				Console.WriteLine("\n\t\t Day {0}", day);
				DayCount = 0;
				if (a.stock <= 0 && (day < b.ShipDay))
				{
					Console.WriteLine("NO ORDERS UNTIL THE STOCK IS REPLENISHED");
					OpenForBusiness = false;
				}
				else if (((a.stock <= LowerLimit) && (day > b.ShipDay)))
				{
					OpenForBusiness = true;
					b.CalcLeadTime(day, r);
					GenReorder(day, r);
				}
				else if (day == b.ShipDay)
				{
					Console.WriteLine("We are open for Busniness again.");
					OpenForBusiness = true;
					GenShipment();
				}
				if (OpenForBusiness == true)
				{
					for (int d = 0; d < Customers.Length; d++)
					{
						if ((rand.NextDouble()) < (31.0 / 365.0))
						{
							Console.WriteLine("Customer Number {0} ordered a GAD", d + 1);
							GenOrders(day, r);
							if (a.stock < a.OrderSize)
							{
								Console.WriteLine("This order cannot be filled.");
								OpenForBusiness = false;
								//break;
							}
							else
							{
								OpenForBusiness = true;
								YearCount++;
								DayCount++;
							}
						}
					}
					if (YearCount >= 600)

						//OpenForBusiness = false;
						break;

				}
				Console.WriteLine("The Number of orders on day {0} is {1}", day, DayCount);
				a.profit =a.profit - (0.30 * a.stock);

				//sw1.WriteLine("{0}\t{1:#.###e+00}", day, a.profit);
				//sw2.WriteLine("{0}\t{1}", day, a.stock);
			}
			Console.WriteLine("The total number of orders for the year is {0}", YearCount);
			//sw1.WriteLine("{0}\t{1}\t{2}\t{3}", a.StdOrder, LowerLimit, YearCount, a.profit);
			if (a.profit > 1.21e6 && YearCount == 600)
			{
				sw3.WriteLine("{0}\t{1}\t{2}\t{3:#.###e+00}", a.StdOrder, LowerLimit, YearCount, a.profit);
				Console.WriteLine("{0}\t{1}\t{2}\t{3:#.###e+00}", a.StdOrder, LowerLimit, YearCount, a.profit);
			}
			Console.WriteLine("{0}\t{1}\t{2}\t{3}", a.profit,b.ShipDay,b.OrderDay, a.profit);
			sw1.Close();
			sw2.Close();
			sw3.Close();
		}
		public void Reset()
		{
			a.profit = 0;
			a.stock = 9000;
			b.ShipDay = 0;
			a.profit = 0;
			a.OrderSize = 0;
			b.LeadTime = 0;
			b.OrderDay = 0;
			a.OrderSize = 0;
			a.StdOrder = 0;
			OpenForBusiness = true;
		}
		//Events
		public static void GenOrders(int h, Random r)
		{
			CustomerEventArgs e1 = new CustomerEventArgs(h, r);
			OnEventCus(e1);
		}
		public static void OnEventCus(CustomerEventArgs e)
		{
			if (EventCUS != null)
			{
				EventCUS(new object(), e);
			}
		}
		public static void GenReorder(int s, Random r)
		{
			ACMEEventArgs e1 = new ACMEEventArgs();
			OnEventACME(e1);
		}
		public static void OnEventACME(ACMEEventArgs e)
		{
			if (EventACME != null)
			{
				EventACME(new object(), e);
			}
		}
		public static void OnEventMAN(ManufacturerEventArgs e)
		{
			if (EventMAN != null)
			{
				EventMAN(new object(), e);
			}
		}
		public static void GenShipment()
		{
			ManufacturerEventArgs e1 = new ManufacturerEventArgs();
			OnEventMAN(e1);
		}
		public static void OnEventShipment(ManufacturerEventArgs e)
		{
			if (EventMAN != null)
			{
				EventMAN(new object(), e);
			}
		}
		public static void CancelOrder()
		{
			CustomerEventArgs e1 = new CustomerEventArgs();
			OnEventCancelOrder(e1);
		}
		public static void OnEventCancelOrder(CustomerEventArgs e)
		{
			if (EventCancelOrder != null)
			{
				EventCancelOrder(new object(), e);
			}
		}
	}
}