using System;
using System.Collections.Generic;

namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered, isVaccinated;
		int recoveryCounter = 0, contactCounter = 0;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[4][];

		public Hooman(bool s = true, bool i = false, bool r = false, bool v = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
			//isVaccinated = v;
		}

		public static void SpreadDisease(double beta, double gamma, double lambda, double mu, double xi, int howLong, int maxDegree) {
			int contactTime = (int)(1 / beta), recoveryTime = (int)(1 / gamma), sAmount = 0, iAmount = 0, rAmount = 0, vAmount = 0;
			Random rand = new Random();
			int birthCounter = 0, deathCounter = 0, maxBirth = 1, maxDeath = 1, stopBirthCounter = 0, stopDeathCounter = 0, length_b = 0, length_d = 0;
			bool birth = false, death = false;
			for (int i = 0; i < 5; i++)
				graphdata[i] = new double[howLong];

			for (int i = 0; i < howLong; i++) {
				sAmount = 0;
				iAmount = 0;
				rAmount = 0;
				//vAmount = 0;

				if (stopBirthCounter == (int)((Math.Pow(10, length_b))/maxBirth)) {
					birth = false;
					stopBirthCounter = 0;
					birthCounter = 0;
				}

				if (birth == false) {
					string maxBirth_ = Convert.ToString(lambda);
					length_b = maxBirth_.Length - 2;
					maxBirth = (int)(lambda * Math.Pow(10, length_b));
					birth = true;
				}

	
				if (birth == true) {				
					if (birthCounter < 1) {
						//rodzimy się i ewentualnie szczepimy
						if (rand.NextDouble() < xi)
							society.AddNodeWithRandomEdges(maxDegree, new Hooman(false, false, false, true));        //narodziny + szczepienie
						else
							society.AddNodeWithRandomEdges(maxDegree, new Hooman());                             // narodziny bez szczepienia
						birthCounter++;
					}
					stopBirthCounter++;
				}
				
				//Console.WriteLine("stopbirthCounter: " + stopBirthCounter);
				//Console.WriteLine();

				foreach (Node<Hooman> hooman in society.Nodes) {
					if (hooman.Data.isInfected) {                               //rozprzestrzeniamy choróbska
						hooman.Data.contactCounter++;
						if (hooman.Data.contactCounter > contactTime) {
							hooman.Data.contactCounter = 0;
							foreach (Node<Hooman> neighbor in hooman.Neighbours) {
								if (neighbor.Data.isSusceptible) {
									neighbor.Data.isSusceptible = false;
									neighbor.Data.isInfected = true;
								}
							}
						}
						hooman.Data.recoveryCounter++;
						if (hooman.Data.recoveryCounter > recoveryTime) {       //i wychodzimy z nich zdrowi (czasem)
							hooman.Data.isInfected = false;
							hooman.Data.isRecovered = true;
						}
					}
				}

				if (stopDeathCounter == (int)((Math.Pow(10, length_d)/maxDeath))) {
					death = false;
					stopDeathCounter = 0;
					deathCounter = 0;
				}

				if (death == false) {
					string maxDeath_ = Convert.ToString(mu);
					length_d = maxDeath_.Length - 2;
					maxDeath = (int)(mu * Math.Pow(10, length_d));
					death = true;
				}

				if (death == true) {
					if (deathCounter < 1) {
						//usmiercamy
						society.RemoveNode(society.Nodes[rand.Next(0, society.Nodes.Count)]);
						deathCounter++;
					}
					stopDeathCounter++;
				}
				
				//Console.WriteLine("stopdeathCounter: " + stopDeathCounter);
				//Console.WriteLine();

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
