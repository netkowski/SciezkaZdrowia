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
    internal class Obiekt{
        public Texture2D tekstura;
        public Vector2 pozycja;
    
    public Rectangle obszar 
        {
            get
            {
              return new Rectangle(
              (int)(pozycja.X*Game1.skalaX), 
              (int)(pozycja.Y*Game1.skalaY),
              (int)(60*Game1.skalaX), 
              (int)(101.3*Game1.skalaY));
            }
        }

        public Obiekt(Texture2D tekstura, Vector2 pozycja){
            this.tekstura = tekstura;
            this.pozycja = pozycja;

        }
        public virtual void Update(GameTime gameTime){}
        public virtual void Draw(SpriteBatch spriteBatch){
            spriteBatch.Draw(tekstura, obszar, Color.White);
        }
    }
}