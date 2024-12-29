using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace SciezkaZdrowia
{
   internal class Obiektinnychrozmiarow : Obiekt {
        private float szerokosc;
        private float wysokosc;

        public override Rectangle obszar {
            
            get {

                return new Rectangle(
                    (int)(pozycja.X * Main.skalaX),
                    (int)(pozycja.Y * Main.skalaY),
                    (int)(szerokosc * Main.rozmiar_bloku * Main.skalaX),
                    (int)(wysokosc * Main.rozmiar_bloku * Main.skalaY)
                );

            }

        }

        public Obiektinnychrozmiarow(Texture2D tekstura, Vector2 pozycja, float szerokosc, float wysokosc): base(tekstura, pozycja) {

            this.szerokosc = szerokosc;
            this.wysokosc = wysokosc;
            
        }

   }
   
}