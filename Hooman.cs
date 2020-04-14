namespace Epidemic_Models {
	class Hooman {
		public bool isSusceptible, isInfected, isRecovered;
		int recoveryCounter = 0, contactCounter = 0;
		public static Graph<Hooman> society = new Graph<Hooman>(false, false);
		public static double[][] graphdata = new double[4][];

		public Hooman(bool s = true, bool i = false, bool r = false) {
			isSusceptible = s;
			isInfected = i;
			isRecovered = r;
		}

		public static void SpreadDisease(double beta, double gamma, int howLong) {
			int contactTime = (int)(1 / beta), recoveryTime = (int)(1 / gamma);
			for (int i = 0; i < 4; i++)
				graphdata[i] = new double[howLong];
			for (int i = 0; i < howLong; i++) {
				foreach (Node<Hooman> hooman in society.Nodes) {
					if (hooman.Data.isInfected) {
						System.Console.WriteLine("this boi is infected: " + hooman.Index);
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
						if (hooman.Data.recoveryCounter > recoveryTime) {
							hooman.Data.isInfected = false;
							hooman.Data.isRecovered = true;
						}
					}
				}

				//policzmy ich
				int sAmount = 0, iAmount = 0, rAmount = 0;
				foreach (Node<Hooman> hooman in society.Nodes) {
					if (hooman.Data.isSusceptible)
						sAmount++;
					else if (hooman.Data.isInfected)
						iAmount++;
					else
						rAmount++;
				}
				//System.Console.WriteLine(i + ", " + sAmount + ", " + iAmount + ", " + rAmount);
				graphdata[0][i] = i;		//czas
				graphdata[1][i] = sAmount;
				graphdata[2][i] = iAmount;
				graphdata[3][i] = rAmount;
			}
		}
	}
}
