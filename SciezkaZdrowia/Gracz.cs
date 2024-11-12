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
       private Rectangle Obszar_gracza;
       
        public Gracz(Texture2D tekstura, Vector2 pozycja, List<Rectangle>kolizje) : base(tekstura,pozycja){
           this.kolizje = kolizje;
        }
public override void Update(GameTime gameTime)
{
    
    
    base.Update(gameTime); 
   
   Obszar_gracza = new Rectangle(
    (int)(pozycja.X*Game1.skalaX),
    (int)(pozycja.Y*Game1.skalaY), 
    (int)(60*Game1.skalaX),
    (int)(101.3*Game1.skalaY));

    if (Keyboard.GetState().IsKeyDown(Keys.Right))
    {
        pozycja.X = pozycja.X + 3;
        Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);
        if (Kolizja(kolizje)){
            pozycja.X = pozycja.X -3;
            Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);
        }
    }

    if (Keyboard.GetState().IsKeyDown(Keys.Left))
    {
        pozycja.X = pozycja.X - 3;
        Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);
        if(Kolizja(kolizje)){
            pozycja.X = pozycja.X +3;
            Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);
        }

    }

    if (Keyboard.GetState().IsKeyDown(Keys.Up))
    {
        pozycja.Y = pozycja.Y - 3;
        Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);
        if(Kolizja(kolizje)){
            pozycja.Y = pozycja.Y +3;
            Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);
        }
    }

    if (Keyboard.GetState().IsKeyDown(Keys.Down))
    {
        pozycja.Y = pozycja.Y + 3;
        Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);
        if(Kolizja(kolizje)){
            pozycja.Y = pozycja.Y -3;
            Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);
        }
    }
}

private void Aktualizacja_Kolizji(){
    kolizje.Clear();
            foreach (var tekstura in Game1.mapa)
        {
            Rectangle skrzyniaObszar = new Rectangle(
                (int)(tekstura.Key.X * 64 * Game1.skalaX),
                (int)(tekstura.Key.Y * 64 * Game1.skalaY),
                (int)(64 * Game1.skalaX),
                (int)(64 * Game1.skalaY)
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