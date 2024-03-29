﻿using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Controls;
using Jypeli.Widgets;
using Jypeli.Effects;
using Jypeli.Assets;
using Jypeli.GameObjects;

/// @author: Ali Nadhum
/// @version: 1.0


/// <summary>
/// Sammakko kadulla.
/// </summary>
public class SammakkoKadulla : PhysicsGame
{
	#region Pelin muuttujat
	Image taustaKuva; //Taustakuvaa varten 
	Image kuollutSammakko;
	Image [] sammakonkieliAnimKuvat;
	Image [] karpanenSuussaAnimKuvat;
	Image[] pyorailijatKuva;
	Image[] autotKuva;
	Image[] sammakkoAnimKuvat;
	Image[] poliisiAutoAnimKuvat;
	Image pensasVaaleaKuva;
	Image pensasTummaKuva;
	Image [] kotkaKuvat;
	Image [] karpanenKuvat;
	PhysicsObject sammakko; //Sammakon hahmo
	PhysicsObject auto;
	PhysicsObject pyorailija;
	PhysicsObject poliisiAuto;
	PhysicsObject kotka;
	PhysicsObject karpanen;
	PhysicsObject vasenReuna;
	PhysicsObject oikeaReuna;
	Angle kulma = new Angle ();
	DoubleMeter	aikaMittari;
	SoundEffect hyppyAani = LoadSoundEffect("Audio/hyppy_aani.wav");
	SoundEffect kotkaAani = LoadSoundEffect("Audio/kotkaAani.wav");
	SoundEffect poliisiSireeni = LoadSoundEffect("Audio/poliisiSireeni.wav");
	SoundEffect karpanenAani = LoadSoundEffect("Audio/karpanenAani.wav");
	SoundEffect sammakkoOsuma = LoadSoundEffect("Audio/SammakkoOsuma.wav");
	SoundEffect sammakonNuolaisu = LoadSoundEffect("Audio/sammakonNuolaisu.wav");
	bool poliisiAutoVasemmalta = false;
	double loikkimisNopeus = 200; 
	bool flipped = false;
	int sydanMaara = 3;
	Widget sydamet;
	Timer aikaLaskuri;
	Timer karpanenAaniAjastin;
	bool sammakollaOnKarpanen = false;
	List<Label> valikonKohdat;
	#endregion

	#region Pelin alustaminen
	public override void Begin ()
	{
		Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
		PhoneBackButton.Listen(ConfirmExit, "Lo sydanMaarapeta peli");
		Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
		LataaKuvat();
		LuoTaustaKuva(1000, 800);
		int x = -500;
		for (int i = 0; i <= 8; i++) {
			LuoPensaat (x, 100, 65, 65);
			LuoPensaat (x, -365, 66, 75);
			LuoPensaat (x, -300, 66, 75);
			if (i == 3)
			{
				x = 0;
			}
			x += 120;
		}
		LuoPeliAika ();
		LuoSammakonSydamet (Screen.Right - 150, Screen.Top - 40, 25, 25, sydanMaara);
		peliAjastin ();
		Camera.ZoomFactor = 1.2;
		Camera.StayInLevel = true;
		LuoSammakko(new Vector (0, -350), 60, 60);
		Karpanen (karpanenKuvat, Screen.Center.X + 100, Screen.Top - 20, 70, 70, new Vector (-10, 0));
		Camera.Follow(sammakko);
		SetWindowSize(1024, 768, false);
		MediaPlayer.Play ("Audio/CityTraffic");
		MediaPlayer.IsRepeating = true;
	}

	protected override void Update (Time time)
	{
		if (sammakko.Y > Screen.Center.Y + 120) {
			MediaPlayer.Volume = 0.3;
		} else if (sammakko.Y < Screen.Center.Y + 120) {
			MediaPlayer.Volume = 1.0;
		}
		if (sammakollaOnKarpanen && sammakko.Position.Y <= -350) {
			sammakollaOnKarpanen = true;
			PeliLoppui ();
			base.StopAll ();
		}
		base.Update (time);
	}
	#endregion

