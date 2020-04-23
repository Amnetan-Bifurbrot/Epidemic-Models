using System;
using System.Collections.Generic;

namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered, isVaccinated;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[4][];

		public Hooman(bool s = true, bool i = false, bool r = false, bool v = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
			//isVaccinated = v;
		}

		public static double[][] SpreadDisease(double beta, double gamma, int howLong, int maxDegree) {
			int sAmount = 0, iAmount = 0, rAmount = 0, vAmount = 0;
			Random rand = new Random();
			List<int> infectedList, healthyList, infectedListTmp;

			for (int i = 0; i < 4; i++)
				graphdata[i] = new double[howLong];

			for (int i = 0; i < howLong; i++) {
				sAmount = 0;
				iAmount = 0;
				rAmount = 0;
				//vAmount = 0;

				if (i != 0) {
					infectedList = new List<int>(); //przechowuje indeksy chorych
					healthyList = new List<int>();//przechowuje indeksy zdrowych
					infectedListTmp = new List<int>();
					// dodajemy indexy zainfekowanych osob do listy
					// liczyymy ile zdrowych jest i ile chorych i dodajemy do list
					foreach (Node<Hooman> hooman in society.Nodes) {
						if (hooman.Data.isInfected == true) {
							infectedList.Add(hooman.Index);
							infectedListTmp.Add(hooman.Index);
						}
						if (hooman.Data.isSusceptible == true) {
							healthyList.Add(hooman.Index);
						//	Console.Write(hooman.Index + "\t");
						}

					}
					//Console.WriteLine();

					int counter = 0;
					foreach (int index in infectedListTmp) {
						foreach (Node<Hooman> neightbour in society.Nodes[index].Neighbours) {
							double p = rand.NextDouble();
							if (p < beta) {
								neightbour.Data.isInfected = true;
								neightbour.Data.isSusceptible = false;
								infectedList.Add(neightbour.Index);
								counter++;
							}
						}
					}
					//Console.WriteLine("infectedTmp: " + infectedListTmp.Count);
					//Console.WriteLine("infected: " + infectedList.Count);
					//Console.WriteLine("dodam tyle: " + counter);

					int counter2 = 0;
					foreach (int index in infectedList) {

						double p = rand.NextDouble();
						if (p < gamma) {
							society.Nodes[index].Data.isRecovered = true;
							society.Nodes[index].Data.isInfected = false; ;
							counter2++;
						}
					}
					//Console.WriteLine("infectedTmp: " + infectedListTmp.Count);
					//Console.WriteLine("infected: " + infectedList.Count);
					//Console.WriteLine("dodam tyle2: " + counter2);

				}
					//policzmy ich
					foreach (Node<Hooman> hooman in society.Nodes) {
						if (hooman.Data.isSusceptible)
							sAmount++;
						else if (hooman.Data.isInfected)
							iAmount++;
						else if (hooman.Data.isRecovered)
							rAmount++;
						else
							vAmount++;
					}

					graphdata[0][i] = i;        //czas
					graphdata[1][i] = sAmount;
					graphdata[2][i] = iAmount;
					graphdata[3][i] = rAmount;
					//graphdata[4][i] = vAmount;

				
			}
			return graphdata;
		}
	}
}
