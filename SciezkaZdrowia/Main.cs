using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Net.Security;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace SciezkaZdrowia;

enum Sceny {
    MENU,
    USTAWIENIA,
    INFORMACJE,
    POZIOM1,
    POZIOM2,
    POZIOM3,
    POZIOM4,
    POZIOM5,
    PLANSZA,
    KONIEC
}

public class Main : Game {

    public static float rozmiar_bloku = 64;
    public static float skalaX;
    public static float skalaY;
    public float skalaTekstu;
    public static bool spozyto_alkohol,spozyto_papierosy,spozyto_marihuane,spozyto_tabletke,spozyto_energetyk;
    public static Dictionary<Vector2, int> aktywnaMapa;
    public static Dictionary<Vector2,int> mapa1, mapa2, mapa3, mapa4, mapa5;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D[] animacja;
    private Texture2D tlo_poziom1;
    private Texture2D tlo_poziom2;
    private Texture2D tlo_poziom3;
    private Texture2D tlo_poziom4;
    private Texture2D tlo_poziom5;
    private int czas;
    private int rozdzielczosc =1;
    private Texture2D skrzynia;
    private Texture2D meta;
    private Texture2D tekstura_alkoholu;
    private Texture2D tekstura_wody;
    private Texture2D tekstura_papierosa;
    private Texture2D tekstura_jablka;
    private Texture2D tekstura_banana;
    private Texture2D tekstura_cukierka;
    private Texture2D tekstura_tabletki;
    private Texture2D tekstura_marihuany;
    private Texture2D energetyk;
    private Texture2D plansza1,plansza2,plansza3,plansza4,plansza5;
    private Texture2D sok_pomaranczowy;
    private Texture2D serce;
    private List<Uzywka> Uzywki;
    private List<Obiekt>Pozostale;
    private List<PozytywnyObiekt> Pozytywne_obiekty;
    private Obiekt gracz;
    private Sceny aktywnascena;
    private SpriteFont font;
    private Rectangle pozycja_myszki,nowagra,ustawienia,informacje,rozdzielczosc1,rozdzielczosc2,rozdzielczosc3,menu,wyjscie,dzwiek1,dzwiek2,dzwiek3;
    private Song theme;
    private SoundEffect koniec_sfx;
    private SoundEffect punkt_sfx;
    private SoundEffect utrata_sfx;
    private SoundEffect levelup_sfx;
    private SoundEffect wygrana_sfx;
    private bool nastepny_poziom = false;
    private bool zagrano_dzwiek;
    private bool reset = false;
    private bool koniec = false;
    private bool isResetting = false;
    private bool obiekty_dodane = false;
    private bool nowagra_hover,ustawienia_hover,informacje_hover,rozdzielczosc1_hover,rozdzielczosc2_hover,rozdzielczosc3_hover,menu_hover,wyjscie_hover,dzwiek1_hover,dzwiek2_hover,dzwiek3_hover;
    private bool koniec_czasu,wygrana;
    private int licznik;
    private float poziom_dzwieku=0.5f;
    private int poprzedni_poziom;
    private int timer_efektu;
    private int ktora_klatka;
    private float czas_sinus;
    private int maxHeight = (int)(16*rozmiar_bloku);
    private int maxWidth = (int)(20*rozmiar_bloku);
    private int timer,licznik_sekund = 0;
    private int limit_czasu=0;
    
    
    public Main() {

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        aktywnascena = Sceny.MENU;
        mapa1 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa1.csv");
        mapa2 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa2.csv");
        mapa3 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa3.csv");
        mapa4 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa4.csv");
        mapa5 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa5.csv");

    }

    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = (int)(20*rozmiar_bloku);
        _graphics.PreferredBackBufferHeight = (int)(16*rozmiar_bloku);
        this.Window.AllowUserResizing = false;
        this.Window.Title = "Ścieżka Zdrowia Nikodem Netkowski s193335";
        this.Window.Position = Point.Zero;
        _graphics.ApplyChanges();
        skalaX = (float)Window.ClientBounds.Width/(float)(20*rozmiar_bloku); 
        skalaY = (float)Window.ClientBounds.Height/(float)(16*rozmiar_bloku);
        aktywnaMapa = mapa1;        
        base.Initialize();

    }

    protected override void LoadContent()
    {

        font = Content.Load<SpriteFont>("Fonts/8bitfont");
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        animacja = new Texture2D[6]; 
        tlo_poziom1 = Content.Load<Texture2D>("tlo");
        tlo_poziom2 = Content.Load<Texture2D>("tlo2");
        tlo_poziom3 = Content.Load<Texture2D>("tlo3");
        tlo_poziom4 = Content.Load<Texture2D>("tlo4");
        tlo_poziom5 = Content.Load<Texture2D>("tlo5");
        plansza1 = Content.Load<Texture2D>("plansza1");
        plansza2 = Content.Load<Texture2D>("plansza2");
        plansza3 = Content.Load<Texture2D>("plansza3");
        plansza4 = Content.Load<Texture2D>("plansza4");
        plansza5 = Content.Load<Texture2D>("plansza5");

        animacja[0] = Content.Load<Texture2D>("front1");  
        animacja[1] = Content.Load<Texture2D>("front2"); 
        animacja[2] = Content.Load<Texture2D>("lewo1");  
        animacja[3] = Content.Load<Texture2D>("lewo2");  
        animacja[4] = Content.Load<Texture2D>("prawo1");  
        animacja[5] = Content.Load<Texture2D>("prawo2");   
        skrzynia = Content.Load<Texture2D>("box");
        tekstura_wody = Content.Load<Texture2D>("woda");
        tekstura_alkoholu = Content.Load<Texture2D>("alkohol");
        serce = Content.Load<Texture2D>("serce");
        meta = Content.Load<Texture2D>("meta");
        tekstura_jablka = Content.Load<Texture2D>("jablko");
        tekstura_papierosa = Content.Load<Texture2D>("papieros");
        tekstura_banana = Content.Load<Texture2D>("banan");
        tekstura_marihuany = Content.Load<Texture2D>("marihuana");
        tekstura_cukierka = Content.Load<Texture2D>("cukierek");
        tekstura_tabletki = Content.Load<Texture2D>("tabletka");
        energetyk = Content.Load<Texture2D>("energetyk");
        sok_pomaranczowy = Content.Load<Texture2D>("sok_pomaranczowy");

        theme = Content.Load<Song>("Audio/theme");
        koniec_sfx = Content.Load<SoundEffect>("Audio/koniec");
        punkt_sfx = Content.Load<SoundEffect>("Audio/punkt");
        utrata_sfx = Content.Load<SoundEffect>("Audio/utrata");
        levelup_sfx = Content.Load<SoundEffect>("Audio/level_up");
        wygrana_sfx = Content.Load<SoundEffect>("Audio/wygrana");
        MediaPlayer.Volume = 0;
        MediaPlayer.IsRepeating = true;


        Uzywki = new();
        Pozytywne_obiekty = new();
        Pozostale = new();

        
        List<Rectangle> kolizje = new List<Rectangle>();

        foreach (var tekstura in mapa1) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );

            kolizje.Add(skrzyniaObszar);

        }

        foreach (var tekstura in mapa2) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );

            kolizje.Add(skrzyniaObszar);

        }

        foreach (var tekstura in mapa3) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );

            kolizje.Add(skrzyniaObszar);

        }

        foreach (var tekstura in mapa4) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );

            kolizje.Add(skrzyniaObszar);

        }

        foreach (var tekstura in mapa5) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );

            kolizje.Add(skrzyniaObszar);

        }

        gracz = new Gracz(animacja[ktora_klatka], new Vector2(70,840), kolizje, 3);
        
    }

    protected override void Update(GameTime gameTime) {
        
        if (isResetting) return;
        

        timer += 1;

        if (timer % 60 == 0) {

            licznik_sekund += 1;
            timer_efektu += 1;

        }

        skalaTekstu = Math.Min(skalaX, skalaY);
        var mouseState = Mouse.GetState();
        pozycja_myszki = new Rectangle(mouseState.X,mouseState.Y,1,1);

       

        switch (aktywnascena) {
            
            case Sceny.MENU:

                
                MediaPlayer.Play(theme);
                

                if ((mouseState.LeftButton == ButtonState.Pressed)&&(nowagra_hover)) {

                    PelnyResetGry();
                    MediaPlayer.Volume = 1f;

                }

                if ((mouseState.LeftButton == ButtonState.Pressed)&&(ustawienia_hover)) {

                    aktywnascena = Sceny.USTAWIENIA;

                } 
                
                if ((mouseState.LeftButton == ButtonState.Pressed)&&(informacje_hover)) {

                    aktywnascena = Sceny.INFORMACJE;

                }     

                if ((mouseState.LeftButton == ButtonState.Pressed)&&(wyjscie_hover)) {

                    Exit();

                }                            

            break;

            case Sceny.POZIOM1:

                poprzedni_poziom = 1;

                if (!reset) {

                    aktywnascena = Sceny.POZIOM1;  
                    aktywnaMapa = mapa1;
                    Reset_poziomu();
                    reset = true;
            
                }

                if (nastepny_poziom) {

                    obiekty_dodane = false;
                    nastepny_poziom = false;
                    reset = false;
                    levelup_sfx.Play();
                    aktywnascena = Sceny.PLANSZA;
                    
                    
                }

                Powrot_do_main_menu(); 

                Koniec();

                if (!obiekty_dodane) {
                    
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_banana, new Vector2(7*rozmiar_bloku,7*rozmiar_bloku),25,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_banana, new Vector2(9*rozmiar_bloku,7*rozmiar_bloku),25,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_banana, new Vector2(11*rozmiar_bloku,7*rozmiar_bloku),25,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_banana, new Vector2(16*rozmiar_bloku,8*rozmiar_bloku),25,1,1));

                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(7*rozmiar_bloku,14*rozmiar_bloku),1,1));
                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(8*rozmiar_bloku,14*rozmiar_bloku),1,1));
                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(9*rozmiar_bloku,14*rozmiar_bloku),1,1));
                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(10*rozmiar_bloku,14*rozmiar_bloku),1,1));
                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(11*rozmiar_bloku,14*rozmiar_bloku),1,1));
                    Uzywki.Add(new Uzywka(tekstura_marihuany,new Vector2(16*rozmiar_bloku,13*rozmiar_bloku),1,1));


                    Pozostale.Add(new Obiektinnychrozmiarow(meta,new Vector2(18*rozmiar_bloku,13*rozmiar_bloku),1f,2f));
                    obiekty_dodane = true;

                }

            break;

            case Sceny.POZIOM2:

                poprzedni_poziom = 2;

                if (!reset) {

                    aktywnaMapa = mapa2;
                    Reset_poziomu();
                    reset = true;

                }

                if (nastepny_poziom) {

                    obiekty_dodane = false;
                    nastepny_poziom = false;
                    reset = false;
                    aktywnascena = Sceny.PLANSZA;
                    levelup_sfx.Play();

                    
                }

                Powrot_do_main_menu();
                Koniec();

                if (!obiekty_dodane) {

                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_jablka, new Vector2(1*rozmiar_bloku,4*rozmiar_bloku),50,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_jablka, new Vector2(1*rozmiar_bloku,8*rozmiar_bloku),50,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_jablka, new Vector2(18*rozmiar_bloku,1*rozmiar_bloku),50,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_jablka, new Vector2(5*rozmiar_bloku,9*rozmiar_bloku),50,1,1));

                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(5*rozmiar_bloku,4*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(6*rozmiar_bloku,4*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(8*rozmiar_bloku,4*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(9*rozmiar_bloku,4*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(11*rozmiar_bloku,4*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(12*rozmiar_bloku,5*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(14*rozmiar_bloku,5*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(15*rozmiar_bloku,5*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(18*rozmiar_bloku,14*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(5*rozmiar_bloku,7*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_papierosa,new Vector2(6*rozmiar_bloku,7*rozmiar_bloku),1,(float)0.5));

                    Pozostale.Add(new Obiektinnychrozmiarow(meta,new Vector2(6*rozmiar_bloku,13*rozmiar_bloku),1f,2f));
                    obiekty_dodane = true;       

                }

            break;

            case Sceny.POZIOM3:

                poprzedni_poziom = 3;

                if (!reset) {

                    aktywnaMapa = mapa3;
                    Reset_poziomu();
                    reset = true;

                }

                if (nastepny_poziom) {

                    obiekty_dodane = false;
                    nastepny_poziom = false;
                    reset = false;
                    aktywnascena = Sceny.PLANSZA;
                    levelup_sfx.Play();

                    
                }

                Powrot_do_main_menu();
                Koniec();

                if (!obiekty_dodane) {

                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)18.25*rozmiar_bloku,12*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)18.25*rozmiar_bloku,13*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)18.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)17.25*rozmiar_bloku,12*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)17.25*rozmiar_bloku,13*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)17.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(tekstura_alkoholu,new Vector2((float)6.25*rozmiar_bloku,9*rozmiar_bloku),(float)0.5,1));

                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_wody, new Vector2((float)8.25*rozmiar_bloku,5*rozmiar_bloku),100,(float)0.5,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_wody, new Vector2((float)1.25*rozmiar_bloku,1*rozmiar_bloku),100,(float)0.5,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_wody, new Vector2((float)1.25*rozmiar_bloku,11*rozmiar_bloku),100,(float)0.5,1));

                    Pozostale.Add(new Obiektinnychrozmiarow(meta,new Vector2(18*rozmiar_bloku,rozmiar_bloku),1f,2f));
                    obiekty_dodane = true;       

                }

            break;

            case Sceny.POZIOM4:

                poprzedni_poziom = 4;

                if (!reset) {

                    aktywnaMapa = mapa4;
                    Reset_poziomu();
                    reset = true;

                }

                if (nastepny_poziom) {

                    obiekty_dodane = false;
                    nastepny_poziom = false;
                    reset = false;
                    aktywnascena = Sceny.PLANSZA;
                    levelup_sfx.Play();

                    
                }

                Powrot_do_main_menu();
                Koniec();

                if (!obiekty_dodane) {

                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(4*rozmiar_bloku,(float)14.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(5*rozmiar_bloku,14*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(4*rozmiar_bloku,(float)13.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(5*rozmiar_bloku,13*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(4*rozmiar_bloku,(float)12.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(5*rozmiar_bloku,12*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(5*rozmiar_bloku,7*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(7*rozmiar_bloku,(float)12.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(8*rozmiar_bloku,(float)12.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(12*rozmiar_bloku,(float)10.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(13*rozmiar_bloku,(float)10.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(14*rozmiar_bloku,(float)10.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(15*rozmiar_bloku,(float)10.25*rozmiar_bloku),1,(float)0.5));
                    Uzywki.Add(new Uzywka(tekstura_tabletki,new Vector2(16*rozmiar_bloku,(float)10.25*rozmiar_bloku),1,(float)0.5));

                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_cukierka, new Vector2(5*rozmiar_bloku,1*rozmiar_bloku),150,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_cukierka, new Vector2(7*rozmiar_bloku,8*rozmiar_bloku),150,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_cukierka, new Vector2(12*rozmiar_bloku,8*rozmiar_bloku),150,1,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(tekstura_cukierka, new Vector2(17*rozmiar_bloku,4*rozmiar_bloku),150,1,1));



                    Pozostale.Add(new Obiektinnychrozmiarow(meta,new Vector2(18*rozmiar_bloku,8*rozmiar_bloku),1f,2f));
                    obiekty_dodane = true;       

                }

            break;
        
            case Sceny.POZIOM5:

                poprzedni_poziom = 5;

                if (!reset) {

                    aktywnaMapa = mapa5;
                    Reset_poziomu();
                    reset = true;

                }

                if (nastepny_poziom) {

                    obiekty_dodane = false;
                    nastepny_poziom = false;
                    reset = false;
                    levelup_sfx.Play();
                    aktywnascena = Sceny.PLANSZA;
                    wygrana = true;
                    
                }

                Powrot_do_main_menu();
                Koniec();

                if (!obiekty_dodane) {

                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)17.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)18.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)3.25*rozmiar_bloku,12*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)4.25*rozmiar_bloku,4*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)5.25*rozmiar_bloku,4*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)7.25*rozmiar_bloku,4*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)8.25*rozmiar_bloku,4*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)10.25*rozmiar_bloku,7*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)11.25*rozmiar_bloku,7*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)9.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)10.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)11.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)12.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)13.25*rozmiar_bloku,14*rozmiar_bloku),(float)0.5,1));
                    Uzywki.Add(new Uzywka(energetyk,new Vector2((float)16.25*rozmiar_bloku,9*rozmiar_bloku),(float)0.5,1));

                    Pozytywne_obiekty.Add(new PozytywnyObiekt(sok_pomaranczowy, new Vector2((float)4.25*rozmiar_bloku,8*rozmiar_bloku),200,(float)0.5,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(sok_pomaranczowy, new Vector2((float)14.25*rozmiar_bloku,4*rozmiar_bloku),200,(float)0.5,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(sok_pomaranczowy, new Vector2((float)18.25*rozmiar_bloku,1*rozmiar_bloku),200,(float)0.5,1));
                    Pozytywne_obiekty.Add(new PozytywnyObiekt(sok_pomaranczowy, new Vector2((float)9.25*rozmiar_bloku,13*rozmiar_bloku),200,(float)0.5,1));



                    Pozostale.Add(new Obiektinnychrozmiarow(meta,new Vector2(13*rozmiar_bloku,1*rozmiar_bloku),1f,2f));
                    obiekty_dodane = true;       

                }

            break;

            case Sceny.USTAWIENIA:

            MediaPlayer.Volume = 0f;

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc1_hover)) {

                _graphics.PreferredBackBufferHeight = (int)(16*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(20*rozmiar_bloku);
                _graphics.ApplyChanges();
                rozdzielczosc = 1;

            }

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc2_hover)) {

                _graphics.PreferredBackBufferHeight = (int)(8*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(10*rozmiar_bloku);
                _graphics.ApplyChanges();
                rozdzielczosc = 2;

            }

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc3_hover)) {

                _graphics.PreferredBackBufferHeight = (int)(4*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(5*rozmiar_bloku);
                _graphics.ApplyChanges();
                rozdzielczosc = 3;

            }

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(dzwiek1_hover)) {

                poziom_dzwieku = 0f;                

            }

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(dzwiek2_hover)) {

                poziom_dzwieku = 0.5f;                

            }

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(dzwiek3_hover)) {

                poziom_dzwieku = 1f;                

            }

            Powrot_do_main_menu();

            break;

            case Sceny.INFORMACJE:

            MediaPlayer.Volume = 0f;

            Powrot_do_main_menu();

            break;

            case Sceny.KONIEC:

            MediaPlayer.Volume = 0f;

            Powrot_do_main_menu();

            break;
            
            case Sceny.PLANSZA:

            if (poprzedni_poziom == 1) {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) {

                    aktywnascena = Sceny.POZIOM2;

                }

            }

            if (poprzedni_poziom == 2) {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) {

                    aktywnascena = Sceny.POZIOM3;

                }

            }

            if (poprzedni_poziom == 3) {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) {

                    aktywnascena = Sceny.POZIOM4;

                }

            }

            if (poprzedni_poziom == 4) {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) {

                    aktywnascena = Sceny.POZIOM5;

                }

            }

            if (poprzedni_poziom == 5) {
                if(Keyboard.GetState().IsKeyDown(Keys.Space)) {

                    aktywnascena = Sceny.KONIEC;

                }

            }


            break;

        }

        if (Gracz.przegrana) {
            koniec = true;
            Koniec();
        }

        List<Uzywka> ZebraneUzywki = new();
        List<PozytywnyObiekt> ZebranePozytywne = new();

        foreach(var obiekt in Uzywki) {

            obiekt.Update(gameTime);

            if (obiekt.obszar.Intersects(gracz.obszar)) {

                ZebraneUzywki.Add(obiekt);
                utrata_sfx.Play();

            }

        }

        foreach(var obiekt in Pozytywne_obiekty) {

            obiekt.Update(gameTime);

            if (obiekt.obszar.Intersects(gracz.obszar)) {

                ZebranePozytywne.Add(obiekt);
                punkt_sfx.Play();

            }

        }

        foreach(var obiekt in Pozostale) {

            obiekt.Update(gameTime);

            if ((obiekt.obszar.Intersects(gracz.obszar))||Keyboard.GetState().IsKeyDown(Keys.K)) {

                nastepny_poziom = true;
               
            }

        }

        foreach(var obiekt in ZebraneUzywki) {

            Uzywki.Remove(obiekt);
            Gracz.Zycie--;

            if (aktywnascena == Sceny.POZIOM1) {

                spozyto_marihuane = true;
                timer_efektu = 0;

            }

            if (aktywnascena == Sceny.POZIOM2) {

                spozyto_papierosy = true;
                timer_efektu = 0;

            }

            if (aktywnascena == Sceny.POZIOM3) {

                spozyto_alkohol = true;
                timer_efektu = 0;

            }

            if (aktywnascena == Sceny.POZIOM4) {

                spozyto_tabletke = true;
                timer_efektu = 0;

            }

            if (aktywnascena == Sceny.POZIOM5) {

                spozyto_energetyk = true;
                timer_efektu = 0;

            }
        

        }
        if ((aktywnascena == Sceny.POZIOM1)||(aktywnascena == Sceny.POZIOM2)||(aktywnascena == Sceny.POZIOM3)||(aktywnascena == Sceny.POZIOM4)||(aktywnascena == Sceny.POZIOM5)||(aktywnascena == Sceny.PLANSZA)) {

            MediaPlayer.Volume = poziom_dzwieku;
            SoundEffect.MasterVolume = poziom_dzwieku;
            
        } else {
        
        if ((aktywnascena == Sceny.KONIEC)) {

            MediaPlayer.Volume = 0f;
            SoundEffect.MasterVolume = poziom_dzwieku;

        }   else {

                MediaPlayer.Volume = 0f;
                SoundEffect.MasterVolume = 0f;

            }

        }

        if (timer_efektu == 6) {

            spozyto_alkohol = false;
            spozyto_papierosy = false;
            spozyto_marihuane = false;
            spozyto_tabletke = false;
            spozyto_energetyk = false;

        }

        foreach (var obiekt in ZebranePozytywne) {

            Pozytywne_obiekty.Remove(obiekt);
            Gracz.Punkty += obiekt.Punkty;

        }

        gracz.Update(gameTime);

        licznik++;

        if (licznik > 60) {

            licznik = 0;

        }

        if (spozyto_marihuane) {
            
            if (Gracz.kierunek == 2){
                Gracz.kierunek = 0;
            } else {
                Gracz.kierunek += 1;
            }

        }



        if (Gracz.kierunek == 0) {

            ktora_klatka = 0;

            if (licznik > 30){

                ktora_klatka = 1;

            }

        }

        if (Gracz.kierunek == 1) {

            ktora_klatka = 2;

            if (licznik > 30){

                ktora_klatka = 3;

            }
        }

        if (Gracz.kierunek == 2) {

            ktora_klatka = 4;

            if (licznik > 30) {

                ktora_klatka = 5;

            }

        }

        if (koniec) {

            koniec = false; 
            aktywnascena = Sceny.KONIEC;
            
        }

        if (Window.ClientBounds.Width>maxWidth) {

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = maxWidth;
            _graphics.ApplyChanges();

        }

        if (Window.ClientBounds.Height>maxHeight) {

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = maxHeight;
            _graphics.ApplyChanges();

        }

        if (_graphics.IsFullScreen) {

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = maxWidth;
            _graphics.PreferredBackBufferHeight = maxHeight;
            _graphics.ApplyChanges();

        }

        skalaY = (float)Window.ClientBounds.Height/(16*rozmiar_bloku);
        skalaX = (float)Window.ClientBounds.Width/(20*rozmiar_bloku);  



        base.Update(gameTime);

    }

    protected override void Draw(GameTime gameTime) {

        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        czas_sinus = (float)gameTime.TotalGameTime.TotalSeconds;

        switch (aktywnascena) {

            case Sceny.MENU:

                nowagra = new Rectangle ((int)(460*skalaX),(int)(400*skalaY),(int)(320*skalaX),(int)(50*skalaY) );
                ustawienia = new Rectangle ((int)(410*skalaX),(int)(500*skalaY),(int)(430*skalaX),(int)(50*skalaY));
                informacje = new Rectangle ((int)(410*skalaX),(int)(600*skalaY),(int)(430*skalaX),(int)(50*skalaY));
                wyjscie = new Rectangle ((int)(450*skalaX),(int)(700*skalaY),(int)(325*skalaX),(int)(50*skalaY));                
                 
                _spriteBatch.DrawString(font, "MENU", new Vector2(450 * skalaX, 150 * skalaY), Color.White, 0, Vector2.Zero, skalaTekstu, SpriteEffects.None, 0);

                if (nowagra.Contains(pozycja_myszki)) {

                    _spriteBatch.DrawString(font, "NOWA GRA", new Vector2(460 * skalaX, 400 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    nowagra_hover = true;

                } else {

                    _spriteBatch.DrawString(font, "NOWA GRA", new Vector2(460 * skalaX, 400 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    nowagra_hover = false;

                }

                if (ustawienia.Contains(pozycja_myszki)) {

                    _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(410 * skalaX, 500 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    ustawienia_hover = true;

                } else {

                    _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(410 * skalaX, 500 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    ustawienia_hover = false;

                }

                if (informacje.Contains(pozycja_myszki)) {

                    _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(410 * skalaX, 600 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    informacje_hover = true;

                } else {

                    _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(410 * skalaX, 600 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    informacje_hover = false;

                }

                if (wyjscie.Contains(pozycja_myszki)) {

                    _spriteBatch.DrawString(font, "WYJSCIE", new Vector2(470 * skalaX, 700 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    wyjscie_hover = true;

                } else {
                
                    _spriteBatch.DrawString(font, "WYJSCIE", new Vector2(470* skalaX, 700 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    wyjscie_hover = false;

                }
                    
            break;

            case Sceny.POZIOM1:

                menu = new Rectangle ((int)(1165*skalaX),(int)(980*skalaY),(int)(100*skalaX),(int)(33*skalaY));

                _spriteBatch.Draw(tlo_poziom1,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

                foreach (var obiekt in Uzywki) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var obiekt in Pozostale) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var obiekt in Pozytywne_obiekty) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var tekstura in mapa1) {

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*Main.skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*Main.skalaY),
                        (int)(rozmiar_bloku*Main.skalaX),
                        (int)(rozmiar_bloku*Main.skalaY)
                    );

                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }

                Info_o_poziomie(1, 30);


            break;

            case Sceny.POZIOM2:

                _spriteBatch.Draw(tlo_poziom2,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

                foreach (var obiekt in Uzywki) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozostale) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozytywne_obiekty) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var tekstura in mapa2) {

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*skalaY),
                        (int)(rozmiar_bloku*skalaX),
                        (int)(rozmiar_bloku*skalaY)
                    );

                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }

                Info_o_poziomie(2, 35);

            break;

            case Sceny.POZIOM3:

                _spriteBatch.Draw(tlo_poziom3,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

                foreach (var obiekt in Uzywki) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozostale) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozytywne_obiekty) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var tekstura in mapa3) {

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*skalaY),
                        (int)(rozmiar_bloku*skalaX),
                        (int)(rozmiar_bloku*skalaY)
                    );

                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }

                Info_o_poziomie(3, 40);

            break;

            case Sceny.POZIOM4:



                _spriteBatch.Draw(tlo_poziom4,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);
                

                foreach (var obiekt in Uzywki) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozostale) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozytywne_obiekty) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                if (!spozyto_tabletke) {
                foreach (var tekstura in mapa4) {
                    

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*skalaY),
                        (int)(rozmiar_bloku*skalaX),
                        (int)(rozmiar_bloku*skalaY)
                    );

                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }
                } else {
                foreach (var tekstura in mapa4) {
                    
                    float przesuniecie = (float)Math.Sin(tekstura.Key.X + czas_sinus * 5) * 10;

                    Rectangle dest = new Rectangle(
                        (int)(tekstura.Key.X * rozmiar_bloku * skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku * skalaY + przesuniecie),
                        (int)(rozmiar_bloku * skalaX),
                        (int)(rozmiar_bloku * skalaY)
                    );

                    _spriteBatch.Draw(skrzynia, dest, Color.White);
                }

                Color mglaKolor = new Color(255, 0, 255, 100); 

                Rectangle mglaProstokat = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
                _spriteBatch.Draw(tlo_poziom4, mglaProstokat, mglaKolor);


                }

                Info_o_poziomie(4, 45);

            break;

            case Sceny.POZIOM5:



                _spriteBatch.Draw(tlo_poziom5,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

                foreach (var obiekt in Uzywki) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozostale) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }
                foreach (var obiekt in Pozytywne_obiekty) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                if(!spozyto_energetyk) {

                foreach (var tekstura in mapa5) {

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*skalaY),
                        (int)(rozmiar_bloku*skalaX),
                        (int)(rozmiar_bloku*skalaY)
                    );

                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }

                } else {

                    foreach (var tekstura in mapa5) {
                    
                    float przesuniecie = (float)Math.Sin(tekstura.Key.X + czas_sinus * 5000) * 4;

                    Rectangle dest = new Rectangle(
                        (int)(tekstura.Key.X * rozmiar_bloku * skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku * skalaY + przesuniecie),
                        (int)(rozmiar_bloku * skalaX),
                        (int)(rozmiar_bloku * skalaY)
                    );

                    _spriteBatch.Draw(skrzynia, dest, Color.White);
                }
                }

                Info_o_poziomie(5, 60);

            break;

            case Sceny.INFORMACJE:

            menu = new Rectangle ((int)(900*skalaX),(int)(950*skalaY),(int)(335*skalaX),(int)(33*skalaY));

            _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(340 * skalaX, 70 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.7), SpriteEffects.None, 0);

            _spriteBatch.DrawString(font, "ABY UKONCZYC POZIOM MUSISZ DOTRZEC DO METY W", new Vector2(20 * skalaX, 180 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "WYZNACZONYM CZASIE.", new Vector2(20 * skalaX, 230 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "PORUSZAJ SIE POSTACIA ZA POMOCA STRZALEK LUB", new Vector2(20 * skalaX, 310 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "KLAWISZY WSAD.", new Vector2(20 * skalaX, 360 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "UNIKAJ PO DRODZE UZYWEK, KTORE UTRUDNIAJA GRE", new Vector2(20 * skalaX, 440 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "I ZABIERAJA ZYCIE.", new Vector2(20 * skalaX, 490 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "ZBIERAJ ZDROWE ALTERNATYWY, ABY DOSTAC PUNKTY", new Vector2(20 * skalaX, 570 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "GRA KONCZY SIE PO UTRACIE WSZYSTKICH ZYC", new Vector2(20 * skalaX, 650 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "PRZEJDZ WSZYSTKIE POZIOMY BY WYGRAC, UZBIERAJ", new Vector2(20 * skalaX, 730 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "PRZY TYM JAK NAJWIECEJ PUNKTOW", new Vector2(20 * skalaX, 770 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "POWODZENIA !!!", new Vector2(480 * skalaX, 870 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.35), SpriteEffects.None, 0);



            if (menu.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            menu_hover = true;

            } else {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            menu_hover = false;

            }

            break;

            case Sceny.KONIEC:

            menu = new Rectangle ((int)(900*skalaX),(int)(950*skalaY),(int)(335*skalaX),(int)(33*skalaY));

            _spriteBatch.DrawString(font, "KONIEC GRY", new Vector2(350 * skalaX, 70 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.7), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "UDALO CI SIE UZYSKAC "+Gracz.Punkty, new Vector2(120 * skalaX, 600 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.4), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "PUNKTOW", new Vector2(920 * skalaX, 600 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.4), SpriteEffects.None, 0);

            if (wygrana) {
                MediaPlayer.Volume = 0;
                if (!zagrano_dzwiek){
                wygrana_sfx.Play();
                zagrano_dzwiek = true;
                }
                GraphicsDevice.Clear(Color.Green);
                _spriteBatch.DrawString(font, "WYGRANA !!!", new Vector2(450 * skalaX, 350 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);

            } else {

                if (koniec_czasu) {

                    GraphicsDevice.Clear(Color.Red);
                    _spriteBatch.DrawString(font, "PRZEGRANA", new Vector2(425 * skalaX, 310 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    _spriteBatch.DrawString(font, "ZABRAKLO CI CZASU", new Vector2(300 * skalaX, 400 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    
                } else {

                    GraphicsDevice.Clear(Color.Red);
                    _spriteBatch.DrawString(font, "PRZEGRANA", new Vector2(425 * skalaX, 310 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                    _spriteBatch.DrawString(font, "ZEBRANO ZA DUZO UZYWEK", new Vector2(200 * skalaX, 400 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);

                }

            }

            if (menu.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            menu_hover = true;

            } else {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            menu_hover = false;

            }

            break;

            case Sceny.USTAWIENIA:

            rozdzielczosc1 = new Rectangle ((int)(500*skalaX),(int)(320*skalaY),(int)(260*skalaX),(int)(33*skalaY) );
            rozdzielczosc2 = new Rectangle ((int)(525*skalaX),(int)(380*skalaY),(int)(200*skalaX),(int)(33*skalaY));
            rozdzielczosc3 = new Rectangle ((int)(525*skalaX),(int)(440*skalaY),(int)(200*skalaX),(int)(33*skalaY));
            dzwiek1 = new Rectangle ((int)(600*skalaX),(int)(620*skalaY),(int)(60*skalaX),(int)(33*skalaY) );
            dzwiek2 = new Rectangle ((int)(586*skalaX),(int)(680*skalaY),(int)(85*skalaX),(int)(33*skalaY));
            dzwiek3 = new Rectangle ((int)(573*skalaX),(int)(740*skalaY),(int)(110*skalaX),(int)(33*skalaY));
            menu = new Rectangle ((int)(900*skalaX),(int)(950*skalaY),(int)(335*skalaX),(int)(33*skalaY));

            _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(340 * skalaX, 70 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.7), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "ROZDZIELCZOSC", new Vector2(410 * skalaX, 240 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.4), SpriteEffects.None, 0);

            if (rozdzielczosc1.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "1280 X 1024", new Vector2(500 * skalaX, 320 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc1_hover = true;

            } else {

            _spriteBatch.DrawString(font, "1280 X 1024", new Vector2(500 * skalaX, 320 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc1_hover = false;

            }

            if (rozdzielczosc == 1) {

            _spriteBatch.DrawString(font, "1280 X 1024", new Vector2(500 * skalaX, 320 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

            }

            if (rozdzielczosc2.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "640 X 512", new Vector2(525 * skalaX, 380 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc2_hover = true;

            } else {

            _spriteBatch.DrawString(font, "640 X 512", new Vector2(525 * skalaX, 380 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc2_hover = false;

            }

            if (rozdzielczosc == 2) {

            _spriteBatch.DrawString(font, "640 X 512", new Vector2(525 * skalaX, 380 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

            }

            if (rozdzielczosc3.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "320 X 256", new Vector2(525 * skalaX, 440 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc3_hover = true;

            } else {

            _spriteBatch.DrawString(font, "320 X 256", new Vector2(525 * skalaX, 440 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            rozdzielczosc3_hover = false;

            }

            if (rozdzielczosc == 3) {

            _spriteBatch.DrawString(font, "320 X 256", new Vector2(525 * skalaX, 440 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

            }

            _spriteBatch.DrawString(font, "POZIOM GLOSNOSCI", new Vector2(370 * skalaX, 540 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.4), SpriteEffects.None, 0);

            if (dzwiek1.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "0 %", new Vector2(600 * skalaX, 620 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            dzwiek1_hover = true;

            } else {

            _spriteBatch.DrawString(font, "0 %", new Vector2(600 * skalaX, 620 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            dzwiek1_hover = false;

            }

            if (poziom_dzwieku == 0) {

            _spriteBatch.DrawString(font, "0 %", new Vector2(600 * skalaX, 620 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);


            }

            if (dzwiek2.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "50 %", new Vector2(586 * skalaX, 680 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            dzwiek2_hover = true;

            } else {

            _spriteBatch.DrawString(font, "50 %", new Vector2(586 * skalaX, 680 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            dzwiek2_hover = false;

            }

            if (poziom_dzwieku == 0.5f) {

            _spriteBatch.DrawString(font, "50 %", new Vector2(586 * skalaX, 680 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

            }

            if (dzwiek3.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "100 %", new Vector2(573 * skalaX, 740 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            dzwiek3_hover = true;

            } else {

            _spriteBatch.DrawString(font, "100 %", new Vector2(573 * skalaX, 740 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            dzwiek3_hover = false;

            }

            if (poziom_dzwieku == 1f) {

            _spriteBatch.DrawString(font, "100 %", new Vector2(573 * skalaX, 740 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
                
            }

            if (menu.Contains(pozycja_myszki)) {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            menu_hover = true;
            
            } else {

            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            menu_hover = false;

            }

            break;

            case Sceny.PLANSZA:

            if (poprzedni_poziom == 1){

                _spriteBatch.Draw(plansza1,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);

            }


            if (poprzedni_poziom == 2){

                _spriteBatch.Draw(plansza2,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);

            }

            if (poprzedni_poziom == 3){

                _spriteBatch.Draw(plansza3,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);

            }


            if (poprzedni_poziom == 4){

                _spriteBatch.Draw(plansza4,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);

            }


            if (poprzedni_poziom == 5){

                _spriteBatch.Draw(plansza5,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);

            }

            break;

        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    void Info_o_poziomie(int nr, int limit) {

        limit_czasu = limit;

        czas  = limit_czasu-licznik_sekund;

        if (czas < 1 && limit_czasu != 0) {

            koniec_czasu = true;
            czas = 0;
            Gracz.Zycie = 0;
            
        } else {
            koniec_czasu = false;

        }

        _spriteBatch.DrawString(font, "POZIOM "+ nr, new Vector2(545 * skalaX, 15 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        _spriteBatch.DrawString(font, "WYNIK: "+Gracz.Punkty, new Vector2(20 * skalaX, 980 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        _spriteBatch.DrawString(font, "CZAS: "+ czas, new Vector2(10 * skalaX, 15 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

        if (menu.Contains(pozycja_myszki)) {

        _spriteBatch.DrawString(font, "MENU", new Vector2(1165 * skalaX, 980 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        menu_hover = true;

        } else {

        _spriteBatch.DrawString(font, "MENU", new Vector2(1165 * skalaX, 980 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
        menu_hover = false;

        }

        poziom_zycia();
        
    }

    void Powrot_do_main_menu() {

       
        var mouseState = Mouse.GetState();

        if ((mouseState.LeftButton == ButtonState.Pressed)&&(menu_hover)) {
 
            aktywnascena = Sceny.MENU;
            reset = false;

        }
    
    }

    void Koniec() {
        
        if (koniec) {
            Reset_poziomu();
            aktywnascena = Sceny.KONIEC;
            koniec = false;
            MediaPlayer.Volume = 0f;
            koniec_sfx.Play();
        }

    }
    void Reset_poziomu() {

        Uzywki.Clear();
        Pozytywne_obiekty.Clear();
        Pozostale.Clear();
        licznik_sekund=0;
        gracz.pozycja.X = 70;
        gracz.pozycja.Y = 850;
        obiekty_dodane = false;
        nastepny_poziom = false;
        wygrana = false;
        zagrano_dzwiek = false;
        spozyto_alkohol=spozyto_energetyk=spozyto_marihuane=spozyto_tabletke=spozyto_papierosy=false;
    }
    void poziom_zycia() {

        if (Gracz.Zycie == 3) {

            _spriteBatch.Draw(serce, new Rectangle((int)(17*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);
            _spriteBatch.Draw(serce, new Rectangle((int)(18*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);
            _spriteBatch.Draw(serce, new Rectangle((int)(19*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);

        }

        if (Gracz.Zycie == 2) {

            _spriteBatch.Draw(serce, new Rectangle((int)(18*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);
            _spriteBatch.Draw(serce, new Rectangle((int)(19*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);

        }

        if (Gracz.Zycie == 1) {

            _spriteBatch.Draw(serce, new Rectangle((int)(19*rozmiar_bloku*skalaX),(int)(0*rozmiar_bloku*skalaY),(int)(rozmiar_bloku*skalaX),(int)(rozmiar_bloku*skalaY)),Color.White);

        }

        if (Gracz.Zycie == 0) {

            koniec = true;
            Koniec();

        }

    }

    void PelnyResetGry() {

        isResetting = true;  
        aktywnascena = Sceny.POZIOM1; 
        aktywnaMapa = mapa1;          
        Gracz.Zycie = 3;              
        Gracz.Punkty = 0;             
        Reset_poziomu();               
        reset = false;              
        obiekty_dodane = false;  
        nastepny_poziom = false;
        isResetting = false;  
        wygrana = false;
        koniec_czasu = false;
        

    }

    private Dictionary<Vector2, int> LadowanieMapy(string sciezka) {

        Dictionary<Vector2, int> result = new();
        StreamReader reader = new(sciezka);
        int y =0;
        string line;

        while((line = reader.ReadLine()) != null) {

            string[] items = line.Split(',');

            for (int x = 0; x< items.Length;x++) {

                if(int.TryParse(items[x],out int value)) {

                    if(value >0){

                        result[new Vector2(x,y)] = value;

                    }

                }

            }
            y++;
        }

        return result;

    }

}