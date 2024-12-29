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

namespace SciezkaZdrowia {

    internal class PozytywnyObiekt : Obiekt {
        public int Punkty { get; set; }

        public PozytywnyObiekt(Texture2D tekstura, Vector2 pozycja, int punkty) : base(tekstura,pozycja){

            Punkty = punkty;
        }

    }

}