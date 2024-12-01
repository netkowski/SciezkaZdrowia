using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SciezkaZdrowia;

public class Main : Game {

    public static float rozmiar_bloku = 64;
    public static float skalaX;
    public static float skalaY;
    public static Dictionary<Vector2,int> mapa;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D[] animacja;
    private Texture2D tlo;
    private Texture2D skrzynia;
    private int licznik;
    private int ktora_klatka;
    private Obiekt gracz;
    private List<Obiekt> wrogowie;
   
    public Main() {

        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        mapa = LadowanieMapy("/home/netkoski/moje/studia/JPWP/projekt_programowanie/Monogame_Sciezka_Zdrowia/SciezkaZdrowia/Pliki/mapa.csv");
       
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

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        tlo = Content.Load<Texture2D>("tlo");
        animacja = new Texture2D[6]; 
        animacja[0] = Content.Load<Texture2D>("front1");  
        animacja[1] = Content.Load<Texture2D>("front2"); 
        animacja[2] = Content.Load<Texture2D>("lewo1");  
        animacja[3] = Content.Load<Texture2D>("lewo2");  
        animacja[4] = Content.Load<Texture2D>("prawo1");  
        animacja[5] = Content.Load<Texture2D>("prawo2");   
        skrzynia = Content.Load<Texture2D>("box");
        Texture2D tekstura_wody = Content.Load<Texture2D>("woda");
        Texture2D tekstura_wroga = Content.Load<Texture2D>("alkohol");

        wrogowie = new();

        List<Rectangle> kolizje = new List<Rectangle>();

        foreach (var tekstura in mapa) {

            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * rozmiar_bloku * Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku * Main.skalaY),
                (int)(rozmiar_bloku * Main.skalaX),
                (int)(rozmiar_bloku * Main.skalaY)
            );
            kolizje.Add(skrzyniaObszar);

        }

        gracz = new Gracz(animacja[ktora_klatka], new Vector2(200,200), kolizje);
        wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(rozmiar_bloku,3*rozmiar_bloku)));
        wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(rozmiar_bloku,4*rozmiar_bloku)));
        wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(2*rozmiar_bloku,5*rozmiar_bloku)));
        wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(2*rozmiar_bloku,2*rozmiar_bloku)));

        wrogowie.Add(new Wrog(tekstura_wody,new Vector2(10*rozmiar_bloku,12*rozmiar_bloku)));
        
    }

    protected override void Update(GameTime gameTime) {

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {

            Exit();

        }

        List<Obiekt> ZebraneObiekty = new();

        foreach(var obiekt in wrogowie) {
            obiekt.Update(gameTime);

            if (obiekt.obszar.Intersects(gracz.obszar)) {

                ZebraneObiekty.Add(obiekt);

            }

        }

        gracz.Update(gameTime);

        foreach(var obiekt in ZebraneObiekty) {

            wrogowie.Remove(obiekt);

        }

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

        int maxHeight = (int)(16*rozmiar_bloku);
        int maxWidth = (int)(20*rozmiar_bloku);

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

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(tlo,new Rectangle (0,0,(int)(20*rozmiar_bloku*Main.skalaX),(int)(16*rozmiar_bloku*Main.skalaY)), Color.White);
        _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);

        foreach (var obiekt in wrogowie) {

            _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);

        }

        foreach (var tekstura in mapa) {

            Rectangle dest = new (
                (int)(tekstura.Key.X * rozmiar_bloku*Main.skalaX),
                (int)(tekstura.Key.Y * rozmiar_bloku*Main.skalaY),
                (int)(rozmiar_bloku*Main.skalaX),
                (int)(rozmiar_bloku*Main.skalaY)
            );
            _spriteBatch.Draw(skrzynia,dest,Color.White);

        }

        _spriteBatch.End();

        base.Draw(gameTime);

    }

}