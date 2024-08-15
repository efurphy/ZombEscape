using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombEscape
{
    internal class Text
    {
        BitmapFont font;

        bool useIntegerPosition;

        Origin origin;
        Vector2 originVector;
        Vector2 position;

        Vector2 size;

        string text;

        Color color;

        public Text(BitmapFont font, Vector2 position, string text)
        {
            useIntegerPosition = true;

            this.font = font;

            SetText(text);

            SetOrigin(Origin.TopLeft);

            this.position = position;
            if (useIntegerPosition) this.position.Floor();

            this.color = Color.White;
        }

        public Text(BitmapFont font, Vector2 position, string text, Color color) : this(font, position, text)
        {
            this.color = color;
        }

        public Text(BitmapFont font, Vector2 position, string text, Color color, Origin origin) : this(font, position, text, color)
        {
            SetOrigin(origin);
        }

        public void SetText(string text)
        {
            this.text = text;
            this.size = this.font.MeasureString(text);
            SetOrigin(origin);
        }

        public void SetOrigin(Origin origin)
        {
            this.origin = origin;

            switch (origin)
            {
                case Origin.Right:          this.originVector = new Vector2(this.size.X, this.size.Y / 2f);   break;
                case Origin.TopRight:       this.originVector = new Vector2(this.size.X, 0f);                 break;
                case Origin.Top:            this.originVector = new Vector2(this.size.X / 2f, 0f);            break;
                case Origin.TopLeft:        this.originVector = new Vector2(0f, 0f);                          break;
                case Origin.Left:           this.originVector = new Vector2(0f, this.size.Y / 2f);            break;
                case Origin.BottomLeft:     this.originVector = new Vector2(0f, this.size.Y);                 break;
                case Origin.Bottom:         this.originVector = new Vector2(this.size.X / 2f, this.size.Y);   break;
                case Origin.BottomRight:    this.originVector = new Vector2(this.size.X, this.size.Y);        break;
                case Origin.Center:         this.originVector = this.size / 2f;                               break;
            }

            if (useIntegerPosition)
            {
                this.originVector.X = (int)this.originVector.X;
                this.originVector.Y = (int)this.originVector.Y;
            }
        }

        public void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, text, position, color, 0f, originVector, 1, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch batch, Vector2 position)
        {
            batch.DrawString(font, text, position, color, 0f, originVector, 1, SpriteEffects.None, 0f);
        }
    }
}
