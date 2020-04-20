using System;

namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered, isVaccinated;
		int recoveryCounter = 0, contactCounter = 0;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[5][];

		public Hooman(bool s = true, bool i = false, bool r = false, bool v = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
			isVaccinated = v;
		}

		public static void SpreadDisease(double beta, double gamma, double lambda, double mu, double xi, int howLong, int maxDegree) {
			int contactTime = (int)(1 / beta), recoveryTime = (int)(1 / gamma), sAmount = 0, iAmount = 0, rAmount = 0, vAmount = 0;
			Random rand = new Random();
			for (int i = 0; i < 5; i++)
				graphdata[i] = new double[howLong];

			for (int i = 0; i < howLong; i++) {
				sAmount = 0; 
				iAmount = 0; 
				rAmount = 0; 
				vAmount = 0;
				if (rand.NextDouble() < lambda) {                               //rodzimy się i ewentualnie szczepimy
					if (rand.NextDouble() < xi)
						society.AddNodeWithRandomEdges(maxDegree, new Hooman(false, false, false, true));
					else
						society.AddNodeWithRandomEdges(maxDegree, new Hooman());
				}

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

				for (int a = 0; a < society.Nodes.Count; a++) {
					if (rand.NextDouble() < mu) {                               //umieramy (trzeba umierać poza pętlą, bo wywala się błąd, że edycja kolekcji)
						society.RemoveNode(society.Nodes[a]);
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
