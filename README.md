# Epidemic-Models
Program wykonany w ramach projektu na Laboratorium Specjalistyczne.

# Trochę teorii
Modele przedziałowe (compartmental models) upraszczają modelowanie matematyczne chorób zakaźnych. Symulowana populacja jest przydzielana do pewnych grup (przedziałów) z oznaczeniami np. M, S, E, I, R, C lub D (*immune via **M**aternal antibodies*, ***S**usceptible*, ***E**xposed*, ***I**nfected*, ***R**ecovered*, ***C**arrier* oraz ***D**eceased*). Kolejność liter w nazwie modelu zazwyczaj pokazuje proces przechodzenia osobnika danej populacji przez różne stany, np. SIS, SIR czy MSEIRS. 

Poniższy program pozwala na zbadanie wpływu szczepień na dynamikę epidemii choroby zakaźnej opisanej modelem SI(V)R (***V**accinated*). Równania opisujące ów model:

![\frac{dS}{dt} = \Lambda - \frac{\beta I S}{N} - (\mu + \xi)S](https://render.githubusercontent.com/render/math?math=%5Cfrac%7BdS%7D%7Bdt%7D%20%3D%20%5CLambda%20-%20%5Cfrac%7B%5Cbeta%20I%20S%7D%7BN%7D%20-%20(%5Cmu%20%2B%20%5Cxi)S)

![\frac{dI}{dt} = \frac{\beta I S}{N} - (\mu + \gamma)I](https://render.githubusercontent.com/render/math?math=%5Cfrac%7BdI%7D%7Bdt%7D%20%3D%20%5Cfrac%7B%5Cbeta%20I%20S%7D%7BN%7D%20-%20(%5Cmu%20%2B%20%5Cgamma)I)

![\frac{dV}{dt} = \xi S - \mu V](https://render.githubusercontent.com/render/math?math=%5Cfrac%7BdV%7D%7Bdt%7D%20%3D%20%5Cxi%20S%20-%20%5Cmu%20V)

![\frac{dR}{dt} = \gamma I - \mu R](https://render.githubusercontent.com/render/math?math=%5Cfrac%7BdR%7D%7Bdt%7D%20%3D%20%5Cgamma%20I%20-%20%5Cmu%20R)

gdzie:

![\Lambda](https://render.githubusercontent.com/render/math?math=%5CLambda) - współczynnik urodzeń

![\mu](https://render.githubusercontent.com/render/math?math=%5Cmu) - współczynnik zgonów

![\beta = \beta_c\beta_i](https://render.githubusercontent.com/render/math?math=%5Cbeta%20%3D%20%5Cbeta_c%5Cbeta_i) - współczynnik zarażeń

![\gamma](https://render.githubusercontent.com/render/math?math=%5Cgamma) - współczynnik wyzdrowień

![\xi](https://render.githubusercontent.com/render/math?math=%5Cxi) - współczynnik szczepień

Ponadto na współczynnik ![\beta](https://render.githubusercontent.com/render/math?math=%5Cbeta) składają się dwie wielkości: ![\beta_c](https://render.githubusercontent.com/render/math?math=%5Cbeta_c) - ilość kontaktów na osobnika w populacji i ![\beta_i](https://render.githubusercontent.com/render/math?math=%5Cbeta_i) - p-stwo zarażenia osobnika podatnego (**S**) podczas kontaktu z osobnikiem zarażonym (**I**).

# Praktyka
Symulację przeprowadzamy w oparciu o nieważony i nieskierowany graf losowy o N wierzchołkach i maksymalnym stopniu wierzchołka ![\frac{N}{10}](https://render.githubusercontent.com/render/math?math=%5Cfrac%7BN%7D%7B10%7D). Interpretacja parametrów z poprzedniej sekcji jest następująca:

![\Lambda](https://render.githubusercontent.com/render/math?math=%5CLambda) - szansa na pojawienie się jednego nowego podatnego osobnika w danym kroku czasowym

![\mu](https://render.githubusercontent.com/render/math?math=%5Cmu) - szansa na usunięcie dowolnego jednego osobnika w danym kroku czasowym

![\beta_c](https://render.githubusercontent.com/render/math?math=%5Cbeta_c) - stopień wierzchołka reprezzentującego danego osobnika

![\beta_i](https://render.githubusercontent.com/render/math?math=%5Cbeta_i) - szansa na zarażenie osobnika podatnego

![\gamma](https://render.githubusercontent.com/render/math?math=%5Cgamma) - odwrotność czasu upływającego od momentu zarażenia do wyzdrowienia (przejścia w stan **R**)

![\xi](https://render.githubusercontent.com/render/math?math=%5Cxi) - szansa na zaszczepienie osobnika podatnego w danym kroku czasowym

Dane na wykresie wykreślamy po uśrednieniu 1000 symulacji.