	public void PeliLoppui()
	{
		IsPaused = true;
		valikonKohdat = new List<Label>();
		Label menuOtsikko;
		if (!sammakollaOnKarpanen) {
			menuOtsikko = new Label ("Peli on ohi et saanut tällä kertaa kärpästä..");
			menuOtsikko.TextColor = Color.BloodRed;
		} else {
			menuOtsikko = new Label ("Onneksi olkoon!! Sait kärpäsen kiini");
			menuOtsikko.TextColor = Color.Green;
		}
		Label kohta2 = new Label("Pelaa uudelleen!");
		Label kohta3 = new Label("Lopeta (Esc)");
		menuOtsikko.Position = new Vector(0, 40);
		kohta2.Position = new Vector(0, 0);
		kohta3.Position = new Vector(0, -40);
		kohta2.TextColor = Color.White;
		kohta3.TextColor = Color.White;
		valikonKohdat.Add(menuOtsikko);
		valikonKohdat.Add(kohta2);
		valikonKohdat.Add(kohta3);

		// Lisätään valikon tekstit peliin.
		foreach (Label valikonKohta in valikonKohdat)
		{
			Add(valikonKohta);
		}

		Keyboard.Listen(Key.Escape, ButtonState.Pressed, Exit, "Lopeta peli");
		Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, Begin, "Peli uudelleen");
		Mouse.ListenOn(kohta3, MouseButton.Left, ButtonState.Pressed, Exit, "Lopeta");
	}
		
		

	#region Kuvan haku
	public void LataaKuvat()
	{
        taustaKuva = LoadImage ("Tausta");
		kuollutSammakko = LoadImage ("Sammakko/kuollutSammakko");
		pyorailijatKuva = LoadImages (
			"Pyorailijat/Pyora1",
			"Pyorailijat/Pyora2",
			"Pyorailijat/Pyora3",
			"Pyorailijat/Pyora4",
			"Pyorailijat/Pyora5",
			"Pyorailijat/Pyora6",
			"Pyorailijat/Pyora7",
			"Pyorailijat/Pyora8",
			"Pyorailijat/Pyora9",
			"Pyorailijat/Pyora10",
			"Pyorailijat/Pyora11",
			"Pyorailijat/Pyora12",
			"Pyorailijat/Pyora13",
			"Pyorailijat/Pyora14",
			"Pyorailijat/Pyora15"
		);
		autotKuva = LoadImages (
			"Autot/Bussi",
			"Autot/MusAuto",
			"Autot/PunAuto",
			"Autot/Rekka",
			"Autot/SinAuto",
			"Autot/Poliisi1.1"
		);
		sammakkoAnimKuvat = LoadImages(
			"Sammakko/sammakko_0", 
			"Sammakko/sammakko_1", 
			"Sammakko/sammakko_2",
			"Sammakko/sammakko_3", 
			"Sammakko/sammakko_4", 
			"Sammakko/sammakko_5", 
			"Sammakko/sammakko_6"
		);
		sammakonkieliAnimKuvat = LoadImages (
			"Sammakko/sammakko_0",
			"Sammakko/sammakko_kieli1",
			"Sammakko/sammakko_kieli2",
			"Sammakko/sammakko_kieli3"
		);
		karpanenSuussaAnimKuvat = LoadImages (
			"Sammakko/karpanenSuussa_0",
			"Sammakko/karpanenSuussa_1",
			"Sammakko/karpanenSuussa_2",
			"Sammakko/karpanenSuussa_3",
			"Sammakko/karpanenSuussa_4",
			"Sammakko/karpanenSuussa_5",
			"Sammakko/karpanenSuussa_6"

		);
		poliisiAutoAnimKuvat = LoadImages (
			"Autot/Poliisi1.1",
			"Autot/Poliisi1.2",
			"Autot/Poliisi1.3",
			"Autot/Poliisi1.2"
		);
		pensasVaaleaKuva = LoadImage ("Pensaat/Pensas1");
		pensasTummaKuva = LoadImage ("Pensaat/Pensas2");
		kotkaKuvat = LoadImages (
			"Kotka/Kotka",
			"Kotka/Kotka1",
			"Kotka/Kotka2",
			"Kotka/Kotka3",
			"Kotka/Kotka4",
			"Kotka/Kotka5",
			"Kotka/Kotka6",
			"Kotka/Kotka7"
		);
		karpanenKuvat = LoadImages (
			"Karpanen/Karpanen1",
			"Karpanen/Karpanen2"
		);
	}
	#endregion

	public void LuoTaustaKuva (int leveys, int korkeus){
	 	Level.Background.Image = taustaKuva;
		Level.Background.Width = leveys;
		Level.Background.Height = korkeus;
		vasenReuna = Level.CreateLeftBorder();
		oikeaReuna = Level.CreateRightBorder();
		Level.CreateBottomBorder();
		Level.CreateTopBorder();
	}

	#region Sammakon logiikka
	public void LuoSammakko(Vector paikka, double leveys, double korkeus)
	{
		Image sammakonKuva;
		if (sammakollaOnKarpanen) {
			sammakonKuva = karpanenSuussaAnimKuvat [0];
		} else {
			sammakonKuva = sammakkoAnimKuvat [0];
		}
		sammakko = new PhysicsObject(leveys, korkeus, Shape.FromImage(sammakonKuva));
		sammakko.Image = sammakonKuva;
		sammakko.Position = paikka;
		sammakko.CanRotate = false;
		sammakko.Tag = "sammakko";
		Add(sammakko);
		LiikutaSammakko ();
	}
		

	public void LiikutaSammakko ()
	{
		Keyboard.Listen( Key.Up, ButtonState.Pressed, LoikiYlos, null, sammakko, loikkimisNopeus);
		Keyboard.Listen( Key.Down, ButtonState.Pressed, LoikiAlas, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Up, ButtonState.Released, sammakko.StopVertical, null );
		Keyboard.Listen( Key.Down, ButtonState.Released,sammakko.StopVertical , null );
		Keyboard.Listen( Key.Left, ButtonState.Pressed, LoikiVasemmalle, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Right, ButtonState.Pressed, LoikiOikealle, null, sammakko, loikkimisNopeus );
		Keyboard.Listen( Key.Left, ButtonState.Released, sammakko.StopHorizontal, null);
		Keyboard.Listen( Key.Right, ButtonState.Released, sammakko.StopHorizontal, null );  
		Keyboard.Listen (Key.Space, ButtonState.Pressed, SammakonKieli, null);
		AddCollisionHandler(sammakko, "pyorailija", SammakkoOsuu);
		AddCollisionHandler(sammakko, "auto", SammakkoOsuu);
		AddCollisionHandler(sammakko, "kotka", SammakkoOsuu);
		AddCollisionHandler(sammakko, "poliisiAuto", SammakkoOsuu);
	}


	void LoikiVasemmalle( PhysicsObject hahmo, double nopeus)
	{
		if (kulma.Degrees == 0)
		{
			kulma.Degrees = 90;

		} 
		else if (kulma.Degrees > 0 && kulma.Degrees < 90)
		{
			kulma.Degrees = -90;
		}
		hahmo.Angle = kulma;
		hahmo.Move(new Vector(-nopeus, 0)); 
		hyppyAani.Play (0.4, 0, 0);
		if (sammakollaOnKarpanen) {
			AnimoiSammakko (sammakko.Animation = new Animation (karpanenSuussaAnimKuvat), true);
		} else {
			AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
		}

	}


	void LoikiOikealle( PhysicsObject hahmo, double nopeus)
	{
		if (kulma.Degrees == 0) 
		{
			kulma.Degrees = -90;
		} 
		else if (kulma.Degrees > 0 && kulma.Degrees < 90)
		{
			kulma.Degrees = 90;
		}
		hahmo.Angle = kulma;
		hahmo.Move(new Vector(nopeus, 0));
		hyppyAani.Play (0.4, 0, 0);
		if (sammakollaOnKarpanen) {
			AnimoiSammakko (sammakko.Animation = new Animation (karpanenSuussaAnimKuvat), true);
		} else {
			AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
		}
	}


	public void LoikiYlos(PhysicsObject hahmo, double nopeus)
	{
		kulma.Degrees = 0;
		hahmo.Angle = kulma;
		if (flipped) {
			hahmo.FlipImage ();
			flipped = false;
		}
		hahmo.Move(new Vector(0, nopeus));
		hyppyAani.Play (0.4, 0, 0);
		if (sammakollaOnKarpanen) {
			AnimoiSammakko (sammakko.Animation = new Animation (karpanenSuussaAnimKuvat), true);
		} else {
			AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
		}
	}


	public void LoikiAlas(PhysicsObject hahmo, double nopeus)
	{
		kulma.Degrees = 1;
		hahmo.Angle = kulma;
		if (!flipped) {
			hahmo.FlipImage ();
			flipped = true;
		}
		hahmo.Move(new Vector(0, -nopeus));
		hyppyAani.Play (0.4, 0, 0);
		if (sammakollaOnKarpanen) {
			AnimoiSammakko (sammakko.Animation = new Animation (karpanenSuussaAnimKuvat), true);
		} else {
			AnimoiSammakko (sammakko.Animation = new Animation (sammakkoAnimKuvat), true);
		}
	}

	public void SammakonKieli(){
		if (!sammakollaOnKarpanen) {
			sammakko.Animation = new Animation (sammakonkieliAnimKuvat);
			sammakko.Animation.FPS = 22;
			sammakko.Animation.Start (1);
			sammakonNuolaisu.Play ();
			Vector sammakonPaikka = sammakko.Position;
			if (sammakonPaikka.Y >= Screen.Top - 30) {
				karpanen.Destroy ();
				karpanenAaniAjastin.Stop ();
				sammakollaOnKarpanen = true;
			}
		}

	}

	public void AnimoiSammakko(Animation animaatio, bool tilanne)
	{
		animaatio.FPS = 40;
		if (tilanne  == true) {
			animaatio.Start (1);
		} else {
			animaatio.Stop ();
		}
	}

	public void LuoSammakonSydamet(double x, double y,double leveys, double korkeus, int sydanMaara)
	{
		HorizontalLayout asettelu = new HorizontalLayout();
		asettelu.Spacing = 10;

		sydamet = new Widget( asettelu );
		sydamet.Color = Color.Transparent;
		sydamet.X = Screen.Center.X + 450;
		sydamet.Y = Screen.Top - 60;
		Add( sydamet );

		for ( int i = 0; i < sydanMaara; i++ )
		{
			Widget sydan = new Widget( 20, 20, Shape.Heart );
			sydan.Color = Color.Red;
			sydamet.Add( sydan );
		}
	}

	public void SammakkoOsuu(PhysicsObject sammakko, PhysicsObject kohde)
	{
		sydamet.Destroy ();
		sydanMaara--;
		if (sydanMaara <= 0) {
			sammakollaOnKarpanen = false;
			sammakko.RotateImage = true;
			sammakko.Image = kuollutSammakko;
			PeliLoppui();
		} else {
			LuoSammakonSydamet (Screen.Right - 150, Screen.Top - 40, 25, 25, sydanMaara);
		}
		sammakkoOsuma.Play ();
		if (kohde.Tag.ToString() == "poliisiAuto") {
			poliisiSireeni.Stop ();
		}
		kohde.Destroy ();
	}
	#endregion

	#region Pensaat
	public void LuoPensaat(double x, double y,double leveys, double korkeus)
	{
		PhysicsObject pensasVaalea = PhysicsObject.CreateStaticObject(leveys, korkeus, Shape.FromImage(pensasVaaleaKuva));
		PhysicsObject pensasTumma = PhysicsObject.CreateStaticObject(leveys, korkeus, Shape.FromImage(pensasTummaKuva));
		pensasVaalea.Image = pensasVaaleaKuva; 
		pensasTumma.Image = pensasTummaKuva;
		pensasVaalea.MakeStatic ();
		pensasTumma.MakeStatic ();
		pensasVaalea.Tag = "pVaalea";
		pensasTumma.Tag = "pTumma";
		pensasTumma.X = x;
		pensasVaalea.X = x + 60;
		pensasTumma.Y = y;
		pensasVaalea.Y = y;
		Add (pensasTumma);
		Add (pensasVaalea);
	}
	#endregion 

	#region Pyorailijat funktiot
	public void Pyorailija(Image pyorailijanKuva, double x, double y,double leveys, double korkeus, Vector pyoranSuunta)
	{
		pyorailija = new PhysicsObject (leveys, korkeus, Shape.FromImage(pyorailijanKuva));
		pyorailija.Image = pyorailijanKuva;
		pyorailija.Tag = "pyorailija";
		pyorailija.X = x;
		pyorailija.Y = y;
		pyorailija.Hit(pyoranSuunta);
		pyorailija.MakeStatic ();
		Add (pyorailija);
	}


	public void LuoPyorailijat()
	{
		int rndPyorat1 = RandomGen.NextInt (0, 15);
		int rndPyorat2 = RandomGen.NextInt (0, 15);
		double rndY1 = RandomGen.NextDouble (-160, -190);
		double rndY2 = RandomGen.NextDouble (-200, -250);
		Pyorailija (pyorailijatKuva[rndPyorat1], -450, rndY1, 40, 20, new Vector(70,0));
		Pyorailija (Image.Mirror(pyorailijatKuva[rndPyorat2]), 450, rndY2, 40, 20, new Vector(-50,0));
		AddCollisionHandler(pyorailija, ObjektiOsuuSeinaan);

	}
	#endregion


	#region Autot
	public void Auto(Image autoKuva, double x, double y,double leveys, double korkeus, Vector autonSuunta)
	{
		auto = new PhysicsObject (leveys, korkeus, Shape.FromImage(autoKuva));
		auto.Image = autoKuva;
		auto.Tag = "auto";
		auto.X = x;
		auto.Y = y;
		auto.Hit(autonSuunta);
		auto.MakeStatic ();
		Add (auto);
	}


	public void Luoautot()
	{
		int rndAuto1 = RandomGen.NextInt (0, 6);
		int rndAuto2 = RandomGen.NextInt (0, 6);
		int leveys = 50;
		if (rndAuto1 == 0 || rndAuto1 == 3) {

			leveys = 100;
		}
		Auto (autotKuva [rndAuto1], -450, 45, leveys, 30, new Vector (110, 0));
		leveys = 50;
		if (rndAuto2 == 0 || rndAuto2 == 3) {
			leveys = 100;
		}
		Auto (Image.Mirror (autotKuva [rndAuto2]), 450, -50, leveys, 30, new Vector (-110, 0));
		AddCollisionHandler(auto, ObjektiOsuuSeinaan);
	}
		
	public void PoliisiAuto(Image[] poliisiAutoAnimKuvat, double x, double y,double leveys, double korkeus, Vector autonSuunta)
	{
		poliisiAuto = new PhysicsObject (leveys, korkeus, Shape.FromImage(poliisiAutoAnimKuvat[0]));
		poliisiAuto.Tag = "poliisiAuto";
		poliisiAuto.X = x;
		poliisiAuto.Y = y;
		Animation poliisiAutoAnim = new Animation (poliisiAutoAnimKuvat);
		poliisiAutoAnim.FPS = 40;
		poliisiAuto.Animation = poliisiAutoAnim;
		poliisiAutoAnim.Start ();
		poliisiAuto.Hit(autonSuunta);
		poliisiAuto.MakeStatic ();
		Add (poliisiAuto);
	}


	public void LuoPoliisiAuto(){
		if (poliisiAutoVasemmalta == false) {
			PoliisiAuto (poliisiAutoAnimKuvat, -450, 0, 50, 30, new Vector (150, 0));
			poliisiAutoVasemmalta = true;
		} else {
			PoliisiAuto (Image.Mirror (poliisiAutoAnimKuvat), 450, 0, 50, 30, new Vector (-150, 0));
			poliisiAutoVasemmalta = false;
		}
		AddCollisionHandler (poliisiAuto, ObjektiOsuuSeinaan);
	}
	#endregion

	#region Kotkan logiikka
	public void Kotka(Image[] kotkaKuvat, double x, double y, double leveys, double korkeus, Vector kotkanSuunta)
	{
		kotka = new PhysicsObject (leveys, korkeus);
		kotka.Tag = "kotka";
		kotka.X = x;
		kotka.Y = y;
		Animation kotkanAnim = new Animation (kotkaKuvat);
		kotkanAnim.FPS = 20;
		kotka.Animation =  kotkanAnim;
		kotkanAnim.Start ();
		kotka.Hit(kotkanSuunta);
		kotka.MakeStatic ();
		Add (kotka);
	}

	public void LuoKotka()
	{
		double rndY = RandomGen.NextDouble (Screen.Top - 180, Screen.Top - 100);
		Kotka (Image.Mirror (kotkaKuvat), -450, rndY, 50, 70, new Vector(50,0));
		AddCollisionHandler(kotka, ObjektiOsuuSeinaan);

	}
	#endregion

	#region Karpasen logiikka
	public void Karpanen(Image[] karpanenKuvat, double x, double y, double leveys, double korkeus, Vector karpanenSuunta){
		karpanen = new PhysicsObject (leveys, korkeus, Shape.FromImage(karpanenKuvat[0]));
		karpanen.Tag = "karpanen";
		karpanen.X = x;
		karpanen.Y = y;
		Animation karpasenAnim = new Animation (karpanenKuvat);
		karpasenAnim.FPS = 15;
		karpanen.Animation =  karpasenAnim;
		karpasenAnim.Start ();
		karpanen.Hit(karpanenSuunta);
		Add (karpanen);
		AddCollisionHandler (karpanen, ObjektiOsuuSeinaan);
	}

	#endregion

	public void ObjektiOsuuSeinaan(PhysicsObject objekti, PhysicsObject seina){
		if(seina == vasenReuna || seina == oikeaReuna)
		{
			if (objekti.Tag.ToString () == "karpanen" && seina == vasenReuna) {
				Karpanen (Image.Mirror(karpanenKuvat), Screen.Center.X - 400, Screen.Top - 20, 100, 100, new Vector (10, 0));
			}else if (objekti.Tag.ToString () == "karpanen" && seina == oikeaReuna){
				Karpanen (karpanenKuvat, Screen.Center.X + 400, Screen.Top - 20, 100, 100, new Vector (-10, 0));
			}
			objekti.Destroy ();
		}
	}

	public void LuoPeliAika()
	{
		aikaMittari = new DoubleMeter(60);
		aikaMittari.MaxValue = 60;

		ProgressBar aikaPalkki = new ProgressBar(400, 10);
		aikaPalkki.X = Screen.Left + 230;
		aikaPalkki.Y = Screen.Top - 60;
		aikaPalkki.Color = Color.Transparent;
		aikaPalkki.BarColor = Color.Red;
		aikaPalkki.BorderColor = Color.Black;
		aikaPalkki.BindTo (aikaMittari);
		Add(aikaPalkki);

		Label aikaTeksti = new Label();
		aikaTeksti.TextColor = Color.Wheat;
		aikaTeksti.Color = Color.Transparent;
		aikaTeksti.Text = "Jäljellä oleva aika: ";
		aikaTeksti.X = Screen.Left + 120;
		aikaTeksti.Y = Screen.Top - 40;
		Add (aikaTeksti);
	}

	#region Pelin olioiden ajastin

	public void peliAjastin()
	{
		// Pelin ajastin
		aikaLaskuri = new Timer();
		aikaLaskuri.Interval = 0.1;
		aikaLaskuri.Timeout += LaskeAlaspain;
		aikaLaskuri.Start();

		// Autojen ajastin
		Timer autoAjastin = new Timer ();
		autoAjastin.Interval = 3;   // tällä voit myös säätää nopeutta
		autoAjastin.Timeout += Luoautot;
		autoAjastin.Start();

		// Poliisi auto ajastin
		Timer poliisiAutoAjastin =  new Timer();
		poliisiAutoAjastin.Interval = 20;
		poliisiAutoAjastin.Timeout += delegate() {
			LuoPoliisiAuto();
			poliisiSireeni.Play();
		};
		poliisiAutoAjastin.Start ();

		//Pyörien ajastin 
		Timer pyoraAjastin = new Timer ();
		pyoraAjastin.Interval = 3;   // tällä voit myös säätää nopeutta
		pyoraAjastin.Timeout += LuoPyorailijat;
		pyoraAjastin.Start();

		// Kotkan ajastin 
		Timer kotkaAjastin = new Timer ();
		kotkaAjastin.Interval = 5;
		kotkaAjastin.Timeout += LuoKotka;
		kotkaAjastin.Start ();

		Timer kotkaAaniAjastin = new Timer ();
		kotkaAaniAjastin.Interval = 15;
		kotkaAaniAjastin.Timeout += delegate() {
			kotkaAani.Play ();
		};
			kotkaAaniAjastin.Start();

		karpanenAaniAjastin = new Timer ();
		karpanenAaniAjastin.Interval = 2;
		karpanenAaniAjastin.Timeout += delegate() {
			karpanenAani.Play();
		};
		karpanenAaniAjastin.Start ();

	}
	#endregion

	#region Pelin aika 
	public void LaskeAlaspain()
	{
		aikaMittari.Value -= 0.1;

		if (aikaMittari.Value <= 0)
		{
			MessageDisplay.Add("Aika on päättynyt");
			MessageDisplay.TextColor = Color.Red;
			MessageDisplay.Position = new Vector (Screen.Center.X, Screen.Center.Y);
			sammakollaOnKarpanen = false;
			aikaLaskuri.Stop();
			PeliLoppui();
		}
	}
	#endregion

}

