﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SciezkaZdrowia;

enum Sceny {
    MENU,
    USTAWIENIA,
    INFORMACJE,
    POZIOM1,
    POZIOM2,
}

public class Main : Game {

    public static float rozmiar_bloku = 64;
    public static float skalaX;
    public static float skalaY;
    public static Dictionary<Vector2, int> aktywnaMapa;
    public static Dictionary<Vector2,int> mapa1, mapa2;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D[] animacja;
    private Texture2D tlo_poziom1;
    private Texture2D skrzynia;
    private int licznik;
    private int ktora_klatka;
    private Obiekt gracz;
    private List<Obiekt> wrogowie;
    private Sceny aktywnascena;
    private int maxHeight = (int)(16*rozmiar_bloku);
    private int maxWidth = (int)(20*rozmiar_bloku);
    private SpriteFont font;
    private Rectangle pozycja_myszki,nowagra,ustawienia,informacje,rozdzielczosc1,rozdzielczosc2,rozdzielczosc3,menu;
    private bool nowagra_hover,ustawienia_hover,informacje_hover,rozdzielczosc1_hover,rozdzielczosc2_hover,rozdzielczosc3_hover,menu_hover;
    public float skalaTekstu;
    private int timer,licznik_sekund = 0;
    private int limit_czasu;
    
    public Main() {

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        aktywnascena = Sceny.MENU;
        mapa1 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa1.csv");
        mapa2 = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa2.csv");
        aktywnaMapa = mapa1;
        
    }

    private Dictionary<Vector2, int> LadowanieMapy(string sciezka){

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
    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = (int)(20*rozmiar_bloku);
        _graphics.PreferredBackBufferHeight = (int)(16*rozmiar_bloku);
        this.Window.AllowUserResizing = true;
        this.Window.Title = "Ścieżka Zdrowia Nikodem Netkowski s193335";
        this.Window.Position = Point.Zero;
        _graphics.ApplyChanges();
        skalaX = (float)Window.ClientBounds.Width/(float)(20*rozmiar_bloku); 
        skalaY = (float)Window.ClientBounds.Height/(float)(16*rozmiar_bloku);
        
        base.Initialize();

    }

    protected override void LoadContent()
    {

        font = Content.Load<SpriteFont>("Fonts/8bitfont");
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        tlo_poziom1 = Content.Load<Texture2D>("tlo");
        animacja = new Texture2D[6]; 
        animacja[0] = Content.Load<Texture2D>("front1");  
        animacja[1] = Content.Load<Texture2D>("front2"); 
        animacja[2] = Content.Load<Texture2D>("lewo1");  
        animacja[3] = Content.Load<Texture2D>("lewo2");  
        animacja[4] = Content.Load<Texture2D>("prawo1");  
        animacja[5] = Content.Load<Texture2D>("prawo2");   
        skrzynia = Content.Load<Texture2D>("box");
        Texture2D tekstura_wody = Content.Load<Texture2D>("woda");
        Texture2D tekstura_alkoholu = Content.Load<Texture2D>("alkohol");
        Texture2D serce = Content.Load<Texture2D>("serce");
        wrogowie = new();

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

        gracz = new Gracz(animacja[ktora_klatka], new Vector2(70,800), kolizje);
        wrogowie.Add(new Uzywka(tekstura_alkoholu,new Vector2(13*rozmiar_bloku,10*rozmiar_bloku)));
        wrogowie.Add(new Uzywka(tekstura_alkoholu,new Vector2(13*rozmiar_bloku,11*rozmiar_bloku)));
        wrogowie.Add(new Uzywka(tekstura_alkoholu,new Vector2(13*rozmiar_bloku,12*rozmiar_bloku)));
        wrogowie.Add(new Uzywka(tekstura_alkoholu,new Vector2(13*rozmiar_bloku,13*rozmiar_bloku)));

        wrogowie.Add(new Uzywka(tekstura_wody,new Vector2(12*rozmiar_bloku,12*rozmiar_bloku)));


        
    }

