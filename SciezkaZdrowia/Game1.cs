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


public class Game1 : Game
{
    public static float skalaX;
    public static float skalaY;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    Texture2D[] animacja;
    Texture2D tlo;
    public static Dictionary<Vector2,int> mapa;
    private Texture2D skrzynia;
    
    int licznik;
    int ktora_klatka;
    Obiekt gracz;
    List<Obiekt> wrogowie;
   
    public Game1()
    {
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
        while((line = reader.ReadLine()) != null){
            string[] items = line.Split(',');
            for (int x = 0; x< items.Length;x++){
                if(int.TryParse(items[x],out int value)){
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
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 1024;
        this.Window.AllowUserResizing = true;
        this.Window.Title = "Ścieżka Zdrowia                                   s193335";
        this.Window.Position = Point.Zero;
        _graphics.ApplyChanges();
            skalaY = (float)Window.ClientBounds.Height/1024f;
            skalaX = (float)Window.ClientBounds.Width/1280f;  

        base.Initialize();

       
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        wrogowie = new();
        // TODO: use this.Content to load your game content here
        tlo = Content.Load<Texture2D>("tlo");
       animacja = new Texture2D[2]; 
       animacja[0] = Content.Load<Texture2D>("Klatka_0");  
       animacja[1] = Content.Load<Texture2D>("Klatka_1");  
       skrzynia = Content.Load<Texture2D>("box");

       List<Rectangle> kolizje = new List<Rectangle>();
        foreach (var tekstura in mapa)
        {
            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * 64 * Game1.skalaX),
                (int)(tekstura.Key.Y * 64 * Game1.skalaY),
                (int)(64 * Game1.skalaX),
                (int)(64 * Game1.skalaY)
            );
            
            kolizje.Add(skrzyniaObszar);
        }
       gracz = new Gracz(animacja[ktora_klatka], new Vector2(200,200), kolizje);
       Texture2D tekstura_wroga = Content.Load<Texture2D>("Wrog");
       wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(100,200)));
       wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(100,300)));
       wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(100,400)));
       wrogowie.Add(new Wrog(tekstura_wroga,new Vector2(100,100)));
       
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        List<Obiekt> ZebraneObiekty = new();
        foreach(var obiekt in wrogowie){
            obiekt.Update(gameTime);
            if (obiekt.obszar.Intersects(gracz.obszar)){
                ZebraneObiekty.Add(obiekt);
            }
        }
        gracz.Update(gameTime);
        foreach(var obiekt in ZebraneObiekty){
            wrogowie.Remove(obiekt);
        }
    
        licznik++;
        if (licznik > 30){
            licznik = 0;
            ktora_klatka++;
            if (ktora_klatka>1){
                ktora_klatka = 0;
            }
        }

        int maxHeight = 1024;
        int maxWidth = 1280;
        if (Window.ClientBounds.Width>maxWidth){
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = maxWidth;
            _graphics.ApplyChanges();
        }
        if(Window.ClientBounds.Height>maxHeight){
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferHeight = maxHeight;
            _graphics.ApplyChanges();
        }
        if(_graphics.IsFullScreen){
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = maxWidth;
            _graphics.PreferredBackBufferHeight = maxHeight;
            _graphics.ApplyChanges();
        }

            skalaY = (float)Window.ClientBounds.Height/1024f;
            skalaX = (float)Window.ClientBounds.Width/1280f;  


        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(tlo,new Rectangle (0,0,(int)(1280*Game1.skalaX),(int)(1024*Game1.skalaY)), Color.White);
        _spriteBatch.Draw(animacja[ktora_klatka], gracz.obszar, Color.White);
        foreach (var obiekt in wrogowie){
            _spriteBatch.Draw(obiekt.tekstura, obiekt.obszar, Color.White);
        }
        foreach (var tekstura in mapa){
            Rectangle dest = new (
                (int)(tekstura.Key.X * 64*Game1.skalaX),
                (int)(tekstura.Key.Y * 64*Game1.skalaY),
                (int)(64*Game1.skalaX),
                (int)(64*Game1.skalaY)
            );
            _spriteBatch.Draw(skrzynia,dest,Color.White);
        }
        _spriteBatch.End();


        base.Draw(gameTime);
    }
}