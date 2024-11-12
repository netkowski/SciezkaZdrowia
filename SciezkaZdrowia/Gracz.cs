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
       private Vector2 Przyspieszenie;
       private bool skok,skok2,lewo,prawo;
       
        public Gracz(Texture2D tekstura, Vector2 pozycja, List<Rectangle>kolizje) : base(tekstura,pozycja){
           this.kolizje = kolizje;
        }

public override void Update(GameTime gameTime)
{
    Aktualizacja_Kolizji();
    Obszar_gracza = new Rectangle(
        (int)(pozycja.X * Game1.skalaX),
        (int)(pozycja.Y * Game1.skalaY), 
        (int)(60 * Game1.skalaX),
        (int)(101.3 * Game1.skalaY)
    );

    // Ruch w poziomie
    if ((Keyboard.GetState().IsKeyDown(Keys.Right))&&prawo)
    {
        Przyspieszenie.X = 3;
    }
    else if ((Keyboard.GetState().IsKeyDown(Keys.Left)&&lewo))
    {
        Przyspieszenie.X = -3;
        
    }
    else
    {
        Przyspieszenie.X = 0;
        lewo = true;
        prawo = true;
    }

    float pozycjaX_przed_kolizja = pozycja.X;
    pozycja.X += Przyspieszenie.X;
    Obszar_gracza.X = (int)(pozycja.X*Game1.skalaX);

    if (Kolizja(kolizje))
    {
        pozycja.X = pozycjaX_przed_kolizja;
        lewo = false; 
        prawo = false;
    }

    if (Keyboard.GetState().IsKeyDown(Keys.Up) && skok && skok2)
    {
        Przyspieszenie.Y = -12;
        skok = skok2 = false;
    }
    if (Keyboard.GetState().IsKeyUp(Keys.Up))
    {
        skok2 = true;
    }

    // Grawitacja
    Przyspieszenie.Y += 1f;
    if (Przyspieszenie.Y > 25f)
    {
        Przyspieszenie.Y = 25f;
    }

    float pozycjaY_przed_kolizja = pozycja.Y;
    pozycja.Y += Przyspieszenie.Y;
    Obszar_gracza.Y = (int)(pozycja.Y*Game1.skalaY);

    if (Kolizja(kolizje))
    {
        if (Przyspieszenie.Y > 0 && pozycjaY_przed_kolizja < pozycja.Y)
        {
            skok = true; 
        }

        Przyspieszenie.Y = 0;
        pozycja.Y = pozycjaY_przed_kolizja;
    }

    base.Update(gameTime);
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