using System;

namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered, isVaccinated;
		int recoveryCounter = 0;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[5][];

		public Hooman(bool s = true, bool i = false, bool r = false, bool v = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
			isVaccinated = v;
		}

		public static void SpreadDisease(double beta, double gamma, double lambda, double mu, double xi, int howLong, int maxDegree) {
			int recoveryTime = (int)(1 / gamma), sAmount = 0, iAmount = 0, rAmount = 0, vAmount = 0;
			Random rand = new Random();

			for (int i = 0; i < 5; i++)
				graphdata[i] = new double[howLong];

			for (int i = 0; i < howLong; i++) {
				sAmount = 0;
				iAmount = 0;
				rAmount = 0;
				vAmount = 0;


				foreach (Node<Hooman> hooman in society.Nodes) {
					//liczenie
					if (hooman.Data.isSusceptible)
						sAmount++;
					else if (hooman.Data.isInfected)
						iAmount++;
					else if (hooman.Data.isRecovered)
						rAmount++;
					else
						vAmount++;


					if (hooman.Data.isSusceptible) {
						foreach (Node<Hooman> neighbor in hooman.Neighbours) {
							if (neighbor.Data.isInfected && rand.NextDouble() < beta / hooman.Neighbours.Count) { //zarażanie
								hooman.Data.isSusceptible = false;
								hooman.Data.isInfected = true;
								break;
							}
							if (rand.NextDouble() < xi / 2) {       //SZCZEPIMYYYYYYYYYYYYYYY
								hooman.Data.isSusceptible = false;
								hooman.Data.isVaccinated = true;
								break;
							}
						}
					} else if (hooman.Data.isInfected) {
						if (hooman.Data.recoveryCounter > recoveryTime) {   //wyzdrawianie
							hooman.Data.isInfected = false;
							hooman.Data.isRecovered = true;
						}
						hooman.Data.recoveryCounter++;
					}
				}

				//robimy ludziuf
				if (rand.NextDouble() < lambda) society.AddNodeWithRandomEdges(maxDegree, new Hooman());

				//umieramy ludziuf
				for (int j = 0; j < society.Nodes.Count; j++) {
					if (rand.NextDouble() < mu) society.RemoveNode(society.Nodes[j]);
				}

				//zapis danych
				graphdata[0][i] = i;        //czas
				graphdata[1][i] = sAmount;
				graphdata[2][i] = iAmount;
				graphdata[3][i] = vAmount;
				graphdata[4][i] = rAmount;
			}
		}
	}
}
