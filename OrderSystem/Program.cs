using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderSystem
{
	public delegate void CustomerHandler(object o, CustomerEventArgs e);
	public delegate void ACMEHandler(object o, ACMEEventArgs e);
	public delegate void ManufacturerHandler(object o, ManufacturerEventArgs e);

	public class CustomerEventArgs : EventArgs      //Customer class
	{
		public int OrderNum=0;                      //Size of the customer order

		public int RandomNumber(int min, int max, int day, Random r)
		{
			return (100 + (day / 365 * (200 - 100)) + r.Next(-10,10));  //OrderNum increaes as the year goes by
		}

		public CustomerEventArgs(int f, Random r)
		{
			OrderNum = RandomNumber(-10, 10, f, r); //Randomly assign the OrderNum
		}
		public CustomerEventArgs()      //Default Constructor
		{

		}
	}

	public class ACMEEventArgs : EventArgs          //ACME class
	{
		public int stock = 9000;                    //Initial Stock level
		public int StdOrder;                        //Amount to be reordered
		public double profit=0;                     //Initial Profit
		public int OrderSize=0;                     //The number of GAD a customer ordered

		public void HeardIt(object o, CustomerEventArgs e)
		{//When a customer places an order, ACME can either fill it or do not have enough stock to fill it.
			//Some messages to the screen have been disabled but, if you wish to see every transaction for every
			//day of the year, then uncomment them.
			OrderSize = e.OrderNum;
			if (OrderSize > stock)
			{
				//Console.WriteLine("We do not have enough stock to fill this order.");
				//profit -= 0.30*stock;
			}
			else
			{
				//Console.WriteLine("The Customer ordered {0} GAD", OrderSize);
				//Console.WriteLine("The remaining stock is {0}", stock - OrderSize);

				profit += ((OrderSize * 25));   //Calculate the profit from as successful order

				//Console.WriteLine("The Profits are: {0}\n", profit);
				stock -= OrderSize;
			}
		}
		public void UpdateStock(object o, ManufacturerEventArgs e)
		{//When a new shipment arrives, the stock is updated on that day.
			stock += StdOrder;
			//Console.WriteLine("->A new shipment arrived! The stock level is now {0}",stock);
		}
	}       

	public class ManufacturerEventArgs : EventArgs  //Manufacturer class
	{
		public int LeadTime;    //The number of days to fill a new shipment to ACME
		public int ShipDay;     //The day the shipment will arrive at ACME
		public int OrderDay;    //The day the order was first placed

		public ManufacturerEventArgs(int day, Random r)
		{

		}
		public ManufacturerEventArgs()
		{

		}

		public int RandomNumber(int day, Random r)
		{
			return (25 + r.Next(-10, 10));      //Random number generator
		}
		public void CalcLeadTime(int day, Random r)
		{
			OrderDay = day;                     
			LeadTime = (RandomNumber(day, r));  //Randomly calculate the LEadTime
			ShipDay = OrderDay + LeadTime;      //Determine when to dispatch the new shipment to ACME
		}
		public void HeardIt(object o, ACMEEventArgs e)
		{//When ACME places an order for a new shipment
			//and that shipment can be delivered before the end of the year,
			//the order fee of 2000 is taken away on that day. Or else, no fee is charged if the
			//shipment will not arrive before the end of the year.
			if (ShipDay > 365)
			{
				//Console.WriteLine("The shipment will not arrive before the end of the year!");
				//Console.WriteLine("The shipment has been cancelled and the order fee has not been deducted.");
			}
			else
			{
				//Console.WriteLine("We are sending you {0} GAD in {1} days!!!",e.StdOrder, LeadTime);
				e.profit -= 2000.0;
			}
		}
	}

	class Program
	{
		public static int i;
		public static int j;

		static void Main(string[] args)
		{
			for (j = 1000; j <= 10000; j += 100)
			{
				for (i = 1000; i <= 10000; i += 100)
				{
					Model z = new Model();      //Creae a new Model of the ACME/Manufacturer/Customer environment
					z.Run(i,j);                 //Run the sim.
				}
			}
			/*StreamReader sr1 = new StreamReader(@"Z:\DataPoints.xls");
            while ((line = sr1.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }*/
		}
	}
}