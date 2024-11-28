using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SciezkaZdrowia
{
    internal class Gracz : Obiekt{
      
    
       private List<Rectangle> kolizje;
       public static int kierunek;
       private int poprzedni_kierunek;
       private Rectangle Obszar_gracza;
       private Vector2 Przyspieszenie;
       private bool skok,drugi_skok,lewo,prawo;
       public override Rectangle obszar{
        get {
            return new Rectangle(
                (int)(pozycja.X * Game1.skalaX),
                (int)(pozycja.Y * Game1.skalaY),
                (int)(0.8*Game1.rozmiar_bloku * Game1.skalaX),
                (int)(1.5 * Game1.rozmiar_bloku * Game1.skalaY)                
            );
        }
       }
        public Gracz(Texture2D tekstura, Vector2 pozycja, List<Rectangle>kolizje) : base(tekstura,pozycja){
           this.kolizje = kolizje;
        }

public override void Update(GameTime gameTime)
{
    Aktualizacja_Kolizji();

    Obszar_gracza = new Rectangle(
        (int)(pozycja.X * Game1.skalaX),
        (int)(pozycja.Y * Game1.skalaY), 
        (int)(0.8*Game1.rozmiar_bloku * Game1.skalaX),
        (int)(1.5*Game1.rozmiar_bloku* Game1.skalaY)
    );
    
    // Ruch w poziomie
    if ((Keyboard.GetState().IsKeyDown(Keys.Right))&&prawo)
    {
        Przyspieszenie.X = 4;
        kierunek = 2;
        poprzedni_kierunek = kierunek;
    }
    else if ((Keyboard.GetState().IsKeyDown(Keys.Left)&&lewo))
    {
        Przyspieszenie.X = -4;
        kierunek = 1;
        poprzedni_kierunek = kierunek;
        
    }
    else
    {
        Przyspieszenie.X = 0;
        lewo = true;
        prawo = true;
        kierunek = 0;
        poprzedni_kierunek = kierunek;
    }

    float pozycjaX_przed_kolizja = pozycja.X;
    pozycja.X += Przyspieszenie.X;
    
    Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);
    

    if (Kolizja(kolizje))
    {
        pozycja.X = pozycjaX_przed_kolizja;
        lewo = false; 
        prawo = false;
        kierunek = 0;
        Przyspieszenie.Y = 0.4f;        
        if (drugi_skok){
        skok = true;
        }
    }

    if (Keyboard.GetState().IsKeyDown(Keys.Up) && skok && drugi_skok)
    {
        Przyspieszenie.Y = -13;
        skok = drugi_skok = false;
    }
    if (Keyboard.GetState().IsKeyUp(Keys.Up))
    {
        drugi_skok = true;
    }


    if (Kolizja(kolizje) && Przyspieszenie.Y > 0)
    {
        Przyspieszenie.Y = 1; 
    }


    Przyspieszenie.Y += 1;
    if (Przyspieszenie.Y > 25f)
    {
        Przyspieszenie.Y = 25f;
    }

    float pozycjaY_przed_kolizja = pozycja.Y;
    pozycja.Y += Przyspieszenie.Y;
    Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);

if (Kolizja(kolizje))
{
    

    foreach (var skrzynia in kolizje)
    {
        if (Obszar_gracza.Intersects(skrzynia))
        {
            
            if (pozycjaY_przed_kolizja + Obszar_gracza.Height <= skrzynia.Top) 
            {
                pozycja.Y = (skrzynia.Top - Obszar_gracza.Height) / Game1.skalaY;
                Przyspieszenie.Y = 1;
                skok = true; 
            }
            else if (pozycjaY_przed_kolizja >= skrzynia.Bottom) 
            {
                pozycja.Y = skrzynia.Bottom / Game1.skalaY;
                Przyspieszenie.Y = -1;
               
            }
            Przyspieszenie.Y = 0;
        }
         
    }
}
    base.Update(gameTime);  
}



private void Aktualizacja_Kolizji(){
    kolizje.Clear();
            foreach (var tekstura in Game1.mapa)
        {
            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * Game1.rozmiar_bloku * Game1.skalaX),
                (int)(tekstura.Key.Y * Game1.rozmiar_bloku * Game1.skalaY),
                (int)(Game1.rozmiar_bloku * Game1.skalaX),
                (int)(Game1.rozmiar_bloku * Game1.skalaY)
            );
            
            kolizje.Add(skrzyniaObszar);
        }
}
private bool Kolizja(List<Rectangle> kolizje)
{
    Aktualizacja_Kolizji();
    foreach (var skrzynia in kolizje)
    {
        
        if (Obszar_gracza.Intersects(skrzynia))
        {
            return true; 
        }
    }
    return false; 
}
    }
}