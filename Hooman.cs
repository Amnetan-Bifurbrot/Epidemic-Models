using System;
using System.Collections.Generic;

namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered, isVaccinated;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[5][];

		public Hooman(bool s = true, bool i = false, bool r = false, bool v = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
			isVaccinated = v;
		}

		public static void SpreadDisease(double beta, double gamma, int howLong, int maxDegree) {
			int sAmount = 0, iAmount = 0, rAmount = 0, vAmount = 0;
			Random rand = new Random();
			List<int> infectedList, healthyList;

			for (int i = 0; i < 5; i++)
				graphdata[i] = new double[howLong];

			for (int i = 0; i < howLong; i++) {
				sAmount = 0;
				iAmount = 0;
				rAmount = 0;
				vAmount = 0;
				
				if (i != 0) {
					infectedList = new List<int>();	//przechowuje indeksy chorych
					healthyList = new List<int>();//przechowuje indeksy zdrowych

					// dodajemy indexy zainfekowanych osob do listy
					// liczyymy ile zdrowych jest i ile chorych i dodajemy do list
					foreach (Node<Hooman> hooman in society.Nodes) {
						if (hooman.Data.isInfected == true) {
							infectedList.Add(hooman.Index);
						}
						if (hooman.Data.isSusceptible == true) {
							healthyList.Add(hooman.Index);
							Console.Write(hooman.Index + "\t");
						}

					}
					Console.WriteLine();
					// mamy zainfekowac Ni ludziuf
					double Ni_ = healthyList.Count * infectedList.Count * beta;
					decimal fraction = (decimal)Ni_;
					decimal dPart = (2 * fraction) % 1.0m;
					int Ni;
					if (decimal.ToDouble(dPart) >= 0.5) {
						Ni = (int)Math.Round(Ni_, 0, MidpointRounding.AwayFromZero);
					} else {
						Ni = (int)Math.Round(Ni_, 0, MidpointRounding.ToEven);
					}
					//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% tu raz działa raz nie, cos poza tablice wychodzi :(
					Console.WriteLine("Ni: " + Ni);
					// chorujemy ludziuf
					int counter = 0;
					while (counter < Ni && (Ni < healthyList.Count)) {
						//losujemy osobe do zarazenia z listy zdrowych, number - index tej osoby w healthyList
						if (healthyList.Count == 0) break;
						int number = rand.Next(healthyList.Count);

						Console.WriteLine();
						Console.WriteLine("ilosc zdrowych: " + healthyList.Count);
						Console.WriteLine("number: " + number);

						society.Nodes[healthyList[number]].Data.isInfected = true;
						society.Nodes[healthyList[number]].Data.isSusceptible = false;
						counter++;
						Console.WriteLine();
						
						Console.WriteLine("counter w 0: " + counter);
						if (counter == Ni) break;
						// dla danej osoby zdrowej zarażamy jej sasiadow
						if (Ni <= society.Nodes[healthyList[number]].Neighbours.Count) {

							for (int j = 0; j < Ni; j++) {
								//if (counter == Ni) break;
								//jesli sasiad jest zdrowy to zaraz zachoruje
								if(society.Nodes[healthyList[number]].Neighbours[j].Data.isSusceptible == true) {
									
									Console.WriteLine("jestem1");
									
									society.Nodes[healthyList[number]].Neighbours[j].Data.isInfected = true;
									society.Nodes[healthyList[number]].Neighbours[j].Data.isSusceptible = false;
									counter++;
									//znajdz numer sasiada w liscie zdrowych
									//int result = healthyList.Find(item => item == society.Nodes[healthyList[number]].Neighbours[j].Index);
									
									Console.WriteLine("ilosc zdrowych w 1: " + healthyList.Count);
									
									Console.WriteLine("index zdrowego w 1: " + society.Nodes[healthyList[number]].Neighbours[j].Index);
									var index = healthyList.FindIndex(w => w == society.Nodes[healthyList[number]].Neighbours[j].Index);
									if (index >= 0) {   // ensure item found
										healthyList.RemoveAt(index);
									}
									Console.WriteLine("index1: " + index);
									Console.WriteLine("counter w 1: " + counter);
									//if (counter == Ni) break;
								}
									
							}

						} else if (Ni > society.Nodes[healthyList[number]].Neighbours.Count) {
							foreach (Node<Hooman> neighbour in society.Nodes[healthyList[number]].Neighbours) {
								Console.WriteLine("jestem2");
								//if (counter == Ni) break;
								if(neighbour.Data.isSusceptible == true) {
									neighbour.Data.isInfected = true;
									neighbour.Data.isSusceptible = false;
									counter++;
									Console.WriteLine("ilosc zdrowych w 2: " + healthyList.Count);
									
									Console.WriteLine("index zdrowego w 2: " + neighbour.Index);
									var index = healthyList.FindIndex(w => w == neighbour.Index); 
									if (index >= 0) {   // ensure item found
										healthyList.RemoveAt(index);
									}
									Console.WriteLine("index2: " + index);

									Console.WriteLine("counter w 2: " + counter);
									//if (counter == Ni) break;
								}

								
							}
						}

						foreach(int item in healthyList) {
							Console.Write(item + "\t");
						}
						Console.WriteLine();
						Console.WriteLine("ilosc zdrowych: " + healthyList.Count);
						Console.WriteLine("number: " + number);
						// COS SIE PIEPRZY Z INDEXAMI< BO WYWALA POZA TABLICE
						var index2 = healthyList.FindIndex(w => w == society.Nodes[healthyList[number]].Index);
						if (index2 >= 0) {   // ensure item found
							healthyList.RemoveAt(index2);
						}
					}
					//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

					//ilosc osob do wyzdrowienia
					double Nr_ = gamma * infectedList.Count;
					decimal fraction2 = (decimal)Nr_;
					decimal dPart2 = (2 * fraction2) % 1.0m;
					int Nr;
					if (decimal.ToDouble(dPart2) >= 0.5) {
						Nr = (int)Math.Round(Nr_, 0, MidpointRounding.AwayFromZero);
					} else {
						Nr = (int)Math.Round(Nr_, 0, MidpointRounding.ToEven);
					}
					// wyzdrawianie
					for (int k = 0; k < Nr; k++) {
						int number = rand.Next(0, infectedList.Count);

						society.Nodes[infectedList[number]].Data.isRecovered = true;
						society.Nodes[infectedList[number]].Data.isInfected = false;
						infectedList.RemoveAt(number);
					}
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
				graphdata[4][i] = vAmount;

			}
		}
	}
}
