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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace SciezkaZdrowia {

    internal class Gracz : Obiekt{
        public static int Zycie;
        public static bool przegrana=false;
        public static int Punkty = 0;
        public static int kierunek;
        private List<Rectangle> kolizje;
        private int poprzedni_kierunek;
        private Rectangle Obszar_gracza;
        private Vector2 Przyspieszenie;
        private bool wcisnieto_W;
        private bool skok,skok2,lewo,prawo;
        public override Rectangle obszar{

            get {

                return new Rectangle(
                    (int)(pozycja.X * Main.skalaX),
                    (int)(pozycja.Y * Main.skalaY),
                    (int)(0.8*Main.rozmiar_bloku * Main.skalaX),
                    (int)(1.5 * Main.rozmiar_bloku * Main.skalaY)                
                );

            }

        }

        public Gracz(Texture2D tekstura, Vector2 pozycja, List<Rectangle>kolizje, int zycie) : base(tekstura,pozycja) {

           this.kolizje = kolizje;
           Zycie=zycie;

        }

public override void Update(GameTime gameTime) {

    if (Zycie < 1) {
        przegrana = true;
        Gracz.Zycie = 3;
    } else {

        przegrana = false;

    }

    Aktualizacja_Kolizji();

    Obszar_gracza = new Rectangle(
        (int)(pozycja.X * Main.skalaX),
        (int)(pozycja.Y * Main.skalaY), 
        (int)(0.8*Main.rozmiar_bloku * Main.skalaX),
        (int)(1.5*Main.rozmiar_bloku* Main.skalaY)
    );
    
    if (((Keyboard.GetState().IsKeyDown(Keys.Right))||(Keyboard.GetState().IsKeyDown(Keys.D)))&&prawo) {

        if (Main.spozyto_energetyk) {

            Przyspieszenie.X = 0;
        } else {

        if (Main.spozyto_alkohol == false) {
            if (Main.spozyto_papierosy == false){
                if (Main.spozyto_energetyk == false){
                Przyspieszenie.X = 4;
                }
            } else {

                Przyspieszenie.X = 1;
            }
        kierunek = 2;
        } else {

        Przyspieszenie.X = -4;
        kierunek = 1;
        }
        
        poprzedni_kierunek = kierunek;
        }

    } else if (((Keyboard.GetState().IsKeyDown(Keys.Left)||(Keyboard.GetState().IsKeyDown(Keys.A)))&&lewo)) {

        if (Main.spozyto_energetyk){

            Przyspieszenie.X = 0;
        } else {

        if (Main.spozyto_alkohol == false) {

            if (Main.spozyto_papierosy == false){
                Przyspieszenie.X = -4;
            } else {

                Przyspieszenie.X = -1;
            }
        kierunek = 1;
        } else {

        Przyspieszenie.X = 4;
        kierunek = 2;
        }


        poprzedni_kierunek = kierunek;
        }
        
    } else {

        Przyspieszenie.X = 0;
        lewo = true;
        prawo = true;
        kierunek = 0;
        poprzedni_kierunek = kierunek;

    }

    float pozycjaX_przed_kolizja = pozycja.X;
    pozycja.X += Przyspieszenie.X;
    Obszar_gracza.X = (int)(pozycja.X*Main.skalaX);

    if (Kolizja(kolizje)) {

        pozycja.X = pozycjaX_przed_kolizja;
        lewo = false; 
        prawo = false;
        kierunek = 0;
        
    }

    if ((Keyboard.GetState().IsKeyDown(Keys.Up)||(Keyboard.GetState().IsKeyDown(Keys.W))) && skok && skok2) {

        Przyspieszenie.Y = -13;
        skok = skok2 = false;
        if (Keyboard.GetState().IsKeyDown(Keys.W)){
            wcisnieto_W = true;
        }

    }

    if (Keyboard.GetState().IsKeyUp(Keys.Up)) {

       

        if (wcisnieto_W) {
            if (Keyboard.GetState().IsKeyUp(Keys.W)) {

                skok2 = true;
                wcisnieto_W = false;
            }
        } else {
             skok2 = true;
        }

    }


    Przyspieszenie.Y += 1f;

    if (Przyspieszenie.Y > 25f) {

        Przyspieszenie.Y = 25f;

    }

    float pozycjaY_przed_kolizja = pozycja.Y;
    pozycja.Y += Przyspieszenie.Y;
    Obszar_gracza.Y = (int)(pozycja.Y*Main.skalaY);

    if (Kolizja(kolizje)) {

        if (Przyspieszenie.Y > 0 && pozycjaY_przed_kolizja < pozycja.Y){

            if (!Main.spozyto_energetyk){
            skok = true; 
            }
        }

        Przyspieszenie.Y = 0;
        pozycja.Y = pozycjaY_przed_kolizja;

    }
    
    base.Update(gameTime);
    
}


private void Aktualizacja_Kolizji() {

    kolizje.Clear();

    foreach (var tekstura in Main.aktywnaMapa) {

        Rectangle skrzyniaObszar = new Rectangle(
            (int)(tekstura.Key.X * Main.rozmiar_bloku * Main.skalaX),
            (int)(tekstura.Key.Y * Main.rozmiar_bloku * Main.skalaY),
            (int)(Main.rozmiar_bloku * Main.skalaX),
            (int)(Main.rozmiar_bloku * Main.skalaY)
        );
        kolizje.Add(skrzyniaObszar);

    }

}

private bool Kolizja(List<Rectangle> kolizje) {

    Aktualizacja_Kolizji();

    foreach (var skrzynia in kolizje) {
        
        if (Obszar_gracza.Intersects(skrzynia)) {

            return true; 

        }

    }

    return false; 

}

    }

}
