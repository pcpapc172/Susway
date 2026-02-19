using System;
using System.Collections.Generic;

namespace Extra
{
	public class TextGame
	{
		public static Dictionary<TextKey, Translation> mTranslation;

		public static void Init()
		{
			if (mTranslation == null)
			{
				mTranslation = new Dictionary<TextKey, Translation>();
				try
				{
					mTranslation.Add(new TextKey("1TitleTip", "Here's a tip!"), new Translation("Hier ist ein Tipp!", "Es un consejo!", "Voici un conseil!", "Ecco un consiglio!", "Aqui vai uma dica!"));
					mTranslation.Add(new TextKey("Label", "Missions"), new Translation("Missionen", "Misiones", "Missions", "missioni", "Missoes"));
					mTranslation.Add(new TextKey("2Title", "2X MULTIPLIER"), new Translation("MULTIPLIKATOR", "MULTIPLICADOR", "MULTIPLICATEUR", "MOLTIPLICATORE", "MULTIPLADOR"));
					mTranslation.Add(new TextKey("Desc", "Affects all sounds and music"), new Translation("Klange und Musik", "Los sonidos y musica", "Les sons et la musique", "I suoni e la musica", "Os sons e musica"));
					mTranslation.Add(new TextKey("Title", "Boosts"), new Translation("Shop", "Shop", "Shop", "Shop", "Shop"));
					mTranslation.Add(new TextKey("Title", "Buy"), new Translation("kauf", "OK", "OK", "OK", "OK"));
					mTranslation.Add(new TextKey("Title", "Characters"), new Translation("Figuren", "Caracteres", "Personnages", "Personaggi", "Personagens"));
					mTranslation.Add(new TextKey("2Title", "COIN MAGNET"), new Translation("MUNZEN MAGNET", "IMAN", "AIMANT", "MAGNETE", "IMA DE MOEDA"));
					mTranslation.Add(new TextKey("3CollectLabel", "Collect letters"), new Translation("Briefe sammeln", "Coleccionar cartas", "Recueillir des lettres", "Raccogli le lettere", "Coletar cartas"));
					mTranslation.Add(new TextKey("3CollectLabel", "Collect Letters"), new Translation("Briefe sammeln", "Coleccionar cartas", "Recueillir des lettres", "Raccogli le lettere", "Coletar cartas"));
					mTranslation.Add(new TextKey("bottomLabel", "Come back and win: "), new Translation("Komm und gewinne: ", "Regresa y gana: ", "Revenir et gagner: ", "Ritorna e vinci: ", "Volte e venca: "));
					mTranslation.Add(new TextKey("bottomLabel", "Come back and win:"), new Translation("Komm und gewinne:", "Regresa y gana:", "Revenir et gagner:", "Ritorna e vinci:", "Volte e venca:"));
					mTranslation.Add(new TextKey("1title", "Daily Challenge"), new Translation("Taglich Mission", "Reto diario", "Du Quotidien", "Sfida Oggi", "Desafio diario"));
					mTranslation.Add(new TextKey("Label", "Daily Challenge"), new Translation("Taglich Mission", "Reto diario", "Du Quotidien", "Sfida Oggi", "Desafio diario"));
					mTranslation.Add(new TextKey("line", "Daily Challenge #complete"), new Translation("Taglich Mission#complete", "Reto diario#completo", "Du Quotidien#complete", "Sfida Oggi#completo", "Desafio diario#completo"));
					mTranslation.Add(new TextKey("1DayLabel", "Today"), new Translation("Heute", "Hoy", "Jour 1", "Oggi", "Hoje"));
					mTranslation.Add(new TextKey("1DayLabel", "Day 1"), new Translation("Tag 1", "Dia 1", "Jour 1", "Giorno 1", "Dia 1"));
					mTranslation.Add(new TextKey("1DayLabel", "Day 2"), new Translation("Tag 2", "Dia 2", "Jour 2", "Giorno 2", "Dia 2"));
					mTranslation.Add(new TextKey("1DayLabel", "Day 3"), new Translation("Tag 3", "Dia 3", "Jour 3", "Giorno 3", "Dia 3"));
					mTranslation.Add(new TextKey("1DayLabel", "Day 4"), new Translation("Tag 4", "Dia 4", "Jour 4", "Giorno 4", "Dia 4"));
					mTranslation.Add(new TextKey("5DayLabel", "Day 5+"), new Translation("Tag 5+", "Dia 5+", "Jour 5+", "Giorno 5+", "Dia 5+"));
					mTranslation.Add(new TextKey("Message", "Double Tap for*#Hoverboard"), new Translation("Drucke zweimal fur#Hoverboard", "Doble toque para#Hoverboard", "Double Tap pour#Hoverboard", "Doppio tocco per#Hoverboard", "Toque duas vezes para#um Hoverboard"));
					mTranslation.Add(new TextKey("desc2", "Double tap to activate when running."), new Translation("Drucke zweimal um zum Aktivieren beim Laufen.", "Toque dos veces para activarla cuando se este ejecutando.", "Appuyez deux fois pour l'activer lors de l'execution.", "Toccare due volte per attivare durante l'esecuzione.", "Toque duas vezes para ativar quando estiver funcionando."));
					mTranslation.Add(new TextKey("2Tip", "Earn coins for your friends#by making multiple runs!"), new Translation("Verdienen Sie Munzen fur Ihre Freunde#, indem Sie mehrere Laufe machen!", "Gana monedas para tus amigos#haciendo multiples tiradas!", "Gagnez des pieces pour vos amis#en effectuant plusieurs tirages!", "Guadagna monete per i tuoi amici#facendo piu run!", "Ganhe moedas para seus amigos#fazendo varias corridas!"));
					mTranslation.Add(new TextKey("Message", "Fresh Moves!"), new Translation("Frische Umzuge!", "Movimientos frescos!", "Ce sont des mouvements cool!", "Movimenti freschi!", "Movimentos muito legais!"));
					mTranslation.Add(new TextKey("Label1", "Get"), new Translation("Bekommen", "Obtener", "Obtenir", "Ottenere", "Obter"));
					mTranslation.Add(new TextKey("2Title", "HEADSTART"), new Translation("VORSPRUNG", "COMIENZO", "DEBUT", "PARTENZA", "COMECO"));
					mTranslation.Add(new TextKey("Title", "Home"), new Translation("Zuhause", "Casa", "Accueil", "Casa", "Casa"));
					mTranslation.Add(new TextKey("1Title", "Hoverboard"), new Translation("Hoverboard", "Hoverboard", "Hoverboard", "Hoverboard", "Hoverboard"));
					mTranslation.Add(new TextKey("2Title", "HOVERBOARD"), new Translation("HOVERBOARD", "HOVERBOARD", "HOVERBOARD", "HOVERBOARD", "HOVERBOARD"));
					mTranslation.Add(new TextKey("3Desc", "Increases the duration of the Coin Magnet pickup."), new Translation("Erhoht die Dauer des Munzmagneten.", "Aumenta la duracion de la recoleccion de iman de monedas.", "Augmente la duree du ramassage d'imbibes de pieces.", "Aumenta la durata del pickup Magnet Coin.", "Aumenta a duracao do captador de ima de moeda."));
					mTranslation.Add(new TextKey("3Desc", "Increases the duration of the Double Multiplier pickup."), new Translation("Erhoht die Dauer des Double Multiplier Pickups.", "Aumenta la duracion de la recoleccion de doble multiplicador.", "Augmente la duree du prelevement double multiplicateur.", "Aumenta la durata del raccoglitore del doppio moltiplicatore.", "Aumenta a duracao do captador de Multiplicadores Duplos."));
					mTranslation.Add(new TextKey("3Desc", "Increases the duration of the Spray Can Jetpack pickup."), new Translation("Erhoht die Dauer der Spraydose Jetpack Pickup.", "Aumenta la duracion de la pastilla Spray Can Jetpack.", "Augmente la duree du prelevement Jetpack Jet Jet.", "Aumenta la durata del raccoglitore Jet Jet Spray.", "Aumenta a duracao do recolhimento Jetpack Jet Jet."));
					mTranslation.Add(new TextKey("3Desc", "Increases the duration of the Super Sneakers."), new Translation("Erhoht die Dauer der Super Sneakers.", "Aumenta la duracion de los Super Sneakers.", "Augmente la duree des Super Sneakers.", "Aumenta la durata delle scarpe da ginnastica super.", "Aumenta a duracao do Super Sneakers."));
					mTranslation.Add(new TextKey("1name", "Jake"), new Translation("Jake", "Jake", "Jake", "Jake", "Jake"));
					mTranslation.Add(new TextKey("2Title", "JETPACK"), new Translation("JETPACK", "JETPACK", "JETPACK", "JETPACK", "MOCHILA A JATO"));
					mTranslation.Add(new TextKey("3Loading", "LOADING..."), new Translation("LADEN...", "CARGANDO...", "CHARGEMENT...", "CARICAMENTO...", "CARREGANDO..."));
					mTranslation.Add(new TextKey("2Title", "MEGA HEADSTART"), new Translation("MEGA VORSPRUNG", "MEGA COMIENZO", "MEGA DEBUT", "MEGA PARTENZA", "MEGA COMECO"));
					mTranslation.Add(new TextKey("Title", "Menu"), new Translation("Menu", "Menu", "Menu", "Menu", "Cardapio"));
					mTranslation.Add(new TextKey("line2", "Mission Complete"), new Translation("Mission komplett", "Mision cumplida", "Mission accomplie", "Completata", "Missao completa"));
					mTranslation.Add(new TextKey("line", "Mission Set#complete"), new Translation("Mission Set#komplett", "Mision#cumplida", "Mission Set#complete", "Mission Set#completato", "Missoes#complete"));
					mTranslation.Add(new TextKey("1mainTitle", "Missions"), new Translation("Missionen", "Misiones", "Missions", "missioni", "Missoes"));
					mTranslation.Add(new TextKey("11missiondescr", "Missions can be Skipped in the Shop"), new Translation("Missionen konnen ubersprungen werden", "Las misiones se pueden omitir en la tienda", "Missions peuvent etre ignorees dans boutique", "Le missioni possono essere saltate in negozio", "Missoes podem ser puladas na loja"));
					mTranslation.Add(new TextKey("10Missions Label", "Missions"), new Translation("Missionen", "Misiones", "Missions", "Missioni", "Missoes"));
					mTranslation.Add(new TextKey("4rewardTextLine2", "Multiplier"), new Translation("Multiplikator", "Multiplicador", "Multiplicateur", "Moltiplicatore", "Multiplicador"));
					mTranslation.Add(new TextKey("4rewardTextLine2Shadow", "Multiplier"), new Translation("Multiplikator", "Multiplicador", "Multiplicateur", "Moltiplicatore", "Multiplicador"));
					mTranslation.Add(new TextKey("2Title", "MYSTERY BOX"), new Translation("MYSTERY BOX", "MYSTERY BOX", "MYSTERY BOX", "MYSTERY BOX", "MYSTERY BOX"));
					mTranslation.Add(new TextKey("topLabel", "NEXT CHALLENGE IN"), new Translation("NACHSTE IN", "SIGUIENTE RETO", "PROCHAIN DEFI", "PROSSIMA SFIDA IN", "PROXIMO DESAFIO"));
					mTranslation.Add(new TextKey("HaveAmount", "Opens immediately"), new Translation("Offnet sich sofort", "Se abre inmediatamente", "S'ouvre immediatement", "Apre immediatamente", "Abre imediatamente"));
					mTranslation.Add(new TextKey("Play Label", "PLAY"), new Translation("SPIEL", "JUGAR", "JOUER", "OK", "OK"));
					mTranslation.Add(new TextKey("Resume label", "RESUME"), new Translation("WIEDER", "JUGAR", "JOUER", "AVVIO", "TOQUE"));
					mTranslation.Add(new TextKey("3rewardTitle", "REWARD"), new Translation("BELOHNUNG", "RECOMPENSA", "RECOMPENSE", "RICOMPENSA", "RECOMPENSA"));
					mTranslation.Add(new TextKey("3rewardTitleShadow", "REWARD"), new Translation("BELOHNUNG", "RECOMPENSA", "RECOMPENSE", "RICOMPENSA", "RECOMPENSA"));
					mTranslation.Add(new TextKey("4MissionGoalLine2", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("4MissionGoalLine2Shadow", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("4rewardTextLine1", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("4rewardTextLine1Shadow", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("bestLabel", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("ScoreLabel", "Score"), new Translation("Score", "Score", "Score", "Punto", "Ponto"));
					mTranslation.Add(new TextKey("text", "SELECT"), new Translation("WAHLEN", "SELECCIONAR", "CHOISIR", "SELEZIONARE", "SELECIONE"));
					mTranslation.Add(new TextKey("text", "SELECTED"), new Translation("AUSGEWAHLT", "SELECCIONADO", "CHOISI", "SELEZIONATO", "SELECIONADO"));
					mTranslation.Add(new TextKey("Title", "Settings"), new Translation("Einstellungen", "Ajustes", "Parametres", "Impostazioni", "Configuracoes"));
					mTranslation.Add(new TextKey("3Title", "Settings"), new Translation("Einstellungen", "Configuracion", "Parametres", "Impostazioni", "Configuracoes"));
					mTranslation.Add(new TextKey("000", "Single Use"), new Translation("Einmal", "De un solo uso", "Usage unique", "Monouso", "Uso unico"));
					mTranslation.Add(new TextKey("Title", "Sound ON"), new Translation("Ton an", "Sonido encendido", "Son ON", "Suono acceso", "Som ligado"));
					mTranslation.Add(new TextKey("Title", "Sound OFF"), new Translation("Ton aus", "Sonido apagado", "Son OFF", "Audio Disattivato", "Som silencioso"));
					mTranslation.Add(new TextKey("CountdownStartingLabel", "Starting in"), new Translation("Beginnend in", "Empezando en", "A partir de", "A partire da", "Comecando em"));
					mTranslation.Add(new TextKey("6RewardLabel", "Super Mystery Box!"), new Translation("Super Mystery Box!", "Super Mystery Box!", "Super Mystery Box!", "Super Mistero Box!", "Super Mystery Box!"));
					mTranslation.Add(new TextKey("2Title", "SUPER SNEAKERS"), new Translation("SUPER SNEAKERS", "SUPER SNEAKERS", "SUPER SNEAKERS", "SUPER SNEAKERS", "SUPER SNEAKERS"));
					mTranslation.Add(new TextKey("ContinueLabel", "Tap to continue"), new Translation("Tippen Sie, um fortzufahren", "Pulse para continuar", "Appuyez pour continuer", "Tocca per continuare", "Clique para continuar"));
					mTranslation.Add(new TextKey("ContinueLabel", "Tap to open"), new Translation("Antippen zum Offnen", "Toque para abrir", "Appuyez pour ouvrir", "Tocca per aprire", "Toque para abrir"));
					mTranslation.Add(new TextKey("Tap to play", "Tap to play"), new Translation("Spielen", "Toque para jugar", "Appuyez", "Tocca per giocare", "Toque para jogar"));
					mTranslation.Add(new TextKey("008", "Upgrades"), new Translation("Upgrades", "Actualizaciones", "Mises a niveau", "aggiornamenti", "Atualizacoes"));
					mTranslation.Add(new TextKey("Label", "View Prizes"), new Translation("Ansehen", "Premios", "Voir les prix", "Visualizza", "Ver premios"));
					mTranslation.Add(new TextKey("Title", "View Prizes"), new Translation("Ansehen", "Premios", "Voir les prix", "Visualizza", "Ver premios"));
					mTranslation.Add(new TextKey("Message", "You rock! Now GO!"), new Translation("Sie schaukeln Jetzt geh!", "Usted rock! ¡Ahora ve!", "Tu geres! Vas y!", "Sei forte! Ora vai!", "Voce e demais! Agora va!"));
					mTranslation.Add(new TextKey("3Desc", "Skip ahead 1000 meters in a run."), new Translation("Vorwarts fahren 1000 Meter in einem Lauf.", "Salte 1000 metros en una carrera.", "Passer devant 1000 metres en course.", "Salta avanti 1000 metri in una corsa.", "Pular a frente 1000 metros em uma corrida."));
					mTranslation.Add(new TextKey("3Desc", "Skip ahead 250 meters in a run."), new Translation("Vorwarts fahren 250 Meter in einem Lauf.", "Salte 250 metros en una carrera.", "Passer devant 250 metres en course.", "Andate avanti 250 metri in una corsa.", "Pise em frente 250 metros em uma corrida."));
					mTranslation.Add(new TextKey("3Desc", "Protect yourself from crashing for 30 seconds. Activate by double tapping."), new Translation("Schutzen Sie sich 30 Sekunden lang vor dem Absturz.", "Protejase de fallar durante 30 segundos.", "Protegez-vous de l'ecrasement pendant 30 secondes.", "Proteggete da crash per 30 secondi.", "Proteja-se de bater durante 30 segundos."));
					mTranslation.Add(new TextKey("desc", "Protect yourself from crashing for 30 seconds."), new Translation("Schutzen Sie sich 30 Sekunden lang vor dem Absturz.", "Protejase de fallar durante 30 segundos.", "Protegez-vous de l'ecrasement pendant 30 secondes.", "Proteggete da crash per 30 secondi.", "Proteja-se de bater durante 30 segundos."));
					mTranslation.Add(new TextKey("3Title", "Prizes"), new Translation("Preise", "Premios", "Des prix", "premi", "Premios"));
					mTranslation.Add(new TextKey("4subTitle", "found in Mystery Box"), new Translation("Gefunden in Mystery Box", "La Mystery Box", "Trouve Mystery Box", "Mystery Box", "Mystery Box"));
					mTranslation.Add(new TextKey("coinsLabel", "100 000 coins!"), new Translation("100 000 munzen", "100 000 monedas", "100 000 pieces!", "100.000 monete!", "100 000 moedas!"));
					mTranslation.Add(new TextKey("TitleLabel", "Jackpot!"), new Translation("Jackpot!", "Jackpot!", "Cagnotte!", "montepremi!", "Jackpot!"));
					mTranslation.Add(new TextKey("TitleLabelShadow", "Jackpot!"), new Translation("Jackpot!", "Jackpot!", "Cagnotte!", "montepremi!", "Jackpot!"));
					mTranslation.Add(new TextKey("coinsLabelShadow", "100 000 coins!"), new Translation("100 000 munzen", "100 000 monedas", "100 000 pieces!", "100.000 monete!", "100 000 moedas!"));
					mTranslation.Add(new TextKey("1prize1topLabel", "Mega"), new Translation("Mega", "Mega", "Mega", "mega", "Mega"));
					mTranslation.Add(new TextKey("2prize1botLabel", "Headstart"), new Translation("Vorsprung", "Comienzo", "Debut", "Partenza", "comeco"));
					mTranslation.Add(new TextKey("2prize2botLabel", "coins"), new Translation("Munzen", "Monedas", "Pieces", "Monete", "Moedas"));
					mTranslation.Add(new TextKey("1prize2Label", "Trophies"), new Translation("Trophaen", "Trofeos", "Trophees", "Trofei", "Trofeus"));
					mTranslation.Add(new TextKey("0titleLabel", "Rare"), new Translation("Selten", "Raro", "Rare", "raro", "raro"));
					mTranslation.Add(new TextKey("1prize2Label", "Up to 10"), new Translation("Bis zu 10", "Hasta 10", "Jusqu 10", "Fino a 10", "Ate 10"));
					mTranslation.Add(new TextKey("1prize2Label", "Hoverboards"), new Translation("Hoverboards", "Hoverboards", "Hoverboards", "Hoverboards", "Hoverboards"));
					mTranslation.Add(new TextKey("1prize2Label", "Headstart"), new Translation("Vorsprung", "Comienzo", "Debut", "Partenza", "comecar"));
					mTranslation.Add(new TextKey("1prize2Label", "Up to 1500"), new Translation("Bis 1500", "Hasta 1500", "Jusqu 1500", "Fino a 1500", "Ate 1500"));
					mTranslation.Add(new TextKey("1prize2Label", "coins"), new Translation("Munzen", "Monedas", "Pieces", "monete", "moedas"));
					mTranslation.Add(new TextKey("0titleLabel", "Special"), new Translation("besondere", "Especial", "Special", "speciale", "especial"));
					mTranslation.Add(new TextKey("0titleLabel", "Common"), new Translation("verbreitet", "Comun", "Commun", "Comune", "comum"));
					mTranslation.Add(new TextKey("callout_label2", "Boards?"), new Translation("Boards?", "Boards?", "Boards?", "tavole?", "Pranchas?"));
					mTranslation.Add(new TextKey("callout_label", "Out of"), new Translation("Keine", "Fuera de", "Hors de", "fuori da", "fora de"));
					mTranslation.Add(new TextKey("solution_label", "Here's 3 to#get you going!"), new Translation("Hier sind 3!", "Aqui hay 3 a#conseguir que va!", "Voici 3 a#aller-y!", "Qui e 3 a#ti farai andare!", "Aqui esta 3#para voce ir!"));
					mTranslation.Add(new TextKey("label2", "SURF AWAY from trouble!"), new Translation("Surfe weg von der Muhe!", "Navegar lejos de problemas!", "Eloignez-vous des ennuis!", "Navigare lontano dai guai!", "Navegue longe dos problemas!"));
					mTranslation.Add(new TextKey("label2a", "Boards save you from crashing"), new Translation("Boards sparen Sie vom Absturz", "Las tablas le ahorran de estrellarse", "Les cartes vous empechent de s'ecraser", "Le schede ti salvarono dall'arresto", "Os conselhos que voce impede de bater"));
					mTranslation.Add(new TextKey("label3", "Have another GO!"), new Translation("Versuch es noch einmal!", "Tener otra oportunidad!", "Essaye encore!", "Avere un'altra via!", "Tenha outro ir!"));
					mTranslation.Add(new TextKey("label3a", "Get a flying start with 3 FREE Boards"), new Translation("Beginnen Sie mit 3 freien Boards", "Comience con 3 tableros libres", "Commencez avec 3 cartes gratuites", "Ottenere un volo iniziale con 3 schede gratuite", "Comece com 3 placas gratuitas"));
					mTranslation.Add(new TextKey("label4", "Where can I get MORE?"), new Translation("Wo kann ich mehr bekommen", "Donde puedo conseguir mas?", "Ou puis-je obtenir plus?", "Dove posso ottenere di piu?", "Onde posso obter mais?"));
					mTranslation.Add(new TextKey("label4a", "Get Hoverboards in the Shop"), new Translation("Hoverboards im Shop bekommen", "Obtener hoverboards en la tienda", "Obtenir des hoverboards dans le Shop", "Ottenere hoverboards nel negozio", "Obter hoverboards na loja"));
					mTranslation.Add(new TextKey("buttonText", "Get 3 Free!"), new Translation("Bekomme 3 gratis!", "Obtener 3 gratis!", "Obtenez 3 gratuits!", "Ottieni 3 gratis!", "Ganhe 3 gratis!"));
					mTranslation.Add(new TextKey("Title", "Coins"), new Translation("Munzen", "Monedas", "Pieces", "monete", "moedas"));
					mTranslation.Add(new TextKey("line1", "You unlocked"), new Translation("Gekauft", "Desbloqueaste", "Deverrouille", "Hai sbloccato", "Desbloqueado"));
					mTranslation.Add(new TextKey("subLabel", "Double tap while running to activate crash protection"), new Translation("Doppeltippen beim Laufen, um den Crash-Schutz zu aktivieren", "Doble toque mientras se ejecuta para activar la proteccion de choque", "Appuyez deux fois pendant l'execution pour activer la protection contre les pannes", "Toccare due volte durante l'esecuzione per attivare la protezione antischiuma", "Toque duas vezes durante a execucao para ativar a protecao contra choque"));
					mTranslation.Add(new TextKey("subLabel", "Activate at the beginning of a game to get a mega headstart"), new Translation("Aktiviere am Anfang eines Spiels, um einen Mega-Headstart zu bekommen", "Activar al principio de un juego para obtener un mega headstart", "Activez au debut d'un jeu pour obtenir un mega tete", "Attiva all'inizio di un gioco per ottenere un mega headstart", "Ative no inicio de um jogo para obter um mega headstart"));
					mTranslation.Add(new TextKey("subLabel", "Activate at the beginning of a game to get a headstart"), new Translation("Aktiviere am Anfang eines Spiels, um einen Vorsprung zu bekommen", "Activar al principio de un juego para obtener un headstart", "Activez au debut d'un jeu pour obtenir un depart", "Attiva all'inizio di un gioco per ottenere un headstart", "Ative no inicio de um jogo para obter uma vantagem"));
					mTranslation.Add(new TextKey("4Price", "Full"), new Translation("Voll", "Lleno", "Rempli", "Pieno", "Pleno"));
					mTranslation.Add(new TextKey("0get", "GET"), new Translation(" ", " ", " ", " ", " "));
					mTranslation.Add(new TextKey("2from", "FROM"), new Translation("AUS", "DE", "DE LA", "DA", "DE"));
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
