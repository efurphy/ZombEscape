using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace ZombEscape
{
    internal class Button
    {
        Texture2D texture;
        Color textureColor;
        
        Text text;

        Vector2 position;
        Vector2 origin;
        Vector2 textPosition;
        Vector2 drawPosition;
        Vector2 textDrawPosition;

        public Action OnClick;
        public Action OnRelease;
        public Action WhileHovered;
        public Action WhilePressed;

        bool hovered;
        bool pressed;

        Action<string> SetText;

        public Button(Vector2 position, Texture2D texture, Text text, Action onClick = null, Action onRelease = null)
        {
            this.position = position;
            this.texture = texture;
            this.text = text;
            SetOrigin(Origin.TopLeft);
            SetTextOrigin(Origin.TopLeft);
            OnClick = onClick;
            OnRelease = onRelease;
            hovered = false;
            pressed = false;
            textureColor = Color.White;
            SetText = text.SetText;
        }

        private bool CollisionPoint(float x, float y)
        {
            return x >= drawPosition.X && x <= drawPosition.X + texture.Width && y >= drawPosition.Y && y <= drawPosition.Y + texture.Height;
        }

        private bool IsHovered() => CollisionPoint(Microsoft.Xna.Framework.Input.Mouse.GetState().X, Microsoft.Xna.Framework.Input.Mouse.GetState().Y);

        public void Update(GameTime gameTime)
        {
            hovered = IsHovered();

            if (hovered)
            {
                WhileHovered();

                if (Mouse.LeftButtonClicked)
                {
                    OnClick?.Invoke();
                    pressed = true;
                }
                else if (Mouse.LeftButtonReleased)
                {
                    OnRelease?.Invoke();
                }
            }

            if (!Mouse.LeftButtonDown) pressed = false;

            if (pressed) { WhilePressed(); }
        }

        public void Draw(SpriteBatch batch)
        {
            var color = Color.White;
            if (hovered) { color = Color.DarkGray; } // for some reason, Color.DarkGray is lighter than Color.Gray
            if (pressed) { color = Color.Gray; }
            batch.Draw(texture, drawPosition, color);
            text.Draw(batch, textDrawPosition);
        }

        public void SetOrigin(Origin origin)
        {
            SetOriginInternal(origin, ref this.origin);
            drawPosition = position - this.origin;
        }

        public void SetTextOrigin(Origin origin)
        {
            SetOriginInternal(origin, ref textPosition);
            text.SetOrigin(origin);
            textPosition = textPosition + position;
            textDrawPosition = textPosition - this.origin;
        }

        private void SetOriginInternal(Origin origin, ref Vector2 vector)
        {
            switch (origin)
            {
                case Origin.Right:          vector.X = this.texture.Width; vector.Y = this.texture.Height / 2f; break;
                case Origin.TopRight:       vector.X = this.texture.Width; vector.Y = 0f; break;
                case Origin.Top:            vector.X = this.texture.Width / 2f; vector.Y = 0f; break;
                case Origin.TopLeft:        vector.X = 0f; vector.Y = 0f; break;
                case Origin.Left:           vector.X = 0f; vector.Y = this.texture.Height / 2f; break;
                case Origin.BottomLeft:     vector.X = 0f; vector.Y = this.texture.Height; break;
                case Origin.Bottom:         vector.X = this.texture.Width / 2f; vector.Y = this.texture.Height; break;
                case Origin.BottomRight:    vector.X = this.texture.Width; vector.Y = this.texture.Height; break;
                case Origin.Center:         vector.X = this.texture.Width / 2f; vector.Y = this.texture.Height / 2f; break;
            }
        }

    }
}