    protected override void Update(GameTime gameTime) {

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {

            Exit();

        }

        timer += 1;

        if (timer % 60 == 0){

            licznik_sekund += 1;

        }

        skalaTekstu = Math.Min(skalaX, skalaY);
        var mouseState = Mouse.GetState();
        pozycja_myszki = new Rectangle(mouseState.X,mouseState.Y,1,1);
             
        switch (aktywnascena) {

            case Sceny.MENU:

                if ((mouseState.LeftButton == ButtonState.Pressed)&&(nowagra_hover)) {
                    aktywnascena = Sceny.POZIOM1;
                    aktywnaMapa = mapa1;
                    Reset_poziomu();
                }

                if ((mouseState.LeftButton == ButtonState.Pressed)&&(ustawienia_hover)) {
                    aktywnascena = Sceny.USTAWIENIA;
                } 
                
                if ((mouseState.LeftButton == ButtonState.Pressed)&&(informacje_hover)) {
                    aktywnascena = Sceny.INFORMACJE;
                }                                

            break;

            case Sceny.POZIOM1:
                
                if (Keyboard.GetState().IsKeyDown(Keys.K)){
                    aktywnascena = Sceny.POZIOM2;
                }
                Powrot_do_main_menu();           

            break;

            case Sceny.POZIOM2:
                aktywnaMapa = mapa2;
                Powrot_do_main_menu();
            break;

            case Sceny.USTAWIENIA:

            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc1_hover)) {
                _graphics.PreferredBackBufferHeight = (int)(16*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(20*rozmiar_bloku);
                _graphics.ApplyChanges();
            }
            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc2_hover)) {
                _graphics.PreferredBackBufferHeight = (int)(8*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(10*rozmiar_bloku);
                _graphics.ApplyChanges();
            }
            if ((mouseState.LeftButton == ButtonState.Pressed)&&(rozdzielczosc3_hover)) {
                _graphics.PreferredBackBufferHeight = (int)(4*rozmiar_bloku);
                _graphics.PreferredBackBufferWidth = (int)(5*rozmiar_bloku);
                _graphics.ApplyChanges();
            }
            Powrot_do_main_menu();
            break;

            case Sceny.INFORMACJE:
            Powrot_do_main_menu();
            break;

        }
        List<Obiekt> ZebraneObiekty = new();

        foreach(var obiekt in wrogowie) {
            obiekt.Update(gameTime);

            if (obiekt.obszar.Intersects(gracz.obszar)) {

                ZebraneObiekty.Add(obiekt);

            }

        }

        foreach(var obiekt in ZebraneObiekty) {

            wrogowie.Remove(obiekt);

        }

        gracz.Update(gameTime);

        licznik++;

        if (licznik > 60) {

            licznik = 0;

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

        switch (aktywnascena){

            case Sceny.MENU:
                nowagra = new Rectangle ((int)(460*skalaX),(int)(400*skalaY),(int)(320*skalaX),(int)(50*skalaY) );
                ustawienia = new Rectangle ((int)(410*skalaX),(int)(500*skalaY),(int)(430*skalaX),(int)(50*skalaY));
                informacje = new Rectangle ((int)(410*skalaX),(int)(600*skalaY),(int)(430*skalaX),(int)(50*skalaY));
                
                 
                
                 _spriteBatch.DrawString(font, "MENU", new Vector2(450 * skalaX, 150 * skalaY), Color.White, 0, Vector2.Zero, skalaTekstu, SpriteEffects.None, 0);

                 if (nowagra.Contains(pozycja_myszki)){
                 _spriteBatch.DrawString(font, "NOWA GRA", new Vector2(460 * skalaX, 400 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 nowagra_hover = true;
                 } else {
                 _spriteBatch.DrawString(font, "NOWA GRA", new Vector2(460 * skalaX, 400 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 nowagra_hover = false;
                 }

                if (ustawienia.Contains(pozycja_myszki)){
                 _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(410 * skalaX, 500 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 ustawienia_hover = true;
                } else {
                 _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(410 * skalaX, 500 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 ustawienia_hover = false;

                }

                if (informacje.Contains(pozycja_myszki)){
                 _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(410 * skalaX, 600 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 informacje_hover = true;
                } else {
                 _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(410 * skalaX, 600 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.5), SpriteEffects.None, 0);
                 informacje_hover = false;

                }
                    
            break;

            case Sceny.POZIOM1:
                limit_czasu = 30;
                menu = new Rectangle ((int)(1165*skalaX),(int)(980*skalaY),(int)(100*skalaX),(int)(33*skalaY));

                _spriteBatch.Draw(tlo_poziom1,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);



                foreach (var obiekt in wrogowie) {

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

                _spriteBatch.Draw(tlo_poziom1,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
                _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

                foreach (var obiekt in wrogowie) {

                    _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

                }

                foreach (var tekstura in mapa2) {

                    Rectangle dest = new (
                        (int)(tekstura.Key.X * rozmiar_bloku*Main.skalaX),
                        (int)(tekstura.Key.Y * rozmiar_bloku*Main.skalaY),
                        (int)(rozmiar_bloku*Main.skalaX),
                        (int)(rozmiar_bloku*Main.skalaY)
                    );
                    _spriteBatch.Draw(skrzynia,dest,Color.White);

                }
                Info_o_poziomie(2, 40);

            break;

            case Sceny.INFORMACJE:
            menu = new Rectangle ((int)(900*skalaX),(int)(950*skalaY),(int)(335*skalaX),(int)(33*skalaY));

            _spriteBatch.DrawString(font, "INFORMACJE", new Vector2(340 * skalaX, 70 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.7), SpriteEffects.None, 0);

            if (menu.Contains(pozycja_myszki)){
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
            menu = new Rectangle ((int)(900*skalaX),(int)(950*skalaY),(int)(335*skalaX),(int)(33*skalaY));

            _spriteBatch.DrawString(font, "USTAWIENIA", new Vector2(340 * skalaX, 70 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.7), SpriteEffects.None, 0);
            _spriteBatch.DrawString(font, "ROZDZIELCZOSC", new Vector2(410 * skalaX, 240 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.4), SpriteEffects.None, 0);

            if (rozdzielczosc1.Contains(pozycja_myszki)){
            _spriteBatch.DrawString(font, "1280 X 1024", new Vector2(500 * skalaX, 320 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc1_hover = true;
            } else {
            _spriteBatch.DrawString(font, "1280 X 1024", new Vector2(500 * skalaX, 320 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc1_hover = false;
            }

            if (rozdzielczosc2.Contains(pozycja_myszki)){
            _spriteBatch.DrawString(font, "640 X 512", new Vector2(525 * skalaX, 380 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc2_hover = true;
            } else {
            _spriteBatch.DrawString(font, "640 X 512", new Vector2(525 * skalaX, 380 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc2_hover = false;
            }

            if (rozdzielczosc3.Contains(pozycja_myszki)){
            _spriteBatch.DrawString(font, "320 X 256", new Vector2(525 * skalaX, 440 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            rozdzielczosc3_hover = true;
            } else {
            _spriteBatch.DrawString(font, "320 X 256", new Vector2(525 * skalaX, 440 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            rozdzielczosc3_hover = false;
            }

            if (menu.Contains(pozycja_myszki)){
            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
            menu_hover = true;
            } else {
            _spriteBatch.DrawString(font, "POWROT DO MENU", new Vector2(900 * skalaX, 950 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
            menu_hover = false;
            }

            break;

        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    void Info_o_poziomie(int nr, int limit_czasu){
        int czas = limit_czasu-licznik_sekund;
        if (czas < 1){
            czas = 0;
        }
        _spriteBatch.DrawString(font, "POZIOM "+ nr, new Vector2(545 * skalaX, 15 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        _spriteBatch.DrawString(font, "WYNIK: ", new Vector2(20 * skalaX, 980 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        _spriteBatch.DrawString(font, "CZAS: "+ czas, new Vector2(10 * skalaX, 15 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);

        if (menu.Contains(pozycja_myszki)){
        _spriteBatch.DrawString(font, "MENU", new Vector2(1165 * skalaX, 980 * skalaY), Color.Yellow, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);
        menu_hover = true;
        } else {
        _spriteBatch.DrawString(font, "MENU", new Vector2(1165 * skalaX, 980 * skalaY), Color.White, 0, Vector2.Zero, (float)(skalaTekstu*0.3), SpriteEffects.None, 0);           
        menu_hover = false;
        }
    }

    void Powrot_do_main_menu(){
            var mouseState = Mouse.GetState();
            if ((mouseState.LeftButton == ButtonState.Pressed)&&(menu_hover)) {
            aktywnascena = Sceny.MENU;
            }
    
    }
    void Reset_poziomu(){
     
    gracz.pozycja.X = 70;
    gracz.pozycja.Y = 800;

    }


}